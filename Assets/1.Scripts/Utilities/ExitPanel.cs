using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPanel : MonoBehaviour
{
    private void OnEnable()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.PopupOpen);
    }

    private void OnDisable()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.PopupClose);
    }
    public void CancelButtonOnClick()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
        gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void QuitButtonOnClick()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
        GameManager.Instance.OnQuitButtonClick();
    }
}
