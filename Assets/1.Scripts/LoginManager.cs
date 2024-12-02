using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{


    public void OnGuestLoginButtonClick()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
        NetworkManager.Instance.GuestLogin();
    }
}
