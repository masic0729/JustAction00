using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipWeapon : EquipRoot
{
    public PlayerWeapon WeaponEquipment;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void UseItem(Character character, SlotBase slot)
    {
        base.UseItem(character, slot);
        player.WeaponInit(WeaponEquipment);
    }
}
