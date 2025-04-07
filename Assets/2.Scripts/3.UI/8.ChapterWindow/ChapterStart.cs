using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChapterStart : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _chapterText;
    [SerializeField] TextMeshProUGUI _stageText;
    [SerializeField] Animator _anim;

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowChapterStart(string chapterText, string stageText)
    {
        this.gameObject.SetActive(true);
        AudioManager.Instance.LoadSFX("SFX/ChapterStart.wav");
        DataManager.Instance.GetHeroManager().SetWaitingHeros(true);
        MonsterManager.Instance.SetWaitingBoss(true);
        _chapterText.text = chapterText;
        _stageText.text = stageText;
        _anim.SetTrigger("Start");
    }

    public void HideChapterStart()
    {
        DataManager.Instance.GetHeroManager().SetWaitingHeros(false);
        MonsterManager.Instance.SetWaitingBoss(false);
        this.gameObject.SetActive(false);
    }
}
