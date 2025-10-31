using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public EquipmentSlot[] equipSlots;
    //안쓸 수도 있음
    public Dictionary<string, EquipmentSlot> equipSlotDic = 
        new Dictionary<string, EquipmentSlot>();
    
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        //init dictionary
        for(int i = 0; i < equipSlots.Length; i++)
        {
            equipSlotDic[equipSlots[i].equipmentType.ToString()] = equipSlots[i];
        }
    }
}
