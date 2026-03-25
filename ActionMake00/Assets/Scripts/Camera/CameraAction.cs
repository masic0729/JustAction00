using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CameraAction : MonoBehaviour
{
    Coroutine co;
    [Header("카메라의 흔들림에 관여하는 데이터")]
    [SerializeField]
    private float m_roughness;      //거칠기 정도
    [SerializeField]
    private float m_magnitude;      //움직임 범위

    [SerializeField] float shakeDuration;   //카메라 흔들림 시간

    float cameraViewDefault;   //카메라 초기 확대값
    float cameraViewAction;    //카메라 액션 확대값

    string playZoomID = null;   //현재 실행중인 줌 트윈 아이디. null일 경우 실행중이 아님

    private void Start()
    {
        //초기 뷰 값을 구한 후, 액션에 사용할 값에 활용하여 조정한다
        if(GetComponent<Camera>().fieldOfView < 20f)
        {
            cameraViewDefault = 20f;
        }
        else
        {
            cameraViewDefault = GetComponent<Camera>().fieldOfView;
        }
        cameraViewAction = cameraViewAction - 20f;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5))
        {
            DOTween.Complete("HARDZOOM");
            DOTween.Restart("NORMALZOOM");
            //playZoomID = "NORMALZOOM";
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            DOTween.Complete("NORMALZOOM");
            DOTween.Restart("HARDZOOM");
            //playZoomID = "HARDZOOM";
        }
    }

    public void PlayCameraShake(float shakeMultyfy = 1f)
    {
        if(co != null)
        {
            StopCoroutine(co);
            co = null;
        }
        co = StartCoroutine(Shake(shakeMultyfy));
    }

    IEnumerator Shake(float shakeMultify)
    {
        Debug.Log("내가 실행됨");
        float halfDuration = shakeDuration / 2;
        float elapsed = 0f;
        float tick = Random.Range(-10f, 10f);

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime / halfDuration;

            tick += Time.deltaTime * m_roughness * shakeMultify;
            transform.position += new Vector3(
                Mathf.PerlinNoise(tick, 0) - .5f,
                Mathf.PerlinNoise(0, tick) - .5f,
                0f) * m_magnitude * shakeMultify * Mathf.PingPong(elapsed, halfDuration);

            yield return null;
        }
    }
    public void OnZoomStart(string zoomID)
    {
        playZoomID = zoomID;
    }

    public void OnZoomEnd()
    {
        playZoomID = null;
    }
}
