using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum MAPTYPE
{
    START,
    HORIZONTAL,
    VERTICAL,
    DOWNLEFT,
    DOWNRIGHT,
    UPLEFT,
    UPRIGHT,
    END
}

public class SpawnedMapData
{
    public List<int> spawnedMapX = new List<int>();                 //맵이 소환되는 x값
    public List<int> spawnedMapZ = new List<int>();                 //맵이 소환되는 y값
    public List<MAPTYPE> mapType = new List<MAPTYPE>();             //맵의 타입
    public List<int> mapRotate= new List<int>();                    //맵의 회전값. 기본은0이나, 마지막 맵은 형태에 따라 다름    
}

public class TestMapMaker : MonoBehaviour
{
    SpawnedMapData mapData;                     //추후 json에 저장할 데이터

    //맵은 테스트용이기에 1개 밖에 없지만,
    //좌/우/마지막 맵(보스 방)으로 구성되어 있다
    public GameObject StartGround;

    public GameObject HorizontalGround;
    public GameObject VerticalGround;

    public GameObject DownLeftGround;
    public GameObject DownRightGround;
    public GameObject UpLeftGround;
    public GameObject UpRightGround;

    public GameObject EndGround;


    List <GameObject> createdMap = new List<GameObject>();



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


        

        //월드 좌표를 기준으로 맵의 위치를 정한다
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

                //SetMapType();
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
                //mapData.mapType.RemoveAt(mapData.mapType.Count);
                //mapData.spawnedMapRotationY.RemoveAt(mapData.spawnedMapRotationY.Count);

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
        //mapData.mapType.Add(MAPTYPE.END);
        //mapData.spawnedMapRotationY.Add(currentMapRotateY);

        Vector3 spawnPos = Vector3.zero;
        for (int i = 0; i < mapIndex.Count - 1; i++)
        {
            spawnPos = new Vector3(mapIndex[i].Item1 * 3, 0, mapIndex[i].Item2 * 3);
            //Debug.Log(mapData.spawnedMapX[i] + " " + mapData.spawnedMapZ[i] + " " + mapData.mapType[i] + " " + mapData.spawnedMapRotationY[i]);

            //createdMap.Add(Instantiate(StartGround, spawnPos, transform.rotation));

            if(i == 0)
            {
                createdMap.Add(Instantiate(StartGround, spawnPos, transform.rotation));
                mapData.mapType.Add(MAPTYPE.START);
            }
            else
            {
                int middleX = mapIndex[i + 1].Item1 - mapIndex[i - 1].Item1;
                int middleZ = mapIndex[i + 1].Item2 - mapIndex[i - 1].Item2;

                GameObject wantMap = CalMiddleMapsType(mapIndex[i - 1], mapIndex[i], mapIndex[i + 1]);
                createdMap.Add(Instantiate(wantMap, spawnPos, transform.rotation));
                //mapData.mapType.Add(MAPTYPE.START);
            }
        }

        spawnPos = new Vector3(mapIndex[mapIndex.Count - 1].Item1 * 3, 0, mapIndex[mapIndex.Count - 1].Item2 * 3);

        int endX = mapIndex[mapIndex.Count - 1].Item1 - mapIndex[mapIndex.Count - 2].Item1;
        int endZ = mapIndex[mapIndex.Count - 1].Item2 - mapIndex[mapIndex.Count - 2].Item2;
        CreateEndGround(endX, endZ, spawnPos);
    }

    GameObject CalMiddleMapsType((int, int) prevMap, (int, int) currentMap, (int, int) nextMap)
    {
        int x = 0;
        int z = 0;

        if(prevMap.Item1 == currentMap.Item1 && currentMap.Item1 == nextMap.Item1)
        {
            mapData.mapType.Add(MAPTYPE.VERTICAL);
            return VerticalGround;
        }
        if (prevMap.Item2 == currentMap.Item2 && currentMap.Item2 == nextMap.Item2)
        {
            mapData.mapType.Add(MAPTYPE.HORIZONTAL);
            return HorizontalGround;
        }



        if (prevMap.Item1 < currentMap.Item1 || nextMap.Item1 < currentMap.Item1  )
        {
            x = 1;
        }
        else// if (prevMap.Item1 - currentMap.Item1 > 0 || nextMap.Item1 - currentMap.Item1 < 0)
        {
            x = -1;
        }

        
        if (prevMap.Item2 > currentMap.Item2 || nextMap.Item2 > currentMap.Item2)
        {
            z = 1;
        }
        else// if (prevMap.Item2 - currentMap.Item2 < 0 || nextMap.Item2 - currentMap.Item2 > 0)
        {
            z = -1;
        }

        // 1) 일단 같은 좌표면 예외
        if (x == 0 && z == 0)
        {
            Debug.LogWarning("prevMap과 nextMap이 같음");
            return null;
        }
        mapData.mapRotate.Add(0);

        // 3) 여기까지 왔으면 코너 처리
        switch (x, z)
        {
            case (1, -1):
                mapData.mapType.Add(MAPTYPE.DOWNLEFT);
                return DownLeftGround;

            case (-1, -1):
                mapData.mapType.Add(MAPTYPE.DOWNRIGHT);
                return DownRightGround;

            case (1, 1):
                mapData.mapType.Add(MAPTYPE.UPLEFT);
                return UpLeftGround;

            case (-1, 1):
                mapData.mapType.Add(MAPTYPE.UPRIGHT);
                return UpRightGround;
        }

        // 2) 직선(세로/가로) 먼저 처리
        if (x == 0 && z != 0)
        {
            mapData.mapType.Add(MAPTYPE.VERTICAL);
            return VerticalGround;
        }

        if (x != 0 && z == 0)
        {
            mapData.mapType.Add(MAPTYPE.HORIZONTAL);
            return HorizontalGround;
        }

        Debug.LogError($"예외 방향 x:{x}, z:{z}");
        return null;
    }
    

    void CreateEndGround(int resultX, int resultZ, Vector3 spawnPosition)
    {
        int resultRotateY = 0;
        if (resultX < 0)
            resultRotateY = -90;
        if (resultX > 0)
            resultRotateY = 90;
        if (resultZ < 0)
            resultRotateY = 180;
        if (resultZ > 0)
            resultRotateY = 0;

        createdMap.Add(Instantiate(EndGround, spawnPosition, transform.rotation));
        createdMap[createdMap.Count - 1].transform.Rotate(0, resultRotateY, 0);
        mapData.mapType.Add(MAPTYPE.END);
        mapData.mapRotate.Add(resultRotateY);

    }
}