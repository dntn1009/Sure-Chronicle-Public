using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Coupon : MonoBehaviour
{
    [SerializeField] Button _backBtn;
    [SerializeField] Button _registerBtn;
    [SerializeField] TMP_InputField _couponCode;
    [SerializeField] SettingWindow _window;

    public void InitlizeSet()
    {
        _backBtn.onClick.AddListener(() => Back());
        _registerBtn.onClick.AddListener(() => RegisterCoupon());
        gameObject.SetActive(false);
    }

    void Back()
    {
        AudioManager.Instance.LoadSFX("SFX/Close.wav");
        gameObject.SetActive(false);
    }

    void RegisterCoupon()
    {
        _window.LoadingSetActive(true);
        StartCoroutine(DataManager.Instance.GetMessageFormCoupon(_couponCode.text , LoadingSetActiveFalse));
    }

    void LoadingSetActiveFalse()
    {
        _window.LoadingSetActive(false);
    }
}
