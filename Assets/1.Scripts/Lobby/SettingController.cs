using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingController : MonoBehaviour
{
    [SerializeField] Text text_UUID;

    [Header("Bgm")]
    [SerializeField] Slider bgmVolumeSlider;
    [SerializeField] Image bgmMuteImg;
    bool bgmMute;

    [Header("Sfx")]
    [SerializeField] Slider sfxVolumeSlider;
    [SerializeField] Image sfxMuteImg;
    bool sfxMute;

    [Header("SoundSprite")]
    [SerializeField] Sprite SpeakerOn;
    [SerializeField] Sprite SpeakerOff;

    private void Awake()
    {
        text_UUID.text = DataManager.Instance.GetUserUUID();

        bgmVolumeSlider.value = SoundManager.Instance.GetBgmVolume();
        sfxVolumeSlider.value = SoundManager.Instance.GetSfxVolume();

        bgmMute = SoundManager.Instance.IsBgmMute();
        sfxMute = SoundManager.Instance.IsSfxMute();

        bgmMuteImg.sprite = bgmMute ? SpeakerOff : SpeakerOn;
        sfxMuteImg.sprite = sfxMute ? SpeakerOff : SpeakerOn;
    }

    /// <summary>
    /// 닫기 버튼 클릭
    /// </summary>
    public void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.PopupClose);
    }

    /// <summary>
    /// UUID 복사 버튼 클릭
    /// </summary>
    public void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = DataManager.Instance.GetUserUUID();
    }

    /// <summary>
    /// BGM 볼륨 조절
    /// </summary>
    public void UpdateBgmVolume()
    {
        SoundManager.Instance.SetBgmVolume(bgmVolumeSlider.value);
    }
    /// <summary>
    /// BGM 음소거 토글 버튼 클릭
    /// </summary>
    public void ToggleBgmMute()
    {
        bgmMute = !bgmMute;
        bgmMuteImg.sprite = bgmMute ? SpeakerOff : SpeakerOn;
        SoundManager.Instance.SetBgmMute(bgmMute);
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
    }

    /// <summary>
    /// SFX 볼륨 조절
    /// </summary>
    public void UpdateSfxVolume()
    {
        SoundManager.Instance.SetSfxVolume(sfxVolumeSlider.value);
    }
    /// <summary>
    /// SFX 음소거 토글 버튼 클릭
    /// </summary>
    public void ToggleSfxMute()
    {
        sfxMute = !sfxMute;
        sfxMuteImg.sprite = sfxMute ? SpeakerOff : SpeakerOn;
        SoundManager.Instance.SetSfxMute(sfxMute);
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
    }
    /// <summary>
    /// 종료 버튼 클릭
    /// </summary>
    public void QuitButtonOnClick()
    {
        SoundManager.Instance.PlayUISfx(SoundManager.UI_SFX_Clip.Click);
        GameManager.Instance.OnQuitButtonClick();
    }
}
