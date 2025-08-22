using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [SerializeField] CameraShake mainCamera;
    public float transPosY, transPosZ;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        FollowCamera();
    }

    void Init()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<CameraShake>();
    }

    /// <summary>
    /// 플레이어의 위치와 해당 스크립트의 설정값을 기반으로 조정
    /// </summary>
    void FollowCamera()
    {
        Vector3 cameraPosition = new Vector3(transform.position.x, transform.position.y + transPosY, transform.position.z + transPosZ);
        mainCamera.transform.position = cameraPosition;
    }

    public void PlayCameraShake(float multify = 1f)
    {
        mainCamera.PlayCameraShake(multify);
    }
}
