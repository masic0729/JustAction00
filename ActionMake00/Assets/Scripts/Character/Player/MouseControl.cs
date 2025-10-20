using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MouseControl : MonoBehaviour
{
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
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                playerCtrl.SetCanAnyInput(true);
                //Apply(MouseControl.AimCursorMode.LockedCenter);
                cameraCtrl.SetCanRotate(true);
                break;

            case AimCursorMode.ConfinedWindow:
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;
                break;

            case AimCursorMode.Free:
            default:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                playerCtrl.SetCanAnyInput(false);
                //Apply(MouseControl.AimCursorMode.Free);
                cameraCtrl.SetCanRotate(false);

                
                break;
        }
    }
}
