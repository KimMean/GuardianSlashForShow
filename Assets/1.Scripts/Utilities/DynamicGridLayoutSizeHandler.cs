using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicGridLayoutSizeHandler : MonoBehaviour
{
    GridLayoutGroup gridLayoutGroup;

    // 열 개수와 행 개수를 설정 (원하는 값으로 조정 가능)
    [SerializeField] int columnCount; // 한 줄에 몇 개의 셀을 배치할지
    [SerializeField] int rowCount; // 한 열에 몇 개의 셀을 배치할지

    private void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        AdjustCellSize();
    }

    private void AdjustCellSize()
    {
        RectTransform parentRect = GetComponentInParent<RectTransform>();
        // 부모 RectTransform의 크기
        float parentWidth = parentRect.rect.width;
        float parentHeight = parentRect.rect.height;

        // 셀 크기 계산
        float cellWidth = (parentWidth - gridLayoutGroup.spacing.x * (columnCount - 1) - gridLayoutGroup.padding.horizontal) / columnCount;
        float cellHeight = (parentHeight - gridLayoutGroup.spacing.y * (rowCount - 1) - gridLayoutGroup.padding.vertical) / rowCount;

        // GridLayoutGroup의 Cell Size를 조정
        gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
    }
}
