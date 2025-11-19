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
}