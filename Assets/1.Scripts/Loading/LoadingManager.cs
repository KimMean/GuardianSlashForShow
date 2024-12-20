using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] Slider loadingBar;

    static string targetSceneName;
    static bool isLoading = false;

    private Dictionary<string, ILoadingOperation> LoadingOperations = new Dictionary<string, ILoadingOperation>();
    // Start is called before the first frame update

    private void Awake()
    {
        if(targetSceneName == "Lobby" && isLoading)
            LoadingOperations["Lobby"] = new LoadUserDataOperation();
    }

    void Start()
    {
        StartCoroutine(LoadSceneAsynchronous());
    }

    /// <summary>
    /// ���� �����մϴ�.
    /// </summary>
    /// <param name="sceneName">�� �̸�</param>
    /// <param name="loadData">�� ����� �ε��� ������ ����</param>
    public static void LoadScene(string sceneName, bool loadData)
    {
        //Debug.Log($"sceneName : {sceneName}, IsLoading {loadData}");
        //Debug.Log("������ �� ��?");
        targetSceneName = sceneName;
        isLoading = loadData;
        RaycastBlockerManager.Instance.RemoveBlocker();
        SceneManager.LoadScene("Loading");
    }

    /// <summary>
    /// �񵿱� �� �ε�
    /// </summary>
    IEnumerator LoadSceneAsynchronous() 
    {
        loadingBar.value = 0;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
        asyncLoad.allowSceneActivation = false;

        //Debug.Log($"LoadScene Contains : {LoadingOperations.ContainsKey(TargetSceneName)}");

        //Debug.Log($"IsLoading : {IsLoading}");
        if (isLoading)
        {
            //Debug.Log("????????");
            if (LoadingOperations.ContainsKey(targetSceneName))
            {
                ILoadingOperation loadingOperation = LoadingOperations[targetSceneName];
                Task dataLoadTask = loadingOperation.Execute();

                while (!asyncLoad.isDone || !dataLoadTask.IsCompleted)
                {
                    // �� �ε� ���� ��Ȳ
                    float sceneProgress = asyncLoad.progress / 0.9f; // 0 ~ 1�� ��ȯ

                    float dataProgress = loadingOperation.progress;
                    //Debug.Log($"DataProgress : {dataProgress}");

                    // ��ü ���� ��Ȳ ������Ʈ
                    loadingBar.value = (sceneProgress + dataProgress) / 2.0f;

                    if (asyncLoad.progress >= 0.9f && dataLoadTask.IsCompleted)
                    {
                        // �� Ȱ��ȭ
                        asyncLoad.allowSceneActivation = true;

                    }
                    yield return null;
                }
            }
        }
        else
        {
            // �ε� �۾��� ���� ��� ���� �ε�
            while (!asyncLoad.isDone)
            {
                // �� �ε� ���� ��Ȳ (90%)
                loadingBar.value = asyncLoad.progress / 0.9f; // 0 ~ 1�� ��ȯ

                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true;
                }
                yield return null;
            }
        }
    }
}
