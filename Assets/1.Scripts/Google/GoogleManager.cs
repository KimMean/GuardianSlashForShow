using GooglePlayGames.BasicApi;
using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;

public class GoogleManager : MonoBehaviour
{
    /// <summary>
    /// ���� �α��� ����
    /// ���� ������ �ʽ��ϴ�.
    /// </summary>
    public void GoogleLogin()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    /// <summary>
    /// ���� ������ �õ��մϴ�.
    /// ���� �� ȸ������ ���μ����� �����մϴ�.
    /// ���� �α��� ����� ���� �������� �ʽ��ϴ�.
    /// </summary>
    /// <param name="status"></param>
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
