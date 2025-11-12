using System;
using UnityEngine;

public enum BuffType
{
    Buff,
    Debuff
}
public abstract class BuffBase
{
    protected Character character;                      // 캐시
    public BuffType buffType;

    public Action onApply;
    public Action onUpdate;

    public Action onExit;

    float buffTimer;
    float buffTime;
    bool isActived = false;

    /// <summary>
    /// 버프류 실행할 때, 만약 버프 이펙트 표시 시 파티클로 보여줄 수 있다.
    /// 하지만 플레이어를 부모로 할 수도 있지만, 특정 파츠에 부모로 설정해 쓸수 있으니
    /// 고려해서 만들고 있음.
    /// 대신 UI 확장까지 하면 코드를 변경할 수도 있음
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="dmgAmount"></param>
    /// <param name="duration">기본적인 버프 지속시간 및 버프 관련 파티클 유지 시간</param>
    /// <param name="spawnParticleName"></param>
    /// <param name="spawnParentName"></param>
    public virtual GameObject ObjectSetup(Character target, float duration, string spawnParticleName, string spawnParentName)
    {
        character = target;
        buffTime = duration;


        GameObject instance = PoolManager.instance.Spawn(spawnParticleName, target.transform.position, target.transform.rotation);

        if(spawnParentName != null)
        {
            instance.transform.parent = target.gameObject.transform.Find(spawnParentName);
        }
        else
        {
            instance.transform.parent = target.transform;
        }

        instance.GetComponentInChildren<ParticlePoolReleaser>().SetReleaseTime(duration);
        Init(duration, ApplyBuff, UpdateBuff, ExitBuff);
        character.GetComponent<CharacterBuff>().AddBuff(this);


        return instance;
    }

    /// <summary>
    /// 이하 동일
    /// </summary>
    /// <param name="target"></param>
    /// <param name="dmgAmount"></param>
    /// <param name="duration"></param>
    public virtual void ObjectSetup(Character target, float duration)
    {
        character = target;
        buffTime = duration;

        Init(duration, ApplyBuff, UpdateBuff, ExitBuff);
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
}
