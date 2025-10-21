
/// <summary>
/// 캐릭터간 기본 상호작용. 대표적으로 데미지, 사망이 존재한다
/// </summary>
public interface ICharacterDamageable
{
    
    abstract void TakeDamage(float amount, int hitLevel = 0);
    abstract void Dead(float animationTime);
}

public interface IAttacker
{
    abstract float GetDamage();
    abstract void SetDamage(float value);
}

public interface ItemInteration
{
    void UseItem(Character character);
    void UpdateInventory(Character character);
}
