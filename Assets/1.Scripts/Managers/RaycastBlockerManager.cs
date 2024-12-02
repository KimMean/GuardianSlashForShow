using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastBlockerManager : MonoBehaviour
{
    public static RaycastBlockerManager Instance;
    public GameObject blockerPrefab; // 블로킹 패널 Prefab

    private GameObject blockerInstance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 블로킹 패널을 활성화하는 메서드
    public void ShowBlocker()
    {
        if (blockerInstance == null)
        {
            blockerInstance = Instantiate(blockerPrefab, transform);
            blockerInstance.transform.SetParent(GameObject.Find("Canvas").transform, false);
        }
        blockerInstance.SetActive(true); // 블로킹 패널 활성화
    }

    // 블로킹 패널을 비활성화하는 메서드
    public void HideBlocker()
    {
        if (blockerInstance != null)
        {
            blockerInstance.SetActive(false); // 블로킹 패널 비활성화
        }
    }

    // 블로킹 패널을 제거하는 메서드
    public void RemoveBlocker()
    {
        if (blockerInstance != null)
        {
            Destroy(blockerInstance); // 블로킹 패널 삭제
            blockerInstance = null;
        }
    }
}
