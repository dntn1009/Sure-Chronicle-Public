using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrograssBar : MonoBehaviour
{
    [Header("Basic PrograssBar")]
    [SerializeField] Image progressBarFill; // 채워진 부분의 이미지
    [SerializeField] RectTransform pointer; // 포인터 이미지
    
    private void Awake()
    {
        ChangeAmountFromStage(1, 30);
    }

    public virtual void ChangeAmountFromStage(int current, int max)
    {
        // 진행 바 채우기 (0 ~ 1 사이 비율로 설정)
        float fillAmount = (float)current / max;
        progressBarFill.fillAmount = fillAmount;

        // 포인터 위치 업데이트
        Vector2 pointerPosition = pointer.anchoredPosition;
        pointerPosition.x = fillAmount * progressBarFill.rectTransform.rect.width;
        pointer.anchoredPosition = pointerPosition;
    }
}
