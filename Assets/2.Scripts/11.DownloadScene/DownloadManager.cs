using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class DownloadManager : MonoBehaviour
{
    [Header("Tip")]
    [SerializeField] TextMeshProUGUI _tipText;

    [Header("Install")]
    [SerializeField] Slider _installBar;
    [SerializeField] TextMeshProUGUI _installText;
    [SerializeField] TextMeshProUGUI _fileNameText;

    [Header("Start")]
    [SerializeField] GameObject _touchScreen;
    bool _isTouch; // ��ġ Ȯ��
    bool _startCheck; //�ٿ� ��ġ �Ϸ�ƴ��� üũ
    bool _clickCheck; // �ߺ� ����
    private AsyncOperationHandle downloadHandle;

    private void Awake()
    {
        _isTouch = false;   
        _startCheck = false;
        _clickCheck = false;
        _touchScreen.SetActive(false);
        StartCoroutine(DownloadAssets());
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _isTouch = true;
            }

            if (touch.phase == TouchPhase.Ended && _isTouch && _startCheck && !_clickCheck)
            {
                _isTouch = false;
                _clickCheck = true;
                GameSceneManager.Instance.ShowRefreshLoading("�ε� ���Դϴ�..");
                MoveToIngame();
            }
        }
    }

    IEnumerator DownloadAssets()
    {
        StartCoroutine(ShowLoadingTips());
        _installBar.value = 0;
        _fileNameText.text = "0%";
        foreach (string label in GameSceneManager.Instance.DownloadLabels)
        {
            downloadHandle = Addressables.DownloadDependenciesAsync(label);
            StartCoroutine(ShowProgress());

            yield return downloadHandle;

            if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"{label} �ٿ�ε� �Ϸ�!");
            }
            else
            {
                Debug.LogError($"{label} �ٿ�ε� ����!");
            }

            Addressables.Release(downloadHandle);
        }

        Debug.Log("��� ���ҽ� �ٿ�ε� �Ϸ�!");
        _installBar.value = 1;
        _installText.text = "��ġ �Ϸ�";
        _fileNameText.text = "Install Complete (100%)";
        OnStartGame();
    }

    IEnumerator ShowProgress()
    {
        yield return new WaitUntil(() => downloadHandle.IsValid());

        int dotCount = 1; // �� ���� (�ʱ� 1��)

        while (downloadHandle.PercentComplete < 1)
        {
            _installBar.value = downloadHandle.PercentComplete;
            _fileNameText.text = $"���� ��ġ ��.. " + "(" + $"{downloadHandle.PercentComplete * 100:F2}%)";
            //  �� ���� ���� (1 �� 2 �� 3 �� 1 ��ȯ)
            _installText.text = "��ġ ��" + new string('.', dotCount);
            dotCount = (dotCount % 3) + 1; // 1, 2, 3 ��ȯ

            yield return null;
        }
    }

    private IEnumerator ShowLoadingTips()
    {
        while (true)
        {
            if (GameSceneManager.Instance.TipList.Count > 0)
            {
                string randomTip = GameSceneManager.Instance.TipList[Random.Range(0, GameSceneManager.Instance.TipList.Count)];
                _tipText.text = randomTip; // �ε� ȭ���� �ؽ�Ʈ ������Ʈ
            }
            else
            {
                _tipText.text = "�����͸� �ҷ����� ���߽��ϴ�.";
            }

            yield return new WaitForSeconds(5f); //5�ʸ��� ����
        }
    }

    public void OnStartGame()
    {
        _touchScreen.SetActive(true);
        _startCheck = true;
    }

    public void MoveToIngame()
    {
        StartCoroutine(GameSceneManager.Instance.LoadScene("IngameScene"));
        GameSceneManager.Instance.HideRefreshLoading();
    }
}
