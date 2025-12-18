using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SlotType
{
    InventorySlot,
    Equipment

}

[System.Serializable]
public class ItemRow
{
    public int id;
    public string itemName;
    public string iconKey;
    public ItemType itemType;
    public EquipmentType equipmentType;
    public int maxCount;
}

public abstract class SlotBase : MonoBehaviour
{
    public ItemType type = ItemType.nullItem;                                               //아이템 정렬을 위한 데이터 타입
    public Character target;

    public TextMeshProUGUI countText;

    [SerializeField] protected Inventory inventory;      
    //슬롯의 인벤토리 주체. 아이템 간 이동 시 활용함

    public SlotType slotType;                                                               //장비 형태
    public Sprite baseSlotImage;                    //비어있을 때 쓰는 이미지
    public Image slotIcon;
    public ItemBase currentItem = null;

    /*public string slotItemName;
    public string slotItemComment;*/

    public Action<Character, SlotBase> OnSlotItemUse;   //아이템을 사용할 때 발생하는 상호작용
    
    //아이템 사용 후 처리에 대한 부분. 예시로 슬롯 데이터 삭제,
    //카운트 및 차감 등등 기본적인 상호작용 이후의 처리를 뜻한다
    public Action<SlotBase> OnSlotItemUpdate;

    public int currentCount = 0, maxCount = 0;
    public int slotIndex = -1;                      //슬롯의 인덱스 정보

    public abstract bool AddItem(ItemObject itemObject);

    public abstract bool AddItem(ItemBase item);

    public abstract void SwapItem(ItemSlot slot);

    public abstract void SwapItem(SlotBase slot);

    public abstract void SumItem(ItemObject itemObject);

    public abstract void SetItemDirect(ItemData data, int count, string comment);

    public abstract void UpdateSlot();

    public abstract void SortSlot(ItemBase itemData);

    public abstract void UpdateUI();


    public abstract void ClearSlot();

    public abstract bool CanAddItem();

    public abstract bool CanSumItem(ItemBase item);

    public Inventory GetInventory() => inventory;

    public void SetTarget(Character character)
    {
        target = character;
    }

    public string GetItemName(ItemBase slotItem)
    {
        return slotItem.data.itemName;
    }

    public string GetItemComment(ItemBase slotItem)
    {
        return slotItem.comment;
    }

    /// <summary>
    /// 기본적으로 툴팁은 슬롯 기준 우측에 등장한다
    /// 
    /// 툴팁의 가로 축과 슬롯의 위치를 고려하여
    /// 가로 화면에 벗어나면 좌측으로 전환한다
    /// </summary>
    /// <param name="slotPosition"></param>
    /// <param name="viewWidth"></param>
    /// <returns></returns>
    public Vector2 CalToolTipPosition(Vector3 slotPosition, float viewWidth)
    {
        Vector2 resultPosition = slotPosition;
        //resultPosition = new Vector2(slotPosition.x + ((float)viewWidth / 2) + 55, slotPosition.y);
        if ((slotPosition.x + viewWidth + 55) > 1920)
        {
            resultPosition = new Vector2(slotPosition.x - ((float)viewWidth / 2) - 55, slotPosition.y);
        }
        else
        {
            resultPosition = new Vector2(slotPosition.x + ((float)viewWidth / 2) + 55, slotPosition.y);

        }
        return resultPosition;
    }




    /// <summary>
    /// csv의 데이터를 불러온다.
    /// id를 기반으로 불러오며, 해당 아이템의 아이콘은
    /// 경로를 통해 불러와서 적용한다
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="iconBasePath"></param>
    /*protected ItemData SetItemData(int itemId)
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
                this.type = instanceItemData.itemType;
                this.maxCount = instanceItemData.maxCount;

                if (slotIcon != null && instanceItemData.icon != null)
                {
                    slotIcon.sprite = instanceItemData.icon;
                    slotIcon.enabled = true;
                    slotIcon.color = Color.white;
                }

                // 찾았으면 더 읽을 필요 없으니 바로 종료
                break;
            }
        }

        return instanceItemData;
    }*/


    /// <summary>
    //// csv의 데이터를 불러온다.
    //// id를 기반으로 불러오며, 해당 아이템의 아이콘은
    //// 경로를 통해 불러와서 적용한다
    /// </summary>
    protected ItemData SetItemData(int itemId)
    {
        ItemData instanceItemData = null;

        string path = Path.Combine(Application.streamingAssetsPath, "ItemData_Template.csv");

        if (!File.Exists(path))
        {
            Debug.LogError($"[SetItemData] CSV 파일을 찾을 수 없음: {path}");
            return null;
        }

        using (StreamReader reader = new StreamReader(path))
        {
            bool isFirstLine = true; // 헤더 스킵용

            while (true)
            {
                string line = reader.ReadLine();

                // 더 이상 읽을 줄이 없으면 종료
                if (line == null)
                    break;

                if (string.IsNullOrWhiteSpace(line))
                    continue;

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

                // 원하는 ID 아니면 패스
                if (id != itemId)
                    continue;

                // 여기 도달 = 찾는 아이템 한 줄 발견
                //instanceItemData = ScriptableObject.CreateInstance<ItemData>();
                instanceItemData = new ItemData();

                instanceItemData.itemName = split[0];

                if (!Enum.TryParse(split[2], true, out ItemType itemType))
                    itemType = ItemType.nullItem;
                instanceItemData.itemType = itemType;

                if (!Enum.TryParse(split[3], true, out EquipmentType equipType))
                    equipType = EquipmentType.None;
                instanceItemData.equipmentType = equipType;

                if (!int.TryParse(split[4], out int parsedMaxCount))
                    parsedMaxCount = 0;
                instanceItemData.maxCount = parsedMaxCount;

                string iconKey = split[1].Trim();

                string normalized = iconKey.Replace("\\", "/");

                if (normalized.StartsWith("Resources/"))
                    normalized = normalized.Substring("Resources/".Length);

                string iconPath = normalized; // 예: "JsonImageData/03_Alchemy"

                Sprite icon = Resources.Load<Sprite>(iconPath);
                if (icon == null)
                {
                    Debug.LogWarning($"[SetItemData] 아이콘 로드 실패: {iconPath}");
                }
                instanceItemData.icon = icon;

                // 슬롯 쪽 기본값도 같이 갱신
                this.type = instanceItemData.itemType;
                this.maxCount = instanceItemData.maxCount;

                if (slotIcon != null && instanceItemData.icon != null)
                {
                    slotIcon.sprite = instanceItemData.icon;
                    slotIcon.enabled = true;
                    slotIcon.color = Color.white;
                }

                break;
            }
        }

        return instanceItemData;
    }
}