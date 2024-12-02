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
    private static string UserUUID = null;

    private const string GoogleKey = "Google";
    private static string UserGoogleID = null;
    
    private const string AccessTokenKey = "Token";
    private static string AccessToken = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        DontDestroyOnLoad(gameObject);

        if (PlayerPrefs.HasKey(UUIDKey))
        {
            UserUUID = PlayerPrefs.GetString(UUIDKey);
            //Debug.Log(UserUUID);
        }
        if (PlayerPrefs.HasKey(GoogleKey))
        {
            UserGoogleID = PlayerPrefs.GetString(GoogleKey);
        }
    }


    /*
     * This function is called during asynchronous socket communication.
     * Works on thread pool
     */
    public void SetUserUUID(string uuid)
    {
        UserUUID = uuid;

        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            PlayerPrefs.SetString(UUIDKey, UserUUID);
            PlayerPrefs.Save();
        });
    }

    public string GetUserUUID()
    {
        if(UserUUID != null) 
            return UserUUID;

        return null;
    }

    public void SetUserGoogleID(string userId)
    {
        UserGoogleID = userId;

        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            PlayerPrefs.SetString(GoogleKey, UserGoogleID);
            PlayerPrefs.Save();
        });
            
    }

    public string GetUserGoogleID()
    {
        if( UserGoogleID != null )
            return UserGoogleID;

        return null;
    }

    public void SetAccessToken(string accessToken)
    {
        AccessToken = accessToken;

        MainThreadDispatcher.Instance.Enqueue(() =>
        {
            PlayerPrefs.SetString(AccessTokenKey, AccessToken);
            PlayerPrefs.Save();
        });
    }

    public string GetAccessToken()
    {
        return AccessToken;
    }
}
