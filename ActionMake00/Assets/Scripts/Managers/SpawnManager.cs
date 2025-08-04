using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject commonEnemy, BossEnemy;
    private List<Enemy> lEnemy;                        // 고정 인덱스 유지 리스트
    public Transform commonEnemySpawn;                // 부모 트랜스폼 (자식들이 스폰 위치)

    public static SpawnManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    void Start()
    {
        Init();
    }

    private void Init()
    {
        int spawnCount = commonEnemySpawn.childCount;
        lEnemy = new List<Enemy>(new Enemy[spawnCount]);  // 고정 크기로 초기화

        for (int i = 0; i < spawnCount; i++)
        {
            lEnemy[i] = SpawnCommonEnemy(i, commonEnemySpawn.GetChild(i));
        }
    }

    public void DestroyCommonEnemy(Enemy enemy)
    {
        int index = enemy.GetEnemyIndex();     // 삭제 전 인덱스 저장

        if (index < 0 || index >= lEnemy.Count || lEnemy[index] != enemy)
        {
            Debug.LogWarning($"Enemy index mismatch or invalid: {index}");
            return;
        }

        lEnemy[index] = null;                  // 자리 비워둠
        //Destroy(enemy.gameObject);

        StartCoroutine(IE_SpawnCommonEnemy(index));
    }

    Enemy SpawnCommonEnemy(int index, Transform tr)
    {
        GameObject instance = Instantiate(commonEnemy, tr.position, tr.rotation);
        Enemy enemy = instance.GetComponent<Enemy>();
        enemy.SetEnemyIndex(index);
        return enemy;
    }

    IEnumerator IE_SpawnCommonEnemy(int index)
    {
        yield return new WaitForSeconds(2f);

        // 해당 위치에 다시 생성
        lEnemy[index] = SpawnCommonEnemy(index, commonEnemySpawn.GetChild(index));
    }
}
