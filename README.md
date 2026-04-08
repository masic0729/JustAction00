# ⚔️ Pure Action

> 3D 액션 RPG 개인 프로젝트 | Unity · C# · Custom Behavior Tree · DOTween Pro

<br>

## 📌 프로젝트 개요

| 항목 | 내용 |
|------|------|
| 장르 | 3D 액션 RPG |
| 플랫폼 | PC |
| 개발 형태 | 개인 프로젝트 |
| 개발 기간 | 2025.07 ~ 2026.01 |
| 기술 스택 | Unity, C#, Custom Behavior Tree (직접 구현), DOTween Pro |

<br>

## 🎯 제작 목적

Pure Action은 3D 액션 게임의 전투 경험을 구현하는 데서 한 걸음 더 나아가,  
**확장성과 제작 효율을 구조 설계로 개선할 수 있는지를 검증하기 위해 제작한 개인 프로젝트입니다.**

- 몬스터 AI는 **Custom Behavior Tree** 기반으로 설계해 행동 트리 구조를 유지한 채 개별 몬스터 확장이 가능하도록 구성
- 전투 공간은 **DFS 기반 절차적 맵 생성**과 **타일 단위 배치 툴**을 통해 반복 테스트와 밸런싱 비용을 낮출 수 있도록 설계

<br>

## 🕹️ 게임 플레이 흐름

```
스테이지 진입
    ↓
전투 지역 이동
    ↓
몬스터 처치 및 보상 획득 / NPC 룰렛 이벤트 및 보상 획득
    ↓
장비 착용 → 소비 아이템 사용 / 회복 → 능력치 반영
    ↓
보스 몬스터 전투
    ↓
스테이지 클리어
```

<br>

## 🗂️ 시스템 구조 요약

| 시스템 | 핵심 설계 판단 | 효과 |
|--------|--------------|------|
| Custom Behavior Tree | FSM 대신 BT 선택, 직접 구현 | 몬스터 확장 시 구조 변경 없이 노드 조합만 변경 |
| DFS 절차적 맵 생성 | 경로 생성과 콘텐츠 배치 역할 분리 | 매 실행 다른 동선, 배치 코드 수정 없이 대응 |
| 무기 & 스킬 전환 | 캐릭터 아닌 무기 타입 기준으로 전투 결정 | 단일 캐릭터로 다양한 전투 스타일 제공 |
| 공격 적중 기반 버프 | 버프를 캐릭터 상태가 아닌 공격 효과로 정의 | 시전자/피격자 효과 명확히 분리, 확장 용이 |
| 장비 슬롯 스탯 | 출처별 누적 스탯 분리 (statDatas[]) | 버프/장비/레벨업 상호 간섭 없이 독립 갱신 |
| 룰렛 보상 | 결과 선확정 + 연출 분리 | 연출 안정성과 보상 일관성 동시 확보 |

<br>

## 🔧 핵심 시스템 설계

---

<details>
<summary><strong>🤖 1. Custom Behavior Tree 기반 전투 AI</strong></summary>

<br>

### 왜 Behavior Tree를 선택했는가

FSM은 상태 흐름이 단순한 객체에는 적합하지만, Pure Action처럼

- 일반 몬스터와 보스가 공존하고
- 확률 기반 패턴과 조건 분기가 빈번하며
- 행동 우선순위가 중요한 전투 구조

에서는 몬스터 수가 늘어날수록 상태 전이와 조건 분기가 급격히 복잡해질 수 있다고 판단했습니다.  
이에 따라 **행동 우선순위 표현 / 조건 기반 분기 / 패턴 조합**에 강점을 가진 Behavior Tree 구조를 전투 AI 공통 기반으로 채택했습니다.

> Unity 내장 AI 기능이나 외부 플러그인을 사용하지 않고, **Node / Composite / Decorator 구조를 직접 구현**했습니다.

<br>

### 설계 목표

- 일반 몬스터 / 보스 AI 구조 통합
- 몬스터 간 차이는 AI 구조가 아닌 **행동 노드 조합**으로만 표현
- 행동 추가 시 트리 구조 변경 최소화
- 전투 흐름을 코드 구조 자체로 표현

