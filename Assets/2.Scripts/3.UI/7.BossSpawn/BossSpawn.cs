using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossSpawn : MonoBehaviour
{
    [Header("Kill Count")]
    [SerializeField] GameObject _killCountObj;
    [SerializeField] TextMeshProUGUI _killCount;

    [Header("Boss Spawn Btn")]
    [SerializeField] Button _bossSpawnBtn;

    StageData _stageData;
    int _roundMax;

    bool _spawnCheck;
    public void InitializeSet(UserData userData)
    {
        _stageData = userData._stageData;
        _roundMax = DataManager.Instance._roundMAX;
        _bossSpawnBtn.onClick.AddListener(() => bossMonsterSpawn());

        if (_stageData._killNumber >= _roundMax)
        {
            _spawnCheck = true;
            _killCountObj.SetActive(false);
            _bossSpawnBtn.gameObject.SetActive(true);
        }
        else
        {
            _spawnCheck = false;
            _killCountObj.SetActive(true);
            _bossSpawnBtn.gameObject.SetActive(false);
            _killCount.text = _stageData._killNumber + "/" + _roundMax;
        }
    }

    public void killCountIncrease()
    {

        if (_spawnCheck)
        {
            return;
        }

        if (_stageData._killNumber >= _roundMax)
        {
            _spawnCheck = true;
            _killCountObj.SetActive(false);
            _bossSpawnBtn.gameObject.SetActive(true);
            if (DataManager.Instance.IsAuto)
                bossMonsterSpawn();
            return;
        }

        _killCount.text = _stageData._killNumber + "/" + _roundMax;
    }

    public void bossMonsterSpawn()
    {
        if (DataManager.Instance.IsRaid)
            return;

        AudioManager.Instance.LoadSFX("SFX/BossButton.wav");
        _bossSpawnBtn.gameObject.SetActive(false);
        MonsterManager.Instance.CreateBoss();
    }

    public void NextChapterKiiCount()
    {
        _spawnCheck = false;
        DataManager.Instance.StageData._killNumber = 0;
        _killCountObj.SetActive(true);
        _killCount.text = _stageData._killNumber + "/" + _roundMax;
    }
}
