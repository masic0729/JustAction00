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

        //if(GameManager.instance.GetIsTest() == false)
        {
            GameObject ins = GameObject.FindWithTag("Player");
            ins.transform.position = playerSpawnZone;

        }

    }

}