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
        // ���� ���� ���� ��������

        Rect safeArea = Screen.safeArea;

        // �θ��� ũ�� �������� ���� ���
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = anchorMin + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // RectTransform�� ��Ŀ �� ������Ʈ
        uiPanel.anchorMin = anchorMin;
        uiPanel.anchorMax = anchorMax;
    }
}
