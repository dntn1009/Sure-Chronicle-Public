using GooglePlayGames;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngameUserInfoUI : MonoBehaviour
{
    [SerializeField] RawImage _userImg;
    [SerializeField] TextMeshProUGUI _goldText;
    [SerializeField] TextMeshProUGUI _jewelText;
    [SerializeField] TextMeshProUGUI _userName;
    RawImage test;

    public void InitializeSet(UserData userData)
    {
        Debug.Log(PlayGamesPlatform.Instance.localUser.id);
        Debug.Log(PlayGamesPlatform.Instance.localUser.image);
        Debug.Log(PlayGamesPlatform.Instance.localUser.state);
        Debug.Log(PlayGamesPlatform.Instance.localUser.userName);
        StartCoroutine(LoadUserInfo(userData));
        
    }

    public void SetGoldJewel(UserData userData)
    {
        _goldText.text = userData._gold.ToString("N0");
        _jewelText.text = userData._jewel.ToString("N0");
    }

    private IEnumerator LoadUserInfo(UserData userData)
    {
        _goldText.text = userData._gold.ToString("N0");
        _jewelText.text = userData._jewel.ToString("N0");
        _userName.text = PlayGamesPlatform.Instance.localUser.userName;
        yield return new WaitUntil(() => PlayGamesPlatform.Instance.localUser.image != null);

        Debug.LogFormat("Social.localUser.image: {0}", PlayGamesPlatform.Instance.localUser.image);

        _userImg.texture = PlayGamesPlatform.Instance.localUser.image;
/*        Texture2D texture = PlayGamesPlatform.Instance.localUser.image;

        Debug.LogFormat("{0}x{1}", texture.width, texture.height);

        imgPlayer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));*/
    }
}
