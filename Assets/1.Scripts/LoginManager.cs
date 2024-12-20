using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField] GameObject updatePanel;

    /// <summary>
    /// �Խ�Ʈ �α��� ��ư�� ������ ȣ��˴ϴ�.
    /// </summary>
    public void OnGuestLoginButtonClick()
    {
        if(GameManager.Instance.GetExistUpdate())
        {
            updatePanel.SetActive(true);
            MessageManager.Instance.ShowMessage("Update�� �ʿ��մϴ�.");
            return;
        }

        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
        NetworkManager.Instance.GuestLogin();
    }

    /// <summary>
    /// ����� �ּҷ� �̵��մϴ�.
    /// </summary>
    public void RedirectToStore()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
        Application.OpenURL(GameManager.Instance.GetStoreURL());
    }
}
