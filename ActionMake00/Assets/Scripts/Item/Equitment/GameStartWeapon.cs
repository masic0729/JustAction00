using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartWeapon : MonoBehaviour
{
    public EquipRoot StartWeapon;

    private void Awake()
    {
        GetComponent<EquipmentSlot>().AddItem(StartWeapon);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

}
