using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMaker : MonoBehaviour
{
    //맵은 테스트용이기에 1개 밖에 없지만,
    //좌/우/마지막 맵(보스 방)으로 구성되어 있다
    public GameObject Ground;

    List<(int, int)> mapIndex = new List<(int, int)>();

    bool[ , ] map = new bool[7, 7];
    public int mapMakeCount;
    public int currentX, currentZ;


    // Start is called before the first frame update
    void Start()
    {
        InitMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitMap()
    {
        int currentMakeCount = 0;

        //처음 시작하는 맵 위치는 이곳이다.
        map[3, 3] = true;
        mapIndex.Add((3, 3));

        //고정적으로 위로 올라가기 때문에 3,4로 이동한다.
        map[3, 4] = true;
        
        currentX = 3;
        currentZ = 4;
        mapIndex.Add((3, 4));

        while(currentMakeCount != mapMakeCount)
        {
            List<(int, int)> canMakeMapList = new List<(int, int)>();

            if (currentX - 1 >= 0)
            {
                if (map[currentX - 1, currentZ] == false) { canMakeMapList.Add(((currentX - 1), currentZ)); }
            }

            if (currentX + 1 <= 6)
            {
                if (map[currentX + 1, currentZ] == false) { canMakeMapList.Add(((currentX + 1), currentZ)); }
            }

            if (currentZ - 1 >= 0)
            {
                if (map[currentX, currentZ - 1] == false) { canMakeMapList.Add(((currentX), currentZ - 1)); }
            }

            if (currentZ + 1 <= 6)
            {
                if (map[currentX, currentZ + 1] == false) { canMakeMapList.Add(((currentX), currentZ + 1)); }
            }

            //제작할 수 있는 맵의 경우의 수면 데이터 삽입 및 진전
            if (canMakeMapList.Count > 0)
            {
                (int, int) targetMap = canMakeMapList[Random.Range(0, canMakeMapList.Count)];

                map[targetMap.Item1, targetMap.Item2] = true;
                mapIndex.Add(targetMap);
                currentMakeCount++;
                currentX = targetMap.Item1;
                currentZ = targetMap.Item2;
            }
            else
            {
                map[currentX, currentZ] = false;

                mapIndex.RemoveAt(mapIndex.Count - 1);
                currentMakeCount--;

                currentX = mapIndex[mapIndex.Count].Item1;
                currentZ = mapIndex[mapIndex.Count].Item2;
            }

            canMakeMapList.Clear();

        }

        //디버깅용
        for (int i = 0; i < mapIndex.Count; i++)
        {
            Debug.Log(mapIndex[i]);
        }
    }
}