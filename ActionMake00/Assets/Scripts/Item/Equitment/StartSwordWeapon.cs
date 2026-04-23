using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
public class StartSwordWeapon : MonoBehaviour
{
    [SerializeField] EquipmentManager equipmentManager;
    public EquipWeapon SwordWeaponBasic;
    [SerializeField] Player player;
    public EquipmentSlot slot;

    private void Awake()
    {
        

    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }


    void Init()
    {
        EquipWeapon instanceWeapon = SwordWeaponBasic;
        instanceWeapon.SetItemData(instanceWeapon.itemId);
        slot.AddItem(SwordWeaponBasic);
        slot.currentItem.OnItemUse?.Invoke(player, slot);

        slot.GetInventory().equipManager.equipSlotDic[slot.currentItem.data.equipmentType.ToString()].equipmentStat = SwordWeaponBasic.equipmentStat;
        slot.GetInventory().equipManager.UpdateCharacterStatResult();

        player.WeaponAwakeInit(SwordWeaponBasic.WeaponEquipment);
    }

    
}
