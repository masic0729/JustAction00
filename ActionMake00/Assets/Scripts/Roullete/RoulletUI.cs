using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class RoulletUI : MonoBehaviour
{
    RoulletPlayData roulletData;
    DOTweenAnimation scoreTextAnim;

    public Text scoreText;
    public Text remainPlayCountText;
    public Slider scoreSlider;

    private void Start()
    {
        scoreTextAnim = scoreText.GetComponent<DOTweenAnimation>();
        roulletData = GetComponent<RoulletPlayData>();

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
        scoreText.text = targetScore.ToString();
        scoreSlider.value = (float)targetScore / roulletData.maxScore;
    }

    public void SetRemainPlayCount(int count)
    {

        remainPlayCountText.text = count.ToString();
    }
}
