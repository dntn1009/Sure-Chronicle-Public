using DesignPattern;
using Firebase.Database;
using Firebase.Extensions;
using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DefineHelper;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Firebase.Auth;
using System.Threading.Tasks;

public class GameSceneManager : DonDestory<GameSceneManager>
{
    [Header("UI")]
    [Header("Loading")]
    [SerializeField] GameObject _loadingScreen; // 로딩 화면 오브젝트

    [Header("Prograss & Tip")]
    [SerializeField] Slider _progressBar; // 로딩 진행도 표시용 슬라이더
    [SerializeField] Image _progressArrow;
    [SerializeField] TextMeshProUGUI _loadingText; // 로딩 상태 텍스트 표시 (선택 사항)
    [SerializeField] TextMeshProUGUI _tipText;

    [Header("Chapter Start")]
    [SerializeField] ChapterStart _chapterStart;

    [Header("Addressable Manager")]
    [SerializeField] string[] _downloadLabels;  // Addressable에 설정한 설치한 Label 이름
    [SerializeField] AddressableDownloadUI _addDownload;

    [Header("Check Internet")]
    [SerializeField] RefreshLoading _refreshLoading;

    private Vector2 _minPos = new Vector2(0f, 0f);  // 화살표 시작 위치
    private Vector2 _maxPos = new Vector2(1650f, 0f);   // 화살표 끝 위치

    private bool isInternetAvailable;

    private List<string> _tipList = new List<string>();

    public List<string> TipList { get { return _tipList; } }
    public string[] DownloadLabels { get { return _downloadLabels; } }

    protected override void OnStart()
    {
        base.OnStart();
        SceneManager.sceneLoaded += OnSceneLoaded;
        HideLoadingScreen();
        _refreshLoading.HideRefreshLoading();
        _addDownload.HideUI();
        StartCoroutine(CheckInternetConnection());
    }

    #region [Intro Scene & new accountData Methods]
    public IEnumerator TryFirebaseLogin(string authCode)
    {
        yield return null;

        Credential credential = PlayGamesAuthProvider.GetCredential(authCode);
        /*FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Firebase login was canceled.");
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Firebase login encountered an error:");
            }
            else
            {
                FirebaseUser user = task.Result;
                string userId = user.UserId;
                Debug.Log($"Firebase login successful! User ID: {userId}");
                if (newCheck)
                    NewAccountDataCheck(userId);
                else
                    StartCoroutine(CheckDownloadSize());
            }
        });*/

        Task<FirebaseUser> loginTask = FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(credential);
        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.IsCanceled)
        {
            Debug.LogError(" Firebase 로그인이 취소됨.");
            yield break;
        }

