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
    bool _isTouch; // 터치 확인
    bool _startCheck; //다운 설치 완료됐는지 체크
    bool _clickCheck; // 중복 방지
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
                GameSceneManager.Instance.ShowRefreshLoading("로딩 중입니다..");
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
                Debug.Log($"{label} 다운로드 완료!");
            }
            else
            {
                Debug.LogError($"{label} 다운로드 실패!");
            }

            Addressables.Release(downloadHandle);
        }

        Debug.Log("모든 리소스 다운로드 완료!");
        _installBar.value = 1;
        _installText.text = "설치 완료";
        _fileNameText.text = "Install Complete (100%)";
        OnStartGame();
    }

    IEnumerator ShowProgress()
    {
        yield return new WaitUntil(() => downloadHandle.IsValid());

        int dotCount = 1; // 점 개수 (초기 1개)

        while (downloadHandle.PercentComplete < 1)
        {
            _installBar.value = downloadHandle.PercentComplete;
            _fileNameText.text = $"파일 설치 중.. " + "(" + $"{downloadHandle.PercentComplete * 100:F2}%)";
            //  점 개수 조절 (1 → 2 → 3 → 1 순환)
            _installText.text = "설치 중" + new string('.', dotCount);
            dotCount = (dotCount % 3) + 1; // 1, 2, 3 순환

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
                _tipText.text = randomTip; // 로딩 화면의 텍스트 업데이트
            }
            else
            {
                _tipText.text = "데이터를 불러오지 못했습니다.";
            }

            yield return new WaitForSeconds(5f); //5초마다 실행
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