<br>

### 핵심 구현 코드

```csharp
// Selector: 자식 노드 중 성공/진행(Running)하는 첫 노드를 선택
public class SelectorNode : Node
{
    public SelectorNode(List<Node> children) => childrenNode = children;

    public override NodeState Evaluate()
    {
        foreach (var node in childrenNode)
        {
            var result = node.Evaluate();
            if (result != NodeState.Failure)
                return state = result;
        }
        return state = NodeState.Failure;
    }
}

// Decorator: 조건이 참일 때만 자식 노드를 실행 (조건 분기 게이트)
public class DecoratorNode : Node
{
    private readonly Func<bool> condition;
    private readonly Node child;

    public DecoratorNode(Node child, Func<bool> condition)
    {
        this.child = child;
        this.condition = condition;
    }

    public override NodeState Evaluate()
        => condition() ? child.Evaluate() : (state = NodeState.Failure);
}
```

```csharp
// 보스 트리 구성 예시: 탐지 → 추적 → (우선순위 공격 선택) / 패링 조건 분기
public class BossEnemyBT : TreeCtrl
{
    public BossEnemyBT()
    {
        root = new SequenceNode(new List<Node>
        {
            new CheckPlayerInNearNode(thisObject),
            new DecoratorNode(
                new SequenceNode(new List<Node>
                {
                    new GoToPlayerNode(player, thisObject),
                    new SelectorNode(new List<Node>
                    {
                        new DecoratorNode(new BossPunchAttack(player, thisObject),
                            () => Random.Range(0, 2) == 1),
                        new DecoratorNode(new BossGroundAttack(player, thisObject),
                            () => Random.Range(0, 2) == 1),
                        new BossThrowStone(player, thisObject) // fallback
                    })
                }),
                () => GetIsWasParried() == false
            )
        });
    }
}
```

<br>

### 결과

- 일반 몬스터 / 보스 AI 구조를 하나의 BT 기반으로 통합
- 새로운 몬스터 추가 시 **행동 노드 조합만 변경**하면 되어 구조 변경 최소화
- 전투 흐름을 코드 구조 자체로 파악 가능한 가독성 확보

</details>

---

<details>
<summary><strong>🗺️ 2. DFS 기반 절차적 맵 생성</strong></summary>

<br>

### 시스템 개요

DFS 기반으로 경로를 생성하고, 생성된 경로에 맞춰 타일 프리팹을 자동 배치하여  
매번 다른 전투 동선을 만드는 절차적 맵 생성 시스템입니다.

```
DFS 기반 경로 생성
        ↓
타일 타입 및 회전 판정
        ↓
타일 프리팹 배치
        ↓
타일 인덱스 기반 콘텐츠 배치
        ↓
전투 스테이지 완성
```

<br>

### 문제 인식

경로 생성 자체는 빠르게 구현됐지만, 실제 플레이 가능한 맵을 구성하기 위해서는  
타일의 형태와 회전이 정확하게 판정되어 이동 경로가 끊기지 않아야 했습니다.

초기 구현 단계에서 반복적으로 발생한 문제:

1. 코너 타일이 반대로 회전되어 이동 경로가 막히는 현상
2. 직선 / 코너 판정 오류로 타일 형태가 어긋나는 현상
3. 마지막 타일(END)이 진입 방향과 무관하게 회전되어 출구가 막히는 현상

<br>

### 해결 과정

**① 좌표 3점(prev / current / next) 기반 타일 판정**

타일 하나만 기준으로는 방향을 확정할 수 없다고 판단하여,  
이전(prev)과 다음(next) 좌표를 함께 사용해 직선/코너 여부를 판정하는 구조로 변경했습니다.

