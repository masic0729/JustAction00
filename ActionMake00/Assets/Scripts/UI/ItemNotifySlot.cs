using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemNotifySlot : MonoBehaviour
{
    // 아이템 아이콘을 표시하는 이미지 컴포넌트
    [SerializeField] Image iconImage;

    // 페이드 인아웃 처리용 캔버스 그룹
    [SerializeField] CanvasGroup canvasGroup;

    // 슬롯 초기화 진입점
    public void Init(Sprite icon, float lifetime)
    {
        iconImage.sprite = icon;
        canvasGroup.alpha = 0f;

        StartCoroutine(LifetimeRoutine(lifetime));
    }

    // 페이드인 유지 페이드아웃 순서로 진행 후 오브젝트 제거
    IEnumerator LifetimeRoutine(float lifetime)
    {
        yield return StartCoroutine(FadeRoutine(0f, 1f, 0.15f));
        yield return new WaitForSeconds(lifetime);
        yield return StartCoroutine(FadeRoutine(1f, 0f, 0.3f));

        Destroy(gameObject);
    }

    // CanvasGroup alpha를 from에서 to로 duration초 동안 보간
    IEnumerator FadeRoutine(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}