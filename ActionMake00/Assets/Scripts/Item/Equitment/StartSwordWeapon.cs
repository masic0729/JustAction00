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
        GetComponent<EquipmentSlot>().AddItem(SwordWeaponBasic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
