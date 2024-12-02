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
        // �̻��
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        
        if (status == SignInStatus.Success) // �α��� ����
        {
            string googleId = PlayGamesPlatform.Instance.GetUserId();
            Debug.Log(googleId);
            // Continue with Play Games Services
        }
        else // �α��� ����
        {
            Debug.Log("Google Play Games Sign in Failed");
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
        }
    }
}
