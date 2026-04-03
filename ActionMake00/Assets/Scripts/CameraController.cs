using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    [SerializeField] Transform cameraPivot;
    [SerializeField] CameraAction mainCamera;

    float currentCameraRotateY;
    float currentCameraRotateX;
    float rotateSpeed = 180f;
    bool canRotate = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        Init();
    }

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
        mainCamera = GameObject.Find("Main Camera").GetComponent<CameraAction>();
        currentCameraRotateY = mainCamera.transform.rotation.y;
        currentCameraRotateX = 15f; // 초기 X각도 (기존 고정값 유지)
    }

    /// <summary>
    /// 플레이어의 위치와 해당 스크립트의 설정값을 기반으로 조정
    /// </summary>
    void FollowCamera()
    {
        Vector3 cameraPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        cameraPivot.transform.position = cameraPosition;
    }

    /// <summary>
    /// 플랫폼에 따라 카메라를 회전하는 방식을 조정한다
    /// </summary>
    public void RotateCameraPivot()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        currentCameraRotateY += mouseX * rotateSpeed * Time.deltaTime;

        currentCameraRotateX -= mouseY * rotateSpeed * Time.deltaTime;
        currentCameraRotateX = Mathf.Clamp(currentCameraRotateX, -30f, 30f);

        cameraPivot.rotation = Quaternion.Euler(currentCameraRotateX, currentCameraRotateY, 0f);
        mainCamera.transform.localPosition = new Vector3(0, 2, -3);
    }

    public void PlayCameraShake(float multify = 1f)
    {
        mainCamera.PlayCameraShake(multify);
    }

    public void SetCanRotate(bool state) => canRotate = state;
    public bool GetCanRotate() => canRotate;
}