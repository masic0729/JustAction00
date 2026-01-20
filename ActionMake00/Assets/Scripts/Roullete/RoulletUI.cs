using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class RoulletUI : MonoBehaviour
{
    RoulletPlayData roulletData;
    RoulletPlaying roullet;
    RoulletItem roulletItem;

    //DOTween 또는 이펙트 관련 데이터
    [SerializeField] DOTweenAnimation tweenText;                    //룰렛 회전이 끝날 때 실행되는 텍스트
    [SerializeField] DOTweenAnimation tweenBar;                    //룰렛 회전이 끝날 때 실행되는 게이지
    [SerializeField] ParticleSystem scoreTextEffect;                                         //점수값이 변경될 때마다 실행할 이펙트

    public Text scoreText;
    public Text remainPlayCountText;
    public Image scoreSlider;
    [HideInInspector] public Sprite resultItemIcon;                    //플레이어가 아이템을 받으려는 아이템 아이콘
    public GameObject resultPanel;                                      //룰렛을 다 돌린 뒤 획득하려는 아이템을 보여주는 창
    public Image resultImage;                                             //룰렛 결과의 이미지




    private void Start()
    {
        roulletData = GetComponent<RoulletPlayData>();
        roullet = GetComponent<RoulletPlaying>();
        roulletItem = GetComponent<RoulletItem>();
        remainPlayCountText.text = roulletData.remainPlayCount.ToString();
    }

    /// <summary>
    /// 텍스트는 룰렛 회전에 의한 현재 점수로 값이 상승한다
    /// 슬라이더 역시 최대 점수와 현재 점수를 고려하여
    /// 수치값을 업데이트를 한다.
    /// 이때 두 UI는 애니메이션 형태로 재생되는 것을 목표로 할 예정
    /// </summary>
    /// <param name="targetScore"></param>
    public void PlayAnimUI(int targetScore)
    {
        //룰렛 결과를 데이터에 반영
        scoreText.text = targetScore.ToString();

        float currentBarValue = (float)targetScore / roulletData.maxScore;
        //scoreSlider.fillAmount = currentBarValue;

        //트위
        tweenBar.endValueFloat = currentBarValue;

        tweenBar.DOKill();

        tweenBar.endValueFloat = currentBarValue;
        tweenBar.RecreateTween();

        //이후 애니메이션 실행.
        tweenBar.DORestart();
        tweenText.DORestart();
        scoreTextEffect.Play();

        //지금은 여기에 있지만, 트윈 애니메이션의 컴플릿에 실행해야함
        CheckCanGive();
    }

    /// <summary>
    /// 룰렛이 끝났다면 플레이어에게 아이템을 제공하는 기능 수행
    /// </summary>
    void CheckCanGive()
    {
        if (roullet.GetIsEnd() == true)
        {
            roulletItem.GiveItemToPlayer(roulletData.currentItemResultIndex);
            //roulletData.currentItemResultIndex++;
        }
    }

    public void SetRemainPlayCount(int count)
    {
        remainPlayCountText.text = count.ToString();
    }

    public void ShowResultPanel(Sprite itemIcon)
    {
        resultImage.sprite = itemIcon;

        //이곳에 아이템 아이콘 할당 및 공개
        resultPanel.SetActive(true);
    }
}
