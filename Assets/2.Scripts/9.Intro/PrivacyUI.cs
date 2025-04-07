using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrivacyUI : MonoBehaviour
{
    [SerializeField] Toggle _privacyCheckBox;
    [SerializeField] Button _privacyBtn;
    [SerializeField] Button _agreeBtn;
    [SerializeField] Button _cancelBtn;


    public void InitializeSet()
    {
        _privacyCheckBox.isOn = false;
        _privacyCheckBox.onValueChanged.AddListener(OnToggleChanged);
        _agreeBtn.onClick.AddListener(() => { IntroManager.Instance.ScreenCheck = true; gameObject.SetActive(false); });
        _cancelBtn.onClick.AddListener(() => { Application.Quit(); });
        _privacyBtn.onClick.AddListener(() => { Application.OpenURL("https://policies.google.com/privacy"); });
        _agreeBtn.interactable = false;
    }

    void OnToggleChanged(bool isChecked)
    {
        if (isChecked)
        {
            Debug.Log("토글이 체크됨!");
            _agreeBtn.interactable = true;
            // 체크되었을 때 실행할 코드
        }
        else
        {
            Debug.Log("토글이 해제됨!");
            _agreeBtn.interactable = false;
            // 체크 해제되었을 때 실행할 코드
        }
    }

}
