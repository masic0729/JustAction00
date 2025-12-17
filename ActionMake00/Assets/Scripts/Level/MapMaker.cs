using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum MAPTYPE
{
    START,
    Straight,
    DOWNLEFT,
    END
}

public class SpawnedMapData
{
    public List<int> spawnedMapX = new List<int>();                                                             //맵이 소환되는 x값
    public List<int> spawnedMapZ = new List<int>();                                                             //맵이 소환되는 y값
    public List<MAPTYPE> mapType = new List<MAPTYPE>();                                                  //맵의 타입
    public List<int> mapRotate = new List<int>();                                                                  //맵의 회전값. 각 맵 생성 후 회전 값을 반영한다
}

public class MapMaker : MonoBehaviour
{

    //맵은 테스트용이기에 1개 밖에 없지만,
    //좌/우/마지막 맵(보스 방)으로 구성되어 있다
    public GameObject StartGround;

    public GameObject StraightGround;

    public GameObject DownLeftGround;

    public GameObject EndGround;


    List<GameObject> createdMap = new List<GameObject>();

    int mapMakeCount;
    int currentX, currentZ;


    void Start()
    {
        Init();
        MapMake();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MapMake();
        }
    }

    void Init()
    {
        mapMakeCount = GetComponent<MapDesign>().GetMapMakeCount();
    }

    
    public void MapMake()
    {
        //GameData.instance.mapData = new SpawnedMapData();

        int resultRotateMap = 0;


        //월드 좌표를 기준으로 맵의 위치를 정한다
        List<(int, int)> mapIndex = new List<(int, int)>();

        if (createdMap.Count > 0)
        {
            while (createdMap.Count > 0)
            {
                Destroy(createdMap[0]);
                createdMap.RemoveAt(0);
            }
        }


        bool[,] map;

        map = new bool[mapMakeCount * 4, mapMakeCount * 4];

        //처음 시작하는 맵 위치는 이곳이다.
        map[mapMakeCount, mapMakeCount] = true;
        mapIndex.Add((mapMakeCount, mapMakeCount));

        GameData.instance.mapData.spawnedMapX.Add(mapMakeCount);
        GameData.instance.mapData.spawnedMapZ.Add(mapMakeCount);
        //mapData.mapType.Add(MAPTYPE.START);


        

        currentX = mapMakeCount;
        currentZ = mapMakeCount + 1;
        //고정적으로 위로 올라가기 때문에 3,4로 이동한다.
        map[currentX, currentZ] = true;
        mapIndex.Add((currentX, currentZ));
        int currentMakeCount = 2;

        //목표 맵 생성을 이루기 까지 DFS 실행한다
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
                GameData.instance.mapData.spawnedMapX.Add(currentX);
                GameData.instance.mapData.spawnedMapZ.Add(currentZ);

                //currentMapRotateY = resultRotateY;

                map[targetMap.Item1, targetMap.Item2] = true;
                mapIndex.Add((targetMap.Item1, targetMap.Item2));

                currentMakeCount++;

                currentX = targetMap.Item1;
                currentZ = targetMap.Item2;
            }
            else
            {
                GameData.instance.mapData.spawnedMapX.RemoveAt(GameData.instance.mapData.spawnedMapX.Count);
                GameData.instance.mapData.spawnedMapZ.RemoveAt(GameData.instance.mapData.spawnedMapZ.Count);
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

        GameData.instance.mapData.spawnedMapX.Add(currentX);
        GameData.instance.mapData.spawnedMapZ.Add(currentZ);

        Vector3 spawnPos = Vector3.zero;
        for (int i = 0; i < mapIndex.Count - 1; i++)
        {
            spawnPos = new Vector3(mapIndex[i].Item1 * StartGround.transform.localScale.x, 0, mapIndex[i].Item2 * StartGround.transform.localScale.z);


            if (i == 0)
            {
                createdMap.Add(Instantiate(StartGround, spawnPos, transform.rotation));
                GameData.instance.mapData.mapType.Add(MAPTYPE.START);
            }
            else
            {
                int middleX = mapIndex[i + 1].Item1 - mapIndex[i - 1].Item1;
                int middleZ = mapIndex[i + 1].Item2 - mapIndex[i - 1].Item2;

                GameObject wantMap = CalMiddleMapsType(mapIndex[i - 1], mapIndex[i], mapIndex[i + 1]);
                GameObject createMap = Instantiate(wantMap, spawnPos, transform.rotation);
                createdMap.Add(createMap);
                
                //해당 기능은 커스텀에디터 기반으로 제작된 맵 데이터를 해당 맵에 연동하여 설계된 액터들을 배치하는 것
                createMap.GetComponent<MapMiddleZone>().ActorSpawn(i - 1);

                resultRotateMap = GameData.instance.mapData.mapRotate[GameData.instance.mapData.mapRotate.Count - 1];
                createdMap[createdMap.Count - 1].transform.Rotate(0, resultRotateMap, 0);
            }
        }

        spawnPos = new Vector3(mapIndex[mapIndex.Count - 1].Item1 * StartGround.transform.localScale.x, 0, mapIndex[mapIndex.Count - 1].Item2 * StartGround.transform.localScale.z);

        int endX = mapIndex[mapIndex.Count - 1].Item1 - mapIndex[mapIndex.Count - 2].Item1;
        int endZ = mapIndex[mapIndex.Count - 1].Item2 - mapIndex[mapIndex.Count - 2].Item2;
        CreateEndGround(endX, endZ, spawnPos);
    }

    GameObject CalMiddleMapsType((int, int) prevMap, (int, int) currentMap, (int, int) nextMap)
    {
        int x = 0;
        int z = 0;

        //Vertical
        if (prevMap.Item1 == currentMap.Item1 && currentMap.Item1 == nextMap.Item1)
        {
            GameData.instance.mapData.mapType.Add(MAPTYPE.Straight);
            GameData.instance.mapData.mapRotate.Add(90);
            return StraightGround;
        }
        
        //Horizontal
        if (prevMap.Item2 == currentMap.Item2 && currentMap.Item2 == nextMap.Item2)
        {
            GameData.instance.mapData.mapType.Add(MAPTYPE.Straight);
            GameData.instance.mapData.mapRotate.Add(0);
            return StraightGround;
        }



        if (prevMap.Item1 < currentMap.Item1 || nextMap.Item1 < currentMap.Item1)
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

        // 3) 여기까지 왔으면 코너 처리
        switch (x, z)
        {
            case (1, -1):
                GameData.instance.mapData.mapType.Add(MAPTYPE.DOWNLEFT);
                GameData.instance.mapData.mapRotate.Add(0);
                return DownLeftGround;

            case (-1, -1):
                GameData.instance.mapData.mapType.Add(MAPTYPE.DOWNLEFT);
                GameData.instance.mapData.mapRotate.Add(270);
                return DownLeftGround;

            case (1, 1):
                GameData.instance.mapData.mapType.Add(MAPTYPE.DOWNLEFT);
                GameData.instance.mapData.mapRotate.Add(90);
                return DownLeftGround;

            case (-1, 1):
                GameData.instance.mapData.mapType.Add(MAPTYPE.DOWNLEFT);
                GameData.instance.mapData.mapRotate.Add(180);
                return DownLeftGround;
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
        GameData.instance.mapData.mapType.Add(MAPTYPE.END);
        GameData.instance.mapData.mapRotate.Add(resultRotateY);

    }
}