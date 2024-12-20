using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<SoundManager>();
            return instance;
        }
    }

    const string BgmVolumeKey = "BgmVolume";
    const string SfxVolumeKey = "SfxVolume";
    const string BgmMuteKey = "BgmMute";
    const string SfxMuteKey = "SfxMute";

    public enum BGM_Clip
    {
        Lobby = 0,
        Game = 1,
        NONE = 255,
    }
    public enum SFX_Clip
    {
        AttackA = 0,
        AttackB = 1,
        Defense,
        AbsolDefense,
        Jump,
        Landing,
        Hit,
        Block,
        Leaf,
        Ice,
        Rock,
        Crash,
        Destroy,
        Frost,
        MAX = 255
    }
    public enum UI_SFX_Clip
    {
        Click = 0,
        GetCoin = 1,
        GetDia = 2,
        EquipSword,
        EquipNecklace,
        EquipRing,
        PopupOpen,
        PopupClose,
        GameClear,
        GameOver,
        StageBelt,

        MAX = 255
    }

    [Header("#BGM")]
    [SerializeField] AudioClip[] bgmClips;
    float bgmVolume = 1f;
    bool bgmMute = false;
    AudioSource bgmPlayer;
    float fadeDuration = 1.0f;
    BGM_Clip currentClip = BGM_Clip.NONE;

    [Header("#SFX")]
    [SerializeField] AudioClip[] sfxClips;
    [SerializeField] AudioClip[] uiSfxClips;
    [SerializeField] int channels;
    float sfxVolume = 1f;
    bool sfxMute = false;
    AudioSource[] sfxPlayers;
    int channelIndex;


    private void Awake()
    {
        if(instance == null)
            instance = this;

        DontDestroyOnLoad(gameObject);
        SoundDataLoad();
        InitSoundManager();
    }

    void SoundDataLoad()
    {
        if (PlayerPrefs.HasKey(BgmVolumeKey))
        {
            bgmVolume = PlayerPrefs.GetFloat(BgmVolumeKey);
        }
        if (PlayerPrefs.HasKey(BgmMuteKey))
        {
            bgmMute = PlayerPrefs.GetInt(BgmMuteKey) == 1 ? true : false;
        }
        if (PlayerPrefs.HasKey(SfxVolumeKey))
        {
            sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey);
        }
        if (PlayerPrefs.HasKey(SfxMuteKey))
        {
            sfxMute = PlayerPrefs.GetInt(SfxMuteKey) == 1 ? true : false;
        }
    }

    private void InitSoundManager()
    {
        // 배경음 초기화
        GameObject bgmObject = new GameObject("BGM_Player");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        


        // 효과음 초기화
        GameObject sfxObject = new GameObject("SFX_Player");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for(int i = 0; i < channels; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].volume = sfxVolume;
        }
    }

    public void ChangeBGM(BGM_Clip bgmClip)
    {
        // 이미 재생 중인 클립과 동일한 경우 변경하지 않음
        if (currentClip == bgmClip) return;

        currentClip = bgmClip;

        // 새 BGM으로 전환
        if (bgmMute)
            bgmPlayer.clip = bgmClips[(int)currentClip];
        else
        {
            StopAllCoroutines();
            StartCoroutine(FadeOutAndChange());
        }
    }

    private IEnumerator FadeOutAndChange()
    {
        float startVolume = bgmPlayer.volume;
        // 페이드 아웃
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            bgmPlayer.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }
        bgmPlayer.volume = 0;

        // 새로운 클립으로 변경
        bgmPlayer.clip = bgmClips[(int)currentClip];
        bgmPlayer.Play();

        // 페이드 인
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            bgmPlayer.volume = Mathf.Lerp(0, bgmVolume, t / fadeDuration);
            yield return null;
        }
        bgmPlayer.volume = bgmVolume;
    }

    public void PlayBgm(bool isPlay)
    {
        if (bgmMute)
            return;

        if (isPlay)
            bgmPlayer.Play();
        else
            bgmPlayer.Stop();
    }

    public void PlaySfx(SFX_Clip clip)
    {
        if(sfxMute)
            return;

        for(int i = 0; i < channels; i++)
        {
            int loopIndex = (i + channelIndex) % channels;

            if (sfxPlayers[loopIndex].isPlaying) continue;

            channelIndex = loopIndex;
            sfxPlayers[channelIndex].clip = sfxClips[(int)clip];
            sfxPlayers[channelIndex].Play();
            break;
        }
    }

    public void PlayUISfx(UI_SFX_Clip clip)
    {
        if (sfxMute)
            return;

        for (int i = 0; i < channels; i++)
        {
            int loopIndex = (i + channelIndex) % channels;

            if (sfxPlayers[loopIndex].isPlaying) continue;

            channelIndex = loopIndex;
            sfxPlayers[channelIndex].clip = uiSfxClips[(int)clip];
            sfxPlayers[channelIndex].Play();
            break;
        }
    }

    public float GetBgmVolume() => bgmVolume;
    public float GetSfxVolume() => sfxVolume;

    public bool IsBgmMute() => bgmMute;
    public bool IsSfxMute() => sfxMute;

    public void SetBgmVolume(float volume)
    {
        bgmVolume = volume;
        bgmPlayer.volume = bgmVolume;

        PlayerPrefs.SetFloat(BgmVolumeKey, bgmVolume);
        PlayerPrefs.Save();
    }
    public void SetSfxVolume(float volume)
    {
        sfxVolume = volume;
        for (int i = 0; i < channels; i++)
        {
            sfxPlayers[i].volume = sfxVolume;
        }

        PlayerPrefs.SetFloat(SfxVolumeKey, sfxVolume);
        PlayerPrefs.Save();
    }
    
    public void SetBgmMute(bool mute)
    {
        bgmMute = mute;

        if(bgmMute)
        {
            if(bgmPlayer.isPlaying)
                bgmPlayer.Stop();
        }
        else
        {
            if (!bgmPlayer.isPlaying)
                bgmPlayer.Play();
        }

        PlayerPrefs.SetInt(BgmMuteKey, bgmMute ? 1 : 0);
        PlayerPrefs.Save();
    }
    public void SetSfxMute(bool mute)
    {
        sfxMute = mute;

        if(sfxMute)
        {
            for (int i = 0; i < channels; i++)
            {
                if(sfxPlayers[i].isPlaying)
                    sfxPlayers[i].Stop();
            }
        }

        PlayerPrefs.SetInt(SfxMuteKey, sfxMute ? 1 : 0);
        PlayerPrefs.Save();
    }
}
