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
    [SerializeField] GameObject _loadingScreen; // �ε� ȭ�� ������Ʈ

    [Header("Prograss & Tip")]
    [SerializeField] Slider _progressBar; // �ε� ���൵ ǥ�ÿ� �����̴�
    [SerializeField] Image _progressArrow;
    [SerializeField] TextMeshProUGUI _loadingText; // �ε� ���� �ؽ�Ʈ ǥ�� (���� ����)
    [SerializeField] TextMeshProUGUI _tipText;

    [Header("Chapter Start")]
    [SerializeField] ChapterStart _chapterStart;

    [Header("Addressable Manager")]
    [SerializeField] string[] _downloadLabels;  // Addressable�� ������ ��ġ�� Label �̸�
    [SerializeField] AddressableDownloadUI _addDownload;

    [Header("Check Internet")]
    [SerializeField] RefreshLoading _refreshLoading;

    private Vector2 _minPos = new Vector2(0f, 0f);  // ȭ��ǥ ���� ��ġ
    private Vector2 _maxPos = new Vector2(1650f, 0f);   // ȭ��ǥ �� ��ġ

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
            Debug.LogError(" Firebase �α����� ��ҵ�.");
            yield break;
        }

        if (loginTask.IsFaulted)
        {
            Debug.LogError(" Firebase �α��� �� ���� �߻�!");
            yield break;
        }
        FirebaseUser user = loginTask.Result;
        bool isNewUser = user.Metadata.CreationTimestamp == user.Metadata.LastSignInTimestamp;
        if (isNewUser)
        {
            Debug.Log($" Firebase�� �� ������ ������! User ID: {user.UserId}");
            NewAccountDataCheck(user.UserId); // �ű� ���� ó��
        }
        else
        {
            Debug.Log($" ���� Firebase �������� �α��ε�. User ID: {user.UserId}");
            StartCoroutine(CheckDownloadSize()); // ���� ���� �α��� ó��
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
                          // ���ο� �����͸� ����
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
                    // ����� ������ ��������
                    UserData userData = JsonUtility.FromJson<UserData>(snapshot.GetRawJsonValue());
                    SaveUserData(userid, userData); // ���� ���� ȣ��
                }
                else
                {
                    Debug.LogWarning("����� �����Ͱ� �������� �ʽ��ϴ�.");
                }
            }
            else
            {
                Debug.LogError("Firebase ������ �������� ����: " + task.Exception);
            }
        });
    }// 4

    private void SaveUserData(string userId, UserData userData)
    {
        if (userData == null)
        {
            Debug.LogWarning("UserData�� null�Դϴ�. ������ �ߴ��մϴ�.");
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
        ShowLoadingScreen(); // �ε� ȭ�� Ȱ��ȭ
        // �⺻ �� �ε�
        AsyncOperation mainSceneLoad = SceneManager.LoadSceneAsync(sceneName);
        mainSceneLoad.allowSceneActivation = false;
       
        while (!mainSceneLoad.isDone)
        {
            float progress = Mathf.Clamp01(mainSceneLoad.progress / 0.9f) * 0.8f;
            UpdateProgress(progress);
            if (mainSceneLoad.progress >= 0.9f && !mainSceneLoad.allowSceneActivation)
            {
                mainSceneLoad.allowSceneActivation = true; // �� Ȱ��ȭ ���
            }
            yield return null; // �ε� �Ϸ���� ���
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

        // �߰� �� �ε�
        AsyncOperation additiveSceneLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Debug.Log("AddtiveScene Ȱ��ȭ");
        while (!additiveSceneLoad.isDone)
            yield return null; // �߰� �� �ε� �Ϸ���� ���

        Scene additiveScene = SceneManager.GetSceneByName(sceneName);
        if (additiveScene.IsValid()) // ���� ����� �ε�Ǿ����� Ȯ��
            SceneManager.SetActiveScene(additiveScene); // Ȱ�� �� ����
        else
            Debug.LogError($"�� {sceneName}�� Ȱ�� ������ ������ �� �����ϴ�.");
    }

    public IEnumerator StageAddtive(string sceneName, string removeName, bool stageCheck = false)
    {
        // �ε� ȭ�� ǥ��
        ShowLoadingScreen();

        // ���� Stage �� ���� (removeName�� ������ ���)
        if (!string.IsNullOrEmpty(removeName))
        {
            Scene sceneToRemove = SceneManager.GetSceneByName(removeName);
            if (sceneToRemove.IsValid())
            {
                AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(removeName);
                while (!unloadOperation.isDone)
                {
                    UpdateProgress(unloadOperation.progress * 0.5f); // ���� ���൵ (0% ~ 50%)
                    yield return null;
                }
                Debug.Log($"�� {removeName} ���� �Ϸ�");
            }
            else
            {
                Debug.LogWarning($"�� {removeName}�� �ε���� �ʾҽ��ϴ�.");
            }
        }

        // ���ο� Stage �� �߰�
        AsyncOperation additiveSceneLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!additiveSceneLoad.isDone)
        {
            float progress = Mathf.Clamp01(additiveSceneLoad.progress / 0.9f);
            UpdateProgress(0.5f + progress * 0.5f); // �ε� ���൵ (50% ~ 100%)
            yield return null;
        }

        // �߰��� �� Ȱ��ȭ
        Scene additiveScene = SceneManager.GetSceneByName(sceneName);
        if (additiveScene.IsValid())
        {
            SceneManager.SetActiveScene(additiveScene);
            Debug.Log($"�� {sceneName} Ȱ��ȭ �Ϸ�");

            // InitializeAddtive ȣ��
            yield return InitializeScene(additiveScene, true);
        }
        else
        {
            Debug.LogError($"�� {sceneName}�� Ȱ�� ������ ������ �� �����ϴ�.");
        }

        // �ε� ȭ�� �����
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

    // �ε� ȭ�� ��Ȱ��ȭ
    private void HideLoadingScreen()
    {
        if (_loadingScreen != null)
        {
            _loadingScreen.SetActive(false);
        }
    }

    // �ε� ���൵ ������Ʈ
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

            // anchoredPosition�� ����ؼ� ���� ��ǥ �������� �̵�
            arrowRect.anchoredPosition = Vector2.Lerp(_minPos, _maxPos, progress);
        }
    }

    public void InitializeTipList()
    {
        ShowRefreshLoading("��ø� ��ٷ� �ּ���..");
        // Firebase Realtime Database���� �����͸� ������
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("tips");
        reference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                // Firebase �����͸� List<string>���� ���� ��ȯ
                foreach (var item in task.Result.Children)
                {
                    _tipList.Add(item.Value.ToString());
                }
                Debug.Log("Firebase �����͸� List<string>���� �������� ����");
            }
            else
            {
                Debug.LogError("Firebase ������ �������� ����");
            }
        });
        HideRefreshLoading();
    }

    private void UpdateLoadingTip()
    {
        if (_tipList.Count > 0)
        {
            string randomTip = _tipList[Random.Range(0, _tipList.Count)];
            _tipText.text = randomTip; // �ε� ȭ���� �ؽ�Ʈ ������Ʈ
        }
        else
        {
            _tipText.text = "�����͸� �ҷ����� ���߽��ϴ�.";
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
                _refreshLoading.ShowLoading("��Ʈ��ũ ���� ���Դϴ�..");
            }
            else
            {
                Time.timeScale = 1f;
                _refreshLoading.HideRefreshLoading();
            }
            Debug.Log($"���ͳ� ����: {(isInternetAvailable ? "�����" : "������� ����")}");
            yield return new WaitForSeconds(5); // 5�ʸ��� Ȯ��
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

    // �ʱ�ȭ �۾� ó�� (ISceneInitializable �������̽� ���)
    private IEnumerator InitializeScene(Scene scene, bool isAdditive = false)
    {
        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            ISceneInitializable initializable = rootObject.GetComponent<ISceneInitializable>();
            if (initializable != null)
            {
                if (isAdditive)
                {
                    Debug.Log($"�� {scene.name}�� InitializeAddtive() ����");
                    yield return initializable.InitializeAddtive();
                    UpdateProgress(0.9f); // 100%�� ����
                }
                else
                {
                    Debug.Log($"�� {scene.name}�� InitializeScene() ����");
                    yield return initializable.InitializeScene();
                    UpdateProgress(0.9f); // 100%�� ����
                }
            }
        }
        UpdateProgress(1f); // 100%�� ����
        yield return new WaitForSeconds(0.5f);
        HideLoadingScreen(); // �ε� ȭ�� ��Ȱ��ȭ
    }
    #endregion region [OnSceneLoaded & InitializeScene Methods]

    #region [Addressable Methods]
    IEnumerator CheckDownloadSize()
    {
        yield return new WaitForSeconds(1f);

        long totalSize = 0;

        // ��� �󺧿� ���� �ٿ�ε� ũ�� Ȯ��
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
            // UI ǥ��
            _refreshLoading.HideRefreshLoading();
            _addDownload.ShowUI(totalSize);
            Debug.Log($"�ٿ�ε��ؾ� �� ��ü ũ��: {totalSize / 1024f:F2} KB");
        }
        else
        {
            Debug.Log("��� ���ҽ��� �̹� �ٿ�ε�Ǿ� ����.");
            StartCoroutine(GameSceneManager.Instance.LoadScene("IngameScene"));
            GameSceneManager.Instance.HideRefreshLoading();
            yield break;
        }
    }

    public void MoveToDownloadScene()
    {
        HideRefreshLoading();
        StartCoroutine(LoadScene("DownloadScene")); //���������� �ڷ�ƾ ����
        _addDownload.HideUI();
    }

    #endregion [Addressable Methods]
}
