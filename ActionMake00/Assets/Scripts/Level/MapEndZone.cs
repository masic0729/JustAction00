using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapEndZone : MonoBehaviour
{
    Vector3 bossSpawn;
    NavMeshSurface navSurface;
    [SerializeField] GameObject boss;

    // Start is called before the first frame update
    void Start()
    {
        bossSpawn = transform.Find("BOSS").position;
        navSurface = GetComponent<NavMeshSurface>();
        navSurface.BuildNavMesh();

        if (GameManager.instance.GetIsTest() == false)
        {
            GameObject ins = Instantiate(boss, bossSpawn, transform.rotation);
        }
        //GameObject.Find("Boss_Main").transform.position = bossSpawn;


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
