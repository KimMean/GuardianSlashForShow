using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] Slider LoadingBar;

    static string TargetSceneName;
    static bool IsLoading = false;

    private Dictionary<string, ILoadingOperation> LoadingOperations = new Dictionary<string, ILoadingOperation>();
    // Start is called before the first frame update

    private void Awake()
    {
        if(TargetSceneName == "Lobby" && IsLoading)
            LoadingOperations["Lobby"] = new LoadUserDataOperation();
    }

    void Start()
    {
        StartCoroutine(LoadSceneAsynchronous());
    }


    public static void LoadScene(string sceneName, bool loadData)
    {
        //Debug.Log($"sceneName : {sceneName}, IsLoading {loadData}");
        //Debug.Log("설정은 잘 돼?");
        TargetSceneName = sceneName;
        IsLoading = loadData;
        RaycastBlockerManager.Instance.RemoveBlocker();
        SceneManager.LoadScene("Loading");
    }

    IEnumerator LoadSceneAsynchronous() 
    {
        LoadingBar.value = 0;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(TargetSceneName);
        asyncLoad.allowSceneActivation = false;

        //Debug.Log($"LoadScene Contains : {LoadingOperations.ContainsKey(TargetSceneName)}");

        //Debug.Log($"IsLoading : {IsLoading}");
        if (IsLoading)
        {
            //Debug.Log("????????");
            if (LoadingOperations.ContainsKey(TargetSceneName))
            {
                ILoadingOperation loadingOperation = LoadingOperations[TargetSceneName];
                Task dataLoadTask = loadingOperation.Execute();

                while (!asyncLoad.isDone || !dataLoadTask.IsCompleted)
                {
                    // 씬 로드 진행 상황
                    float sceneProgress = asyncLoad.progress / 0.9f; // 0 ~ 1로 변환

                    float dataProgress = loadingOperation.Progress;
                    //Debug.Log($"DataProgress : {dataProgress}");

                    // 전체 진행 상황 업데이트
                    LoadingBar.value = (sceneProgress + dataProgress) / 2.0f;

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
            //Debug.Log("여기로 와야지 ,,");
            // 로딩 작업이 없는 경우 씬만 로드
            while (!asyncLoad.isDone)
            {
                // 씬 로드 진행 상황 (90%)
                LoadingBar.value = asyncLoad.progress / 0.9f; // 0 ~ 1로 변환

                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true;
                }
                yield return null;
            }
        }
    }
}
