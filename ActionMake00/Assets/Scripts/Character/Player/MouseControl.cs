using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MouseControl : MonoBehaviour
{
    public static MouseControl instance;
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

    public enum AimCursorMode { LockedCenter, ConfinedWindow, Free }

    [Header("Start Mode")]
    [SerializeField] AimCursorMode modeAtStart = AimCursorMode.LockedCenter;

    void Start()
    {
        Apply(modeAtStart);

    }

    // Alt+Tab / 창 전환 후 복구
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) Apply(modeAtStart);
    }

    /// <summary>
    /// 커서 모드 적용:
    /// - LockedCenter: 화면 중앙 고정 + 커서 숨김(원신 스타일)
    /// - ConfinedWindow: 창 밖으로 못 나가게 + 커서 숨김(윈도우 모드 권장)
    /// - Free: 잠금 해제 + 커서 표시(메뉴/UI용)
    /// </summary>
    public void Apply(AimCursorMode mode)
    {
        modeAtStart = mode;

        CameraController cameraCtrl = GetComponent<CameraController>();
        PlayerController playerCtrl = GetComponent<PlayerController>();

        switch (mode)
        {

            case AimCursorMode.LockedCenter:
                //Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                cameraCtrl.SetCanRotate(true);
                break;

            case AimCursorMode.ConfinedWindow:
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                cameraCtrl.SetCanRotate(false);

                break;

            case AimCursorMode.Free:
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                cameraCtrl.SetCanRotate(false);

                break;

            default:
                //Cursor.lockState = CursorLockMode.Confined;
                Debug.Log("마우스 커서 설정 간 예외 발생");
                Cursor.visible = true;
                //playerCtrl.SetCanAnyInput(false);
                //Apply(MouseControl.AimCursorMode.Free);
                cameraCtrl.SetCanRotate(false);
            break;
        }
    }
}
