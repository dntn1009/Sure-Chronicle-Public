using System.Collections;
using TMPro;
using UnityEngine;

public class DamageFont : MonoBehaviour
{
    public TextMeshProUGUI _textMesh;
    private float _moveSpeed = 1f; // ���� �̵� �ӵ�
    private float _fadeSpeed = 1f; // ������� �ӵ�
    private float _lifetime = 1f; // ���� �ð�
    private float _defaultFontSize; // �⺻ ��Ʈ ũ�� ����

    private void Awake()
    {
        _defaultFontSize = _textMesh.fontSize; // ó�� ��Ʈ ũ�� ����
    }

    public void SetDamageText(int damage, bool isCritical, float moveSpeed = 1f, float lifetime = 1f)
    {
        this.gameObject.SetActive(true);
        _textMesh.text = damage.ToString();
        _moveSpeed = moveSpeed;
        _lifetime = lifetime;
        if (isCritical)
        {
            _textMesh.fontSize = _defaultFontSize * 1.5f; // ũ��Ƽ�� �� 1.5�� Ȯ��
            _textMesh.color = Color.red;
        }
        else
        {
            _textMesh.fontSize = _defaultFontSize; // �Ϲ� ������ �⺻ ũ�� ����
            _textMesh.color = Color.white;
        }

        StartCoroutine(FadeOut());
    }
    public void SetHealText(int amount, float moveSpeed = 1f, float lifetime = 1f)
    {
        this.gameObject.SetActive(true);
        _textMesh.text = amount.ToString();
        _moveSpeed = moveSpeed;
        _lifetime = lifetime;
        _textMesh.color = Color.green;

        StartCoroutine(FadeOut());
    }

    public void SetGoldText(int amount, float moveSpeed = 1f, float lifetime = 1f, bool _isJewel = false)
    {
        this.gameObject.SetActive(true);
        _textMesh.text = "G " + amount.ToString();
        _moveSpeed = moveSpeed;
        _lifetime = lifetime;

        if (!_isJewel)
            _textMesh.color = Color.yellow;
        else
            _textMesh.color = Color.magenta;

        StartCoroutine(FadeOut());
    }

    public void ResetFont()
    {
        _textMesh.fontSize = _defaultFontSize; //  ��Ʈ ũ�� �������
        _textMesh.color = new Color(_textMesh.color.r, _textMesh.color.g, _textMesh.color.b, 1f); //  ���� �� �������
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(Random.Range(-0.2f, 0.2f), 1f, 0); // �����ϰ� ���� �̵�

        while (elapsedTime < _lifetime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / _lifetime);
            _textMesh.color = new Color(_textMesh.color.r, _textMesh.color.g, _textMesh.color.b, Mathf.Lerp(1, 0, elapsedTime / _lifetime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        UIManager.Instance.RemoveDamageFont(this);
    }
}
