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
    /// 씬을 변경합니다.
    /// </summary>
    /// <param name="sceneName">씬 이름</param>
    /// <param name="loadData">씬 변경시 로드할 데이터 유무</param>
    public static void LoadScene(string sceneName, bool loadData)
    {
        //Debug.Log($"sceneName : {sceneName}, IsLoading {loadData}");
        //Debug.Log("설정은 잘 돼?");
        targetSceneName = sceneName;
        isLoading = loadData;
        RaycastBlockerManager.Instance.RemoveBlocker();
        SceneManager.LoadScene("Loading");
    }

    /// <summary>
    /// 비동기 씬 로드
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
                    // 씬 로드 진행 상황
                    float sceneProgress = asyncLoad.progress / 0.9f; // 0 ~ 1로 변환

                    float dataProgress = loadingOperation.progress;
                    //Debug.Log($"DataProgress : {dataProgress}");

                    // 전체 진행 상황 업데이트
                    loadingBar.value = (sceneProgress + dataProgress) / 2.0f;

                    if (asyncLoad.progress >= 0.9f && dataLoadTask.IsCompleted)
                    {
                        // 씬 활성화
                        asyncLoad.allowSceneActivation = true;

                    }
                    yield return null;
                }
            }
        }
        else
        {
            // 로딩 작업이 없는 경우 씬만 로드
            while (!asyncLoad.isDone)
            {
                // 씬 로드 진행 상황 (90%)
                loadingBar.value = asyncLoad.progress / 0.9f; // 0 ~ 1로 변환

                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true;
                }
                yield return null;
            }
        }
    }
}