```csharp
// 1) 직선 판정 - prev/current/next가 동일 축이면 Straight로 판정
if (prevMap.Item1 == currentMap.Item1 && currentMap.Item1 == nextMap.Item1)
{
    GameData.instance.mapData.mapType.Add(MAPTYPE.Straight);
    GameData.instance.mapData.mapRotate.Add(90);
    return StraightGround;
}

// 2) 코너 판정 - 방향 조합(x,z)에 따라 DOWNLEFT 프리팹 + 회전값 매핑
int x = (prevMap.Item1 < currentMap.Item1 || nextMap.Item1 < currentMap.Item1) ? 1 : -1;
int z = (prevMap.Item2 > currentMap.Item2 || nextMap.Item2 > currentMap.Item2) ? 1 : -1;

// 3) 코너 회전 처리
switch (x, z)
{
    case (1, -1):
        GameData.instance.mapData.mapType.Add(MAPTYPE.DOWNLEFT);
        GameData.instance.mapData.mapRotate.Add(0);
        return DownLeftGround;
    case (-1, -1):
        GameData.instance.mapData.mapType.Add(MAPTYPE.DOWNLEFT);
        GameData.instance.mapData.mapRotate.Add(270);
        return DownLeftGround;
    case (1, 1):
        GameData.instance.mapData.mapType.Add(MAPTYPE.DOWNLEFT);
        GameData.instance.mapData.mapRotate.Add(90);
        return DownLeftGround;
    case (-1, 1):
        GameData.instance.mapData.mapType.Add(MAPTYPE.DOWNLEFT);
        GameData.instance.mapData.mapRotate.Add(180);
        return DownLeftGround;
}
```

**② 프리팹 수 증가 없이 회전값 데이터로 해결**

코너 프리팹을 방향별로 여러 개 만드는 방식 대신,  
하나의 코너 프리팹(DOWNLEFT)을 기준으로 회전값(0 / 90 / 180 / 270)을 계산해 적용했습니다.  
불필요한 프리팹 리소스를 기존 대비 절반 수준으로 감소시켰습니다.

**③ END 타일 전용 회전 처리**

END 타일은 prev/current/next 구조를 사용할 수 없기 때문에,  
직전 타일에서 들어오는 방향 벡터(end - prev)를 기준으로 회전값을 별도 처리했습니다.

<br>

### 콘텐츠 배치 시스템 연동

DFS는 경로 생성까지만 담당하고, 콘텐츠 배치는 별도 시스템으로 역할을 분리했습니다.

- DFS로 생성된 타일은 "몇 번째 타일인가"라는 **인덱스 기반 의미**를 가짐
- 런타임에서 인덱스를 기준으로 배치 데이터를 참조해 몬스터 / NPC 자동 배치
- mapType / mapRotate를 데이터로 저장해 **재현 가능한 구조** 확보

```csharp
// 타일 인덱스 기반 콘텐츠 배치 연동
createMap.GetComponent<MapMiddleZone>().ActorSpawn(i - 1);
```

<br>

### 결과

- 매 실행마다 다른 전투 동선 제공
- 이동이 끊기지 않는 안정적인 스테이지 생성 보장
- 맵 구조 변경 시 배치 코드 수정 없이 대응 가능

</details>

---

<details>
<summary><strong>⚔️ 3. 전투 구조 설계 — 무기 & 스킬 전환 + 공격 적중 기반 버프 시스템</strong></summary>

<br>

### 설계 목표

하나의 캐릭터가 상황에 따라 서로 다른 전투 스타일을 선택할 수 있도록  
**무기 타입 전환을 중심으로 설계된 전투 구조**입니다.

- 플레이어의 전투 방식은 캐릭터 자체가 아닌 **장착 중인 무기 타입**에 의해 결정
- 검(Sword) / 지팡이(Staff) 전환 시 공격 방식, 스킬 구성, 전투 리듬이 함께 전환
- 입력 흐름과 전투 판정 구조는 무기와 무관하게 유지

<br>

### 전투 흐름

```
입력 → 무기 타입 확인 → 공격/스킬 실행 → 공격 객체 활성화(Attacker) → 적중 처리
```

모든 공격 판정은 **무기 또는 스킬 오브젝트의 Collider 활성화 / 비활성화**를 기준으로 동작합니다.  
플레이어는 공격을 결정하는 역할만 수행하며, 실제 적중 판정과 효과 전달은 공격 객체가 담당합니다.

<br>

### 무기 전환 구조

```
변경 요소: 활성화되는 무기 GameObject, 무기 타입에 따른 스킬 데이터 및 구성
유지 요소: 입력 방식, 공격 흐름, 전투 판정 구조, 버프 및 스탯 계산 방식
```

