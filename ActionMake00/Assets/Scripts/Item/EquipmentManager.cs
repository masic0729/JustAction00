using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] ItemSlot[] equipmentSlots;
    Dictionary<string, ItemSlot> equipmentSlotsDic = new Dictionary<string, ItemSlot>();
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Init()
    {
        for(int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlotsDic[equipmentSlots[i].gameObject.name] = equipmentSlots[i];
        }
    }


}
