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
    /// ������ �����մϴ�.
    /// </summary>
    private void TileGenerate()
    {
        if (groundTilePrefab == null) return;

        Camera camera = Camera.main;
        // ȭ���� ���� �ϴܰ� ������ �ϴ� ��ǥ ���
        Vector3 leftBottomCorner = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        Vector3 rightBottomCorner = camera.ViewportToWorldPoint(new Vector3(1, 0, camera.nearClipPlane));

        // ��������Ʈ�� �ʺ� ���
        SpriteRenderer spriteRenderer = groundTilePrefab.GetComponent<SpriteRenderer>();
        float spriteWidth = spriteRenderer.bounds.size.x;

        // ��������Ʈ ��ġ
        for (float x = leftBottomCorner.x; x <= rightBottomCorner.x + spriteWidth; x += spriteWidth)
        {
            Vector3 position = new Vector3(x, leftBottomCorner.y, 0);
            GameObject obj = Instantiate(groundTilePrefab, position, Quaternion.identity, this.transform);

            groundTiles.Add(obj);
        }
    }
}
