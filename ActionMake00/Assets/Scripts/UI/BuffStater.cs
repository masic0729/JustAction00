using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffStater : MonoBehaviour
{
    Slider slider;
    Image sliderBackGround;
    float currentTimer = 0f;
    float buffTime = -1f;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        float remainTime = buffTime - currentTimer;
        slider.value = remainTime / buffTime;
    }

    /// <summary>
    /// 해당 버프 UI 생성 시 기본 설정을 한다
    /// 버프시간의 경우 시작 부터 풀이기 때문에 둘 다 최대 버프 시간을 부여받는다
    /// </summary>
    /// <param name="buffIconPath">버프 아이콘 경로</param>
    /// <param name="buffMaxTime">버프의 최대 시간</param>
    public void InitBuffData(string buffIconPath, float buffMaxTime)
    {
        currentTimer = buffMaxTime;
        buffTime = buffMaxTime;

        Sprite instanceSprite = Resources.Load<Sprite>(buffIconPath);
        if(instanceSprite != null)
        {
            sliderBackGround = GetComponentInChildren<Image>();
            sliderBackGround.sprite = Resources.Load<Sprite>(buffIconPath);
        }
        else
        {
            Debug.Log("아이콘 경로 알 수 없음. 경로 : " + buffIconPath);
        }
    }

    public void SetCurrentTimer(float timer) => currentTimer = timer;
}
