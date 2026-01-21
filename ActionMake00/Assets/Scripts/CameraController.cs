using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    [SerializeField] Transform cameraPivot;
    [SerializeField] CameraShake mainCamera;

    float currentCameraRotateY;
    float rotateSpeed = 180f;

    bool canRotate = true;

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
        if (canRotate == false)
            return;
        FollowCamera();
        RotateCameraPivot();
    }

    void Init()
    {
        if (mainCamera != null)
            return;
        mainCamera = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        currentCameraRotateY = mainCamera.transform.rotation.y;
    }

    /// <summary>
    /// 플레이어의 위치와 해당 스크립트의 설정값을 기반으로 조정
    /// </summary>
    void FollowCamera()
    {
        Vector3 cameraPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        cameraPivot.transform.position = cameraPosition;
    }

    public void RotateCameraPivot()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");

        currentCameraRotateY += mouseX * rotateSpeed * Time.deltaTime;
        cameraPivot.rotation = Quaternion.Euler(15f, currentCameraRotateY, 0f);

        mainCamera.transform.localPosition = new Vector3(0, 2, -3);
    }
    public void PlayCameraShake(float multify = 1f)
    {
        mainCamera.PlayCameraShake(multify);
    }


    public void SetCanRotate(bool state) => canRotate = state;

    public bool GetCanRotate() => canRotate;

}
