using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class MapMiddleZone : MonoBehaviour
{
    int mapDataIndex = -1;

    public void ActorSpawn(int mapIndex)
    {
        mapDataIndex = mapIndex;
        TileContentConfig data = MapDesign.instance.GetMiddleTiles()[mapDataIndex];
        switch(data.contentType)
        {
            case TileContentType.None:
                Debug.Log("타입이 없음. 확인 요망");
                
                break;
            case TileContentType.Monster:
                SpawnEnemies(data.monsterPreset.entries);
                break;
            case TileContentType.NPC:
                SpawnNPC(data.npcPrefab);
                break;
        }

    }

    void SpawnEnemies(List<SpawnEntry> enemyDatas)
    {
        Transform spawnEnemy = transform.Find("ENEMY");
        for(int i = 0; i < enemyDatas.Count; i++)
        {
            for(int j = 0; j < enemyDatas[i].spawnCount; j++)
            {
                SpawnActorAroundSpawnPoint(enemyDatas[i].enemyPrefab, spawnEnemy);
            }
        }
    }

    void SpawnNPC(GameObject npc)
    {
        Transform spawnNPC = transform.Find("NPC");
        SpawnActorAroundSpawnPoint(npc, spawnNPC);

    }

    /// <summary>
    /// 각 액터들의 생성 포인트를 기준으로
    /// 변동 값을 주어 다양한 위치에 생성하는 방식
    /// 몬스터 NPC 모두 같은 방식으로 소환한다
    /// 
    /// 추후 후순위 적으로 json기반 맵 저장 시,
    /// 각 객체의 위치, 체력 등등 기본 데이터를 저장해야함
    /// </summary>
    /// <param name="spawnActor">생성하려는 오브젝트</param>
    /// <param name="spawnTransform">생성하려는 위치 베이스</param>
    void SpawnActorAroundSpawnPoint(GameObject spawnActor, Transform spawnTransform)
    {
        GameObject ins = Instantiate(spawnActor, spawnTransform.position, spawnTransform.rotation);

        float randX = 0f, randZ = 0f;

        randX = Random.Range(-3f, 3f);
        randZ = Random.Range(-3f, 3f);

        ins.transform.Translate(randX, 0, randZ);
    }
}
