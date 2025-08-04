
/// <summary>
/// 캐릭터간 기본 상호작용. 대표적으로 데미지, 사망이 존재한다
/// </summary>
public interface ICharacterDamageable
{
    
    abstract void TakeDamage(int amount);
    abstract void Dead(float animationTime);
}

public interface IWeapon
{
    abstract int GetDamage();
    abstract void SetDamage(int value);
}