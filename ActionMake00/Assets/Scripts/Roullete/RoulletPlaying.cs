using DG.Tweening;
using UnityEngine;

public class RoulletPlaying : MonoBehaviour
{

    //룰렛에 관한 데이터들
    RoulletPlayData roulleteData;
    RoulletUI roulletUI;

    DOTweenAnimation roulletAnim;
    [SerializeField] Inventory playerInventory;

    [SerializeField] GameObject PlayButton;

    bool isRotating = false;
    bool isEnd = false;                                                     //잔여 룰렛 회전 횟수가 끝났다면

    private void Start()
    {
        Init();
    }

    void Update()
    {
        //PlayRoullet();

        //룰렛 회전 중이 아니라면 조금씩 회전할 것
        if (isRotating == false)
        {
            transform.Rotate(0, 0, 0.1f);
        }
    }

    void Init()
    {
        roulletAnim = GetComponent<DOTweenAnimation>();
        roulletUI = GetComponent<RoulletUI>();
        roulleteData = GetComponent<RoulletPlayData>();
    }

    public void PlayRoullet()
    {
        //룰렛 작동 시 잔여 횟수 차감 및 결과도출을 즉각적으로 실행하며,
        //이후 룰렛 연출로 결과를 보여준다
        //하지만, 플레이어의 인벤토리 공간이 최소 1칸이라도 있어야 실행할 자격이 있다(구현 요구)
        if (/*Input.GetKeyDown(KeyCode.F1) && */isEnd == false && isRotating == false)
        {
            PlayButton.SetActive(false);

            roulleteData.remainPlayCount--;
            roulletUI.SetRemainPlayCount(roulleteData.remainPlayCount);
            roulletAnim.DOKill();

            CalResultRoullete();


            roulletAnim.endValueV3 = new Vector3(0, 0, roulleteData.targetRotate);
            roulletAnim.CreateTween();
            roulletAnim.DORestart();
            isRotating = true;
        }
    }

    /// <summary>
    /// 룰렛을 돌리기 시작할 때, 미리 결과를 정하고,
    /// 이에 맞는 결과가 나오도록 회전값을 설정한다
    /// </summary>
    void CalResultRoullete()
    {
        //GetComponent<RectTransform>().rotation = ;
        //기본적으로 룰렛은 5번 회전한다
        roulleteData.targetRotate = 1800;


        int result = Random.Range(0, roulleteData.roulletBlockCount);

        //이렇게 해야 해당 값에 맞는 회전 값으로 1차 보정이 된다.
        //룰렛의 경우의 수(칸) 간 회전 요구값 + 랜덤에 의한 곱 = 목표칸과 직전 칸의 경계
        int addRotate = (360 / roulleteData.roulletBlockCount) * result;

        Debug.Log(result);

        //해당 값은 그 값으로 가리킬 때, 경계선으로 애매하게 가리키는 일을 막기 위해 설정하는 데이터이다
        roulleteData.targetRotate += (360 / roulleteData.roulletBlockCount) / 2;

        //해당 값의 경우 실제로 돌아가는 듯한 회전값 추가로, 정 중앙에 고정적으로 움직이지 않는 시각적 디테일 부여
        roulleteData.targetRotate += Random.Range(-15f, 15f);

        //최종 회전값 산정
        roulleteData.targetRotate += addRotate;

        //룰렛 실행에 의한 점수를 데이터에 저장
        roulleteData.currentScore += roulleteData.roulletScoreData[result];
    }

    /// <summary>
    /// 회전이 끝났기에
    /// UI반영을 한다.
    /// 이때 scoreText의 경우 애니메이션으로 값이 오른다.
    /// 또한 다시 룰렛을 회전할 수 있도록 설정한다.
    /// </summary>
    public void RotateEnd()
    {
        //잔여 횟수가 존재하면 회전 가능상태로 전환, 미존재 시 종료 및 보상 지급(구현 예정)
        if (roulleteData.remainPlayCount != 0)
        {
            isRotating = false;
            PlayButton.SetActive(true);
        }
        else
        {
            isEnd = true;
        }

        roulletUI.PlayAnimUI(roulleteData.currentScore);
    }

    public Inventory GetPlayerInventory() => playerInventory;

    public bool GetIsEnd() => isEnd;
}
