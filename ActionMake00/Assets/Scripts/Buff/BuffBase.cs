using System;
using UnityEngine;

public enum BuffType
{
    Buff,
    Debuff
}
public abstract class BuffBase
{
    protected Character buffCharacter;                      //
    public BuffType buffType;

    public Action onApply;
    public Action onUpdate;
    public Action onExit;

    float buffTimer;
    float buffTime;

    string particleName;
    string particleParentName;
    bool isActived = false;
    BuffBase buffData;                                  //같은 버프형태인 지 확인하는 데이터
    Character caster;                                   //버프 시전자


    public BuffBase(float duration, string spawnParticleName, string spawnParentName, BuffType buffType) 
    {
        buffTime = duration;
        particleName = spawnParticleName;
        particleParentName = spawnParentName;
        this.buffType = buffType;
    }

    /// <summary>
    /// 버프류 실행할 때, 만약 버프 이펙트 표시 시 파티클로 보여줄 수 있다.
    /// 하지만 플레이어를 부모로 할 수도 있지만, 특정 파츠에 부모로 설정해 쓸수 있으니
    /// 고려해서 만들고 있음.
    /// 대신 UI 확장까지 하면 코드를 변경할 수도 있음
    /// </summary>
    /// <param name="target">버프 효과를 주려는 대상</param>
    /// <param name="buffCaster">버프를 시전한 캐릭터</param>
    /// <returns></returns>
    public virtual GameObject ObjectSetup(Character target, Character buffCaster)
    {
        buffCharacter = target;
        buffData = this;
        caster = buffCaster;

        GameObject instance = PoolManager.instance.Spawn(particleName, target.transform.position, target.transform.rotation);

        if(particleParentName != null)
        {
            instance.transform.parent = target.gameObject.transform.Find(particleParentName);
        }
        else
        {
            instance.transform.parent = target.transform;
        }

        instance.GetComponentInChildren<ParticlePoolReleaser>().SetReleaseTime(buffTime);
        Init(buffTime, ApplyBuff, UpdateBuff, ExitBuff);
        buffCharacter.GetComponent<CharacterBuff>().AddBuff(this);


        return instance;
    }

    /// <summary>
    /// 버프 업데이트의 경우 기본적으로 버프 시간은 초기화가 되는 것은 기본이다.
    /// 하지만 특정 버프 한정으로 
    /// </summary>
    public virtual void BuffUpdate()
    {
        buffTimer = buffTime;
        
    }

    /// <summary>
    /// 이하 동일
    /// </summary>
    /// <param name="target"></param>
    /// <param name="dmgAmount"></param>
    /// <param name="duration"></param>
    public virtual void ObjectSetup(Character target, float duration, BuffType buffType)
    {
        buffCharacter = target;
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

    public BuffBase GetBuffData() => buffData;
    public Character GetCaster() => caster;
}
