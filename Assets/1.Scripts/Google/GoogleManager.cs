using GooglePlayGames.BasicApi;
using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;

public class GoogleManager : MonoBehaviour
{
    /// <summary>
    /// 구글 로그인 제공
    /// 현재 사용되지 않습니다.
    /// </summary>
    public void GoogleLogin()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    /// <summary>
    /// 구글 인증을 시도합니다.
    /// 인증 후 회원가입 프로세스를 진행합니다.
    /// 구글 로그인 기능은 현재 지원되지 않습니다.
    /// </summary>
    /// <param name="status"></param>
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
