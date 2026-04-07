using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public List<int> spawnedMapX = new List<int>();
    public List<int> spawnedMapZ = new List<int>();
    public List<MAPTYPE> mapType = new List<MAPTYPE>();
    public List<int> mapRotate = new List<int>();
}

public class MapMaker : MonoBehaviour
{
    public static MapMaker instance;

    public GameObject StartGround;
    public GameObject StraightGround;
    public GameObject DownLeftGround;
    public GameObject EndGround;

    List<GameObject> createdMap = new List<GameObject>();

    int mapMakeCount;
    int currentX, currentZ;

    [SerializeField] int mapSize = 30;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Init();
        MapMake();
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }*/
    }

    void Init()
    {
        mapMakeCount = GetComponent<MapDesign>().GetMapMakeCount();
    }

    public void MapMake()
    {
        int resultRotateMap = 0;

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
        int mapBlock = 4;
        map = new bool[mapBlock, mapBlock];

        int firstMapPosition = mapBlock / 2;
        map[firstMapPosition, firstMapPosition] = true;
        mapIndex.Add((firstMapPosition, firstMapPosition));

        GameData.instance.mapData.spawnedMapX.Add(firstMapPosition);
        GameData.instance.mapData.spawnedMapZ.Add(firstMapPosition);

        currentX = firstMapPosition;
        currentZ = firstMapPosition + 1;

        map[currentX, currentZ] = true;
        mapIndex.Add((currentX, currentZ));
        int currentMakeCount = 2;

        while (currentMakeCount < mapMakeCount)
        {
            List<(int, int)> canMakeMapList = new List<(int, int)>();

            if (currentX - 1 >= 0)
            {
                if (map[currentX - 1, currentZ] == false)
                {
                    canMakeMapList.Add((currentX - 1, currentZ));
                }
            }

            if (currentX + 1 <= mapBlock - 1)
            {
                if (map[currentX + 1, currentZ] == false)
                {
                    canMakeMapList.Add((currentX + 1, currentZ));
                }
            }

            if (currentZ - 1 >= 0)
            {
                //map[currentX, mapBlock - 1] → map[currentX, currentZ - 1]
                if (map[currentX, currentZ - 1] == false)
                {
                    canMakeMapList.Add((currentX, currentZ - 1));
                }
            }

            if (currentZ + 1 <= mapBlock - 1)
            {
                if (map[currentX, currentZ + 1] == false)
                {
                    canMakeMapList.Add((currentX, currentZ + 1));
                }
            }

            if (canMakeMapList.Count > 0)
            {
                (int, int) targetMap = canMakeMapList[Random.Range(0, canMakeMapList.Count)];

                GameData.instance.mapData.spawnedMapX.Add(currentX);
                GameData.instance.mapData.spawnedMapZ.Add(currentZ);

                map[targetMap.Item1, targetMap.Item2] = true;
                mapIndex.Add((targetMap.Item1, targetMap.Item2));

                currentMakeCount++;

                currentX = targetMap.Item1;
                currentZ = targetMap.Item2;
            }
            else
            {
                GameData.instance.mapData.spawnedMapX.RemoveAt(GameData.instance.mapData.spawnedMapX.Count - 1);
                GameData.instance.mapData.spawnedMapZ.RemoveAt(GameData.instance.mapData.spawnedMapZ.Count - 1);

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
            spawnPos = new Vector3(mapIndex[i].Item1 * mapSize, 0, mapIndex[i].Item2 * mapSize);

            if (i == 0)
            {
                createdMap.Add(Instantiate(StartGround, spawnPos, transform.rotation));
                GameData.instance.mapData.mapType.Add(MAPTYPE.START);
            }
            else
            {
                GameObject wantMap = CalMiddleMapsType(mapIndex[i - 1], mapIndex[i], mapIndex[i + 1]);
                GameObject createMap = Instantiate(wantMap, spawnPos, transform.rotation);
                createdMap.Add(createMap);

                createMap.GetComponent<MapMiddleZone>().ActorSpawn(i - 1);

                resultRotateMap = GameData.instance.mapData.mapRotate[GameData.instance.mapData.mapRotate.Count - 1];
                createdMap[createdMap.Count - 1].transform.Rotate(0, resultRotateMap, 0);
            }
        }

        spawnPos = new Vector3(mapIndex[mapIndex.Count - 1].Item1 * mapSize, 0, mapIndex[mapIndex.Count - 1].Item2 * mapSize);

        int endX = mapIndex[mapIndex.Count - 1].Item1 - mapIndex[mapIndex.Count - 2].Item1;
        int endZ = mapIndex[mapIndex.Count - 1].Item2 - mapIndex[mapIndex.Count - 2].Item2;
        CreateEndGround(endX, endZ, spawnPos);

        // 모든 타일 배치 및 회전 완료 후 수풀 생성 실행
        // createdMap에는 Start ~ End 모든 타일이 담겨 있음
        for (int i = 0; i < createdMap.Count; i++)
        {
            // 타일에 MapDeco 컴포넌트가 없는 경우 건너뜀
            MapDeco deco = createdMap[i].GetComponent<MapDeco>();
            if (deco == null) continue;

            deco.InitGrass();
        }
    }

    GameObject CalMiddleMapsType((int, int) prevMap, (int, int) currentMap, (int, int) nextMap)
    {
        int x = 0;
        int z = 0;

        if (prevMap.Item1 == currentMap.Item1 && currentMap.Item1 == nextMap.Item1)
        {
            GameData.instance.mapData.mapType.Add(MAPTYPE.Straight);
            GameData.instance.mapData.mapRotate.Add(90);
            return StraightGround;
        }

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
        else
        {
            x = -1;
        }

        if (prevMap.Item2 > currentMap.Item2 || nextMap.Item2 > currentMap.Item2)
        {
            z = 1;
        }
        else
        {
            z = -1;
        }

        if (x == 0 && z == 0)
        {
            Debug.LogWarning("prevMap과 nextMap이 같음");
            return null;
        }

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

    public int GetMapMakeCount() => mapMakeCount;
}