using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// item_data.CSV 테스트용 DB 읽기 (헤더 1줄 있는 기준)
/// </summary>
public class TestObject : MonoBehaviour
{
    // 읽어 올 파일 이름 (확장자 제외)
    public string csvFileName = "item_data";

    // key:value 형태로 저장
    public Dictionary<string, ItemStat> dicItem = new Dictionary<string, ItemStat>();

    [System.Serializable]
    public class ItemStat
    {
        public string itemName;
        public int hp;
        public int damage;
        public int defense;
        public float test;
    }

    private void Start()
    {
        ReadCSV();
    }

    private void ReadCSV()
    {
        // Resources/Jsons/item_data.CSV 기준
        string path = "Resources/Jsons/" + csvFileName + ".CSV";
        StreamReader reader = new StreamReader(Application.dataPath + "/" + path);

        bool isFinish = false;
        bool isFirstLine = true;   // 첫 줄(헤더)인지 체크용

        while (!isFinish)
        {
            string data = reader.ReadLine(); // 한 줄 읽기

            if (data == null)
            {
                isFinish = true;
                break;
            }

            // 빈 줄은 스킵
            if (string.IsNullOrWhiteSpace(data))
                continue;

            // 첫 줄은 헤더니까 건너뛴다
            if (isFirstLine)
            {
                isFirstLine = false;
                // 예: "itemName,hp,damage,defense"
                continue;
            }

            var splitData = data.Split(',');

            ItemStat item = new ItemStat();
            item.itemName = splitData[0];
            item.hp = int.Parse(splitData[1]);
            item.damage = int.Parse(splitData[2]);
            item.defense = int.Parse(splitData[3]);
            item.test = float.Parse(splitData[3]);

            dicItem.Add(item.itemName, item);

            Debug.Log(item.itemName);
            Debug.Log(dicItem.Count); // 잘 들어갔는지 체크
        }

        // 예시로 하나 찍어보기
        if (dicItem.ContainsKey("testItem"))
        {
            Debug.Log($"testItem : hp {dicItem["testItem"].hp}, dmg {dicItem["testItem"].damage}, def {dicItem["testItem"].defense}");
        }
    }

    /*protected ItemData SetItemData(int itemId, string iconBasePath)
    {
        ItemData instanceItemData = null;

        // CSV 위치: Assets/Resources/Jsons/ItemData.CS
        string path = Application.dataPath + "/Resources/Jsons/ItemData.CSV";

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
                string iconKey = split[1]; // CSV에 적어둔 키 (예: "03_Alchemy")
                string iconPath = string.IsNullOrEmpty(iconBasePath)
                    ? iconKey
                    : $"{iconBasePath}/{iconKey}";

                Sprite icon = Resources.Load<Sprite>(iconPath);
                if (icon == null)
                {
                    Debug.LogWarning($"[SetItemData] 아이콘 로드 실패: {iconPath}");
                }
                instanceItemData.icon = icon;

                // 4) 슬롯 공통 필드도 같이 갱신해 주면 편함
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
}