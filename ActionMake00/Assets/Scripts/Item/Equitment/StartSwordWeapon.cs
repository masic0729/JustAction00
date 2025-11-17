using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSwordWeapon : MonoBehaviour
{
    [SerializeField] EquipmentManager equipmentManager;
    public EquipRoot SwordWeaponBasic;
    [SerializeField] Player player;

    private void Awake()
    {
        Init();

    }

    // Start is called before the first frame update
    void Start()
    {
    }


    void Init()
    {
        SlotBase slot = GetComponent<EquipmentSlot>();
        slot.AddItem(SwordWeaponBasic);
        //slot.OnItemUse?.Invoke(player, slot);
        player.WeaponAwakeInit(SwordWeaponBasic.WeaponEquipment);
        equipmentManager.gameObject.SetActive(false);
    }

    
}