```csharp
/* [1] 입력 처리 (PlayerController)
 * - 입력은 Controller에서만 처리
 * - 실제 공격 판정은 애니메이션 / 공격 객체에 위임
 */
void PlayerAttack()
{
    if (!Input.GetMouseButtonDown(0)) return;
    if (EventSystem.current.IsPointerOverGameObject()) return;
    if (player.anim.GetBool("isAttacking")) return;

    player.anim.SetTrigger("Attack");
    player.anim.SetBool("isAttacking", true);

    // 검 무기일 경우 콤보 공격 허용
    if (canCombo && player.GetWeaponType() == "Sword")
        player.anim.SetBool("isReAttack", true);
}

/* [2] 무기 타입 전환 (Player)
 * - 공격 로직 변경 없이 실행되며,
 * - 무기 오브젝트 + 스킬 데이터 교체됨
 */
public void TransWeapon(string weaponName)
{
    if (weaponTypeString == weaponName) return;

    weaponDic[weaponTypeString]?.gameObject.SetActive(false);
    weaponTypeString = weaponName;
    skillManager.SetCurrentWeaponType(weaponName);
    skillManager.WeaponSkillLoad(weaponName);
    weaponDic[weaponName].gameObject.SetActive(true);
}

/* [3] 스킬 실행 구조 (SkillManager)
 * - 무기 타입에 따라 로드된 SkillProcessor를 통해 실행
 * - Controller는 스킬 내부 구현을 알 필요 없음
 */
public void UseSkill(int key)
{
    weaponSkillDic["Skill" + key]();
}
```

<br>

### 공격 적중 기반 버프 시스템

버프는 **캐릭터에 상시 귀속되는 상태가 아닌, 공격 적중 시 전달되는 효과**로 정의했습니다.  
Attacker가 공격 판정뿐만 아니라 **버프 분배의 기준 역할**도 수행합니다.

```
Attacker(공격 객체)
    ↓ 공격 적중
    ├── Owner Buffs (시전자 강화)  →  CharacterBuff 버프 관리  →  Buff LifeCycle (적용/유지/만료/해제)
    └── Target Buffs (피격자 약화) →  CharacterBuff 버프 관리  →  Buff LifeCycle (적용/유지/만료/해제)
```

```csharp
public class Attacker : MonoBehaviour
{
    public Character owner; // 공격 시전자

    // 시전자에게 적용될 버프
    List<BuffBase> ownerBuffs = new List<BuffBase>();
    // 피격자에게 적용될 버프
    List<BuffBase> targetBuffs = new List<BuffBase>();

    void OnTriggerEnter(Collider other)
    {
        Character target = other.GetComponent<Character>();
        if (target == null) return;

        // 시전자에게 적용되는 버프
        for (int i = 0; i < ownerBuffs.Count; i++)
            ownerBuffs[i].ObjectSetup(owner, owner);

        // 피격자에게 적용되는 버프
        for (int i = 0; i < targetBuffs.Count; i++)
            targetBuffs[i].ObjectSetup(target, owner);
    }
}
```

<br>

### 버프/장비/레벨업 기반 최종 능력치 산출

상태 변화는 누적 영역에 기록하고, 최종 스탯은 Getter에서 일괄 계산합니다.

```csharp
/* Character Stat Calculation
 * - Buff는 statDatas[Buff] 영역만 수정
 * - 결과 스탯은 Getter에서 일괄 산출
 */
public class Character : MonoBehaviour
{
    // 기본 스탯 + (버프 / 장비 / 레벨업) 누적 스탯 분리 관리
    public AddStatData[] statDatas = new AddStatData[3];

    [SerializeField] float baseDamage = 10f;

    // 최종 공격력 산출
    public int GetResultDamage()
    {
        float result = baseDamage;
        for (int i = 0; i < statDatas.Length; i++)
            result += statDatas[i].Damage;
        return Mathf.Max(0, (int)result);
    }
}
```

<br>

### 결과

- 하나의 캐릭터로 다양한 전투 스타일 제공
- 공격 / 스킬 / 버프 구조의 일관성 유지
- 신규 버프 추가 시 Attacker 설정만으로 확장 가능
- 전투 시스템 확장 시 코드 수정 범위 최소화

