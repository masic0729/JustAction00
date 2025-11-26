using System;
using UnityEngine;

public enum BuffType
{
    Buff,
    Debuff
}
public abstract class BuffBase
{
    protected Character buffCharacter;
    Character caster;                                   //버프 시전자
    GameObject spawnedParticle = null;
    public BuffType buffType;

    public Action onApply;
    public Action onUpdate;
    public Action onExit;

    float buffTimer;
    float buffTime;

    string particleName;
    string particleParentName;
    protected string iconPath = null;                   //버프 창에 등록될 아이콘 리소스 경로. 각각의 클래스에 적용한다

    bool isActived = false;

    


    public BuffBase(float duration, string spawnParticleName, string spawnParentName, BuffType buffType) 
    {
        
        buffTime = duration;
        particleName = spawnParticleName;
        particleParentName = spawnParentName;
        this.buffType = buffType;
    }

    /// <summary>
    /// 버프류 실행할 때, 만약 버프 이펙트 표시 시 파티클로 보여줄 수 있다.
    /// 하지만 플레이어를 부모로 할 수도 있지만, 특정 파츠에 부모로 설정해 쓸수 있으니
    /// 고려해서 만들고 있음.
    /// 대신 UI 확장까지 하면 코드를 변경할 수도 있음
    /// </summary>
    /// <param name="target">버프 효과를 주려는 대상</param>
    /// <param name="buffCaster">버프를 시전한 캐릭터</param>
    /// <returns></returns>
    public virtual GameObject ObjectSetup(Character target, Character buffCaster)
    {
        buffCharacter = target;
        caster = buffCaster;

        BuffEffectActive();
        Init(buffTime, ApplyBuff, UpdateBuff, ExitBuff);
        buffCharacter.GetComponent<CharacterBuff>().AddBuff(this);
        Debug.Log("버프 등록됨");


        return spawnedParticle;
    }

    /// <summary>
    /// 버프 업데이트의 경우 기본적으로 버프 시간은 초기화가 되는 것은 기본이다.
    /// 하지만 특정 버프 한정으로 
    /// </summary>
    public virtual void BuffUpdate()
    {
        buffTimer = buffTime;
        
        BuffEffectActive();

        Debug.Log("중복 처리됨");
        
    }

    /// <summary>
    /// 버프 이펙트를 소환하는 함수
    /// 버프가 생성되는 것이 아닌 업데이트를 하는 경우
    /// 해당 버프 이펙트의 재생시간을 다시 초기화한다
    /// </summary>
    void BuffEffectActive()
    {
        if (spawnedParticle != null)
        {
            spawnedParticle.GetComponentInChildren<ParticlePoolReleaser>().SetReleaseTime(buffTime);
            return;
        }

        spawnedParticle = PoolManager.instance.Spawn(particleName, buffCharacter.transform.position, buffCharacter.transform.rotation);

        if (particleParentName != null)
        {
            spawnedParticle.transform.parent = buffCharacter.gameObject.transform.Find(particleParentName);
        }
        else
        {
            spawnedParticle.transform.parent = buffCharacter.transform;
        }

        spawnedParticle.GetComponentInChildren<ParticlePoolReleaser>().SetReleaseTime(buffTime);
    }

    /// <summary>
    /// 지속시간 및 각 버프들에 대한 수치값을 object를 통해 추상적으로 받아 쓴다
    /// 각 기능들은 하위 클래스의 함수를 받아 쓰는 것이다.
    /// </summary>
    /// <param name="duration"></param>
    public void Init(float duration, Action apply, Action update, Action exit)
    {
        buffTime = duration;
        buffTimer = buffTime;
        onApply = apply;
        onUpdate = update;
        onExit = exit;
    }

    public void Activate()
    {
        if (isActived == true)
            return;

        isActived = true;
        Init(buffTime, ApplyBuff, UpdateBuff, ExitBuff);

        onApply?.Invoke();   // 부가 훅

        
    }

    public void Deactivate()
    {

        onExit?.Invoke();    // 부가 훅
        isActived = false;
    }

    public bool UpdateTime()
    {

        buffTimer -= Time.deltaTime;
        onUpdate?.Invoke();

        if (buffTimer <= 0f)
        {
            Deactivate();
            return true;     // 만료됨
        }
        return false;
    }

    public float GetBuffTimePercent()
    {
        return (buffTime > 0f) ? (buffTimer / buffTime) : 0f;
    }

    protected abstract void ApplyBuff();
    protected abstract void UpdateBuff();
    protected abstract void ExitBuff();
    public Character GetCaster() => caster;
}
