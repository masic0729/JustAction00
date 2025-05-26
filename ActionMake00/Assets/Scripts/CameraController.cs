using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject player;
    public float transPosY, transPosZ;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
    }

    void Init()
    {
        player = GameObject.Find("Player");
    }

    void FollowPlayer()
    {
        Vector3 cameraPosition = new Vector3(player.transform.position.x, player.transform.position.y + transPosY, player.transform.position.z + transPosZ);
        transform.position = cameraPosition;
    }
}
