using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform _object;
    // Update is called once per frame
    [SerializeField] Vector3 _moveVec;
    [SerializeField] float _Speed = 5f;

    private bool _isFocusingOnBoss = false; // 보스에 집중 중인지 확인하는 변수

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
        Transform originalObject = _object;  // 기존 타겟을 로컬 변수로 저장
        _object = boss; // 보스를 타겟으로 변경

        // 부드럽게 보스로 이동
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = _object.position + _moveVec;

        while (elapsedTime < 1f)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            elapsedTime += Time.deltaTime * _Speed;
            yield return null;
        }

        transform.position = targetPosition; // 정확한 위치 보정

        // **여기서 그냥 일정 시간 대기**
        yield return new WaitForSeconds(duration);

        // 원래 위치로 복귀
        elapsedTime = 0f;
        startPosition = transform.position;
        targetPosition = originalObject.position + _moveVec;

        while (elapsedTime < 1f)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            elapsedTime += Time.deltaTime * _Speed;
            yield return null;
        }

        transform.position = targetPosition; // 정확한 위치 보정
        _object = originalObject; // 원래 타겟으로 복귀
        _isFocusingOnBoss = false;
        string chapter = "Chapter " + DataManager.Instance.StageData._stageNumber + "-" + DataManager.Instance.StageData._roundNumber;
        GameSceneManager.Instance.ShowChapterStart(chapter, "BOSS");
    }
}
