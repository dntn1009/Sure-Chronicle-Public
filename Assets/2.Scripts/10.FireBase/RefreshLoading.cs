using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RefreshLoading : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _loadingText;

    public void ShowRefreshLoading()
    {
        gameObject.SetActive(true);
    }

    public void HideRefreshLoading()
    {
        gameObject.SetActive(false);
    }

    public void ShowLoading(string text)
    {
        gameObject.SetActive(true);
        _loadingText.text = text;
    }
}
