using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddressableDownloadUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _fileText;
    [SerializeField] Button _installBtn;
    [SerializeField] Button _quitBtn;

    private void Awake()
    {
        _installBtn.onClick.AddListener(installScene);
        _quitBtn.onClick.AddListener(() => { Application.Quit(); });    
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }

    public void ShowUI(long totalSize)
    {
        gameObject.SetActive(true);
        _fileText.text = $"파일 크기: {totalSize / 1048576f:F2} MB";
    }

    public void installScene()
    {
        GameSceneManager.Instance.MoveToDownloadScene();
    }
}
