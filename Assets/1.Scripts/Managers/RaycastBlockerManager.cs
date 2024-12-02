using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastBlockerManager : MonoBehaviour
{
    public static RaycastBlockerManager Instance;
    public GameObject blockerPrefab; // ���ŷ �г� Prefab

    private GameObject blockerInstance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �ÿ��� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ���ŷ �г��� Ȱ��ȭ�ϴ� �޼���
    public void ShowBlocker()
    {
        if (blockerInstance == null)
        {
            blockerInstance = Instantiate(blockerPrefab, transform);
            blockerInstance.transform.SetParent(GameObject.Find("Canvas").transform, false);
        }
        blockerInstance.SetActive(true); // ���ŷ �г� Ȱ��ȭ
    }

    // ���ŷ �г��� ��Ȱ��ȭ�ϴ� �޼���
    public void HideBlocker()
    {
        if (blockerInstance != null)
        {
            blockerInstance.SetActive(false); // ���ŷ �г� ��Ȱ��ȭ
        }
    }

    // ���ŷ �г��� �����ϴ� �޼���
    public void RemoveBlocker()
    {
        if (blockerInstance != null)
        {
            Destroy(blockerInstance); // ���ŷ �г� ����
            blockerInstance = null;
        }
    }
}
