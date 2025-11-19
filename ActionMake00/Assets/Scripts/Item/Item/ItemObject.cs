using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public abstract class ItemObject : MonoBehaviour, ItemInteration, ItemUseChecker
{
    public int itemID;
    public ItemBase item;
    protected string itemComment;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //item.OnCheckUse += CheckUseItem;
        /*item.OnItemUse += UseItem;
        item.OnItemUpdate += UpdateInventory;*/
    }


    /// <summary>
    /// 아이템 기능 실행은 슬롯 내 액션에 할당도었기에,
    /// 이 스크립트에 관리되는 방식은 아니다.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="slot"></param>
    public virtual void UseItem(Character character, SlotBase slot)
    {

        UpdateInventory(slot);
    }



    abstract public void UpdateInventory(SlotBase slot);


    public abstract bool ItemUseCheck(Character character);
    public string GetItemComment()
    {
        item.comment = SetItemComment();
        return SetItemComment();
    }

    public abstract string SetItemComment();

    protected void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            Debug.Log("아이템 획득" + other.transform.name);
            Inventory playerInventory = other.GetComponent<GUI_PlayerInput>().inventory;
            AddITemInInventory(playerInventory, this);
        }
    }

    protected void AddITemInInventory(Inventory inven, ItemObject itemObject)
    {

        if (inven.AddItemInList(itemObject) == true)
        {
            Destroy(this.gameObject);
        }

    }
}
