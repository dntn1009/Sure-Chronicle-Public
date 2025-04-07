using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform _object;
    // Update is called once per frame
    [SerializeField] Vector3 _moveVec;
    [SerializeField] float _Speed = 5f;

    private bool _isFocusingOnBoss = false; // ������ ���� ������ Ȯ���ϴ� ����

    private void LateUpdate()
    {
        CameraFollowToHero();
    }

    public void CameraFollowToHero()
    {
        if (!_isFocusingOnBoss && _object != null)
            transform.position = _object.position + _moveVec;
    }

    public void FirstSlotFollow(Transform obj)
    {
        _object = obj;
    }

    public void FocusOnBoss(Transform boss, float duration)
    {
        StartCoroutine(FocusOnBossCoroutine(boss, duration));
    }

    private IEnumerator FocusOnBossCoroutine(Transform boss, float duration)
    {
        if (boss == null) yield break;

        DataManager.Instance.GetHeroManager().SetWaitingHeros(true);
        _isFocusingOnBoss = true;
        Transform originalObject = _object;  // ���� Ÿ���� ���� ������ ����
        _object = boss; // ������ Ÿ������ ����

        // �ε巴�� ������ �̵�
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = _object.position + _moveVec;

        while (elapsedTime < 1f)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            elapsedTime += Time.deltaTime * _Speed;
            yield return null;
        }

        transform.position = targetPosition; // ��Ȯ�� ��ġ ����

        // **���⼭ �׳� ���� �ð� ���**
        yield return new WaitForSeconds(duration);

        // ���� ��ġ�� ����
        elapsedTime = 0f;
        startPosition = transform.position;
        targetPosition = originalObject.position + _moveVec;

        while (elapsedTime < 1f)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            elapsedTime += Time.deltaTime * _Speed;
            yield return null;
        }

        transform.position = targetPosition; // ��Ȯ�� ��ġ ����
        _object = originalObject; // ���� Ÿ������ ����
        _isFocusingOnBoss = false;
        string chapter = "Chapter " + DataManager.Instance.StageData._stageNumber + "-" + DataManager.Instance.StageData._roundNumber;
        GameSceneManager.Instance.ShowChapterStart(chapter, "BOSS");
    }
}
