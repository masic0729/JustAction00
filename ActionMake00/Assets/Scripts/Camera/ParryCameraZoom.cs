using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ParryCameraZoom : MonoBehaviour
{
    [SerializeField] Camera cam;

    [Header("Ortho Size (smaller = zoom in)")]
    [SerializeField] float zoomNormal = 5.0f;
    [SerializeField] float zoomReady = 4.6f;  // 살짝 확대
    [SerializeField] float zoomSuccess = 5.6f; // 성공 시 살짝 축소(와이드)

    Tween zoomTween;
    bool parryWindowActive;

    void Awake()
    {
        if (!cam) cam = Camera.main;
        cam.orthographicSize = zoomNormal;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F9))
        {
            OnParryWindowStart();
            Debug.Log("실행은 됨");
        }
        if (Input.GetKeyDown(KeyCode.F10))
        {
            OnParryWindowEnd_NoSuccess();
            Debug.Log("실행은 됨");

        }
        if (Input.GetKeyDown(KeyCode.F11))
        {
            Debug.Log("실행은 됨");
            OnParrySuccess();
        }
    }

    public void OnParryWindowStart()
    {
        parryWindowActive = true;
        KillZoom();
        zoomTween = cam.DOOrthoSize(zoomReady, 0.10f).SetEase(Ease.OutQuad);
    }

    public void OnParryWindowEnd_NoSuccess()
    {
        parryWindowActive = false;
        KillZoom();
        zoomTween = cam.DOOrthoSize(zoomNormal, 0.16f).SetEase(Ease.OutCubic);
    }

    public void OnParrySuccess()
    {
        parryWindowActive = false;
        KillZoom();

        // 성공: 더 와이드로 “쾌감” -> 원복
        Sequence seq = DOTween.Sequence();
        seq.Append(cam.DOOrthoSize(zoomSuccess, 0.08f).SetEase(Ease.OutBack));
        seq.Append(cam.DOOrthoSize(zoomNormal, 0.22f).SetEase(Ease.OutCubic));

        zoomTween = seq;
    }

    void KillZoom()
    {
        if (zoomTween != null && zoomTween.IsActive())
            zoomTween.Kill();
    }
}
