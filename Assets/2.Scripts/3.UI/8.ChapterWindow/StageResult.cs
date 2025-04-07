using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageResult : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TextMeshProUGUI _mainText;
    [SerializeField] TextMeshProUGUI _clickText;

    bool _isClick;
    bool _isClear;

    public void Initialize()
    {
        this.gameObject.SetActive(false);
        _isClick = false;
    }
    public void OpenChapterWindow(bool Clear)
    {
        this.gameObject.SetActive(true);
        _isClear = Clear;
        _clickText.text = "- Touch To Screen -";

        if (_isClear)
        {
            _mainText.text = "보스 처치 성공";

            if (DataManager.Instance.IsAuto)
            {
                StartCoroutine(AutoNextChapter(3f));
            }

            return;
        }

        _mainText.text = "보스 처치 실패";
        if (DataManager.Instance.IsAuto)
        {
            StartCoroutine(AutoDuringChapter(3f));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_isClick && !DataManager.Instance.IsAuto)
        {
            _isClick = true;
            this.gameObject.SetActive(false);

            if (_isClear)
            {
                NextChapter();
                return;
            }

            DuringChapter();
        }
    }

    public void NextChapter()
    {
        MonsterManager.Instance.BossNextChapter();
        DataManager.Instance.NextStage();
        this.gameObject.SetActive(false);
    }

    public void DuringChapter()
    {
        MonsterManager.Instance.BossDuringChapter();
        DataManager.Instance.GetHeroManager().HerosDuringChapter();
        DataManager.Instance.GetHeroManager().SetWaitingHeros(false);
        //현재 챕터 UI문구 넣어주기 그러고 Waiting 풀어주기
        this.gameObject.SetActive(false);
    }

    IEnumerator AutoNextChapter(float time)
    {
        for (int i = (int)time; i > 0; i--)
        {
            _clickText.text = "- Auto Moving Wait.. " + i + " -";
            yield return new WaitForSeconds(1f); // 1초 대기 후 다음 숫자로 변경
        }

        _clickText.text = "- Auto Moving... -"; // 카운트다운 후 최종 메시지 변경
        yield return new WaitForSeconds(0.5f); // 짧은 대기 후 챕터 전환
        NextChapter();
    }

    IEnumerator AutoDuringChapter(float time)
    {
        for (int i = (int)time; i > 0; i--)
        {
            _clickText.text = "- Auto Moving Wait.. " + i + " -";
            yield return new WaitForSeconds(1f); // 1초 대기 후 다음 숫자로 변경
        }

        _clickText.text = "- Auto Moving... -"; // 카운트다운 후 최종 메시지 변경
        yield return new WaitForSeconds(0.5f); // 짧은 대기 후 챕터 전환
        DuringChapter();
    }
}
