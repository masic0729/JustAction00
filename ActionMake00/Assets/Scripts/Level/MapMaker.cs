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
    //DFS 결과로 생성된 타일들의 "Grid 좌표"를 저장한다 (월드 좌표는 mapSize로 환산됨)
    public List<int> spawnedMapX = new List<int>();                                                             //맵이 소환되는 x값
    public List<int> spawnedMapZ = new List<int>();                                                             //맵이 소환되는 y값

    //생성된 각 타일의 타입(시작/직선/코너/끝)을 기록한다
    public List<MAPTYPE> mapType = new List<MAPTYPE>();                                                  //맵의 타입

    //타일 타입과 함께, 연결이 자연스럽도록 회전값을 기록한다
    public List<int> mapRotate = new List<int>();                                                                  //맵의 회전값. 각 맵 생성 후 회전 값을 반영한다
}

public class MapMaker : MonoBehaviour
{
    public static MapMaker instance;

    //맵은 테스트용이기에 1개 밖에 없지만,
    //좌/우/마지막 맵(보스 방)으로 구성되어 있다
    public GameObject StartGround;

    public GameObject StraightGround;

    public GameObject DownLeftGround;

    public GameObject EndGround;


    //현재 생성된 타일(프리팹 인스턴스) 목록 - 리셋/재생성 시 파괴용
    List<GameObject> createdMap = new List<GameObject>();

    //생성할 타일 개수(길이) / 현재 DFS 좌표
    int mapMakeCount;
    int currentX, currentZ;

    //Grid 좌표를 월드 좌표로 환산할 때 사용하는 타일 간격
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
        //테스트 편의: 스페이스로 씬 리로드 -> 맵 재생성 확인
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void Init()
    {
        //커스텀 설정(MapDesign)에서 목표 타일 개수를 가져온다
        mapMakeCount = GetComponent<MapDesign>().GetMapMakeCount();
    }


