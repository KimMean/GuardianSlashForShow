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

    public void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
    }

    public void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = DataManager.Instance.GetUserUUID();
    }

    public void UpdateBgmVolume()
    {
        SoundManager.Instance.SetBgmVolume(bgmVolumeSlider.value);
    }

    public void ToggleBgmMute()
    {
        bgmMute = !bgmMute;
        bgmMuteImg.sprite = bgmMute ? SpeakerOff : SpeakerOn;
        SoundManager.Instance.SetBgmMute(bgmMute);
    }

    public void UpdateSfxVolume()
    {
        SoundManager.Instance.SetSfxVolume(sfxVolumeSlider.value);
    }
    public void ToggleSfxMute()
    {
        sfxMute = !sfxMute;
        sfxMuteImg.sprite = sfxMute ? SpeakerOff : SpeakerOn;
        SoundManager.Instance.SetSfxMute(sfxMute);
    }
}
