using GooglePlayGames.BasicApi;
using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;

public class GoogleManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void GoogleLogin()
    {
        // 미사용
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        
        if (status == SignInStatus.Success) // 로그인 성공
        {
            string googleId = PlayGamesPlatform.Instance.GetUserId();
            Debug.Log(googleId);
            // Continue with Play Games Services
        }
        else // 로그인 실패
        {
            Debug.Log("Google Play Games Sign in Failed");
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
        }
    }
}