    public void MapMake()
    {
        int resultRotateMap = 0;


        //DFS로 확정된 "경로 좌표"를 저장한다 (prev/current/next 관계로 타일 타입 판정에 사용)
        List<(int, int)> mapIndex = new List<(int, int)>();

        //이전에 만든 맵이 있으면 전부 파괴 후 재생성
        if (createdMap.Count > 0)
        {
            while (createdMap.Count > 0)
            {
                Destroy(createdMap[0]);
                createdMap.RemoveAt(0);
            }
        }


        //방문 여부 체크용: true면 이미 경로에 포함된 좌표
        bool[,] map;

        //여유 공간을 크게 잡아 DFS가 좌표 경계에 걸릴 확률을 낮춘다
        map = new bool[mapMakeCount, mapMakeCount];

        //처음 시작하는 맵 위치는 이곳이다.
        int firstMapPosition = mapMakeCount / 2;
        map[firstMapPosition, firstMapPosition] = true;
        mapIndex.Add((firstMapPosition, firstMapPosition));

        //생성 결과를 GameData에 기록 (추후 배치/저장/디버그에 활용)
        GameData.instance.mapData.spawnedMapX.Add(firstMapPosition);
        GameData.instance.mapData.spawnedMapZ.Add(firstMapPosition);




        currentX = (firstMapPosition);
        currentZ = (firstMapPosition) + 1;

        //고정적으로 위로 올라가기 때문에 첫 맵 위치에서 Z + 1한 채로 고정설정된다.
        //=> 시작 방 다음 타일을 "일단 1개" 확정하고 DFS를 시작한다
        map[currentX, currentZ] = true;
        mapIndex.Add((currentX, currentZ));
        int currentMakeCount = 2;

        //목표 맵 생성을 이루기 까지 DFS 실행한다
        while (currentMakeCount < mapMakeCount)
        {
            //현재 좌표에서 만들 수 있는 다음 후보(상하좌우 중, 아직 방문하지 않은 칸)
            List<(int, int)> canMakeMapList = new List<(int, int)>();

            if (currentX - 1 >= 0)
            {
                if (map[currentX - 1, currentZ] == false)
                {
                    canMakeMapList.Add(((currentX - 1), currentZ));
                }
            }

            if (currentX + 1 <= mapMakeCount - 1)
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

            if (currentZ + 1 <= mapMakeCount - 1)
            {
                if (map[currentX, currentZ + 1] == false)
                {
                    canMakeMapList.Add(((currentX), currentZ + 1));
                }
            }

            //제작할 수 있는 맵의 경우의 수라면 데이터 삽입 및 진전
            if (canMakeMapList.Count > 0)
            {
                //순수 확률 기반으로 가능한 타일들을 추려낸 것들을 추첨하여 맵을 연결한다
                //=> DFS지만 "랜덤 선택"을 넣어서 매번 다른 경로가 나오도록 구성
                (int, int) targetMap = canMakeMapList[Random.Range(0, canMakeMapList.Count)];
                //float resultRotateY = currentMapRotateY + targetMap.Item4;

                //현재 좌표를 "결과 데이터"에 기록
                //=> 이후 시스템(배치/저장/디버그)이 이 정보를 기반으로 동작
                GameData.instance.mapData.spawnedMapX.Add(currentX);
                GameData.instance.mapData.spawnedMapZ.Add(currentZ);

                //currentMapRotateY = resultRotateY;

                //다음 좌표 방문 처리 + 경로 리스트에 추가
                map[targetMap.Item1, targetMap.Item2] = true;
                mapIndex.Add((targetMap.Item1, targetMap.Item2));

                currentMakeCount++;

                //DFS 진행: 현재 좌표를 다음 좌표로 갱신
                currentX = targetMap.Item1;
                currentZ = targetMap.Item2;
            }
            else
            {
                //막히는 경우(더 이상 진행 불가) -> 백트래킹
                //=> 마지막 좌표를 경로에서 제거하고 이전 좌표로 돌아간다

                //주의: RemoveAt 인덱스는 Count-1이 마지막 요소다
                GameData.instance.mapData.spawnedMapX.RemoveAt(GameData.instance.mapData.spawnedMapX.Count - 1);
                GameData.instance.mapData.spawnedMapZ.RemoveAt(GameData.instance.mapData.spawnedMapZ.Count - 1);
                //mapData.mapType.RemoveAt(mapData.mapType.Count - 1);
                //mapData.spawnedMapRotationY.RemoveAt(mapData.spawnedMapRotationY.Count - 1);

                //현재 칸 방문 해제
                map[currentX, currentZ] = false;

                //경로도 마지막을 제거
                mapIndex.RemoveAt(mapIndex.Count - 1);
                currentMakeCount--;

                //이전 좌표로 복귀
                var last = mapIndex[mapIndex.Count - 1];
                currentX = last.Item1;
                currentZ = last.Item2;
            }

            canMakeMapList.Clear();

        }

        //최종 도착 좌표도 기록(끝 타일 계산에 사용)
        GameData.instance.mapData.spawnedMapX.Add(currentX);
        GameData.instance.mapData.spawnedMapZ.Add(currentZ);

        Vector3 spawnPos = Vector3.zero;

        //확정된 경로(mapIndex)를 바탕으로 실제 타일 프리팹을 Instantiate 한다
        //중간 타일은 prev/current/next 관계를 보고 타입과 회전이 결정된다
        for (int i = 0; i < mapIndex.Count - 1; i++)
        {
            spawnPos = new Vector3(mapIndex[i].Item1 * mapSize, 0, mapIndex[i].Item2 * mapSize);


            if (i == 0)
            {
                //시작 타일은 고정 프리팹
                createdMap.Add(Instantiate(StartGround, spawnPos, transform.rotation));
                GameData.instance.mapData.mapType.Add(MAPTYPE.START);
            }
            else
            {
                //현재 타일이 "직선인지/코너인지"는 이전과 다음 좌표 관계로 판단
                //CalMiddleMapsType에서 mapType과 mapRotate도 같이 기록한다
                int middleX = mapIndex[i + 1].Item1 - mapIndex[i - 1].Item1;
                int middleZ = mapIndex[i + 1].Item2 - mapIndex[i - 1].Item2;

                GameObject wantMap = CalMiddleMapsType(mapIndex[i - 1], mapIndex[i], mapIndex[i + 1]);
                GameObject createMap = Instantiate(wantMap, spawnPos, transform.rotation);
                createdMap.Add(createMap);

                //맵 생성과 동시에 "배치 시스템"과 연동:
                //타일 인덱스를 기반으로, 커스텀 에디터에서 정의한 스폰/배치 데이터를 호출한다
                createMap.GetComponent<MapMiddleZone>().ActorSpawn(i - 1);

                //타일 타입 판정 시 함께 계산된 회전값을 적용해 길 연결이 자연스럽게 이어지도록 한다
                resultRotateMap = GameData.instance.mapData.mapRotate[GameData.instance.mapData.mapRotate.Count - 1];
                createdMap[createdMap.Count - 1].transform.Rotate(0, resultRotateMap, 0);
            }
        }

        //마지막 좌표는 EndGround(보스 방)으로 생성한다
        spawnPos = new Vector3(mapIndex[mapIndex.Count - 1].Item1 * mapSize, 0, mapIndex[mapIndex.Count - 1].Item2 * mapSize);

        //끝 타일의 방향은 "마지막-직전" 이동 방향 벡터로 결정한다
        int endX = mapIndex[mapIndex.Count - 1].Item1 - mapIndex[mapIndex.Count - 2].Item1;
        int endZ = mapIndex[mapIndex.Count - 1].Item2 - mapIndex[mapIndex.Count - 2].Item2;
        CreateEndGround(endX, endZ, spawnPos);
    }

