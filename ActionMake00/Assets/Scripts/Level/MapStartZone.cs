using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapStartZone : MonoBehaviour
{
    Vector3 playerSpawnZone;

    // Start is called before the first frame update
    void Start()
    {
        playerSpawnZone = transform.Find("PLAYER").position;

        if(SceneManager.GetActiveScene().name != "TestScene")
        {
            GameObject.FindWithTag("Player").transform.position = playerSpawnZone;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
