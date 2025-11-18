using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSwordWeapon : MonoBehaviour
{
    [SerializeField] EquipmentManager equipmentManager;
    public EquipRoot SwordWeaponBasic;
    [SerializeField] Player player;
    public SlotBase slot;

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
        slot.AddItem(SwordWeaponBasic);
        //slot.OnItemUse?.Invoke(player, slot);
        player.WeaponAwakeInit(SwordWeaponBasic.WeaponEquipment);
        equipmentManager.gameObject.SetActive(false);
    }

    
}
