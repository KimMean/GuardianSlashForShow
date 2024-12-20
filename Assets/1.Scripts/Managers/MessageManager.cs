using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    private static MessageManager instance;
    public static MessageManager Instance
    {
        get
        {
            return instance;
        }
    }


    [SerializeField] GameObject messageBoxPrefab;

    List<GameObject> messages = new List<GameObject>();

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 토스트 메시지를 띄웁니다.
    /// </summary>
    public void ShowMessage(string message)
    {
        GameObject messageBox = null;
        foreach(GameObject obj in messages)
        {
            if (obj.activeSelf)
                continue;

            messageBox = obj;
            break;
        }

        if(messageBox == null)
        {
            messageBox = Instantiate(messageBoxPrefab, FindObjectOfType<Canvas>().transform);
            messages.Add(messageBox);
        }

        messageBox.GetComponent<MessageBox>().SetText(message);

    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        messages.Clear();
    }
}
