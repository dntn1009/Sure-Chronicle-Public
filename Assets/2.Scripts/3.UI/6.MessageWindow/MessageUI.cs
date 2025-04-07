using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DefineHelper;

public class MessageUI : MonoBehaviour
{
    [Header("Message Data UI")]
    [SerializeField] Image _itemImg;
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] TextMeshProUGUI _bodyText;
    [SerializeField] Button _receiveBtn;
    [SerializeField] TextMeshProUGUI _valueText;

    [Header("Message Sprite")]
    [SerializeField] Sprite[] _itemSprite;

    MessageData _myMessageData;

    MessageWindow _messageWindow;

    public void Initialize(MessageData messageData, MessageWindow window)
    {
        _myMessageData = messageData;
        _messageWindow = window;
        _receiveBtn.onClick.AddListener(() => Receive());
        SpriteType(_myMessageData);
        SetValue(_myMessageData);
        _titleText.text = _myMessageData._title;
        _bodyText.text = _myMessageData._body;
    }

    void SpriteType(MessageData messageData)
    {
        switch(messageData._type)
        {
            case MessageType.Gold:
                _itemImg.sprite = _itemSprite[0];
                break;
            case MessageType.Jewel:
                _itemImg.sprite = _itemSprite[1];
                break;
            case MessageType.Card:
                _itemImg.sprite = _itemSprite[2];
                break;
        }
    }
    void SetValue(MessageData messageData)
    {
        if(messageData._type == MessageType.Card)
        {
            string value = DataManager.Instance.CardDataGetName(messageData._value);
            return;
        }
        _valueText.text = messageData._value.ToString("N0");
    }

    void Receive()
    {
        _messageWindow.LoadingSetActive(true);
        StartCoroutine(DataManager.Instance.DeleteMessage(_myMessageData, DeleteMessageUI));
    }
    void DeleteMessageUI()
    {
        _messageWindow.RemoveMessageUI(this);
        Destroy(gameObject);
        _messageWindow.LoadingSetActive(false);
        _messageWindow.SetMessageBtnCount(DataManager.Instance.MessageList);
    }
    public MessageData GetMessageData()
    {
        return _myMessageData;
    }
}
