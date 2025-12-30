using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_PlayerInput : MonoBehaviour
{
    public static GUI_PlayerInput instance;

    private PlayerController playerCtrl;
    [SerializeField] GameObject UI_View;
    public Inventory inventory;
    public EquipmentManager equip;

    public GameObject NPC_InventoryView;

    //public GameObject testItem;
    public ItemObject testitem1;
    public ItemObject testitem2;
    public ItemObject testitem3;

    //어떠한 창이 이미 활성화중인 지 확인하는 데이터. 다시 말해 여러 창을 동시에 활성화되는 방식은 아니다
    bool isShowing = false;        
    
    void Awake()
    {
        instance = this;
    }

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
            EnableUI(UI_View.gameObject);
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            EnableUI(UI_View.gameObject);
        }

        if(Input.GetKeyDown(KeyCode.U))
        {
            TestInputItem1();
        }

        if(Input.GetKeyDown(KeyCode.Y))
        {
            TestInputItem2();
            TestInputItem3();
        }
        
        /*if(Input.GetKeyDown(KeyCode.F))
        {
            EnableUI(NPC_InventoryView);
        }*/

        if (Input.GetKeyDown(KeyCode.T))
        {
            inventory.SortInventoryTest();
        }
    }
    
    /// <summary>
    /// 원하는 오브젝트의 활성화 여부를 확인하여 상태를 항상 반대로 전환한다
    /// </summary>
    /// <param name="target">활성화/비활성화 하려는 오브젝트 대상</param>
    public void EnableUI(GameObject target)
    {
        /*CameraController cameraCtrl = GetComponent<CameraController>();*/
        //어떠한 창이 호라성화되지 않은 채로 창 활성화를 시도해야 할 수 있다.
        if (target.activeSelf == false && isShowing == false)
        {
            playerCtrl.SetCanAnyInput(false);
            target.SetActive(true);
            MouseControl.instance.Apply(MouseControl.AimCursorMode.ConfinedWindow);

            /*cameraCtrl.SetCanRotate(false);*/
            isShowing = true;
        }
        else if(target.activeSelf == true)
        {
            //해당 창이 활성화중일 때만 해당 창 비활성화

            playerCtrl.SetCanAnyInput(true);
            target.SetActive(false);
            MouseControl.instance.Apply(MouseControl.AimCursorMode.LockedCenter);
            isShowing = false;
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

    void TestInputItem3()
    {
        inventory.AddItemInList(testitem3);
    }
}
