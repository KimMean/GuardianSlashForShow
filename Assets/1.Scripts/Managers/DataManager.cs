using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            return instance;
        }
    }

    private const string UUIDKey = "UserUUID";
    private static string userUUID = null;

    private const string GoogleKey = "Google";
    private static string userGoogleID = null;
    
    private const string AccessTokenKey = "Token";
    private static string accessToken = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        DontDestroyOnLoad(gameObject);

        if (PlayerPrefs.HasKey(UUIDKey))
        {
            userUUID = PlayerPrefs.GetString(UUIDKey);
            //Debug.Log(UserUUID);
        }
        if (PlayerPrefs.HasKey(GoogleKey))
        {
            userGoogleID = PlayerPrefs.GetString(GoogleKey);
        }
    }


    /// <summary>
    /// UUID를 저장합니다.
    /// </summary>
    public void SetUserUUID(string uuid)
    {
        userUUID = uuid;

        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            PlayerPrefs.SetString(UUIDKey, userUUID);
            PlayerPrefs.Save();
        });
    }

    public string GetUserUUID()
    {
        if(userUUID != null) 
            return userUUID;

        return null;
    }

    /// <summary>
    /// 구글 아이디를 저장합니다.
    /// </summary>
    public void SetUserGoogleID(string userId)
    {
        userGoogleID = userId;

        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            PlayerPrefs.SetString(GoogleKey, userGoogleID);
            PlayerPrefs.Save();
        });
            
    }

    public string GetUserGoogleID()
    {
        if(userGoogleID != null )
            return userGoogleID;

        return null;
    }

    /// <summary>
    /// 액세스 토큰을 저장합니다.
    /// </summary>
    public void SetAccessToken(string token)
    {
        accessToken = token;

        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            PlayerPrefs.SetString(AccessTokenKey, accessToken);
            PlayerPrefs.Save();
        });
    }

    public string GetAccessToken()
    {
        return accessToken;
    }
}
