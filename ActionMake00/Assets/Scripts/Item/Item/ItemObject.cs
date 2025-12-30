using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public abstract class ItemObject : MonoBehaviour, ItemInteration, ItemUseChecker
{
    public int itemId;                                  //csv 내 존재하는 id와 매치하기 위한 값

    public ItemBase item;
    protected string itemComment;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {

    }


    /// <summary>
    /// 아이템 기능 실행은 슬롯 내 액션에 할당도었기에,
    /// 이 스크립트에 관리되는 방식은 아니다.
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

    /// <summary>
    /// csv의 데이터를 불러온다.
    /// id를 기반으로 불러오며, 해당 아이템의 아이콘은
    /// 경로를 통해 불러와서 적용한다
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="iconBasePath"></param>
    /*public void SetItemData(int itemId)
    {
        ItemData instanceItemData = null;

        // CSV 위치: Assets/Resources/Jsons/ItemData.CS
        string path = Application.dataPath + "/Resources/Jsons/ItemData_Template.CSV";

        using (StreamReader reader = new StreamReader(path))
        {
            bool isFirstLine = true; // 헤더 스킵용

            while (true)
            {
                string line = reader.ReadLine();

                // 더 이상 읽을 줄이 없으면 종료
                if (line == null)
                    break;

                // 빈 줄은 스킵
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // 첫 줄은 헤더라고 가정하고 스킵
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                // CSV: itemName,iconKey,itemType,equipmentType,maxCount,ID
                string[] split = line.Split(',');
                if (split.Length < 6)
                {
                    Debug.LogWarning($"[SetItemData] 잘못된 라인: {line}");
                    continue;
                }

                // 마지막 컬럼이 ID
                if (!int.TryParse(split[5], out int id))
                {
                    Debug.LogWarning($"[SetItemData] ID 파싱 실패: {split[5]}");
                    continue;
                }

                // 원하는 ID가 아니면 패스
                if (id != itemId)
                    continue;

                // 여기 도달하면: 찾고 싶은 ID의 줄이다

                // 런타임용 ItemData 인스턴스 생성
                instanceItemData = ScriptableObject.CreateInstance<ItemData>();

                // 기본 정보 세팅
                instanceItemData.itemName = split[0];

                // itemType 파싱
                if (!Enum.TryParse(split[2], true, out ItemType itemType))
                    itemType = ItemType.nullItem;
                instanceItemData.itemType = itemType;

                // equipmentType 파싱
                if (!Enum.TryParse(split[3], true, out EquipmentType equipType))
                    equipType = EquipmentType.None;
                instanceItemData.equipmentType = equipType;

                // maxCount 파싱
                if (!int.TryParse(split[4], out int parsedMaxCount))
                    parsedMaxCount = 0;
                instanceItemData.maxCount = parsedMaxCount;

                // 3) 아이콘 로드
                string iconKey = split[1].Trim(); // CSV의 iconKey 값 전체 (예: "Resources\\JsonImageData\\03_Alchemy")

                // 역슬래시(\)를 슬래시(/)로 통일
                string normalized = iconKey.Replace("\\", "/");

                // 앞에 "Resources/"가 붙어 있으면 제거
                if (normalized.StartsWith("Resources/"))
                {
                    normalized = normalized.Substring("Resources/".Length);
                }

                // 최종적으로 Resources.Load에 넘길 경로
                // 예: "JsonImageData/03_Alchemy"
                string iconPath = normalized;

                Sprite icon = Resources.Load<Sprite>(iconPath);
                if (icon == null)
                {
                    Debug.LogWarning($"[SetItemData] 아이콘 로드 실패: {iconPath}");
                }
                instanceItemData.icon = icon;

                // 슬롯 공통 필드도 그대로 유지
                *//*this.type = instanceItemData.itemType;
                this.maxCount = instanceItemData.maxCount;

                if (slotIcon != null && instanceItemData.icon != null)
                {
                    slotIcon.sprite = instanceItemData.icon;
                    slotIcon.enabled = true;
                    slotIcon.color = Color.white;
                }*//*

                // 찾았으면 더 읽을 필요 없으니 바로 종료
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
            Debug.LogError("[SetItemData] CSV 파일 없음: " + path);
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

                // 아이템 데이터 생성
                //instanceItemData = ScriptableObject.CreateInstance<ItemData>();
                instanceItemData = new ItemData();

                instanceItemData.itemName = split[0];

                Enum.TryParse(split[2], true, out ItemType itemType);
                instanceItemData.itemType = itemType;

                Enum.TryParse(split[3], true, out EquipmentType equipType);
                instanceItemData.equipmentType = equipType;

                int.TryParse(split[4], out int maxCountParsed);
                instanceItemData.maxCount = maxCountParsed;

                // 아이콘 로드 (Resources 기준 그대로)
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
