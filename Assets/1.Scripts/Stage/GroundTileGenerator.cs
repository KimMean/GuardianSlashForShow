using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTileGenerator : MonoBehaviour
{
    [SerializeField] GameObject groundTilePrefab;

    private List<GameObject> groundTiles;

    private void Awake()
    {
        groundTiles = new List<GameObject>();
    }
    private void OnEnable()
    {
        TileGenerate();
    }

    private void OnDisable()
    {
        foreach (var tile in groundTiles)
        {
            Destroy(tile);
        }
        groundTiles.Clear();
    }

    /// <summary>
    /// 지면을 생성합니다.
    /// </summary>
    private void TileGenerate()
    {
        if (groundTilePrefab == null) return;

        Camera camera = Camera.main;
        // 화면의 왼쪽 하단과 오른쪽 하단 좌표 계산
        Vector3 leftBottomCorner = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        Vector3 rightBottomCorner = camera.ViewportToWorldPoint(new Vector3(1, 0, camera.nearClipPlane));

        // 스프라이트의 너비 계산
        SpriteRenderer spriteRenderer = groundTilePrefab.GetComponent<SpriteRenderer>();
        float spriteWidth = spriteRenderer.bounds.size.x;

        // 스프라이트 배치
        for (float x = leftBottomCorner.x; x <= rightBottomCorner.x + spriteWidth; x += spriteWidth)
        {
            Vector3 position = new Vector3(x, leftBottomCorner.y, 0);
            GameObject obj = Instantiate(groundTilePrefab, position, Quaternion.identity, this.transform);

            groundTiles.Add(obj);
        }
    }
}
