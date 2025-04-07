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
            _mainText.text = "���� óġ ����";

            if (DataManager.Instance.IsAuto)
            {
                StartCoroutine(AutoNextChapter(3f));
            }

            return;
        }

        _mainText.text = "���� óġ ����";
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
        //���� é�� UI���� �־��ֱ� �׷��� Waiting Ǯ���ֱ�
        this.gameObject.SetActive(false);
    }

    IEnumerator AutoNextChapter(float time)
    {
        for (int i = (int)time; i > 0; i--)
        {
            _clickText.text = "- Auto Moving Wait.. " + i + " -";
            yield return new WaitForSeconds(1f); // 1�� ��� �� ���� ���ڷ� ����
        }

        _clickText.text = "- Auto Moving... -"; // ī��Ʈ�ٿ� �� ���� �޽��� ����
        yield return new WaitForSeconds(0.5f); // ª�� ��� �� é�� ��ȯ
        NextChapter();
    }

    IEnumerator AutoDuringChapter(float time)
    {
        for (int i = (int)time; i > 0; i--)
        {
            _clickText.text = "- Auto Moving Wait.. " + i + " -";
            yield return new WaitForSeconds(1f); // 1�� ��� �� ���� ���ڷ� ����
        }

        _clickText.text = "- Auto Moving... -"; // ī��Ʈ�ٿ� �� ���� �޽��� ����
        yield return new WaitForSeconds(0.5f); // ª�� ��� �� é�� ��ȯ
        DuringChapter();
    }
}
