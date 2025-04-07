using Firebase.Auth;
using Firebase.Database;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class IntroManager : SingletonMonobehaviour<IntroManager>
{
    [Header("UI")]
    [Header("Title & Version Text")]
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] TextMeshProUGUI _verText;
    [SerializeField] TextMeshProUGUI _companyText;

    [Header("String Data")]
    [SerializeField] string _titleStr;
    [SerializeField] string _versionStr;
    [SerializeField] string _companyStr;

    [Header("Login")]
    [SerializeField] TextMeshProUGUI _screenTouch;

    [Header("Privacy")]
    [SerializeField] PrivacyUI _privacyUI;

    FirebaseAuth _auth;

    bool _isTouch; // 터치확인
    bool _clickCheck; // 중복확인
    //bool _newCheck;

    bool _screenCheck;
    public bool ScreenCheck { set { _screenCheck = value; } }

    string userId;
    protected override void OnStart()
    {
        base.OnStart();
        InitializeSet();
        GameSceneManager.Instance.InitializeTipList();
        CheckAndShowPrivacyUI();
    }
   
    private void Update()
    {
        if (_screenCheck)
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    _isTouch = true;
                }

                if (touch.phase == TouchPhase.Ended && _isTouch && !_clickCheck)
                {
                    _isTouch = false;
                    _clickCheck = true; // 중복 실행 방지
                    _screenCheck = false;
                    MoveToIngame();
                }
            }
    }

    #region [UI Setting Methods]

    public void InitializeSet()
    {
        _auth = FirebaseAuth.DefaultInstance;
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        _screenCheck = false;
        _privacyUI.InitializeSet();
        _screenTouch.gameObject.SetActive(true);
        _clickCheck = false;
        _isTouch = false;
        _versionStr = Application.version;
        _verText.text = "Ver " + _versionStr;
        _titleText.text = _titleStr;
        _companyText.text = _companyStr;
    }

    #endregion [UI Setting Methods]

    #region [Login & Move Scene Methods]
    public void MoveToIngame()
    {
        GameSceneManager.Instance.ShowRefreshLoading("로그인 중입니다..");
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // 서버 인증 코드 요청
            PlayGamesPlatform.Instance.RequestServerSideAccess(false, (authCode) =>
            {
                if (!string.IsNullOrEmpty(authCode))
                {
                    Debug.Log($"Server Auth Code: {authCode}");
                    //GameSceneManager.Instance.GetGooglePlayUserInfo();
                    StartCoroutine(GameSceneManager.Instance.TryFirebaseLogin(authCode));
                }
                else
                {
                    Debug.LogError("Failed to retrieve Server Auth Code. Check Web Client ID and SHA-1 settings.");
                }
            });
        }
        else
            Debug.LogError($"Google Sign-In Failed: {status}");

    }// 1

    private void CheckAndShowPrivacyUI()
    {
       
        if (_auth.CurrentUser == null)
        {
            Debug.Log(" Firebase Auth에 로그인된 계정 없음 → 보호정책 UI 띄우기");
            _privacyUI.gameObject.SetActive(true); // 보호정책 UI 표시
        }
        else
        {
            Debug.Log(" Firebase Auth에 기존 계정 존재 → 보호정책 UI 숨기기");
            _privacyUI.gameObject.SetActive(false); // 보호정책 UI 숨기기
            _screenCheck = true;
        }
    }

    #endregion [Login & Move Scene Methods]
}