using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_PlayerInput : MonoBehaviour
{
    private PlayerController playerCtrl;
    public Inventory inventory;
    //public GameObject testItem;
    public Item testitem1;
    public Item testitem2;

    private void Start()
    {
        playerCtrl = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        InputKey();
    }

    void InputKey()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            EnableUI(inventory.gameObject);
            
        }
        if(Input.GetKeyDown(KeyCode.U))
        {
            TestInputItem1();
        }
        if(Input.GetKeyDown(KeyCode.Y))
        {
            TestInputItem2();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            inventory.SortInventoryTest();
        }
    }
    
    /// <summary>
    /// 원하는 오브젝트의 활성화 여부를 확인하여 상태를 항상 반대로 전환한다
    /// </summary>
    /// <param name="target">활성화/비활성화 하려는 오브젝트 대상</param>
    void EnableUI(GameObject target)
    {
        MouseControl mouseCtrl = GetComponent<MouseControl>();
        /*CameraController cameraCtrl = GetComponent<CameraController>();*/
        if (target.activeSelf == false)
        {
            //playerCtrl.SetCanAnyInput(false);
            target.SetActive(true);
            mouseCtrl.Apply(MouseControl.AimCursorMode.Free);
            /*cameraCtrl.SetCanRotate(false);*/

        }
        else if(target.activeSelf == true)
        {
            //playerCtrl.SetCanAnyInput(true);
            target.SetActive(false);
            mouseCtrl.Apply(MouseControl.AimCursorMode.LockedCenter);
            /*cameraCtrl.SetCanRotate(true);*/
        }
    }

    void TestInputItem1()
    {
        inventory.AddItemInList(testitem1);
    }
    
    void TestInputItem2()
    {
        inventory.AddItemInList(testitem2);
    }
}
