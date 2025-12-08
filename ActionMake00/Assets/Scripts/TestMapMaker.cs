using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MAPTYPE
{
    START = 0,
    STRAIGHT = 1,
    LEFT,
    RIGHT,
    END
}

public class SpawnedMapData
{
    public List<int> spawnedMapX = new List<int>();
    public List<int> spawnedMapZ = new List<int>();
    public List<MAPTYPE> mapType = new List<MAPTYPE>();
    public List<float> spawnedMapRotationY = new List<float>();
}

public class TestMapMaker : MonoBehaviour
{
    SpawnedMapData mapData;                     //추후 json에 저장할 데이터

    //맵은 테스트용이기에 1개 밖에 없지만,
    //좌/우/마지막 맵(보스 방)으로 구성되어 있다
    public GameObject Ground;

    List <GameObject> createdMap = new List<GameObject>();

    //맵의 형태에 따라서 회전값이 다를 것이다.
    //이를 고려하여 맵 생성 후 데이터를 저장할 때 사용한다
    float currentMapRotateY = 0f;

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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            InitMap();
        }
    }


    public void InitMap()
    {
        mapData = new SpawnedMapData();


        


        List<(int, int)> mapIndex = new List<(int, int)>();

        if (createdMap.Count > 0)
        {
            while(createdMap.Count >0)
            {
                Destroy(createdMap[0]);
                createdMap.RemoveAt(0);
            }
        }


        bool[,] map;

        map = new bool[mapMakeCount, mapMakeCount];

        //처음 시작하는 맵 위치는 이곳이다.
        map[3, 3] = true;
        mapIndex.Add((3, 3));

        mapData.spawnedMapX.Add(3);
        mapData.spawnedMapZ.Add(3);
        mapData.mapType.Add(MAPTYPE.START);
        mapData.spawnedMapRotationY.Add(0f);


        //고정적으로 위로 올라가기 때문에 3,4로 이동한다.
        map[3, 4] = true;

        currentX = 3;
        currentZ = 4;
        mapIndex.Add((3, 4));
        int currentMakeCount = 2;


        while (currentMakeCount < mapMakeCount)
        {
            List<(int, int)> canMakeMapList = new List<(int, int)>();

            if (currentX - 1 >= 0)
            {
                if (map[currentX - 1, currentZ] == false) 
                {
                    canMakeMapList.Add(((currentX - 1), currentZ));
                }
            }

            if (currentX + 1 <= 6)
            {
                if (map[currentX + 1, currentZ] == false) 
                {
                    canMakeMapList.Add(((currentX + 1), currentZ)); 
                }
            }

            if (currentZ - 1 >= 0)
            {
                if (map[currentX, currentZ - 1] == false) 
                {
                    canMakeMapList.Add(((currentX), currentZ - 1)); 
                }
            }

            if (currentZ + 1 <= 6)
            {
                if (map[currentX, currentZ + 1] == false) 
                {
                    canMakeMapList.Add(((currentX), currentZ + 1)); 
                }
            }

            //제작할 수 있는 맵의 경우의 수라면 데이터 삽입 및 진전
            if (canMakeMapList.Count > 0)
            {
                (int, int) targetMap = canMakeMapList[Random.Range(0, canMakeMapList.Count)];
                //float resultRotateY = currentMapRotateY + targetMap.Item4;

                SetMapType();
                mapData.spawnedMapX.Add(currentX);
                mapData.spawnedMapZ.Add(currentZ);

                //currentMapRotateY = resultRotateY;

                map[targetMap.Item1, targetMap.Item2] = true;
                mapIndex.Add((targetMap.Item1, targetMap.Item2));

                currentMakeCount++;
                
                currentX = targetMap.Item1;
                currentZ = targetMap.Item2;    
            }
            else
            {
                mapData.spawnedMapX.RemoveAt(mapData.spawnedMapX.Count);
                mapData.spawnedMapZ.RemoveAt(mapData.spawnedMapZ.Count);
                mapData.mapType.RemoveAt(mapData.mapType.Count);
                mapData.spawnedMapRotationY.RemoveAt(mapData.spawnedMapRotationY.Count);

                map[currentX, currentZ] = false;

                mapIndex.RemoveAt(mapIndex.Count - 1);
                currentMakeCount--;

                var last = mapIndex[mapIndex.Count - 1];
                currentX = last.Item1;
                currentZ = last.Item2;
            }

            canMakeMapList.Clear();

        }

        mapData.spawnedMapX.Add(currentX);
        mapData.spawnedMapZ.Add(currentZ);
        mapData.mapType.Add(MAPTYPE.END);
        mapData.spawnedMapRotationY.Add(currentMapRotateY);

        //디버깅용
        for (int i = 0; i < mapIndex.Count; i++)
        {
            Vector3 spawnPos = new Vector3(mapIndex[i].Item1 * 3, 0, mapIndex[i].Item2 * 3);
            Debug.Log(mapData.spawnedMapX[i] + " " + mapData.spawnedMapZ[i] + " " + mapData.mapType[i] + " " + mapData.spawnedMapRotationY[i]);

            createdMap.Add(Instantiate(Ground, spawnPos, transform.rotation));

            //3D 타일 회전
            createdMap[createdMap.Count - 1].transform.Rotate(0, mapData.spawnedMapRotationY[i], 0);
        }

        

    }

    void SetMapType()
    {
        
    }

    void SetMapRotate()
    {

    }


}