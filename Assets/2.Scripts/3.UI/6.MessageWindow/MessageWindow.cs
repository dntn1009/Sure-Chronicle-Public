using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageWindow : TWindowController
{
    [Header("Receive & Message")]
    [SerializeField] Button _receiveBtn;
    [SerializeField] Transform _messageContent;
    [SerializeField] GridLayoutGroup _grid;
    [SerializeField] MessageUI _message;

    [Header("Message Count View")]
    [SerializeField] GameObject _messageCount;
    [SerializeField] TextMeshProUGUI _count;

    [Header("Loading")]
    [SerializeField] GameObject _loading;

    [SerializeField] List<MessageUI> _messageUIList = new List<MessageUI>();

    public override void Operate()
    {
        base.Operate();
        InitializeSet();
    }

    public override void OpenWindow()
    {
        base.OpenWindow();
        _loading.SetActive(true);
        StartCoroutine(DataManager.Instance.UpdateSetMessage(() => openMessageWindow(this)));
    }

    #region [Initialize Methods]
    public void InitializeSet()
    {
        _loading.SetActive(false);
        _receiveBtn.onClick.AddListener(Receive);
        var messageList = DataManager.Instance.MessageList;
        SetMessageBtnCount(messageList);
        InitSetMessageBox(messageList);
    }
    void InitSetMessageBox(List<MessageData> messageList)
    {
        if (messageList.Count == 0)
            return;

        SetScrollSize(messageList);
        foreach (MessageData message in messageList)
        {
            var messageUI = Instantiate(_message, _messageContent);
                messageUI.Initialize(message, this);
            _messageUIList.Add(messageUI);
        }
    }
    #endregion [Initialize Methods]

    #region [Message Methods]
    void openMessageWindow(MessageWindow window)
    {
        var messageList = DataManager.Instance.MessageList;

        if(messageList.Count == _messageUIList.Count)
        {
            _loading.SetActive(false);
            return;
        }

        foreach (MessageData message in messageList)
        {
            bool isAlreadyInUI = _messageUIList.Exists(ui => ui.GetMessageData()._key == message._key);

            if (!isAlreadyInUI)
            {
                var messageUI = Instantiate(_message, _messageContent);
                messageUI.Initialize(message, window);
                SetScrollSize(DataManager.Instance.MessageList);
                _messageUIList.Add(messageUI);
            }
        }
        _loading.SetActive(false);
        SetMessageBtnCount(messageList);
    }
    public void SetMessageBtnCount(List<MessageData> messageList)
    {
        Debug.Log("�޽�����ư ī��Ʈ");
        if (messageList.Count != 0)
        {
            _messageCount.SetActive(true);
            _count.text = messageList.Count.ToString();
        }
        else
        {
            _messageCount.SetActive(false);
        }
    } // �޽��� ��ư���� ī��Ʈ ��
    void SetScrollSize(List<MessageData> messageList)
    {
        float cellHeight = _grid.cellSize.y; // ���� ����
        float spacingY = _grid.spacing.y;   // �� ���� ���� ����

        float totalHeight = messageList.Count * (cellHeight + spacingY);
        _messageContent.GetComponent<RectTransform>().sizeDelta = new Vector2(_messageContent.GetComponent<RectTransform>().sizeDelta.x, totalHeight);
    } // ��ũ�� ������ (Height) ����
    public void LoadingSetActive(bool check)
    {
        _loading.SetActive(check);
    }

    void Receive()
    {
        _loading.SetActive(true);
        StartCoroutine(DataManager.Instance.AllDeleteMessage(AllDeleteMessageUI));
    }

    public void AllDeleteMessageUI()
    {
        Debug.Log("Message UI Delete");
        foreach(MessageUI messageUI in _messageUIList)
        {
            Destroy(messageUI.gameObject);
        }
        _messageUIList.Clear();
        Debug.Log("Message UI Delete2");
        SetScrollSize(DataManager.Instance.MessageList);
        Debug.Log("Message UI Delete3");
        _loading.SetActive(false);
        Debug.Log("Message UI Delete4");
        SetMessageBtnCount(DataManager.Instance.MessageList);
    }

    public void RemoveMessageUI(MessageUI messageUI)
    {
        // _messageUIList���� ����
        _messageUIList.Remove(messageUI);
        SetScrollSize(DataManager.Instance.MessageList);
        Debug.Log("UI ����Ʈ���� �޽����� ���ŵǾ����ϴ�.");
    }
    #endregion [Message Methods]

}
