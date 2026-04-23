

public class PlayerCommonSwordWeapon : EquipWeapon
{
    public override string SetItemComment()
    {
        return $"평범한 한손검이다. 공격력 <color=#ff5555>{equipmentStat.Damage}</color> 상승한다";
    }
}