    GameObject CalMiddleMapsType((int, int) prevMap, (int, int) currentMap, (int, int) nextMap)
    {
        int x = 0;
        int z = 0;

        //직선 판정(세로): prev -> current -> next 가 동일 X축이면 세로 직선
        if (prevMap.Item1 == currentMap.Item1 && currentMap.Item1 == nextMap.Item1)
        {
            GameData.instance.mapData.mapType.Add(MAPTYPE.Straight);
            GameData.instance.mapData.mapRotate.Add(90); //세로 방향 회전값
            return StraightGround;
        }

        //직선 판정(가로): prev -> current -> next 가 동일 Z축이면 가로 직선
        if (prevMap.Item2 == currentMap.Item2 && currentMap.Item2 == nextMap.Item2)
        {
            GameData.instance.mapData.mapType.Add(MAPTYPE.Straight);
            GameData.instance.mapData.mapRotate.Add(0);  //가로 방향 회전값
            return StraightGround;
        }



        //코너 판정: 현재를 기준으로 좌/우 방향 변화가 있는지 계산
        if (prevMap.Item1 < currentMap.Item1 || nextMap.Item1 < currentMap.Item1)
        {
            x = 1;
        }
        else
        {
            x = -1;
        }

        //코너 판정: 현재를 기준으로 상/하 방향 변화가 있는지 계산
        if (prevMap.Item2 > currentMap.Item2 || nextMap.Item2 > currentMap.Item2)
        {
            z = 1;
        }
        else
        {
            z = -1;
        }

        //같은 좌표면 예외(경로 생성상 정상 케이스가 아니므로 방어)
        if (x == 0 && z == 0)
        {
            Debug.LogWarning("prevMap과 nextMap이 같음");
            return null;
        }

        //여기까지 오면 코너 타일:
        //방향 조합(x,z)에 따라 코너 프리팹은 동일하되 회전값만 달리 적용
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
        //끝 타일(보스 방)의 방향은 마지막 이동 벡터(resultX, resultZ)로 결정한다
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

        //END 타입 및 회전값 기록(디버그/재현/툴 연동에 활용 가능)
        GameData.instance.mapData.mapType.Add(MAPTYPE.END);
        GameData.instance.mapData.mapRotate.Add(resultRotateY);

    }

    public int GetMapMakeCount() => mapMakeCount;
}
