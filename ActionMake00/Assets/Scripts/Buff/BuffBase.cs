using System;
using UnityEngine;


public sealed class BuffBase
{
    public string buffName;          // 디버깅용(선택)
    public float remainTime;         // 남은 시간
    public float updateInterval;   // 0이면 틱 없음

    // 내부 상태
    float _tickTimer;
    bool _active = false;

    // 콜백(액션형)
    public Action onApply;                 // 등록 시 1회
    public Action onExit;                  // 만료/해제 시 1회(멱등)
    public Action onUpdate;                // tickInterval마다

    public BuffBase(float duration, float tick = 0f)
    {
        remainTime = duration;
        updateInterval = tick;
        _tickTimer = tick > 0f ? tick : float.PositiveInfinity;
    }

    // 컨테이너가 호출
    public void Activate()
    {
        if (_active) return;
        _active = true;
        onApply?.Invoke();
    }

    // 컨테이너가 호출
    public void Deactivate()
    {
        if (!_active) return;
        _active = false;
        onExit?.Invoke();
    }

    // 컨테이너가 매 프레임 호출
    public bool UpdateTime(float dt)
    {
        remainTime -= dt;
        if (updateInterval > 0f)
        {
            _tickTimer -= dt;
            while (_tickTimer <= 0f)
            {
                _tickTimer += updateInterval;
                onUpdate?.Invoke();
            }
        }
        // true = 만료
        return remainTime <= 0f;
    }
}
