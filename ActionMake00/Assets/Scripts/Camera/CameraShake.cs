using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    Coroutine co;
    [SerializeField]
    private float m_roughness;      //거칠기 정도
    [SerializeField]
    private float m_magnitude;      //움직임 범위

    [SerializeField] float duration;

    private void Update()
    {
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
        float halfDuration = duration / 2;
        float elapsed = 0f;
        float tick = Random.Range(-10f, 10f);

        while (elapsed < duration)
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
}
