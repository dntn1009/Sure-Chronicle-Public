using DefineHelper;
using DesignPattern;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Functions;
using GoogleMobileAds.Api;
using GooglePlayGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DataManager : SingletonMonobehaviour<DataManager>, ISceneInitializable
{
    #region [SerializeField & Var]
    [Header("Camera")]
    [SerializeField] CameraTestController _camera;

    [Header("Public Cost")]
    [SerializeField] PublicCost _publicCost;

    [Header("Card & Card Data")]
    [SerializeField] Card _card;
    [SerializeField] CardData[] _cardDatas;

    [Header("Box & Box Data")]
    [SerializeField] DrawBox _box;
    [SerializeField] BoxData[] _boxDatas;

    [Header("User Data")]
    [SerializeField] UserData _userData;

    [Header("Weight Random Draw Item")]
    [SerializeField] List<WeightedItem<int>> _drawItem;
    [SerializeField] List<WeightedItem<int>> _specialDrawItem;

    [Header("Monster Base Stat")]
    [SerializeField] MonsterData _monBaseData;

    [Header("Field Hero Prefabs")]
    [SerializeField] HeroManager _heroManager;

    [Header("Build Date")]
    [SerializeField] string _buildDate;

    [Header("Messages")]
    [SerializeField] List<MessageData> _messageList = new List<MessageData>();

    [Header("Google Ads")]
    [SerializeField] string adUnitId;

    [Header("Round")]
    public int _roundMAX = 30;

    bool _isRaid;

    #endregion [SerializeField & Var]

    #region [Property]
    public List<MessageData> MessageList { get { return _messageList; } }
    public PublicCost PublicCost { get { return _publicCost; } }
    public StageData StageData { get { return _userData._stageData; } }
    public TicketData TicketData { get { return _userData._ticketData; } }
    public List<UserCardData> UserCardList { get { return _userData._userCardList; } }
    public SharedEnforceData SharedEnforceData { get { return _userData._sharedEnforceData; } }
    public Card Card { get { return _card; } }
    public CardData[] CardDatas { get { return _cardDatas; } }
    public DrawBox DrawBox { get { return _box; } }
    public BoxData[] BoxDatas { get { return _boxDatas; } }

    public int Gold { get { return _userData._gold; } set { _userData._gold = value; } }
    public int Jewel { get { return _userData._jewel; } set { _userData._jewel = value; } }
    public bool IsAuto { get { return _userData._isAuto; } set { _userData._isAuto = value; } }
    public Vector3 InitHeroPosition { get { return _userData._initPos; } set { _userData._initPos = value; } }

    public string BuildDate { get { return _buildDate; } }

    public bool IsRaid { get { return _isRaid; } set { _isRaid = value; } }

    #endregion [Property]

    private FirebaseAuth _auth;
    private DatabaseReference _databaseRef;
    private RewardedAd _rewardedAd;

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnStart()
    {
        base.OnStart();
        GoogleAdsInitialize();
        _isRaid = false;
    }

    #region [Initialze Methods]
    public IEnumerator InitializeScene()
    {
        Debug.Log("Firebase 데이터 초기화 시작");

        // Firebase 초기화
        _auth = FirebaseAuth.DefaultInstance;
        _databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        // 사용자 데이터 가져오기
        Task<DataSnapshot> task = _databaseRef.Child("users").Child(_auth.CurrentUser.UserId).GetValueAsync();
        while (!task.IsCompleted) // Firebase 작업 완료 대기
        {
            yield return null;
        }

        if (task.IsCompleted && task.Result != null)
        {
            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                _userData = JsonUtility.FromJson<UserData>(snapshot.GetRawJsonValue());
                Debug.Log("Firebase 데이터 로드 성공");

                // StageAdditive 호출
                yield return GameSceneManager.Instance.StageAddtive(_userData._stageData);

                // UI 관련 작업 실행
                UnityMainThreadDispatcher.Instance.Enqueue(() =>
                {
                    UIManager.Instance.InitializeUserInfoUI(_userData);
                    UIManager.Instance.BOSSSpawn.InitializeSet(_userData);
                    UIManager.Instance.SetStagePrograssBar(_userData._stageData);
                    UIManager.Instance.WindowsOperate();
                    CheckPendingPurchases();
                });

                yield return null;
            }
            else
                Debug.LogWarning("사용자 데이터가 존재하지 않습니다.");
        }
        else
            Debug.LogError("Firebase 데이터 가져오기 실패: " + task.Exception);
        //Window & StageBar & more UI..

        Task<DataSnapshot> task_T = _databaseRef.Child("messages").Child(_auth.CurrentUser.UserId).GetValueAsync();
        while (!task_T.IsCompleted) // Firebase 작업 완료 대기
        {
            yield return null;
        }
        Debug.Log("message tag 확인");
        if (task_T.IsCompleted && task_T.Result != null)
        {
            DataSnapshot snapshot = task_T.Result;
            if (snapshot.Exists)
            {
                foreach (var item in snapshot.Children)
                {
                    Debug.Log("message tag 추가");
                    string json = item.GetRawJsonValue();
                    MessageData messageData = JsonUtility.FromJson<MessageData>(json);

                    _messageList.Add(messageData);
                }
                yield return null;
            }
        }
        else
            Debug.LogError("Firebase 데이터 가져오기 실패: " + task_T.Exception);

        UnityMainThreadDispatcher.Instance.Enqueue(() =>
        {
            UIManager.Instance.TWindowOperate();
            string chapter = "Chapter " + StageData._stageNumber + "-" + StageData._roundNumber;
            GameSceneManager.Instance.ShowChapterStart(chapter, "STAGE");
        });
        //TWindow & Message
    }

    public IEnumerator InitializeAddtive()
    {
        Debug.Log("Additive 실행 디리리리리리링");
        yield return null;
    }
    #endregion [Initialze Methods]

    #region [Firebase Methods]
    public void TestLoadUserInit()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                _databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase 초기화 성공!");
                TestLoadUserData("tyBNAUWIqQhgjA94LUqRp72w35q2"); // 데이터 가져오기 실행
            }
            else
            {
                Debug.LogError("Firebase 초기화 실패: " + task.Result);
            }
        });
    }
    public void TestLoadUserData(string userId)
    {
        Debug.Log("FirebaseGetUserData 진입");

        _databaseRef.Child("users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result.Exists)
                {
                    string jsonData = task.Result.GetRawJsonValue();
                    Debug.Log("가져온 데이터: " + jsonData);

                    // JSON 데이터를 UserData 객체로 변환
                    _userData = JsonUtility.FromJson<UserData>(jsonData);

                }
                else
                {
                    Debug.LogWarning("데이터를 찾을 수 없습니다.");
                }
            }
            else
            {
                Debug.LogError("데이터 가져오기 실패: " + task.Exception);
            }
        });
    }
    private void SaveUserData(Action action)
    {
        if (_userData == null)
        {
            Debug.LogWarning("UserData가 null입니다. 저장을 중단합니다.");
            return;
        }

        string jsonData = JsonUtility.ToJson(_userData);
        FirebaseDatabase.DefaultInstance.RootReference
            .Child("users")
            .Child(_auth.CurrentUser.UserId)
            .SetRawJsonValueAsync(jsonData)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        action();
                    });
                    Debug.Log("User data saved successfully.");
                }
                else
                {
                    Debug.LogError($"Failed to save user data: {task.Exception}");
                }
            });
    }
    #endregion [Firebase Methods]

    #region [User Data Methods]

    public void SaveAuto(Action action)
    {
        _userData._isAuto = !_userData._isAuto;
        SaveUserData(action);
    }

    public int FindUserCardDataIndex(int cardCode)
    {
        return _userData._userCardList.FindIndex(x => x._cardCode.Equals(cardCode));
    } // cardcode를 이용하여 List 인덱스 찾기

    public int WeightRandomDrawHero()
    {
        return WeightedRandomUtility.GetWeightedRandom<int>(_drawItem);
    }

    public int WeightRandomDrawHero_Special()
    {
        return WeightedRandomUtility.GetWeightedRandom<int>(_specialDrawItem);
    }

    public CardData RandomDrawUserCardData(bool isSpecial = false)
    {
        int randomCode = -1;
        if (!isSpecial)
            randomCode = WeightRandomDrawHero();
        else
            randomCode = WeightRandomDrawHero_Special();

        int fIndex = FindUserCardDataIndex(randomCode);

        if (fIndex < 0)
        {// 현재 카드데이터가 없는 경우
            _userData._userCardList.Add(new UserCardData(randomCode));
            _userData._userCardList.OrderBy(obj => obj._cardCode).ToList();
        }
        else
            _userData._userCardList[fIndex]._count += 1;

        return _cardDatas[randomCode];
    }//랜덤 카드 뽑기 후 카운트 증가 맟 카드 추가 생성 메서드
    public void MessageUserCardData(int cardCode)
    {
        int fIndex = FindUserCardDataIndex(cardCode);

        if (fIndex < 0)
        {// 현재 카드데이터가 없는 경우
            _userData._userCardList.Add(new UserCardData(cardCode));
            _userData._userCardList.OrderBy(obj => obj._cardCode).ToList();
        }
        else
            _userData._userCardList[fIndex]._count += 1;
    }//메시지 카드 받기 메서드
    public bool CountRandomDrawUserCardData(BoxData boxData)
    {
        if (_userData._jewel < boxData._pay)
            return false;

        AudioManager.Instance.LoadSFX("SFX/HeroPurchase.wav");
        _userData._jewel -= boxData._pay;

        List<CardData> _codeList = new List<CardData>();
        for (int i = 0; i < boxData._count; i++)
        {
            var cardData = RandomDrawUserCardData(boxData._isSpecial);
            _codeList.Add(cardData);
            IncreaseAchieveByCardType(cardData);
        }
        SaveUserData(() => UIManager.Instance.NotifyObservers(_userData));
        UIManager.Instance.GetDrawWindow().SetDrawWindow(_codeList);
        return true;
    }//원하는 숫자만큼 카드 뽑기

    public bool JewelExChangeGoldUserData(BoxData boxData)
    {
        if (_userData._jewel < boxData._pay)
            return false;
        AudioManager.Instance.LoadSFX("JewelPurchase.wav");
        _userData._jewel -= boxData._pay;
        _userData._gold += boxData._gold;

        SaveUserData(() => UIManager.Instance.NotifyObservers(_userData));
        return true;
    }//보석을 골드로 교환

    public void CashExchangeJewelUserData(BoxData boxData)
    {
        _userData._jewel += boxData._jewel;
        SaveUserData(() => UIManager.Instance.NotifyObservers(_userData));
    }//현금으로 보석 결제
    public bool ADExchangeJewelUserData(BoxData boxData)
    {
        return ShowRewardedAd();
    } //광고로 쥬얼 얻기

    public void EnforceCardStar(Card card, int pay)
    {
        _userData._gold -= pay;
        int fIndex = FindUserCardDataIndex(card.CardCode);
        UserCardData temp = _userData._userCardList[fIndex];
        temp._count -= card.CountMax;
        temp._star += 1;
        CardStarUnlockAchievement(temp._star);
        //Update 참조해서 문제 없을꺼라 생각
        SaveUserData(() => UIManager.Instance.NotifyObservers(_userData));
    }//카드 초월

    public void LvUpCard(Card card, int pay)
    {
        _userData._gold -= pay;
        int fIndex = FindUserCardDataIndex(card.CardCode);
        UserCardData temp = _userData._userCardList[fIndex];
        temp._level += 1;
        CardLevelUnlockAchievement(temp._level);
        //Update 참조해서 문제 없을꺼라 생각
        SaveUserData(() => UIManager.Instance.NotifyObservers(_userData));
    }//카드레벨업

    public void EquipCardToSlot(Card card, int index = -1)
    {
        int fIndex = FindUserCardDataIndex(card.CardCode);
        UserCardData temp = _userData._userCardList[fIndex];
        temp._equipIndex = index;
        //Update 참조해서 문제 없을꺼라 생각
        SaveUserData(() => UIManager.Instance.NotifyObservers(_userData));
    }

    public void SaveDataAndUINotifyObservers()
    {
        SaveUserData(() => UIManager.Instance.NotifyObservers(_userData));
    }

    public void GetDataFromMessageData(MessageData messageData)
    {
        switch (messageData._type)
        {
            case MessageType.Gold:
                _userData._gold += messageData._value;
                break;
            case MessageType.Jewel:
                _userData._jewel += messageData._value;
                break;
            case MessageType.Card:
                MessageUserCardData(messageData._value);
                break;
        }
    }

    public void NextStage()
    {
        UIManager.Instance.WindowOpenOrClose(WindowState.None);
        _userData._stageData._killNumber = 0;
        if (_userData._stageData._roundNumber < _roundMAX)
        {
            _userData._stageData._roundNumber++;
            MonsterManager.Instance.StartSpawnMonster();
            SaveUserData(() => {
                string chapter = "Chapter " + DataManager.Instance.StageData._stageNumber + "-" + DataManager.Instance.StageData._roundNumber;
                GameSceneManager.Instance.ShowChapterStart(chapter, "STAGE");
            });
        }
        else
        {
            _userData._stageData._roundNumber = 1;
            _userData._stageData._stageNumber++;
            SaveUserData(() =>
            {
                string sceneName = string.Empty;
                string removeName = string.Empty;
                if (_userData._stageData._stageNumber % 2 == 1)
                {
                    sceneName = "Stage1";
                    removeName = "Stage2";
                }
                else
                {
                    sceneName = "Stage2";
                    removeName = "Stage1";
                }
                StartCoroutine(GameSceneManager.Instance.StageAddtive(sceneName, removeName, true));
            });
        }

        _heroManager.HerosDuringChapter();
        UIManager.Instance.SetStagePrograssBar(_userData._stageData);
    }

    public void BackToStage(bool isJewel)
    {
        _isRaid = false;
        UIManager.Instance.WindowOpenOrClose(WindowState.None);
        AudioManager.Instance.LoadSFX("SFX/Button.wav");
        SaveUserData(() =>
        {
            string sceneName = string.Empty;
            string removeName = string.Empty;
            if (_userData._stageData._stageNumber % 2 == 1)
                sceneName = "Stage1";
            else
                sceneName = "Stage2";

            if (isJewel)
                removeName = "JewelRaid";
            else
                removeName = "GoldRaid";
            StartCoroutine(GameSceneManager.Instance.StageAddtive(sceneName, removeName, true));
        });

        _heroManager.HerosDuringChapter();
        UIManager.Instance.SetStagePrograssBar(_userData._stageData);
    }
    #endregion [User Data Methods]

    #region [Message & Coupon Methods]
    public IEnumerator UpdateSetMessage(Action action)
    {
        Task<DataSnapshot> task_T = _databaseRef.Child("messages").Child(_auth.CurrentUser.UserId).GetValueAsync();
        while (!task_T.IsCompleted) // Firebase 작업 완료 대기
        {
            yield return null;
        }

        if (task_T.IsCompleted && task_T.Result != null)
        {
            DataSnapshot snapshot = task_T.Result;
            if (snapshot.Exists)
            {
                int snapshotCount = snapshot.Children.Count();
                if (_messageList.Count == snapshotCount)
                {
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        Debug.Log("업데이트 메시지 추가");
                        action?.Invoke();
                    });
                    yield break;
                }

                foreach (var item in snapshot.Children)
                {
                    string json = item.GetRawJsonValue();
                    MessageData messageData = JsonUtility.FromJson<MessageData>(json);
                    // 중복 확인 후 추가
                    if (!_messageList.Contains(messageData))
                    {
                        _messageList.Add(messageData);
                    }
                }

                yield return null;
            }
        }
        else
            Debug.LogError("Firebase 데이터 가져오기 실패: " + task_T.Exception);

        UnityMainThreadDispatcher.Instance.Enqueue(() =>
        {
            Debug.Log("업데이트 메시지 추가");
            action?.Invoke();
        });
        //TWindow & Message
    }
    public IEnumerator DeleteMessage(MessageData messageData, Action action)
    {
        GetDataFromMessageData(messageData);
        string key = messageData._key;
        yield return null;

        _databaseRef.Child("messages").Child(_auth.CurrentUser.UserId).Child(key).RemoveValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"메시지({key})가 성공적으로 삭제되었습니다.");
                _messageList.Remove(messageData);
                SaveUserData(() => UIManager.Instance.NotifyObservers(_userData));
                UnityMainThreadDispatcher.Instance.Enqueue(() =>
                {
                    action();
                });
            }
            else
            {
                Debug.LogError($"메시지({key}) 삭제 중 오류 발생: {task.Exception}");
            }
        });
    }

    public IEnumerator AllDeleteMessage(Action action)
    {
        foreach (MessageData messageData in new List<MessageData>(_messageList))
        {
            GetDataFromMessageData(messageData);
            string key = messageData._key;
            _databaseRef.Child("messages").Child(_auth.CurrentUser.UserId).Child(key).RemoveValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"메시지({key})가 성공적으로 삭제되었습니다.");
                }
                else
                {
                    Debug.LogError($"메시지({key}) 삭제 중 오류 발생: {task.Exception}");
                }
            });
            yield return null;
        }

        _messageList.Clear();

        while (_messageList.Count > 0)
        {
            yield return null; // 다음 프레임까지 대기
        }

        SaveUserData(() => UIManager.Instance.NotifyObservers(_userData));
        Debug.Log("메시지 user 데이터 저장");
        UnityMainThreadDispatcher.Instance.Enqueue(() =>
        {
            action();
        });

        yield return null;
    }

    public IEnumerator GetMessageFormCoupon(string code, Action action)
    {// Firebase에서 데이터를 가져오기 위한 경로
        Task<DataSnapshot> task = _databaseRef.Child("coupon").Child(code).GetValueAsync();
        // Firebase 작업 완료 대기
        while (!task.IsCompleted)
        {
            yield return null;
        }

        if (task.IsCompleted && task.Result != null)
        {
            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                // 데이터를 JSON으로 변환하여 CouponData로 변환
                string json = snapshot.GetRawJsonValue();
                CouponData couponData = JsonUtility.FromJson<CouponData>(json);
                if (!couponData.userCheck(_auth.CurrentUser.UserId))
                {

                    Debug.Log("useriD : " + _auth.CurrentUser.UserId);
                    couponData._users.Add(_auth.CurrentUser.UserId);
                    string couponjson = JsonUtility.ToJson(couponData);
                    _databaseRef.Child("coupon").Child(code).SetRawJsonValueAsync(couponjson).ContinueWith(task =>
                    {
                        if (task.IsCompleted)
                        {
                            AudioManager.Instance.LoadSFX("SFX/JewelPurchase.wav");
                            StartCoroutine(SaveMessageData(couponData, action));
                            Debug.Log("쿠폰 데이터가 성공적으로 업데이트되었습니다.");
                        }
                        else
                        {
                            UnityMainThreadDispatcher.Instance.Enqueue(() =>
                            {
                                UIManager.Instance.ShowToast("쿠폰을 불러오지 못하였습니다.");
                                action?.Invoke();
                            });
                            Debug.LogError("쿠폰 데이터 업데이트 실패: " + task.Exception?.Message);
                        }
                    });
                }
                else
                {
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        UIManager.Instance.ShowToast("이미 사용한 쿠폰 코드입니다.");
                        action?.Invoke();
                    });
                    Debug.LogError("쿠폰 데이터 업데이트 실패: " + task.Exception?.Message);
                }
            }
            else
            {
                UnityMainThreadDispatcher.Instance.Enqueue(() =>
                {
                    UIManager.Instance.ShowToast("정확하지 않은 쿠폰 코드입니다.");
                    action?.Invoke();
                });
                Debug.LogError("Firebase에서 데이터를 찾을 수 없습니다: " + code);
            }
        }
        else
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                UIManager.Instance.ShowToast("불러올 수 없는 상태입니다.");
                action?.Invoke();
            });
            Debug.LogError("Firebase 작업 실패: " + task.Exception?.Message);
        }
    }

    IEnumerator SaveMessageData(CouponData couponData, Action action)
    {
        DatabaseReference messageRef = _databaseRef.Child("messages") // 'messages' 경로
            .Child(_auth.CurrentUser.UserId)     // 사용자 ID
            .Push();           // 랜덤 키 생성

        string generatedKey = messageRef.Key;
        MessageData messageData = new MessageData(generatedKey, couponData._title, couponData._body, couponData._type, couponData._value);
        Task messageTask = messageRef.SetRawJsonValueAsync(JsonUtility.ToJson(messageData));

        // 비동기 작업 완료 대기
        while (!messageTask.IsCompleted)
        {
            yield return null;
        }

        if (messageTask.IsFaulted)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                UIManager.Instance.ShowToast("쿠폰을 저장하지 못하였습니다. 문의 바랍니다.");
                action?.Invoke();
            });
            Debug.LogError($"메시지 저장 실패: {messageTask.Exception}");
        }
        else
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                UIManager.Instance.ShowToast("쿠폰을 성공적으로 사용하였습니다.");
                action?.Invoke();
            });
            Debug.Log($"메시지 저장 성공! 랜덤 키: {generatedKey}");
        }
    }
    #endregion

    #region [Hero Object Methods]
    public HeroManager GetHeroManager()
    {
        return _heroManager;
    }
    #endregion [Hero Object Methods]

    #region [Monster Data Methods]
    public MonsterData SetMonsterStat(MonsterType type)
    {
        MonsterData monster = new MonsterData();

        int stageNum = _userData._stageData._stageNumber;
        int roundNum = _userData._stageData._roundNumber;

        // 난이도 배율 (기존보다 완만하게 증가하도록 조정)
        float stageMultiplier = 1f + (stageNum - 1) * 0.15f;  // 기존 20% → 15%
        float roundMultiplier = 1f + (roundNum - 1) * 0.08f;  // 기존 10% → 8%

        // 타입별 배율 설정
        float hpMultiplier = 1f, attMultiplier = 1f, defMultiplier = 1f;
        float critChanceBonus = 0f, critDamageMultiplier = 1.5f;
        monster._gold = Mathf.RoundToInt(_monBaseData._gold * stageMultiplier * roundMultiplier);
        switch (type)
        {
            case MonsterType.MeleeStrong:
                hpMultiplier = 2.0f;
                attMultiplier = 1.4f; // 기존 1.5 → 1.4
                defMultiplier = 1.2f;
                break;
            case MonsterType.MeleeWeak:
                hpMultiplier = 1.3f; // 기존 1.2 → 1.3
                attMultiplier = 1.0f;
                defMultiplier = 0.9f; // 기존 0.8 → 0.9
                break;
            case MonsterType.RangedStrong:
                hpMultiplier = 1.5f;
                attMultiplier = 1.8f; // 기존 2.0 → 1.8
                defMultiplier = 1.0f;
                break;
            case MonsterType.RangedWeak:
                hpMultiplier = 1.1f; // 기존 1.0 → 1.1
                attMultiplier = 1.2f;
                defMultiplier = 0.85f; // 기존 0.8 → 0.85
                break;
            case MonsterType.StageBoss:
                hpMultiplier = 5.0f + (stageNum * 0.2f); // 보스는 체력이 5배 이상, 스테이지마다 추가 증가
                attMultiplier = 2.5f + (stageNum * 0.1f); // 보스는 공격력이 2.5배 이상 증가
                defMultiplier = 1.8f + (stageNum * 0.08f); // 보스는 방어력이 1.8배 이상 증가
                critChanceBonus = 0.025f; // 기본적으로 10% 크리티컬 확률 추가
                critDamageMultiplier = 2f; // 크리티컬 배율 2배
                monster._gold *= 5;
                break;
            case MonsterType.RaidBoss:
                hpMultiplier = 10.0f + (stageNum * 0.3f); // 레이드 보스는 체력이 10배 이상
                attMultiplier = 3.5f + (stageNum * 0.15f); // 레이드 보스는 공격력이 3.5배 이상
                defMultiplier = 2.2f + (stageNum * 0.1f); // 방어력도 높게 설정
                critChanceBonus = 0.05f; // 크리티컬 확률 기본 15% 추가
                critDamageMultiplier = 2f; // 크리티컬 배율 2.5배
                monster._gold *= 10;
                break;
        }

        // 최종 스탯 계산
        monster._hp = Mathf.RoundToInt(_monBaseData._hp * stageMultiplier * roundMultiplier * hpMultiplier);
        monster._att = Mathf.RoundToInt(_monBaseData._att * stageMultiplier * roundMultiplier * attMultiplier);
        monster._def = Mathf.RoundToInt(_monBaseData._def * stageMultiplier * roundMultiplier * defMultiplier);
        monster._attSpeed = _monBaseData._attSpeed;
        // 크리티컬 관련 (스테이지 진행 시 확률 증가)
        monster._critChance = 0.05f + (stageNum * 0.005f) + critChanceBonus; // 기본 5% + 스테이지마다 0.5% + 추가 보너스
        monster._critDamage = critDamageMultiplier;
        return monster;
    }

    public string CardDataGetName(int value)
    {
        return _cardDatas[value]._name;
    }
    #endregion [Monster Data Methods]

    #region [GPGS AChieve Increase Methods]
    public void CheckAndIncreaseAchievement(string achievementID, int steps, System.Action onReward = null)
    {
        PlayGamesPlatform.Instance.LoadAchievements(achievements =>
        {
            if (achievements == null || achievements.Length == 0)
            {
                Debug.LogError("업적 데이터를 불러오는 데 실패했습니다.");
                return;
            }

            foreach (var achievement in achievements)
            {
                if (achievement.id == achievementID)
                {
                    Debug.Log($"업적 {achievementID} 진행도: {achievement.percentCompleted}%");

                    // 1. 업적이 이미 달성되었는지 체크 (completed == true or percentCompleted == 100)
                    if (achievement.completed)
                    {
                        Debug.Log($"업적 {achievementID}는 이미 달성됨. 중복 지급 방지.");
                        return; // 이미 달성된 업적이므로 종료
                    }

                    IncreaseGPGSAchievement(achievementID, steps, onReward);
                    return;
                }
            }
        });
    }
    public void IncreaseGPGSAchievement(string achievementID, int steps, System.Action onReward = null)
    {
        PlayGamesPlatform.Instance.IncrementAchievement(achievementID, steps, success =>
        {
            if (success)
            {
                Debug.Log($"업적 {achievementID} {steps}단계 증가!");

                //업적 진행도를 다시 확인하여 최종 목표 도달 여부 판단
                CheckAchievementProgress(achievementID, onReward);
            }
            else
            {
                Debug.LogError($"업적 {achievementID} 단계 증가 실패!");
            }
        });
    } // 단계 업적
    public void CheckAchievementProgress(string achievementID, System.Action onReward = null)
    {
        PlayGamesPlatform.Instance.LoadAchievements(achievements =>
        {
            if (achievements.Length > 0)
            {
                foreach (var achievement in achievements)
                {
                    if (achievement.id == achievementID)
                    {
                        Debug.Log($"업적 {achievementID} 진행도: {achievement.percentCompleted}%");

                        // 업적이 100% 완료되었거나 completed가 true라면 보상 지급
                        if (achievement.completed)
                        {
                            Debug.Log($"업적 {achievementID} 최종 달성! 보상 지급!");
                            onReward?.Invoke(); // 보상 지급 함수 실행
                        }
                        return;
                    }
                }
            }
            else
            {
                Debug.LogError("업적 데이터를 불러오는 데 실패했습니다.");
            }
        });
    }

    public void UnlockAchievement(string achievementID, System.Action onReward = null)
    {
        PlayGamesPlatform.Instance.LoadAchievements(achievements =>
        {
            if (achievements == null || achievements.Length == 0)
            {
                Debug.LogError("업적 데이터를 불러오는 데 실패했습니다.");
                return;
            }

            foreach (var achievement in achievements)
            {
                if (achievement.id == achievementID)
                {
                    // 이미 완료된 업적인 경우, Unlock 실행 X (중복 방지)
                    if (achievement.completed)
                    {
                        Debug.Log($"업적 {achievementID}는 이미 완료됨. 중복 지급 방지!");
                        return;
                    }

                    // 아직 완료되지 않은 업적이면 Unlock 진행
                    PlayGamesPlatform.Instance.ReportProgress(achievementID, 100.0f, success =>
                    {
                        if (success)
                        {
                            Debug.Log($"업적 {achievementID} 해제 완료!");
                            onReward?.Invoke(); // 보상 지급
                        }
                        else
                        {
                            Debug.LogError($"업적 {achievementID} 해제 실패!");
                        }
                    });
                    return;
                }
            }
        });
    }
    public void CheckAchievement(string achievementID)
    {
        PlayGamesPlatform.Instance.LoadAchievements((achievements) =>
        {
            if (achievements.Length > 0)
            {
                foreach (var achievement in achievements)
                {
                    if (achievement.id == achievementID)
                    {
                        Debug.Log($"Achievement {achievementID} progress: {achievement.percentCompleted}");
                    }
                }
            }
            else
            {
                Debug.LogError("No achievements found.");
            }
        });
    }

    public void IncreaseAchieveByCardType(CardData cardData)
    {
        switch (cardData._cardType)
        {
            case CardType.NORMAL:
                CheckAndIncreaseAchievement(GPGSIds.achievement, 1, () => { _userData._jewel += 1000; });
                break;
            case CardType.RARE:
                CheckAndIncreaseAchievement(GPGSIds.achievement_2, 1, () => { _userData._jewel += 1500; });
                break;
            case CardType.EPIC:
                CheckAndIncreaseAchievement(GPGSIds.achievement_3, 1, () => { _userData._jewel += 2000; });
                break;
            case CardType.LEGENDARY:
                CheckAndIncreaseAchievement(GPGSIds.achievement_4, 1, () => { _userData._jewel += 3000; });
                break;
        }
    }//카드 수집 업적

    public void IncreaseAchieveByMonsterKill()
    {
        CheckAndIncreaseAchievement(GPGSIds.achievement__100, 1, () => { _userData._jewel += 1000; });
    }//몬스터 킬 업적

    public void CardStarUnlockAchievement(int star)
    {
        switch (star)
        {
            case 1:
                UnlockAchievement(GPGSIds.achievement__1, () => { _userData._jewel += 1000; });
                break;
            case 2:
                UnlockAchievement(GPGSIds.achievement__2, () => { _userData._jewel += 2000; });
                break;
            case 5:
                UnlockAchievement(GPGSIds.achievement__5, () => { _userData._jewel += 3000; });
                break;
        }
    }

    public void CardLevelUnlockAchievement(int level)
    {
        if(level == 30)
            UnlockAchievement(GPGSIds.achievement____, () => { _userData._jewel += 1000; });
    }

    #endregion [GPGS AChieve Increase Methods]

    #region [Google Ads Methods]
    public void GoogleAdsInitialize()
    {
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("Google Mobile Ads Initialized");
            RequestRewardedAd();
        });
    }

    public void RequestRewardedAd()
    {
        AdRequest request = new AdRequest();
        // 광고 로드 이벤트 핸들러
        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogError($"보상형 광고 로드 실패: {error.GetMessage()}");
                return;
            }

            _rewardedAd = ad;
            Debug.Log("보상형 광고가 성공적으로 로드되었습니다.");

            _rewardedAd.OnAdFullScreenContentClosed += HandleRewardedAdClosed;
            _rewardedAd.OnAdFullScreenContentFailed += HandleRewardedAdFailedToShow;
        });
    }
    public bool ShowRewardedAd()
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show(reward =>
            {
                AudioManager.Instance.LoadSFX("SFX/JewelPurchase.wav");
                Debug.Log($"보상 획득: {reward.Amount} {reward.Type}");
                _userData._jewel += (int)reward.Amount;
                // 보상 처리 로직 추가
                SaveUserData(() => UIManager.Instance.NotifyObservers(_userData));
            });
            return true;
        }
        else
        {
            Debug.Log("보상형 광고가 준비되지 않았습니다.");
            return false;
        }
    }

    private void HandleRewardedAdClosed()
    {
        Debug.Log("보상형 광고가 닫혔습니다. 새 광고 요청.");
        RequestRewardedAd();
    }

    private void HandleRewardedAdFailedToShow(AdError error)
    {
        Debug.LogError($"보상형 광고 표시 실패: {error.GetMessage()}");
    }
    #endregion [Google Ads Methods]

    #region [In-App purchase Methods]
    public async Task VerifyPurchase(string packageName, string productId, string purchaseToken, BoxData boxdata)
    {
        FirebaseFunctions functions = FirebaseFunctions.DefaultInstance;

        // Google Play 결제 정보를 Firebase Cloud Functions에 전달
        // 값이 정상적으로 전달되는지 확인
        Debug.Log($" packageName: {packageName}");
        Debug.Log($" productId: {productId}");
        Debug.Log($" purchaseToken: {purchaseToken}");

        if (string.IsNullOrEmpty(packageName) || string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(purchaseToken))
        {
            Debug.LogError("하나 이상의 값이 null 또는 비어 있음! Firebase로 요청을 보낼 수 없음.");
            return;
        }


        var requestData = new Dictionary<string, object>
    {
        { "packageName", packageName },
        { "productId", productId },
        { "purchaseToken", purchaseToken }
    };

        Debug.Log(" 보내는 데이터: " + requestData); // 여기서 JSON 변환 없이 출력됨
        GameSceneManager.Instance.ShowRefreshLoading("결제 확인중입니다.. \n잠시만 기다려주세요..");
        HttpsCallableReference function = functions.GetHttpsCallable("verifyPurchase");

        try
        {
            var result = await function.CallAsync(requestData);
            Debug.Log(" Purchase verification result: " + result.Data);

            //  Dictionary<object, object>  Dictionary<string, object> 변환
            if (result.Data is Dictionary<object, object> rawResponseData)
            {
                var responseData = rawResponseData.ToDictionary(
                    kvp => kvp.Key.ToString(),
                    kvp => kvp.Value
                );

                // 검증 성공 여부 확인
                if (responseData.ContainsKey("success") && responseData["success"] is bool isSuccess && isSuccess)
                {
                    PlayerPrefs.SetString("purchased_" + productId, purchaseToken);
                    PlayerPrefs.Save();

                    Debug.Log(" 구매 검증 성공! 데이터를 저장합니다.");
                    SavePurchaseData(productId, purchaseToken, boxdata);
                }
                else
                {
                    Debug.LogWarning(" 구매 검증 실패! " + (responseData.ContainsKey("message") ? responseData["message"] : "알 수 없는 오류"));
                    GameSceneManager.Instance.HideRefreshLoading();
                }
            }
            else
            {
                Debug.LogError(" Firebase 응답을 Dictionary<string, object>로 변환할 수 없음.");
                GameSceneManager.Instance.HideRefreshLoading();
            }
        }
        catch (System.Exception e)
        {
            GameSceneManager.Instance.HideRefreshLoading();
            Debug.LogError(" Error calling Firebase Function: " + e);
        }
    }

    // 검증 성공 시 구매 데이터를 저장하는 함수
    private void SavePurchaseData(string productId, string purchaseToken, BoxData boxdata)
    {
        AudioManager.Instance.LoadSFX("SFX/JewelPurchase.wav");
        CashExchangeJewelUserData(boxdata);
        Debug.Log($" {productId} 구매하여 아이템 지급 완료!");

        //  PlayerPrefs에서 구매 내역 삭제 (중복 지급 방지)
        string key = "purchased_" + productId;
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }

        GameSceneManager.Instance.HideRefreshLoading();
    }
    void CheckPendingPurchases()
    {
        Debug.Log("미지급된 구매 확인 중...");

        List<string> keysToRemove = new List<string>();

        foreach (var key in PlayerPrefs.GetString("purchased_keys", "").Split(','))
        {
            if (!string.IsNullOrEmpty(key) && PlayerPrefs.HasKey(key))
            {
                string purchaseToken = PlayerPrefs.GetString(key);
                string productId = key.Replace("purchased_", "");

                Debug.Log($" 미지급된 구매 발견: {productId}");
                ProcessPurchase(productId);

                keysToRemove.Add(key);
            }
        }

        //지급 완료된 구매 내역 삭제
        foreach (var key in keysToRemove)
        {
            PlayerPrefs.DeleteKey(key);
        }

        // 저장된 키 목록 업데이트
        PlayerPrefs.SetString("purchased_keys", string.Join(",", PlayerPrefs.GetString("purchased_keys", "").Split(',').Where(k => !keysToRemove.Contains(k))));
        PlayerPrefs.Save();
    }

    void ProcessPurchase(string productId)
    {
        Debug.Log($" {productId} 아이템 지급 완료!");

        switch (productId)
        {
            case "jewel_5000":
                _userData._jewel += 5000;
                break;
            case "jewel_10000":
                _userData._jewel += 10000;
                break;
            default:
                Debug.LogWarning($" 알 수 없는 상품 ID: {productId}");
                return;
        }

        //저장된 사용자 데이터 업데이트
        SaveUserData(() => UIManager.Instance.NotifyObservers(_userData));
    }
    #endregion [In-App purchase Methods]

    #region [Addressable Methods]
    public void LoadAddressable<T>(string key, Action<T> onLoaded) where T : UnityEngine.Object
    {
        if(key.Equals("Lengendary/Erylia.png"))
        {
            Debug.Log("에릴리아 들어옴");
        }

        Addressables.LoadAssetAsync<T>(key).Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"{key} 로드 성공: {op.Result}");
                onLoaded?.Invoke(op.Result);  // 불러온 오브젝트를 콜백(Action)으로 전달
            }
            else
            {
                Debug.LogError($"{key} 로드 실패!");

                if (key.Equals("Lengendary/Erylia.png"))
                {
                    Debug.Log("에릴리아 실패함.");
                }
            }
        };
    }
    #endregion [Addressable Methods]

}