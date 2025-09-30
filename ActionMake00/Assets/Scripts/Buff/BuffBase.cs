using System;
using UnityEngine;
using UnityEngine.Timeline;


public class BuffBase : MonoBehaviour
{
    public string buffName;          // 디버깅용(선택)
    public float remainTime;         // 지속 시간 값

    // 내부 상태
    float buffTimer;                //버프 타이머
    //bool _active = false;

    // 콜백(액션형)
    public Action onApply;                 // 등록 시 1회
    public Action onExit;                  // 만료/해제 시 1회(멱등)
    public Action onUpdate;                // UpdateTime마다 실행

    public BuffBase(float duration, float tick = 10f)
    {
        remainTime = duration;
        buffTimer = tick;
    }

    // 컨테이너가 호출
    public void Activate()
    {
        onApply?.Invoke();
    }

    // 컨테이너가 호출
    public void Deactivate()
    {
        onExit?.Invoke();
    }

    // 컨테이너가 매 프레임 호출
    public void UpdateTime(float dt)
    {
        buffTimer += Time.deltaTime;
        onUpdate?.Invoke();
        Deactivate();

    }
}
