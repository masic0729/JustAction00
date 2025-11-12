using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseTracerImage : MonoBehaviour
{
    public RectTransform canvasRect;   // 부모 캔버스
    public RectTransform targetImage;  // 마우스 따라올 이미지

    void Update()
    {
        Vector2 localPoint;
        // 마우스 위치 → 캔버스 로컬 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            canvasRect.GetComponent<Canvas>().worldCamera,
            out localPoint
        );

        // 이미지 위치 갱신
        targetImage.localPosition = localPoint;
    }
}