        if (loginTask.IsFaulted)
        {
            Debug.LogError(" Firebase 로그인 중 오류 발생!");
            yield break;
        }
        FirebaseUser user = loginTask.Result;
        bool isNewUser = user.Metadata.CreationTimestamp == user.Metadata.LastSignInTimestamp;
        if (isNewUser)
        {
            Debug.Log($" Firebase에 새 계정이 생성됨! User ID: {user.UserId}");
            NewAccountDataCheck(user.UserId); // 신규 계정 처리
        }
        else
        {
            Debug.Log($" 기존 Firebase 계정으로 로그인됨. User ID: {user.UserId}");
            StartCoroutine(CheckDownloadSize()); // 기존 계정 로그인 처리
        }

    } // 2

    public void NewAccountDataCheck(string userId)
    {
        FirebaseDatabase.DefaultInstance.RootReference
              .Child("users")
              .Child(userId)
              .GetValueAsync()
              .ContinueWith(databaseTask =>
              {
                  if (databaseTask.IsFaulted)
                  {
                      Debug.LogError("Error checking user existence: " + databaseTask.Exception);
                  }
                  else if (databaseTask.IsCompleted)
                  {
                      DataSnapshot snapshot = databaseTask.Result;

                      if (snapshot.Exists)
                      {
                          StartCoroutine(CheckDownloadSize());
                      }
                      else
                      {
                          Debug.Log($"User ID {userId} does not exist. Creating new data...");
                          // 새로운 데이터를 생성
                          SetNewAccountData(userId);
                      }
                  }
              });
    }// 3

    public void SetNewAccountData(string userid)
    {
        FirebaseDatabase.DefaultInstance.RootReference
        .Child("newAccount")
        .Child("userData")
        .GetValueAsync()
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result != null)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    // 사용자 데이터 가져오기
                    UserData userData = JsonUtility.FromJson<UserData>(snapshot.GetRawJsonValue());
                    SaveUserData(userid, userData); // 저장 로직 호출
                }
                else
                {
                    Debug.LogWarning("사용자 데이터가 존재하지 않습니다.");
                }
            }
            else
            {
                Debug.LogError("Firebase 데이터 가져오기 실패: " + task.Exception);
            }
        });
    }// 4

    private void SaveUserData(string userId, UserData userData)
    {
        if (userData == null)
        {
            Debug.LogWarning("UserData가 null입니다. 저장을 중단합니다.");
            return;
        }

        string jsonData = JsonUtility.ToJson(userData);
        FirebaseDatabase.DefaultInstance.RootReference
            .Child("users")
            .Child(userId)
            .SetRawJsonValueAsync(jsonData)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("User data saved successfully.");
                    StartCoroutine(CheckDownloadSize());
                }
                else
                {
                    Debug.LogError($"Failed to save user data: {task.Exception}");
                }
            });
    }//5
    #endregion [Intro Scene & new accountData Methods]

    #region [Load Scene & Addtive Stage Scene Methods]
    public IEnumerator LoadScene(string sceneName)
    {
        ShowLoadingScreen(); // 로딩 화면 활성화
        // 기본 씬 로드
        AsyncOperation mainSceneLoad = SceneManager.LoadSceneAsync(sceneName);
        mainSceneLoad.allowSceneActivation = false;
       
        while (!mainSceneLoad.isDone)
        {
            float progress = Mathf.Clamp01(mainSceneLoad.progress / 0.9f) * 0.8f;
            UpdateProgress(progress);
            if (mainSceneLoad.progress >= 0.9f && !mainSceneLoad.allowSceneActivation)
            {
                mainSceneLoad.allowSceneActivation = true; // 씬 활성화 허용
            }
            yield return null; // 로드 완료까지 대기
        }
        AudioManager.Instance.LoadBGMFromScene(sceneName);
        _addDownload.HideUI();
    }

    public IEnumerator StageAddtive(StageData stageData)
    {
        string sceneName = string.Empty;
        if (stageData._stageNumber % 2 == 1)
            sceneName = "Stage1";
        else
            sceneName = "Stage2";

        // 추가 씬 로드
        AsyncOperation additiveSceneLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Debug.Log("AddtiveScene 활성화");
        while (!additiveSceneLoad.isDone)
            yield return null; // 추가 씬 로드 완료까지 대기

        Scene additiveScene = SceneManager.GetSceneByName(sceneName);
        if (additiveScene.IsValid()) // 씬이 제대로 로드되었는지 확인
            SceneManager.SetActiveScene(additiveScene); // 활성 씬 설정
        else
            Debug.LogError($"씬 {sceneName}를 활성 씬으로 설정할 수 없습니다.");
    }

    public IEnumerator StageAddtive(string sceneName, string removeName, bool stageCheck = false)
    {
        // 로딩 화면 표시
        ShowLoadingScreen();

        // 기존 Stage 씬 제거 (removeName이 지정된 경우)
        if (!string.IsNullOrEmpty(removeName))
        {
            Scene sceneToRemove = SceneManager.GetSceneByName(removeName);
            if (sceneToRemove.IsValid())
            {
                AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(removeName);
                while (!unloadOperation.isDone)
                {
                    UpdateProgress(unloadOperation.progress * 0.5f); // 제거 진행도 (0% ~ 50%)
                    yield return null;
                }
                Debug.Log($"씬 {removeName} 제거 완료");
            }
            else
            {
                Debug.LogWarning($"씬 {removeName}가 로드되지 않았습니다.");
            }
        }

        // 새로운 Stage 씬 추가
        AsyncOperation additiveSceneLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!additiveSceneLoad.isDone)
        {
            float progress = Mathf.Clamp01(additiveSceneLoad.progress / 0.9f);
            UpdateProgress(0.5f + progress * 0.5f); // 로드 진행도 (50% ~ 100%)
            yield return null;
        }

        // 추가된 씬 활성화
        Scene additiveScene = SceneManager.GetSceneByName(sceneName);
        if (additiveScene.IsValid())
        {
            SceneManager.SetActiveScene(additiveScene);
            Debug.Log($"씬 {sceneName} 활성화 완료");

            // InitializeAddtive 호출
            yield return InitializeScene(additiveScene, true);
        }
        else
        {
            Debug.LogError($"씬 {sceneName}를 활성 씬으로 설정할 수 없습니다.");
        }

        // 로딩 화면 숨기기
        HideLoadingScreen();

        if (stageCheck)
        {
            string chapter = "Chapter " + DataManager.Instance.StageData._stageNumber + "-" + DataManager.Instance.StageData._roundNumber;
            ShowChapterStart(chapter, "STAGE");
        }
    }
    #endregion region [Load Scene & Addtive Stage Scene Methods]

    #region [PrograssBar & Tip Methods]
    private void ShowLoadingScreen()
    {
        if (_loadingScreen != null)
        {
            UpdateLoadingTip();
            _loadingScreen.SetActive(true);
        }
    }

    // 로딩 화면 비활성화
    private void HideLoadingScreen()
    {
        if (_loadingScreen != null)
        {
            _loadingScreen.SetActive(false);
        }
    }

    // 로딩 진행도 업데이트
    private void UpdateProgress(float progress)
    {
        if (_progressBar != null)
        {
            _progressBar.value = progress;
        }

        if (_loadingText != null)
        {
            _loadingText.text = $"{(progress * 100):0}%";
        }
        if(_progressArrow != null)
        {
            RectTransform arrowRect = _progressArrow.GetComponent<RectTransform>();

            // anchoredPosition을 사용해서 로컬 좌표 기준으로 이동
            arrowRect.anchoredPosition = Vector2.Lerp(_minPos, _maxPos, progress);
        }
    }

    public void InitializeTipList()
    {
        ShowRefreshLoading("잠시만 기다려 주세요..");
        // Firebase Realtime Database에서 데이터를 가져옴
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("tips");
        reference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                // Firebase 데이터를 List<string>으로 직접 변환
                foreach (var item in task.Result.Children)
                {
                    _tipList.Add(item.Value.ToString());
                }
                Debug.Log("Firebase 데이터를 List<string>으로 가져오기 성공");
            }
            else
            {
                Debug.LogError("Firebase 데이터 가져오기 실패");
            }
        });
        HideRefreshLoading();
    }

    private void UpdateLoadingTip()
    {
        if (_tipList.Count > 0)
        {
            string randomTip = _tipList[Random.Range(0, _tipList.Count)];
            _tipText.text = randomTip; // 로딩 화면의 텍스트 업데이트
        }
        else
        {
            _tipText.text = "데이터를 불러오지 못했습니다.";
        }
    }
    #endregion [PrograssBar & Tip Methods]

    #region [Chapter Start Methods]

    public void ShowChapterStart(string chapterText, string stageText)
    {
        _chapterStart.ShowChapterStart(chapterText, stageText);
    }

    public void HideChapterStart()
    {
        _chapterStart.HideChapterStart();
    }

    #endregion [Chapter Start Methods]

    #region [Check Internet Connection Methods]
    private IEnumerator CheckInternetConnection()
    {
        while (true)
        {
            isInternetAvailable = Application.internetReachability != NetworkReachability.NotReachable;
            if (!isInternetAvailable)
            {
                Time.timeScale = 0f;
                _refreshLoading.ShowLoading("네트워크 연결 중입니다..");
            }
            else
            {
                Time.timeScale = 1f;
                _refreshLoading.HideRefreshLoading();
            }
            Debug.Log($"인터넷 상태: {(isInternetAvailable ? "연결됨" : "연결되지 않음")}");
            yield return new WaitForSeconds(5); // 5초마다 확인
        }
    }

    public void ShowRefreshLoading(string text)
    {
        _refreshLoading.ShowLoading(text);
    }

    public void HideRefreshLoading()
    {
        _refreshLoading.HideRefreshLoading();
    }

    #endregion [Check Internet Connection Methods]

    #region [OnSceneLoaded & InitializeScene Methods]

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(mode == LoadSceneMode.Additive)
            StartCoroutine(InitializeScene(scene, true));
        else
            StartCoroutine(InitializeScene(scene));
    }

    // 초기화 작업 처리 (ISceneInitializable 인터페이스 사용)
    private IEnumerator InitializeScene(Scene scene, bool isAdditive = false)
    {
        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            ISceneInitializable initializable = rootObject.GetComponent<ISceneInitializable>();
            if (initializable != null)
            {
                if (isAdditive)
                {
                    Debug.Log($"씬 {scene.name}의 InitializeAddtive() 실행");
                    yield return initializable.InitializeAddtive();
                    UpdateProgress(0.9f); // 100%로 설정
                }
                else
                {
                    Debug.Log($"씬 {scene.name}의 InitializeScene() 실행");
                    yield return initializable.InitializeScene();
                    UpdateProgress(0.9f); // 100%로 설정
                }
            }
        }
        UpdateProgress(1f); // 100%로 설정
        yield return new WaitForSeconds(0.5f);
        HideLoadingScreen(); // 로딩 화면 비활성화
    }
    #endregion region [OnSceneLoaded & InitializeScene Methods]

    #region [Addressable Methods]
    IEnumerator CheckDownloadSize()
    {
        yield return new WaitForSeconds(1f);

        long totalSize = 0;

        // 모든 라벨에 대해 다운로드 크기 확인
        foreach (string label in _downloadLabels)
        {
            var getSizeHandle = Addressables.GetDownloadSizeAsync(label);
            yield return getSizeHandle;

            if (getSizeHandle.Status == AsyncOperationStatus.Succeeded)
            {
                totalSize += getSizeHandle.Result;
            }
            Addressables.Release(getSizeHandle);
        }

        if (totalSize > 0)
        {
            // UI 표시
            _refreshLoading.HideRefreshLoading();
            _addDownload.ShowUI(totalSize);
            Debug.Log($"다운로드해야 할 전체 크기: {totalSize / 1024f:F2} KB");
        }
        else
        {
            Debug.Log("모든 리소스가 이미 다운로드되어 있음.");
            StartCoroutine(GameSceneManager.Instance.LoadScene("IngameScene"));
            GameSceneManager.Instance.HideRefreshLoading();
            yield break;
        }
    }

    public void MoveToDownloadScene()
    {
        HideRefreshLoading();
        StartCoroutine(LoadScene("DownloadScene")); //정상적으로 코루틴 실행
        _addDownload.HideUI();
    }

    #endregion [Addressable Methods]
}
