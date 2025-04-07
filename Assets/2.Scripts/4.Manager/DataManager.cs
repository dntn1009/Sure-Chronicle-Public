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
        Debug.Log("Firebase ������ �ʱ�ȭ ����");

        // Firebase �ʱ�ȭ
        _auth = FirebaseAuth.DefaultInstance;
        _databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        // ����� ������ ��������
        Task<DataSnapshot> task = _databaseRef.Child("users").Child(_auth.CurrentUser.UserId).GetValueAsync();
        while (!task.IsCompleted) // Firebase �۾� �Ϸ� ���
        {
            yield return null;
        }

        if (task.IsCompleted && task.Result != null)
        {
            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                _userData = JsonUtility.FromJson<UserData>(snapshot.GetRawJsonValue());
                Debug.Log("Firebase ������ �ε� ����");

                // StageAdditive ȣ��
                yield return GameSceneManager.Instance.StageAddtive(_userData._stageData);

                // UI ���� �۾� ����
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
                Debug.LogWarning("����� �����Ͱ� �������� �ʽ��ϴ�.");
        }
        else
            Debug.LogError("Firebase ������ �������� ����: " + task.Exception);
        //Window & StageBar & more UI..

        Task<DataSnapshot> task_T = _databaseRef.Child("messages").Child(_auth.CurrentUser.UserId).GetValueAsync();
        while (!task_T.IsCompleted) // Firebase �۾� �Ϸ� ���
        {
            yield return null;
        }
        Debug.Log("message tag Ȯ��");
        if (task_T.IsCompleted && task_T.Result != null)
        {
            DataSnapshot snapshot = task_T.Result;
            if (snapshot.Exists)
            {
                foreach (var item in snapshot.Children)
                {
                    Debug.Log("message tag �߰�");
                    string json = item.GetRawJsonValue();
                    MessageData messageData = JsonUtility.FromJson<MessageData>(json);

                    _messageList.Add(messageData);
                }
                yield return null;
            }
        }
        else
            Debug.LogError("Firebase ������ �������� ����: " + task_T.Exception);

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
        Debug.Log("Additive ���� �𸮸���������");
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
                Debug.Log("Firebase �ʱ�ȭ ����!");
                TestLoadUserData("tyBNAUWIqQhgjA94LUqRp72w35q2"); // ������ �������� ����
            }
            else
            {
                Debug.LogError("Firebase �ʱ�ȭ ����: " + task.Result);
            }
        });
    }
    public void TestLoadUserData(string userId)
    {
        Debug.Log("FirebaseGetUserData ����");

        _databaseRef.Child("users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result.Exists)
                {
                    string jsonData = task.Result.GetRawJsonValue();
                    Debug.Log("������ ������: " + jsonData);

                    // JSON �����͸� UserData ��ü�� ��ȯ
                    _userData = JsonUtility.FromJson<UserData>(jsonData);

                }
                else
                {
                    Debug.LogWarning("�����͸� ã�� �� �����ϴ�.");
                }
            }
            else
            {
                Debug.LogError("������ �������� ����: " + task.Exception);
            }
        });
    }
    private void SaveUserData(Action action)
    {
        if (_userData == null)
        {
            Debug.LogWarning("UserData�� null�Դϴ�. ������ �ߴ��մϴ�.");
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
    } // cardcode�� �̿��Ͽ� List �ε��� ã��

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
        {// ���� ī�嵥���Ͱ� ���� ���
            _userData._userCardList.Add(new UserCardData(randomCode));
            _userData._userCardList.OrderBy(obj => obj._cardCode).ToList();
        }
        else
            _userData._userCardList[fIndex]._count += 1;

        return _cardDatas[randomCode];
    }//���� ī�� �̱� �� ī��Ʈ ���� �� ī�� �߰� ���� �޼���
    public void MessageUserCardData(int cardCode)
    {
        int fIndex = FindUserCardDataIndex(cardCode);

        if (fIndex < 0)
        {// ���� ī�嵥���Ͱ� ���� ���
            _userData._userCardList.Add(new UserCardData(cardCode));
            _userData._userCardList.OrderBy(obj => obj._cardCode).ToList();
        }
        else
            _userData._userCardList[fIndex]._count += 1;
    }//�޽��� ī�� �ޱ� �޼���
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
    }//���ϴ� ���ڸ�ŭ ī�� �̱�

    public bool JewelExChangeGoldUserData(BoxData boxData)
    {
        if (_userData._jewel < boxData._pay)
            return false;
        AudioManager.Instance.LoadSFX("JewelPurchase.wav");
        _userData._jewel -= boxData._pay;
        _userData._gold += boxData._gold;

        SaveUserData(() => UIManager.Instance.NotifyObservers(_userData));
        return true;
    }//������ ���� ��ȯ

    public void CashExchangeJewelUserData(BoxData boxData)
    {
        _userData._jewel += boxData._jewel;
        SaveUserData(() => UIManager.Instance.NotifyObservers(_userData));
    }//�������� ���� ����
    public bool ADExchangeJewelUserData(BoxData boxData)
    {
        return ShowRewardedAd();
    } //����� ��� ���

    public void EnforceCardStar(Card card, int pay)
    {
        _userData._gold -= pay;
        int fIndex = FindUserCardDataIndex(card.CardCode);
        UserCardData temp = _userData._userCardList[fIndex];
        temp._count -= card.CountMax;
        temp._star += 1;
        CardStarUnlockAchievement(temp._star);
        //Update �����ؼ� ���� �������� ����
        SaveUserData(() => UIManager.Instance.NotifyObservers(_userData));
    }//ī�� �ʿ�

    public void LvUpCard(Card card, int pay)
    {
        _userData._gold -= pay;
        int fIndex = FindUserCardDataIndex(card.CardCode);
        UserCardData temp = _userData._userCardList[fIndex];
        temp._level += 1;
        CardLevelUnlockAchievement(temp._level);
        //Update �����ؼ� ���� �������� ����
        SaveUserData(() => UIManager.Instance.NotifyObservers(_userData));
    }//ī�巹����

    public void EquipCardToSlot(Card card, int index = -1)
    {
        int fIndex = FindUserCardDataIndex(card.CardCode);
        UserCardData temp = _userData._userCardList[fIndex];
        temp._equipIndex = index;
        //Update �����ؼ� ���� �������� ����
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
        while (!task_T.IsCompleted) // Firebase �۾� �Ϸ� ���
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
                        Debug.Log("������Ʈ �޽��� �߰�");
                        action?.Invoke();
                    });
                    yield break;
                }

                foreach (var item in snapshot.Children)
                {
                    string json = item.GetRawJsonValue();
                    MessageData messageData = JsonUtility.FromJson<MessageData>(json);
                    // �ߺ� Ȯ�� �� �߰�
                    if (!_messageList.Contains(messageData))
                    {
                        _messageList.Add(messageData);
                    }
                }

                yield return null;
            }
        }
        else
            Debug.LogError("Firebase ������ �������� ����: " + task_T.Exception);

        UnityMainThreadDispatcher.Instance.Enqueue(() =>
        {
            Debug.Log("������Ʈ �޽��� �߰�");
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
                Debug.Log($"�޽���({key})�� ���������� �����Ǿ����ϴ�.");
                _messageList.Remove(messageData);
                SaveUserData(() => UIManager.Instance.NotifyObservers(_userData));
                UnityMainThreadDispatcher.Instance.Enqueue(() =>
                {
                    action();
                });
            }
            else
            {
                Debug.LogError($"�޽���({key}) ���� �� ���� �߻�: {task.Exception}");
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
                    Debug.Log($"�޽���({key})�� ���������� �����Ǿ����ϴ�.");
                }
                else
                {
                    Debug.LogError($"�޽���({key}) ���� �� ���� �߻�: {task.Exception}");
                }
            });
            yield return null;
        }

        _messageList.Clear();

        while (_messageList.Count > 0)
        {
            yield return null; // ���� �����ӱ��� ���
        }

        SaveUserData(() => UIManager.Instance.NotifyObservers(_userData));
        Debug.Log("�޽��� user ������ ����");
        UnityMainThreadDispatcher.Instance.Enqueue(() =>
        {
            action();
        });

        yield return null;
    }

    public IEnumerator GetMessageFormCoupon(string code, Action action)
    {// Firebase���� �����͸� �������� ���� ���
        Task<DataSnapshot> task = _databaseRef.Child("coupon").Child(code).GetValueAsync();
        // Firebase �۾� �Ϸ� ���
        while (!task.IsCompleted)
        {
            yield return null;
        }

        if (task.IsCompleted && task.Result != null)
        {
            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                // �����͸� JSON���� ��ȯ�Ͽ� CouponData�� ��ȯ
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
                            Debug.Log("���� �����Ͱ� ���������� ������Ʈ�Ǿ����ϴ�.");
                        }
                        else
                        {
                            UnityMainThreadDispatcher.Instance.Enqueue(() =>
                            {
                                UIManager.Instance.ShowToast("������ �ҷ����� ���Ͽ����ϴ�.");
                                action?.Invoke();
                            });
                            Debug.LogError("���� ������ ������Ʈ ����: " + task.Exception?.Message);
                        }
                    });
                }
                else
                {
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        UIManager.Instance.ShowToast("�̹� ����� ���� �ڵ��Դϴ�.");
                        action?.Invoke();
                    });
                    Debug.LogError("���� ������ ������Ʈ ����: " + task.Exception?.Message);
                }
            }
            else
            {
                UnityMainThreadDispatcher.Instance.Enqueue(() =>
                {
                    UIManager.Instance.ShowToast("��Ȯ���� ���� ���� �ڵ��Դϴ�.");
                    action?.Invoke();
                });
                Debug.LogError("Firebase���� �����͸� ã�� �� �����ϴ�: " + code);
            }
        }
        else
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                UIManager.Instance.ShowToast("�ҷ��� �� ���� �����Դϴ�.");
                action?.Invoke();
            });
            Debug.LogError("Firebase �۾� ����: " + task.Exception?.Message);
        }
    }

    IEnumerator SaveMessageData(CouponData couponData, Action action)
    {
        DatabaseReference messageRef = _databaseRef.Child("messages") // 'messages' ���
            .Child(_auth.CurrentUser.UserId)     // ����� ID
            .Push();           // ���� Ű ����

        string generatedKey = messageRef.Key;
        MessageData messageData = new MessageData(generatedKey, couponData._title, couponData._body, couponData._type, couponData._value);
        Task messageTask = messageRef.SetRawJsonValueAsync(JsonUtility.ToJson(messageData));

        // �񵿱� �۾� �Ϸ� ���
        while (!messageTask.IsCompleted)
        {
            yield return null;
        }

        if (messageTask.IsFaulted)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                UIManager.Instance.ShowToast("������ �������� ���Ͽ����ϴ�. ���� �ٶ��ϴ�.");
                action?.Invoke();
            });
            Debug.LogError($"�޽��� ���� ����: {messageTask.Exception}");
        }
        else
        {
            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                UIManager.Instance.ShowToast("������ ���������� ����Ͽ����ϴ�.");
                action?.Invoke();
            });
            Debug.Log($"�޽��� ���� ����! ���� Ű: {generatedKey}");
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

        // ���̵� ���� (�������� �ϸ��ϰ� �����ϵ��� ����)
        float stageMultiplier = 1f + (stageNum - 1) * 0.15f;  // ���� 20% �� 15%
        float roundMultiplier = 1f + (roundNum - 1) * 0.08f;  // ���� 10% �� 8%

        // Ÿ�Ժ� ���� ����
        float hpMultiplier = 1f, attMultiplier = 1f, defMultiplier = 1f;
        float critChanceBonus = 0f, critDamageMultiplier = 1.5f;
        monster._gold = Mathf.RoundToInt(_monBaseData._gold * stageMultiplier * roundMultiplier);
        switch (type)
        {
            case MonsterType.MeleeStrong:
                hpMultiplier = 2.0f;
                attMultiplier = 1.4f; // ���� 1.5 �� 1.4
                defMultiplier = 1.2f;
                break;
            case MonsterType.MeleeWeak:
                hpMultiplier = 1.3f; // ���� 1.2 �� 1.3
                attMultiplier = 1.0f;
                defMultiplier = 0.9f; // ���� 0.8 �� 0.9
                break;
            case MonsterType.RangedStrong:
                hpMultiplier = 1.5f;
                attMultiplier = 1.8f; // ���� 2.0 �� 1.8
                defMultiplier = 1.0f;
                break;
            case MonsterType.RangedWeak:
                hpMultiplier = 1.1f; // ���� 1.0 �� 1.1
                attMultiplier = 1.2f;
                defMultiplier = 0.85f; // ���� 0.8 �� 0.85
                break;
            case MonsterType.StageBoss:
                hpMultiplier = 5.0f + (stageNum * 0.2f); // ������ ü���� 5�� �̻�, ������������ �߰� ����
                attMultiplier = 2.5f + (stageNum * 0.1f); // ������ ���ݷ��� 2.5�� �̻� ����
                defMultiplier = 1.8f + (stageNum * 0.08f); // ������ ������ 1.8�� �̻� ����
                critChanceBonus = 0.025f; // �⺻������ 10% ũ��Ƽ�� Ȯ�� �߰�
                critDamageMultiplier = 2f; // ũ��Ƽ�� ���� 2��
                monster._gold *= 5;
                break;
            case MonsterType.RaidBoss:
                hpMultiplier = 10.0f + (stageNum * 0.3f); // ���̵� ������ ü���� 10�� �̻�
                attMultiplier = 3.5f + (stageNum * 0.15f); // ���̵� ������ ���ݷ��� 3.5�� �̻�
                defMultiplier = 2.2f + (stageNum * 0.1f); // ���µ� ���� ����
                critChanceBonus = 0.05f; // ũ��Ƽ�� Ȯ�� �⺻ 15% �߰�
                critDamageMultiplier = 2f; // ũ��Ƽ�� ���� 2.5��
                monster._gold *= 10;
                break;
        }

        // ���� ���� ���
        monster._hp = Mathf.RoundToInt(_monBaseData._hp * stageMultiplier * roundMultiplier * hpMultiplier);
        monster._att = Mathf.RoundToInt(_monBaseData._att * stageMultiplier * roundMultiplier * attMultiplier);
        monster._def = Mathf.RoundToInt(_monBaseData._def * stageMultiplier * roundMultiplier * defMultiplier);
        monster._attSpeed = _monBaseData._attSpeed;
        // ũ��Ƽ�� ���� (�������� ���� �� Ȯ�� ����)
        monster._critChance = 0.05f + (stageNum * 0.005f) + critChanceBonus; // �⺻ 5% + ������������ 0.5% + �߰� ���ʽ�
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
                Debug.LogError("���� �����͸� �ҷ����� �� �����߽��ϴ�.");
                return;
            }

            foreach (var achievement in achievements)
            {
                if (achievement.id == achievementID)
                {
                    Debug.Log($"���� {achievementID} ���൵: {achievement.percentCompleted}%");

                    // 1. ������ �̹� �޼��Ǿ����� üũ (completed == true or percentCompleted == 100)
                    if (achievement.completed)
                    {
                        Debug.Log($"���� {achievementID}�� �̹� �޼���. �ߺ� ���� ����.");
                        return; // �̹� �޼��� �����̹Ƿ� ����
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
                Debug.Log($"���� {achievementID} {steps}�ܰ� ����!");

                //���� ���൵�� �ٽ� Ȯ���Ͽ� ���� ��ǥ ���� ���� �Ǵ�
                CheckAchievementProgress(achievementID, onReward);
            }
            else
            {
                Debug.LogError($"���� {achievementID} �ܰ� ���� ����!");
            }
        });
    } // �ܰ� ����
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
                        Debug.Log($"���� {achievementID} ���൵: {achievement.percentCompleted}%");

                        // ������ 100% �Ϸ�Ǿ��ų� completed�� true��� ���� ����
                        if (achievement.completed)
                        {
                            Debug.Log($"���� {achievementID} ���� �޼�! ���� ����!");
                            onReward?.Invoke(); // ���� ���� �Լ� ����
                        }
                        return;
                    }
                }
            }
            else
            {
                Debug.LogError("���� �����͸� �ҷ����� �� �����߽��ϴ�.");
            }
        });
    }

    public void UnlockAchievement(string achievementID, System.Action onReward = null)
    {
        PlayGamesPlatform.Instance.LoadAchievements(achievements =>
        {
            if (achievements == null || achievements.Length == 0)
            {
                Debug.LogError("���� �����͸� �ҷ����� �� �����߽��ϴ�.");
                return;
            }

            foreach (var achievement in achievements)
            {
                if (achievement.id == achievementID)
                {
                    // �̹� �Ϸ�� ������ ���, Unlock ���� X (�ߺ� ����)
                    if (achievement.completed)
                    {
                        Debug.Log($"���� {achievementID}�� �̹� �Ϸ��. �ߺ� ���� ����!");
                        return;
                    }

                    // ���� �Ϸ���� ���� �����̸� Unlock ����
                    PlayGamesPlatform.Instance.ReportProgress(achievementID, 100.0f, success =>
                    {
                        if (success)
                        {
                            Debug.Log($"���� {achievementID} ���� �Ϸ�!");
                            onReward?.Invoke(); // ���� ����
                        }
                        else
                        {
                            Debug.LogError($"���� {achievementID} ���� ����!");
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
    }//ī�� ���� ����

    public void IncreaseAchieveByMonsterKill()
    {
        CheckAndIncreaseAchievement(GPGSIds.achievement__100, 1, () => { _userData._jewel += 1000; });
    }//���� ų ����

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
        // ���� �ε� �̺�Ʈ �ڵ鷯
        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogError($"������ ���� �ε� ����: {error.GetMessage()}");
                return;
            }

            _rewardedAd = ad;
            Debug.Log("������ ���� ���������� �ε�Ǿ����ϴ�.");

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
                Debug.Log($"���� ȹ��: {reward.Amount} {reward.Type}");
                _userData._jewel += (int)reward.Amount;
                // ���� ó�� ���� �߰�
                SaveUserData(() => UIManager.Instance.NotifyObservers(_userData));
            });
            return true;
        }
        else
        {
            Debug.Log("������ ���� �غ���� �ʾҽ��ϴ�.");
            return false;
        }
    }

    private void HandleRewardedAdClosed()
    {
        Debug.Log("������ ���� �������ϴ�. �� ���� ��û.");
        RequestRewardedAd();
    }

    private void HandleRewardedAdFailedToShow(AdError error)
    {
        Debug.LogError($"������ ���� ǥ�� ����: {error.GetMessage()}");
    }
    #endregion [Google Ads Methods]

    #region [In-App purchase Methods]
    public async Task VerifyPurchase(string packageName, string productId, string purchaseToken, BoxData boxdata)
    {
        FirebaseFunctions functions = FirebaseFunctions.DefaultInstance;

        // Google Play ���� ������ Firebase Cloud Functions�� ����
        // ���� ���������� ���޵Ǵ��� Ȯ��
        Debug.Log($" packageName: {packageName}");
        Debug.Log($" productId: {productId}");
        Debug.Log($" purchaseToken: {purchaseToken}");

        if (string.IsNullOrEmpty(packageName) || string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(purchaseToken))
        {
            Debug.LogError("�ϳ� �̻��� ���� null �Ǵ� ��� ����! Firebase�� ��û�� ���� �� ����.");
            return;
        }


        var requestData = new Dictionary<string, object>
    {
        { "packageName", packageName },
        { "productId", productId },
        { "purchaseToken", purchaseToken }
    };

        Debug.Log(" ������ ������: " + requestData); // ���⼭ JSON ��ȯ ���� ��µ�
        GameSceneManager.Instance.ShowRefreshLoading("���� Ȯ�����Դϴ�.. \n��ø� ��ٷ��ּ���..");
        HttpsCallableReference function = functions.GetHttpsCallable("verifyPurchase");

        try
        {
            var result = await function.CallAsync(requestData);
            Debug.Log(" Purchase verification result: " + result.Data);

            //  Dictionary<object, object>  Dictionary<string, object> ��ȯ
            if (result.Data is Dictionary<object, object> rawResponseData)
            {
                var responseData = rawResponseData.ToDictionary(
                    kvp => kvp.Key.ToString(),
                    kvp => kvp.Value
                );

                // ���� ���� ���� Ȯ��
                if (responseData.ContainsKey("success") && responseData["success"] is bool isSuccess && isSuccess)
                {
                    PlayerPrefs.SetString("purchased_" + productId, purchaseToken);
                    PlayerPrefs.Save();

                    Debug.Log(" ���� ���� ����! �����͸� �����մϴ�.");
                    SavePurchaseData(productId, purchaseToken, boxdata);
                }
                else
                {
                    Debug.LogWarning(" ���� ���� ����! " + (responseData.ContainsKey("message") ? responseData["message"] : "�� �� ���� ����"));
                    GameSceneManager.Instance.HideRefreshLoading();
                }
            }
            else
            {
                Debug.LogError(" Firebase ������ Dictionary<string, object>�� ��ȯ�� �� ����.");
                GameSceneManager.Instance.HideRefreshLoading();
            }
        }
        catch (System.Exception e)
        {
            GameSceneManager.Instance.HideRefreshLoading();
            Debug.LogError(" Error calling Firebase Function: " + e);
        }
    }

    // ���� ���� �� ���� �����͸� �����ϴ� �Լ�
    private void SavePurchaseData(string productId, string purchaseToken, BoxData boxdata)
    {
        AudioManager.Instance.LoadSFX("SFX/JewelPurchase.wav");
        CashExchangeJewelUserData(boxdata);
        Debug.Log($" {productId} �����Ͽ� ������ ���� �Ϸ�!");

        //  PlayerPrefs���� ���� ���� ���� (�ߺ� ���� ����)
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
        Debug.Log("�����޵� ���� Ȯ�� ��...");

        List<string> keysToRemove = new List<string>();

        foreach (var key in PlayerPrefs.GetString("purchased_keys", "").Split(','))
        {
            if (!string.IsNullOrEmpty(key) && PlayerPrefs.HasKey(key))
            {
                string purchaseToken = PlayerPrefs.GetString(key);
                string productId = key.Replace("purchased_", "");

                Debug.Log($" �����޵� ���� �߰�: {productId}");
                ProcessPurchase(productId);

                keysToRemove.Add(key);
            }
        }

        //���� �Ϸ�� ���� ���� ����
        foreach (var key in keysToRemove)
        {
            PlayerPrefs.DeleteKey(key);
        }

        // ����� Ű ��� ������Ʈ
        PlayerPrefs.SetString("purchased_keys", string.Join(",", PlayerPrefs.GetString("purchased_keys", "").Split(',').Where(k => !keysToRemove.Contains(k))));
        PlayerPrefs.Save();
    }

    void ProcessPurchase(string productId)
    {
        Debug.Log($" {productId} ������ ���� �Ϸ�!");

        switch (productId)
        {
            case "jewel_5000":
                _userData._jewel += 5000;
                break;
            case "jewel_10000":
                _userData._jewel += 10000;
                break;
            default:
                Debug.LogWarning($" �� �� ���� ��ǰ ID: {productId}");
                return;
        }

        //����� ����� ������ ������Ʈ
        SaveUserData(() => UIManager.Instance.NotifyObservers(_userData));
    }
    #endregion [In-App purchase Methods]

    #region [Addressable Methods]
    public void LoadAddressable<T>(string key, Action<T> onLoaded) where T : UnityEngine.Object
    {
        if(key.Equals("Lengendary/Erylia.png"))
        {
            Debug.Log("�������� ����");
        }

        Addressables.LoadAssetAsync<T>(key).Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"{key} �ε� ����: {op.Result}");
                onLoaded?.Invoke(op.Result);  // �ҷ��� ������Ʈ�� �ݹ�(Action)���� ����
            }
            else
            {
                Debug.LogError($"{key} �ε� ����!");

                if (key.Equals("Lengendary/Erylia.png"))
                {
                    Debug.Log("�������� ������.");
                }
            }
        };
    }
    #endregion [Addressable Methods]

}