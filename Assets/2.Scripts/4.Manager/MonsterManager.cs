using System.Collections;
using UnityEngine;

public class MonsterManager : SingletonMonobehaviour<MonsterManager>
{
    [Header("STAGE")]
    [Header("Monster Obj & Count")]
    [SerializeField] MonsterController[] _monObj;
    [SerializeField] int _monMaxCount;
    GameObjectPool<MonsterController>[] _monPool;

    [Header("Spawn Settings")]
    [SerializeField] GameObject _spawnPositions;
    [SerializeField] MonsterSpawn[] _mSpawns;

    [Header("Boss Monster")]
    [SerializeField] BossMonsterController _boss;
    [SerializeField] GameObject _bossObj;

    [Header("RAID")]
    [Header("Raid Transform & Check")]
    [SerializeField] bool _isRaid;
    [SerializeField] Transform _raidPos;
    [SerializeField] Transform _heroRaidPos;

    bool _isBoss;
    public bool _IsBoss { get { return _isBoss; } }

    private Coroutine _spawnCoroutine;

    private void Start()
    {
        if (!_isRaid)
        {
            InitializePooling();
            return;
        }

        InitializeRaid();
    }

    #region [Initialize Methods]
    void InitializePooling()
    {
        _isBoss = false;
        /*_genPosition = _genPositionPrefab.GetComponentsInChildren<SpawnPos>();
        _genCheck = new bool[_genPosition.Length];*/
        _mSpawns = _spawnPositions.GetComponentsInChildren<MonsterSpawn>();

        _monPool = new GameObjectPool<MonsterController>[_monObj.Length];

        for (int i = 0; i < _monObj.Length; i++)
        {
            _monPool[i] = new GameObjectPool<MonsterController>(_monMaxCount, () =>
            {
                var obj = Instantiate(_monObj[i]);
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(transform);
                obj.transform.localPosition = Vector3.zero;
                var _monster = obj.GetComponent<MonsterController>();
                _monster.InitializeSet();
                return _monster;
            });
        }

        _spawnCoroutine = StartCoroutine(CreateMonster(_mSpawns, 4f));
    }

    void InitializeRaid()
    {
        Debug.Log("작동 완료");
        CreateRaidBoss(_raidPos);
    }


    #endregion [Initialize Methods]

    #region [Monster Methods]
    public IEnumerator CreateMonster(MonsterSpawn[] spawns, float spawnInterval)
    {
        while (true)
        {
            // 각 스폰 포인트에 대해 별도의 코루틴을 시작하여 몬스터 생성
            foreach (MonsterSpawn spawn in spawns)
            {
                CreateMonsterForSpawn(spawn);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void CreateMonsterForSpawn(MonsterSpawn spawn)
    {
        for (int i = 0; i < spawn.COUNTS.Length; i++)
        {
            while (spawn._currentMCounts[i] < spawn.COUNTS[i])
            {
                var mon = _monPool[spawn.MONNUMS[i]].Get();
                mon.GetSpawn(spawn, spawn.RADIUS, i);
                spawn._currentMCounts[i]++;
            }
        }
    }

    public IEnumerator RemoveMonsterDelay(MonsterController mon)
    {
        yield return new WaitForSeconds(3f);
        RemoveMonster(mon);
    }

    public void RemoveMonster(MonsterController mon, bool coroutineCheck = false)
    {
        if (coroutineCheck)
            mon.StopRemoveCoroutine();
        mon.UnitListClear();
        mon.gameObject.SetActive(false);
        mon.MySpawn._currentMCounts[mon.MyNumber]--;
        _monPool[mon.MyNumber].Set(mon);
    }

    public void StartSpawnMonster()
    {
        _spawnCoroutine = StartCoroutine(CreateMonster(_mSpawns, 4f));
    }//몬스터 생성 코루틴 다시 시작.
    #endregion [Monster Methods]

    #region [Boss Monster Methods]
    public void CreateBoss()
    {
        StopCoroutine(_spawnCoroutine);
        _isBoss = true;
        var camera = Camera.main.GetComponent<CameraController>();
        var boss = Instantiate(_boss, this.transform).GetComponent<BossMonsterController>();
        boss.InitializeSet();
        _bossObj = boss.gameObject;
        camera.FocusOnBoss(_bossObj.transform, 2f);
    }//보스 생성 메서드

    public void CreateRaidBoss(Transform spawnPos)
    {
        DataManager.Instance.GetHeroManager().HerosDuringChapter(_heroRaidPos);
        var camera = Camera.main.GetComponent<CameraController>();
        var boss = Instantiate(_boss, spawnPos).GetComponent<BossMonsterController>();
        boss.InitializeSet();
        _bossObj = boss.gameObject;
        string chapter = "Chapter " + DataManager.Instance.StageData._stageNumber + "-" + DataManager.Instance.StageData._roundNumber;
        GameSceneManager.Instance.ShowChapterStart(chapter, "RAID");
    }

    public void SetWaitingBoss(bool check)
    {
        if (_bossObj != null)
            _bossObj.GetComponent<BossMonsterController>().WaitingCheck(check);
    }// 보스 Wait

    public void BossDuringChapter()
    {
        Destroy(_bossObj);
        _bossObj = null;
        _isBoss = false;
        StartSpawnMonster();
    }

    public void BossNextChapter()
    {
        Destroy(_bossObj);
        _bossObj = null;
        _isBoss = false;
    }
    #endregion [Boss Monster Methods]
}
