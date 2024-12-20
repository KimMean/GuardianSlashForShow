using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicGridLayoutSizeHandler : MonoBehaviour
{
    GridLayoutGroup gridLayoutGroup;

    // �� ������ �� ������ ���� (���ϴ� ������ ���� ����)
    [SerializeField] int columnCount; // �� �ٿ� �� ���� ���� ��ġ����
    [SerializeField] int rowCount; // �� ���� �� ���� ���� ��ġ����

    private void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        AdjustCellSize();
    }

    private void AdjustCellSize()
    {
        RectTransform parentRect = GetComponentInParent<RectTransform>();
        // �θ� RectTransform�� ũ��
        float parentWidth = parentRect.rect.width;
        float parentHeight = parentRect.rect.height;

        // �� ũ�� ���
        float cellWidth = (parentWidth - gridLayoutGroup.spacing.x * (columnCount - 1) - gridLayoutGroup.padding.horizontal) / columnCount;
        float cellHeight = (parentHeight - gridLayoutGroup.spacing.y * (rowCount - 1) - gridLayoutGroup.padding.vertical) / rowCount;

        // GridLayoutGroup�� Cell Size�� ����
        gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
    }
}