</details>

---

<details>
<summary><strong>🎒 4. 아이템 & 장비 시스템 — 슬롯 기반 누적 스탯 산출</strong></summary>

<br>

### 시스템 개요

슬롯 단위로 아이템을 관리하고, 장비 슬롯에 장착된 스탯의 누적 합을 캐릭터 스탯에 반영하는 구조입니다.

- 버프 / 레벨업 / 장비 등 **출처별 스탯 누적을 명확히 분리** (statDatas[] 배열 인덱스로 구분)
- SlotBase를 중심으로 인벤토리 슬롯 / 장비 슬롯 공통 책임 통합
- CSV 기반 아이템 데이터 로딩으로 런타임 데이터 구성
- 장비 UI 및 캐릭터 UI 갱신은 **이벤트(onTransStatData) 기반**으로 처리해 UI 결합도 최소화

<br>

### 핵심 구현 코드

```csharp
// 장비 아이템 사용(장착) 시 처리
// - 인벤 슬롯이면 장비 슬롯으로 Swap
// - 장비 슬롯에 스탯 저장
// - 장비 누적 스탯 재계산 트리거
public override void UseItem(Character character, SlotBase slot)
{
    base.UseItem(character, slot);
    if (slot == null || ItemUseCheck(character) == false) return;

    player = slot.target.GetComponent<Player>();

    // 인벤토리 슬롯일 경우, 대응되는 장비 슬롯과 교체
    if (slot.GetComponent<EquipmentSlot>() == null)
    {
        slot.SwapItem(
            slot.GetInventory().equipManager
                .equipSlotDic[item.data.equipmentType.ToString()]
        );
    }

    // 장비 슬롯에 스탯 데이터 저장
    slot.GetInventory().equipManager
        .equipSlotDic[item.data.equipmentType.ToString()]
        .equipmentStat = statData;

    // 전체 장비 누적 스탯 재계산
    slot.GetInventory().equipManager.UpdateCharacterStatResult();
}
```

</details>

---

<details>
<summary><strong>🎰 5. 룰렛 보상 시스템 — DOTween 기반 UX 연출 + 인벤토리 연동</strong></summary>

<br>

### 시스템 개요

전투 이후 보상 획득 과정을 단순 클릭이 아닌 **연출 기반 선택 경험**으로 설계했습니다.

<br>

### 핵심 설계 판단

룰렛은 실제 물리 시뮬레이션이 아닌 **게임 연출 요소**로 정의했습니다.

- 결과는 회전 시작 시점에 **선확정**
- 회전 연출은 결과를 보여주기 위한 과정
- OnComplete 이벤트로 연출 종료 시점과 보상 처리를 정확히 연결
- 타이머나 코루틴에 의존하지 않는 구조

<br>

### 핵심 구현 코드

```csharp
/// <summary>
/// 룰렛 실행 시 결과를 선확정하고,
/// 해당 결과가 나오도록 목표 회전값을 설정한다
/// </summary>
void CalResultRoullete()
{
    // 기본 회전 횟수
    roulleteData.targetRotate = 1800;

    int result = Random.Range(0, roulleteData.roulletBlockCount);
    int addRotate = (360 / roulleteData.roulletBlockCount) * result;

    // 경계선 보정 - 칸 경계선에 멈추는 현상 방지
    roulleteData.targetRotate += (360 / roulleteData.roulletBlockCount) / 2;

    // 자연스러운 연출을 위한 랜덤 오프셋
    roulleteData.targetRotate += Random.Range(-15f, 15f);

    // 최종 회전값
    roulleteData.targetRotate += addRotate;

    // 결과 점수 반영
    roulleteData.currentScore += roulleteData.roulletScoreData[result];
}
```

<br>

### 결과

- 전투 이후 보상 UX를 연출 중심 구조로 구성하여 보상 체감 강화
- DOTween을 단순 효과가 아닌 **연출 종료 시점 제어 수단**으로 활용
- 보상이 Inventory에 직접 반영되어 보상 → 아이템 → 성장 시스템이 자연스럽게 연결

</details>
