using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class RoulletUI : MonoBehaviour
{
    RoulletPlayData roulletData;
    RoulletPlaying roullet;
    RoulletItem roulletItem;
    DOTweenAnimation scoreTextAnim;

    public Text scoreText;
    public Text remainPlayCountText;
    public Slider scoreSlider;

    

    bool isAnimationEnd = true;                 //룰렛 결과 UI연출이 끝났는 지 확인하는 변수.해당 애니메이션은 필요없을 수 있음

    private void Start()
    {
        scoreTextAnim = scoreText.GetComponent<DOTweenAnimation>();
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
        isAnimationEnd = false;

        scoreText.text = targetScore.ToString();
        scoreSlider.value = (float)targetScore / roulletData.maxScore;

        isAnimationEnd = true;

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
}
