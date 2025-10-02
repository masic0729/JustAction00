using System;
using UnityEngine;

public abstract class BuffBase : MonoBehaviour
{
    protected Character character;                      // 캐시

    public Action onApply;
    public Action onUpdate;

    public Action onExit;

    float buffTimer;
    float buffTime;


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

        onApply?.Invoke();   // 부가 훅
    }

    public void Deactivate()
    {

        onExit?.Invoke();    // 부가 훅
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
}
