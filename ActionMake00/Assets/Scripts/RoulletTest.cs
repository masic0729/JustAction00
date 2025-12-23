using DG.Tweening;
using UnityEngine;

public class RoulletTest : MonoBehaviour
{
    private DOTweenAnimation tweenAnim;
    private float targetRotate = 0f;

    [Header("룰렛의 칸 개수")]
    [SerializeField] int roulletBlockCount;

    //룰렛에 존재하는 점수값에 대한 정보
    [SerializeField] int[] roulletScoreData;

    bool isRotating = false;

    void Awake()
    {
        tweenAnim = GetComponent<DOTweenAnimation>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            tweenAnim.DOKill();

            CalResultRoullete();


            tweenAnim.endValueV3= new Vector3(0, 0, targetRotate);
            tweenAnim.CreateTween();
            tweenAnim.DORestart();
            isRotating = true;

        }
        if(isRotating == false)
        {
            transform.Rotate(0, 0, 0.5f);
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
        targetRotate = 1800;


        int result = Random.Range(0, roulletBlockCount);

        //이렇게 해야 해당 값에 맞는 회전 값으로 1차 보정이 된다.
        //룰렛의 경우의 수(칸) 간 회전 요구값 + 랜덤에 의한 곱 = 목표칸과 직전 칸의 경계
        int addRotate = (360 / roulletBlockCount) * result;
        
        Debug.Log(result);

        //해당 값은 그 값으로 가리킬 때, 경계선으로 애매하게 가리키는 일을 막기 위해 설정하는 데이터이다
        targetRotate += (360 / roulletBlockCount) / 2;

        //해당 값의 경우 실제로 돌아가는 듯한 회전값 추가로, 정 중앙에 고정적으로 움직이지 않는 시각적 디테일 부여
        targetRotate += Random.Range(-10f, 10f);

        //최종 회전값 산정
        targetRotate += addRotate;

    }
}
