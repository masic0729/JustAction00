using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSwordWeapon : MonoBehaviour
{
    public EquipRoot SwordWeaponBasic;
    [SerializeField] Player player;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }


    void Init()
    {
        SlotBase slot = GetComponent<EquipmentSlot>();
        //slot.AddItem(SwordWeaponBasic);
        slot.OnItemUse?.Invoke(player, slot);
        player.WeaponInit(SwordWeaponBasic.WeaponEquipment);
    }

    
}
