using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.TextCore.Text;

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

    public GameObject Spawn(string effectName, Vector3 pos, Quaternion rot)
    {
        GameObject pool = skillPrefabs[effectName].Get();
        Debug.Log("풀 성공" + pool.name);

        pool.transform.SetPositionAndRotation(pos, rot);
        pool.transform.name = effectName;

        /*ParticleSystem ps = pool.GetComponent<ParticleSystem>();
        ps.Play();*/
        return pool;
    }

    /// <summary>
    /// 해당 함수의 경우 특정 파티클 오브젝트의 회전값이 예민한 경우
    /// 이를 고려하여 해당 파티클 오브젝트를  반환 후 파티클에 맞는 위치 및 회전값을 조정한다
    /// </summary>
    /// <param name="effectName"></param>
    /// <returns></returns>
    public GameObject Spawn(string effectName, Vector3 pos)
    {
        GameObject pool = skillPrefabs[effectName].Get();
        Debug.Log("풀 성공" + pool.name);

        pool.transform.name = effectName;

        pool.transform.position = pos;
        /*ParticleSystem ps = pool.GetComponent<ParticleSystem>();

        ps.Play();*/
        return pool;
    }

    /// <summary>
    /// 캐릭터 공격을 위한 풀링의 경우, 시전자의 데이터도 최신화한다
    /// </summary>
    /// <param name="effectName"></param>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    /// <param name="character"></param>
    /// <returns></returns>
    public GameObject Spawn(string effectName, Vector3 pos, Quaternion rot, Character character)
    {
        GameObject pool = skillPrefabs[effectName].Get();
        Debug.Log("풀 성공" + pool.name);

        pool.transform.SetPositionAndRotation(pos, rot);
        pool.transform.name = effectName;
        pool.GetComponent<Attacker>().SetOwner(character);

        return pool;
    }

    /// <summary>
    /// 이하 동일
    /// </summary>
    /// <param name="effectName"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public GameObject Spawn(string effectName, Vector3 pos, Character character)
    {
        GameObject pool = skillPrefabs[effectName].Get();
        Debug.Log("풀 성공" + pool.name);

        pool.transform.name = effectName;

        pool.transform.position = pos;
        pool.GetComponent<Attacker>().SetOwner(character);

        /*ParticleSystem ps = pool.GetComponent<ParticleSystem>();

        ps.Play();*/
        return pool;
    }
}
     