using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaHandler : MonoBehaviour
{
    private RectTransform uiPanel;
    void Start()
    {
        uiPanel = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void ApplySafeArea()
    {
        // 안전 영역 정보 가져오기

        Rect safeArea = Screen.safeArea;

        // 부모의 크기 기준으로 비율 계산
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = anchorMin + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // RectTransform의 앵커 값 업데이트
        uiPanel.anchorMin = anchorMin;
        uiPanel.anchorMax = anchorMax;
    }
}
