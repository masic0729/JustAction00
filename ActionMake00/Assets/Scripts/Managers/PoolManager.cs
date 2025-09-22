using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;
    [SerializeField] PoolData[] prefabData;
    public Dictionary<string, IObjectPool<GameObject>>  skillPrefabs;


    

    private void Awake()
    {
        if (instance == null)
            instance = this;


        skillPrefabs = new Dictionary<string, IObjectPool<GameObject>>();
    
        for(int i = 0; i < prefabData.Length; i++)
        {
            for(int j = 0; j < prefabData[i].GameObjects.Length; j++)
            {
                GameObject prefab = prefabData[i].GameObjects[j];                           // 데이터 임시 저장
                string poolName = prefab.name;                                              //오브젝트 이름 저장

                skillPrefabs[poolName] = new ObjectPool<GameObject>(
                    createFunc: () => Instantiate(prefab),                                  //직접 참조가 아닌, 데이터를 미리 저장 후 할당한다
                    actionOnGet: pool => pool.SetActive(true),
                    actionOnRelease: pool => pool.SetActive(false),
                    actionOnDestroy: pool => Destroy(pool),
                    maxSize: 5
                );
            }
        }
    }

    public void Spawn(string effectName, Vector3 pos, Quaternion rot)
    {
        GameObject pool = skillPrefabs[effectName].Get();
        Debug.Log("풀 성공" + pool.name);
        pool.transform.SetPositionAndRotation(pos, rot);
        pool.transform.name = effectName;

        ParticleSystem ps = pool.GetComponent<ParticleSystem>();
    }


}
     