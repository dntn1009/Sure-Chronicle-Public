using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrograssBar : MonoBehaviour
{
    [Header("Basic PrograssBar")]
    [SerializeField] Image progressBarFill; // ä���� �κ��� �̹���
    [SerializeField] RectTransform pointer; // ������ �̹���
    
    private void Awake()
    {
        ChangeAmountFromStage(1, 30);
    }

    public virtual void ChangeAmountFromStage(int current, int max)
    {
        // ���� �� ä��� (0 ~ 1 ���� ������ ����)
        float fillAmount = (float)current / max;
        progressBarFill.fillAmount = fillAmount;

        // ������ ��ġ ������Ʈ
        Vector2 pointerPosition = pointer.anchoredPosition;
        pointerPosition.x = fillAmount * progressBarFill.rectTransform.rect.width;
        pointer.anchoredPosition = pointerPosition;
    }
}
