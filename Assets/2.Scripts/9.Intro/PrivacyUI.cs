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
            Debug.Log("����� üũ��!");
            _agreeBtn.interactable = true;
            // üũ�Ǿ��� �� ������ �ڵ�
        }
        else
        {
            Debug.Log("����� ������!");
            _agreeBtn.interactable = false;
            // üũ �����Ǿ��� �� ������ �ڵ�
        }
    }

}
