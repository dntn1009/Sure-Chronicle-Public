using Firebase.Auth;
using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingWindow : TWindowController
{
    [Header("UI")]
    [Header("Sound")]
    [SerializeField] Slider _backSound;
    [SerializeField] Slider _sfxSound;
    [SerializeField] Toggle _soundCheckBox;
    [Header("Auth")]
    [SerializeField] TextMeshProUGUI _authCodeText;
    [SerializeField] Button _couponBtn;
    [Header("Version & Inquire")]
    [SerializeField] TextMeshProUGUI _verText;
    [SerializeField] TextMeshProUGUI _monText;
    [Header("Coupon")]
    [SerializeField] Coupon _coupon;

    [Header("Loading")]
    [SerializeField] GameObject _loading;

    private const string BGM_VOLUME = "BGMVolume";
    private const string SFX_VOLUME = "SFXVolume";
    private const string MUTE_KEY = "Mute";// 0 = 소리 켜짐, 1 = 음소거
    public override void Operate()
    {
        base.Operate();
        _loading.SetActive(false);
        // 슬라이더 값이 변경될 때 볼륨 조절 이벤트 연결
        SoundInitialize();
        _authCodeText.text = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        _verText.text = Application.version;
        _monText.text = DataManager.Instance.BuildDate;
        _couponBtn.onClick.AddListener(() => SetCoupon());
        _coupon.InitlizeSet();
    }

    void SoundInitialize()
    {
        _backSound.value = PlayerPrefs.GetFloat(BGM_VOLUME, 1.0f);
        _sfxSound.value = PlayerPrefs.GetFloat(SFX_VOLUME, 1.0f);
        _backSound.onValueChanged.AddListener(AudioManager.Instance.SetBGMVolume);
        _sfxSound.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
        _soundCheckBox.isOn = PlayerPrefs.GetInt(MUTE_KEY, 0) == 1;
        _soundCheckBox.onValueChanged.AddListener(AudioManager.Instance.ToggleMute);
    }

    void SetCoupon()
    {
        _coupon.gameObject.SetActive(true);
    }

    public void LoadingSetActive(bool check)
    {
        _loading.SetActive(check);
    }
}
