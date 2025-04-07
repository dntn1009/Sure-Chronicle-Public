using DefineHelper;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;
using UnityEngine.UI;

public class DrawBox : MonoBehaviour
{
    [Header("UI Var")]
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] Image _boxImg;
    [SerializeField] Image _payImg;
    [SerializeField] TextMeshProUGUI _payText;
    [SerializeField] Button _drawBtn;
    [SerializeField] TextMeshProUGUI _btnText;
    [Header("Draw Data")]
    [SerializeField] BoxData _boxData;
    [SerializeField] bool _isStart;

    CodelessIAPButton _IAP;

    private void Start()
    {
        if (_isStart)
            InitDrawSet();
    }

    public DrawBox Initialize(BoxData boxData)
    {
        _boxData = boxData;
        InitDrawSet();
        return this;
    }

    void InitDrawSet()
    {
        _titleText.text = _boxData._title;
        _titleText.color = _boxData._imgColor;
        _boxImg.sprite = _boxData._boxSprite;
        _boxImg.color = _boxData._imgColor;
        _payImg.sprite = _boxData._paySprite;
        _payText.text = _boxData._pay.ToString("N0");
        _btnText.text = _boxData._btnStr;
        switch (_boxData._boxType)
        {
            case BoxType.Gold:
                _drawBtn.onClick.AddListener(() => payToGold(_boxData));
                break;
            case BoxType.Card:
                _drawBtn.onClick.AddListener(() => payToDraw(_boxData));
                break;
            case BoxType.Jewel:
                    break;
            case BoxType.AD:
                _drawBtn.onClick.AddListener(() => adToJewel(_boxData));
                _payText.text = "AD";
                break;
        }
    }

    void payToDraw(BoxData boxData)
    {
        bool isPay = DataManager.Instance.CountRandomDrawUserCardData(boxData);
        if (!isPay)
        {

        }
    }

    void payToGold(BoxData boxData)
    {
        bool isPay = DataManager.Instance.JewelExChangeGoldUserData(boxData);
        if (!isPay)
        {

        }
    }

    void adToJewel(BoxData boxData)
    {
        bool isPay = DataManager.Instance.ADExchangeJewelUserData(boxData);
        if (!isPay)
        {

        }
    }


    #region [In-App-Purchase Methods]
    public void OnPurchaseComplete(Product product)
    {
        Debug.Log($" ���� ����: {product.definition.id}");

        string packageName = Application.identifier; // ��Ű�� �̸�
        string productId = product.definition.id; // ��ǰ ID
        string purchaseToken = null;

        if (!string.IsNullOrEmpty(product.receipt))
        {
            Debug.Log($" Raw Receipt JSON: {product.receipt}");

            try
            {
                // Step 1: �ֻ��� Payload JSON �Ľ�
                ReceiptWrapper payloadData = JsonUtility.FromJson<ReceiptWrapper>(product.receipt);
                if (payloadData != null && !string.IsNullOrEmpty(payloadData.Payload))
                {
                    // Step 2: Payload ������ JSON ���ڿ��� �ٽ� �Ľ�
                    PurchaseData purchaseData = JsonUtility.FromJson<PurchaseData>(payloadData.Payload);
                    if (purchaseData != null && !string.IsNullOrEmpty(purchaseData.json))
                    {
                        // Step 3: ���������� purchaseToken�� ����
                        PurchaseJson purchaseInfo = JsonUtility.FromJson<PurchaseJson>(purchaseData.json);
                        if (purchaseInfo != null)
                        {
                            purchaseToken = purchaseInfo.purchaseToken;
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($" JSON �Ľ� ����! Error: {e.Message}");
            }
        }

        Debug.Log($" packageName: {packageName}");
        Debug.Log($" productId: {productId}");
        Debug.Log($" Extracted purchaseToken: {purchaseToken}");


        // ���� ���� ����
        if (!string.IsNullOrEmpty(purchaseToken))
        {
            Debug.Log(" Firebase Functions�� ���� ���� ��û ��...");
            Task.Run(() => DataManager.Instance.VerifyPurchase(packageName, productId, purchaseToken, _boxData));
        }
        else
        {
            Debug.LogError(" ���� ��ū�� �����ϴ�. ������ �� ����.");
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogError($"���� ����: {failureDescription.reason}, ���� ����: {failureDescription.message}");
        // ����ڿ��� ���� �޽��� ǥ�� ���� �߰�
    }

    private bool VerifyReceipt(Product product)
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            try
            {
                // Google �� Apple ������� ����ȭ�� Ű �����͸� �ҷ���
                var validator = new CrossPlatformValidator(
                    GooglePlayTangle.Data(),   // Google Play ����ȭ Ű
                    null,        // Apple App Store ����ȭ Ű
                    Application.identifier     // ���� ���� ��Ű����
                );

                // ������ ����
                var result = validator.Validate(product.receipt);

                foreach (IPurchaseReceipt receipt in result)
                {
                    Debug.Log($"��ǰ ID: {receipt.productID}");
                    Debug.Log($"���� ��¥: {receipt.purchaseDate}");
                    Debug.Log($"Ʈ����� ID: {receipt.transactionID}");
                }

                return true;  // ���� ����
            }
            catch (IAPSecurityException ex)
            {
                Debug.LogError("������ ���� ����: " + ex.Message);
                return false; // ���� ����
            }
        }

        // ������ �׽�Ʈ �� �׻� true ��ȯ
        Debug.LogWarning("������ ȯ�濡�� ���� ��ŵ");
        return Application.isEditor;
    }
    #endregion [In-App-Purchase Methods]
}

[System.Serializable]
public class ReceiptWrapper
{
    public string Payload;
}

[System.Serializable]
public class PurchaseData
{
    public string json;
}

[System.Serializable]
public class PurchaseJson
{
    public string orderId;
    public string packageName;
    public string productId;
    public long purchaseTime;
    public int purchaseState;
    public string purchaseToken;
    public int quantity;
    public bool acknowledged;
}