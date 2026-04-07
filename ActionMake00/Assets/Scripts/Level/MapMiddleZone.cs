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
        if (mapIndex > MapMaker.instance.GetMapMakeCount())
        {
            return;
        }

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

    // SpawnEnemies 수정
    // 스폰 후 PatrolPointManager에서 포인트를 배정한다
    void SpawnEnemies(List<SpawnEntry> enemyDatas)
    {
        Transform spawnEnemy = transform.Find("ENEMY");

        // PatrolPos 자식에서 PatrolPointManager를 찾는다
        Transform patrolPosRoot = transform.Find("PatrolPos");
        PatrolPointManager patrolManager = patrolPosRoot?.GetComponent<PatrolPointManager>();

        for (int i = 0; i < enemyDatas.Count; i++)
        {
            for (int j = 0; j < enemyDatas[i].spawnCount; j++)
            {
                // 반환값을 받아야 포인트 배정이 가능하므로 반환형 변경
                GameObject spawnedObj = SpawnEnemyAroundSpawnPoint(enemyDatas[i].enemyPrefab, spawnEnemy);

                if (patrolManager != null && spawnedObj != null)
                {
                    Enemy enemy = spawnedObj.GetComponent<Enemy>();
                    Transform point = patrolManager.ReserveRandomPoint();

                    // 포인트가 없으면 배정 없이 스폰만 유지
                    if (enemy != null && point != null)
                        enemy.SetAssignedPatrolPos(point, patrolManager);
                }
            }
        }
    }

    // 반환형을 GameObject로 변경해야 위 코드가 동작한다
    GameObject SpawnEnemyAroundSpawnPoint(GameObject spawnActor, Transform spawnTransform)
    {
        GameObject ins = Instantiate(spawnActor, spawnTransform.position, spawnTransform.rotation);
        float randX = Random.Range(-3f, 3f);
        float randZ = Random.Range(-3f, 3f);
        float randRotateY = Random.Range(0f, 359f);
        ins.transform.Translate(randX, 0, randZ);
        ins.transform.Rotate(0, randRotateY, 0);
        ins.transform.parent = this.transform;
        return ins;   // 배정을 위해 반환
    }

    void SpawnNPC(GameObject npc)
    {
        Transform spawnNPC = transform.Find("NPC");
        SpawnNPC_AroundSpawnPoint(npc, spawnNPC);

    }


    void SpawnNPC_AroundSpawnPoint(GameObject spawnActor, Transform spawnTransform)
    {
        GameObject ins = Instantiate(spawnActor, spawnTransform.position, spawnTransform.rotation);

        ins.transform.parent = this.transform;

    }
}
