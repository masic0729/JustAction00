using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


[System.Serializable]
class testStat
{
    public string itemName;
    public int hp, damage, defense;
}

class TestStatTable
{
    public testStat[] Item;
}

public class TestObject : MonoBehaviour
{
    testStat stat;

    // Start is called before the first frame update
    void Start()
    {
        TextAsset testJson = Resources.Load<TextAsset>("Jsons/test");

        TestStatTable testTable = JsonUtility.FromJson<TestStatTable>(testJson.text);

        if (testTable.Item != null && testTable.Item.Length > 1)
        {
            stat = testTable.Item[1];
            // 이제 stat.itemName, stat.hp 이런 거 다 사용 가능
            Debug.Log(stat.hp + " " + stat.damage + " " + stat.defense);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
