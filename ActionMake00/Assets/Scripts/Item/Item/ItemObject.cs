using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public abstract class ItemObject : MonoBehaviour, ItemInteration, ItemUseChecker
{
    public int itemId;                                  //csv �� �����ϴ� id�� ��ġ�ϱ� ���� ��

    public ItemBase item;
    protected string itemComment;


    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //collider = GetComponent<Collider>();
        //collider.enabled = false;

        Invoke("EnableCol", 1.5f);
    }

    protected virtual void Update()
    {
        transform.Rotate(0, 60f * Time.deltaTime, 0);
    }

    void EnableCol()
    {
        GetComponent<Collider>().enabled = true;
    }



    /// <summary>
    /// ������ ��� ������ ���� �� �׼ǿ� �Ҵ絵���⿡,
    /// �� ��ũ��Ʈ�� �����Ǵ� ����� �ƴϴ�.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="slot"></param>
    public virtual void UseItem(Character character, SlotBase slot)
    {
        SetItemData(itemId);

        UpdateInventory(slot);
    }



    abstract public void UpdateInventory(SlotBase slot);


    public abstract bool ItemUseCheck(Character character);
    public string GetItemComment()
    {
        //item.comment = SetItemComment();
        return SetItemComment();
    }

    public abstract string SetItemComment();

    protected void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            Debug.Log("������ ȹ��" + other.transform.name);
            Inventory playerInventory = other.GetComponent<GUI_PlayerInput>().inventory;
            AddItemInInventory(playerInventory, this);
            other.GetComponent<Player>().PlayItemSound();
        }
    }

    protected void AddItemInInventory(Inventory inven, ItemObject itemObject)
    {

        if (inven.AddItemInList(itemObject) == true)
        {
            Destroy(this.gameObject);
        }

    }

    /// <summary>
    /// csv�� �����͸� �ҷ��´�.
    /// id�� ������� �ҷ�����, �ش� �������� ��������
    /// ��θ� ���� �ҷ��ͼ� �����Ѵ�
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="iconBasePath"></param>
    /*public void SetItemData(int itemId)
    {
        ItemData instanceItemData = null;

        // CSV ��ġ: Assets/Resources/Jsons/ItemData.CS
        string path = Application.dataPath + "/Resources/Jsons/ItemData_Template.CSV";

        using (StreamReader reader = new StreamReader(path))
        {
            bool isFirstLine = true; // ��� ��ŵ��

            while (true)
            {
                string line = reader.ReadLine();

                // �� �̻� ���� ���� ������ ����
                if (line == null)
                    break;

                // �� ���� ��ŵ
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // ù ���� ������ �����ϰ� ��ŵ
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                // CSV: itemName,iconKey,itemType,equipmentType,maxCount,ID
                string[] split = line.Split(',');
                if (split.Length < 6)
                {
                    Debug.LogWarning($"[SetItemData] �߸��� ����: {line}");
                    continue;
                }

                // ������ �÷��� ID
                if (!int.TryParse(split[5], out int id))
                {
                    Debug.LogWarning($"[SetItemData] ID �Ľ� ����: {split[5]}");
                    continue;
                }

                // ���ϴ� ID�� �ƴϸ� �н�
                if (id != itemId)
                    continue;

                // ���� �����ϸ�: ã�� ���� ID�� ���̴�

                // ��Ÿ�ӿ� ItemData �ν��Ͻ� ����
                instanceItemData = ScriptableObject.CreateInstance<ItemData>();

                // �⺻ ���� ����
                instanceItemData.itemName = split[0];

                // itemType �Ľ�
                if (!Enum.TryParse(split[2], true, out ItemType itemType))
                    itemType = ItemType.nullItem;
                instanceItemData.itemType = itemType;

                // equipmentType �Ľ�
                if (!Enum.TryParse(split[3], true, out EquipmentType equipType))
                    equipType = EquipmentType.None;
                instanceItemData.equipmentType = equipType;

                // maxCount �Ľ�
                if (!int.TryParse(split[4], out int parsedMaxCount))
                    parsedMaxCount = 0;
                instanceItemData.maxCount = parsedMaxCount;

                // 3) ������ �ε�
                string iconKey = split[1].Trim(); // CSV�� iconKey �� ��ü (��: "Resources\\JsonImageData\\03_Alchemy")

                // ��������(\)�� ������(/)�� ����
                string normalized = iconKey.Replace("\\", "/");

                // �տ� "Resources/"�� �پ� ������ ����
                if (normalized.StartsWith("Resources/"))
                {
                    normalized = normalized.Substring("Resources/".Length);
                }

                // ���������� Resources.Load�� �ѱ� ���
                // ��: "JsonImageData/03_Alchemy"
                string iconPath = normalized;

                Sprite icon = Resources.Load<Sprite>(iconPath);
                if (icon == null)
                {
                    Debug.LogWarning($"[SetItemData] ������ �ε� ����: {iconPath}");
                }
                instanceItemData.icon = icon;

                // ���� ���� �ʵ嵵 �״�� ����
                *//*this.type = instanceItemData.itemType;
                this.maxCount = instanceItemData.maxCount;

                if (slotIcon != null && instanceItemData.icon != null)
                {
                    slotIcon.sprite = instanceItemData.icon;
                    slotIcon.enabled = true;
                    slotIcon.color = Color.white;
                }*//*

                // ã������ �� ���� �ʿ� ������ �ٷ� ����
                break;
            }
        }

        item.data = instanceItemData;
    }*/


    public void SetItemData(int itemId)
    {
        ItemData instanceItemData = null;

        string path = Path.Combine(Application.streamingAssetsPath, "ItemData_Template.csv");

        if (!File.Exists(path))
        {
            Debug.LogError("[SetItemData] CSV ���� ����: " + path);
            item.data = null;
            return;
        }

        using (StreamReader reader = new StreamReader(path))
        {
            bool isFirstLine = true;

            while (true)
            {
                string line = reader.ReadLine();
                if (line == null) break;

                if (string.IsNullOrWhiteSpace(line)) continue;

                if (isFirstLine) { isFirstLine = false; continue; }

                string[] split = line.Split(',');
                if (split.Length < 6) continue;

                if (!int.TryParse(split[5], out int id)) continue;

                if (id != itemId) continue;

                // ������ ������ ����
                //instanceItemData = ScriptableObject.CreateInstance<ItemData>();
                instanceItemData = new ItemData();

                instanceItemData.itemName = split[0];

                Enum.TryParse(split[2], true, out ItemType itemType);
                instanceItemData.itemType = itemType;

                Enum.TryParse(split[3], true, out EquipmentType equipType);
                instanceItemData.equipmentType = equipType;

                int.TryParse(split[4], out int maxCountParsed);
                instanceItemData.maxCount = maxCountParsed;

                // ������ �ε� (Resources ���� �״��)
                string normalized = split[1].Replace("\\", "/");
                if (normalized.StartsWith("Resources/"))
                    normalized = normalized.Substring("Resources/".Length);

                Sprite icon = Resources.Load<Sprite>(normalized);
                instanceItemData.icon = icon;

                break;
            }
        }

        item.data = instanceItemData;
    }

}
