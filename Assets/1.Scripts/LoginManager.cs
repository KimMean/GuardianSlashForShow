using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField] GameObject updatePanel;

    /// <summary>
    /// 게스트 로그인 버튼을 누르면 호출됩니다.
    /// </summary>
    public void OnGuestLoginButtonClick()
    {
        if(GameManager.Instance.GetExistUpdate())
        {
            updatePanel.SetActive(true);
            MessageManager.Instance.ShowMessage("Update가 필요합니다.");
            return;
        }

        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
        NetworkManager.Instance.GuestLogin();
    }

    /// <summary>
    /// 스토어 주소로 이동합니다.
    /// </summary>
    public void RedirectToStore()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
        Application.OpenURL(GameManager.Instance.GetStoreURL());
    }
}
