using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AudioManager : DonDestory<AudioManager>
{
    [Header("Audio Mixer & Source")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource bgmSource; // BGM ����� �ҽ�
    [SerializeField] private AudioSource sfxSource; // SFX ����� �ҽ�

    private const string BGM_VOLUME = "BGMVolume";
    private const string SFX_VOLUME = "SFXVolume";
    private const string MUTE_KEY = "Mute";// 0 = �Ҹ� ����, 1 = ���Ұ�

    protected override void OnAwake()
    {
        base.OnAwake();
    }
    protected override void OnStart()
    {
        base.OnStart();
        Initialize();
    }

    #region [Initialize Methods]
    void Initialize()
    {
        float bgmVol = PlayerPrefs.GetFloat(BGM_VOLUME, 1.0f);
        float sfxVol = PlayerPrefs.GetFloat(SFX_VOLUME, 1.0f);
        bool isMuted = PlayerPrefs.GetInt(MUTE_KEY, 0) == 1;

        audioMixer.SetFloat(BGM_VOLUME, Mathf.Log10(bgmVol) * 20);
        audioMixer.SetFloat(SFX_VOLUME, Mathf.Log10(sfxVol) * 20);
        // Mute ���� ����
        ToggleMute(isMuted);
        LoadBGM("BGM/Intro.mp3");
    }
    #endregion [Initialize Methods]

    #region [Volume & Mute Methods]
    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat(BGM_VOLUME, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(BGM_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat(SFX_VOLUME, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(SFX_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public void ToggleMute(bool isMuted)
    {
        // PlayerPrefs�� MUTE_KEY ����
        PlayerPrefs.SetInt(MUTE_KEY, isMuted ? 1 : 0);
        PlayerPrefs.Save();

        if (isMuted)
        {
            //  ���Ұ�: BGM�� SFX�� ������ ���� & Audio Mixer ������ -80dB�� ����
            bgmSource.Stop();
            sfxSource.Stop();
            audioMixer.SetFloat(BGM_VOLUME, -80f);
            audioMixer.SetFloat(SFX_VOLUME, -80f);
        }
        else
        {
            //  �ٽ� �ѱ�: ����� ���� ������ ���� & BGM ���
            bgmSource.Play();
            float bgmVol = PlayerPrefs.GetFloat(BGM_VOLUME, 1.0f);
            float sfxVol = PlayerPrefs.GetFloat(SFX_VOLUME, 1.0f);
            audioMixer.SetFloat(BGM_VOLUME, Mathf.Log10(bgmVol) * 20);
            audioMixer.SetFloat(SFX_VOLUME, Mathf.Log10(sfxVol) * 20);
        }
    }
    #endregion [Volume & Mute Methods]

    #region [Addressable Load Methods]

    public void LoadBGMFromScene(string scene)
    {
        Debug.Log(scene);

        switch(scene)
        {
            case "DownloadScene":
                LoadBGM("BGM/Download.mp3");
                break;
            case "IngameScene":
                LoadBGM("BGM/Ingame.mp3");
                Debug.Log("�ΰ��� ������ ����");
                break;
        }
    }

    public void LoadBGM(string bgmKey)
    {
        if(bgmKey.Equals("BGM/Ingame.mp3"))
        {
            Debug.Log("������ �ε���� �Ǵ°� Ȯ��");
        }
        StartCoroutine(LoadBGMCoroutine(bgmKey));
    }

    private IEnumerator LoadBGMCoroutine(string bgmKey)
    {
        AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(bgmKey);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            bgmSource.clip = handle.Result;
            bgmSource.Play();
        }
        else
        {
            Debug.LogError($"Addressables���� {bgmKey} �ε� ����");
        }
    }

    public void LoadSFX(string sfxKey)
    {
        StartCoroutine(LoadSFXCoroutine(sfxKey));
    }

    private IEnumerator LoadSFXCoroutine(string sfxKey)
    {
        AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(sfxKey);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            sfxSource.PlayOneShot(handle.Result); // ȿ���� ��� ���
        }
        else
        {
            Debug.LogError($"Addressables���� {sfxKey} �ε� ����");
        }
    }
    #endregion [Addressable Load Methods]

}
