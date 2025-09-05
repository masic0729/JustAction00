using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_PlayerInput : MonoBehaviour
{
    public Inventory inventory;
    //public GameObject testItem;
    public Item testitem;



    // Update is called once per frame
    void Update()
    {
        InputKey();
    }

    void InputKey()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            EnableGameObject(inventory.gameObject);
            
        }
        if(Input.GetKeyDown(KeyCode.U))
        {
            TestInputItem();
        }
    }
    
    /// <summary>
    /// 원하는 오브젝트의 활성화 여부를 확인하여 상태를 항상 반대로 전환한다
    /// </summary>
    /// <param name="target">활성화/비활성화 하려는 오브젝트 대상</param>
    void EnableGameObject(GameObject target)
    {
        if (target.activeSelf == false)
        {
            target.SetActive(true);
        }
        else if(target.activeSelf == true)
        {
            target.SetActive(false);
        }
    }

    void TestInputItem()
    {
        inventory.AddItemInList(testitem);
    }
}
