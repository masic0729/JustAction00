using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    [SerializeField] PoolData[] prefabData;
    Dictionary<string, IObjectPool<GameObject>>  skillPrefabs;


    

    private void Awake()
    {
        skillPrefabs = new Dictionary<string, IObjectPool<GameObject>>();
    
        for(int i = 0; i < prefabData.Length; i++)
        {
            for(int j = 0; i < prefabData[i].GameObjects.Length; j++)
            {
                string gameObjectName = prefabData[i].GameObjects[j].name;
                skillPrefabs[gameObjectName] = new ObjectPool<GameObject>(
                    createFunc: () => Instantiate(prefabData[i].GameObjects[j]),
                    actionOnGet: go => go.SetActive(true),
                    actionOnRelease: go => go.SetActive(false),
                    actionOnDestroy: go => Destroy(go),
                    maxSize: 10  // ªÛ«—º±
                );
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
     