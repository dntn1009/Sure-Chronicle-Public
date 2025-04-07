using DefineHelper;
using DesignPattern;
using fsmStatePattern;
using GooglePlayGames;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMonobehaviour<UIManager>, UIFacadePattern, IUISubject
{

    [Header("Bottom UI Window")]
    [SerializeField] WindowController[] _windows;
    [SerializeField] Button _heroBtn;
    [SerializeField] Button _pullBtn;
    [SerializeField] Button _enforceBtn;
    [SerializeField] Button _raidBtn;
    [SerializeField] HeroDrawWindow _drawWindow;

    [Header("Top UI")]
    [Header("Top Single")]
    [SerializeField] StagePrograssBar _stageBar;
    [SerializeField] Button _autoBtn;
    [SerializeField] IngameUserInfoUI _userInfoUI;

    [Header("Top Window")]
    [SerializeField] TWindowController[] _topWindows;
    [SerializeField] Button _settingBtn;
    [SerializeField] Button _messageBtn;
    [SerializeField] Button _rankBtn;

    [Header("Color")]
    [SerializeField] Color _initColorB;
    [SerializeField] Color _pushColorB;

    public WindowState _windowState;

    [Header("JoyStick")]
    [SerializeField] Joystick _joy;
    public Joystick JoyStick { get { return _joy; } }

    [Header("Damage Font")]
    [SerializeField] DamageFont _damageFont;
    GameObjectPool<DamageFont> _damagePool;

    [Header("Boss Spawn")]
    [SerializeField] BossSpawn _bossSpawn;
    public BossSpawn BOSSSpawn { get { return _bossSpawn; } }

    [Header("Chapter Window")]
    [SerializeField] ChapterWindow _chapterWindow;
    public ChapterWindow ChapterWindow { get { return _chapterWindow; } }

    [Header("Observer List")]
    [SerializeField] List<IUIObserver> _uiObserver;

    protected override void OnAwake()
    {
        base.OnAwake();
        InitializeSet();
    }

    #region [Init Methods]
    public void InitializeSet()
    {
        _chapterWindow.Initialize();
        InitDamageFont();
        InitWindowBtn();
        InitTWindowBtn();
        _autoBtn.onClick.AddListener(AutoRefresh);
        _uiObserver = new List<IUIObserver>();
        InitRegusterObservers();
    }
    public void InitWindowBtn()
    {
        _heroBtn.onClick.AddListener(() => WindowOpenOrClose(WindowState.Hero));
        _pullBtn.onClick.AddListener(() => WindowOpenOrClose(WindowState.Pull));
        _enforceBtn.onClick.AddListener(() => WindowOpenOrClose(WindowState.Enforce));
        _raidBtn.onClick.AddListener(() => WindowOpenOrClose(WindowState.Raid));
    }
    public void InitTWindowBtn()
    {
        _settingBtn.onClick.AddListener(() => _topWindows[0].OpenWindow());
        _messageBtn.onClick.AddListener(() => _topWindows[1].OpenWindow());
        _rankBtn.onClick.AddListener(() => AchieveMentShowUI());
    }


    public void InitRegusterObservers()
    {
        foreach (IUIObserver observer in _windows)
        {
            RegisterObserver(observer);
        }
    }

    public void WindowsOperate()
    {
        foreach (WindowController window in _windows)
        {
            window.Operate();
        }
    }

    public void TWindowOperate()
    {
        foreach (TWindowController window in _topWindows)
        {
            window.Operate();
        }
    }

    #endregion [Init Methods]

    #region [Bottom UI Methods]

    public void WindowOpenOrClose(WindowState state)
    {
        bool noneCheck = false;
        foreach (WindowController window in _windows)
        {
            if (window.MyState == _windowState)
            {
                window.CloseWindow();
                if (window.MyState == state)
                    noneCheck = true;
            }
            else if (window.MyState != state)
                window.CloseWindow();
            else
            {
                window.OpenWindow();
            }
        }
        if (noneCheck)
            _windowState = WindowState.None;
        else
            _windowState = state;
        btnColorChange();
    }

    public void btnColorChange()
    {
        switch (_windowState)
        {
            case WindowState.Hero:
                PushColor(_heroBtn);
                InitColor(_pullBtn);
                InitColor(_enforceBtn);
                InitColor(_raidBtn);
                break;
            case WindowState.Pull:
                PushColor(_pullBtn);
                InitColor(_heroBtn);
                InitColor(_enforceBtn);
                InitColor(_raidBtn);
                break;
            case WindowState.Enforce:
                PushColor(_enforceBtn);
                InitColor(_pullBtn);
                InitColor(_heroBtn);
                InitColor(_raidBtn);
                break;
            case WindowState.Raid:
                PushColor(_raidBtn);
                InitColor(_pullBtn);
                InitColor(_enforceBtn);
                InitColor(_heroBtn);
                break;
            case WindowState.None:
                InitColor(_heroBtn);
                InitColor(_pullBtn);
                InitColor(_enforceBtn);
                InitColor(_raidBtn);
                break;
        }
    }
    public void PushColor(Button button)
    {
        Image img = button.GetComponent<Image>();
        img.color = _pushColorB;
    }

    public void InitColor(Button button)
    {
        Image img = button.GetComponent<Image>();
        img.color = _initColorB;
    }
    #endregion [Bottom UI Methods]

    #region [Top UI Methods]

    public void InitializeUserInfoUI(UserData userData)
    {
        _userInfoUI.InitializeSet(userData);
    }

    public void SetStagePrograssBar(StageData stageData)
    {
        _stageBar.ChangeAmountFromStageByUserData(DataManager.Instance.StageData);
    }

    public bool InitAutoRefresh()
    {
        bool auto = DataManager.Instance.IsAuto;
        //Debug.Log("AUTO : " + DataManager.Instance.IsAuto);
        if (auto)
            _autoBtn.GetComponent<Animator>().SetBool("AutoCheck", true);
        else
            _autoBtn.GetComponent<Animator>().SetBool("AutoCheck", false);

        return auto;
    }

    public void AutoRefresh()
    {
        DataManager.Instance.SaveAuto(() =>
        {
            if (DataManager.Instance.IsAuto)
            {
                _autoBtn.GetComponent<Animator>().SetBool("AutoCheck", true);
                DataManager.Instance.GetHeroManager().HeroChangeState(new hSearchState());
            }
            else
            {
                _autoBtn.GetComponent<Animator>().SetBool("AutoCheck", false);
                DataManager.Instance.GetHeroManager().HeroChangeState(new hIdleState());
            }
            AudioManager.Instance.LoadSFX("SFX/AutoButton.wav");
        });
    }

    #endregion [Top UI Methods]

    #region [Observer Pattern Methods]
    public void RegisterObserver(IUIObserver observer)
    {
        _uiObserver.Add(observer);
    }

    public void RemoveObserver(IUIObserver observer)
    {
        _uiObserver.Remove(observer);
    }

    public void NotifyObservers(UserData userdata)
    {
        foreach (IUIObserver observer in _uiObserver)
        {
            observer.Notify(userdata);
        }
        _userInfoUI.SetGoldJewel(userdata);
    }
    #endregion [Observer Pattern Methods]

    #region [Window UI Methods]

    public WindowController GetWindows(WindowState windowState)
    {
        return _windows[(int)windowState - 1];
    }

    public HeroWindow GetHeroWindow()
    {
        return GetWindows(WindowState.Hero).GetComponent<HeroWindow>();
    }

    public PullWindow GetPullWindow()
    {
        return GetWindows(WindowState.Pull).GetComponent<PullWindow>();
    }

    public HeroDrawWindow GetDrawWindow()
    {
        return _drawWindow;
    }

    #endregion [Window UI Methods]

    #region [Android & GPGS UI Methods]
    public void ShowToast(string message)
    {
        // 안드로이드 플랫폼에서만 실행
        if (Application.platform == RuntimePlatform.Android)
        {
            // Android Java를 호출
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                using (AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast"))
                {
                    activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                    {
                        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
                        AndroidJavaObject toast = toastClass.CallStatic<AndroidJavaObject>("makeText", context, message, 0); // 0 = Toast.LENGTH_SHORT
                        toast.Call("show");
                    }));
                }
            }
        }
        else
        {
            Debug.Log($"Toast Message: {message} (이 메시지는 에디터에서 확인할 수 없습니다)");
        }
    }

    public void AchieveMentShowUI()
    {
        PlayGamesPlatform.Instance.ShowAchievementsUI();
    }

    #endregion [Android Methods]

    #region [Damage Font Methods]
    private void InitDamageFont()
    {
        _damagePool = new GameObjectPool<DamageFont>(20, () =>
        {
            var obj = Instantiate(_damageFont);
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
            var damage = obj.GetComponent<DamageFont>();
            //damage.InitializeSet();
            return damage;
        });
    }

    public void CreateDemageFont(int damage, bool isCritical, Vector3 position)
    {
        var font = _damagePool.Get();
        font.transform.position = position;
        font.SetDamageText(damage, isCritical, 1f, 1.5f);
    }

    public void CreateHealFont(int amount, Vector3 position)
    {
        var font = _damagePool.Get();
        font.transform.position = position;
        font.SetHealText(amount, 1f, 1.5f);
    }

    public void CreateGoldFont(int amount, Vector3 position, bool isjewel = false)
    {
        var font = _damagePool.Get();
        font.transform.position = position;
        font.SetGoldText(amount, 1f, 1.5f, isjewel);
    }

    public void RemoveDamageFont(DamageFont font)
    {
        font.ResetFont();
        font.gameObject.SetActive(false);
        _damagePool.Set(font);
    }

    #endregion [Damage Font Methods]

}
