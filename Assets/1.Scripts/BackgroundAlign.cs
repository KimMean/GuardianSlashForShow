using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomAlign : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FitSpriteToScreen();
        AlignToBottom();
    }

    void FitSpriteToScreen()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        transform.localScale = Vector3.one;

        float width = sr.sprite.bounds.size.x;
        float height = sr.sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        Vector3 newScale = transform.localScale;
        newScale.x = worldScreenWidth / width * 3;
        newScale.y = worldScreenHeight / height * 3;

        transform.localScale = newScale;
    }

    void AlignToBottom()
    {
        // 화면의 월드 좌표에서 하단 중앙 지점을 계산합니다.
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogWarning("Main Camera not found. Please ensure a camera is tagged as 'MainCamera'.");
            return;
        }

        float screenHeight = Screen.height;
        float screenWidth = Screen.width;

        // 하단 중앙 지점 계산 (Orthographic 카메라 기준)
        Vector3 bottomCenterScreen = new Vector3(screenWidth / 2, 0, mainCamera.nearClipPlane);
        Vector3 bottomCenterWorld = mainCamera.ScreenToWorldPoint(bottomCenterScreen);

        // 오브젝트의 새로운 위치 설정
        Vector3 newPosition = transform.position;
        newPosition.x = bottomCenterWorld.x;
        newPosition.y = bottomCenterWorld.y;

        transform.position = newPosition;
    }
}
