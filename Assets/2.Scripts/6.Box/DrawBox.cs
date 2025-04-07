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
        Debug.Log($" 구매 성공: {product.definition.id}");

        string packageName = Application.identifier; // 패키지 이름
        string productId = product.definition.id; // 상품 ID
        string purchaseToken = null;

        if (!string.IsNullOrEmpty(product.receipt))
        {
            Debug.Log($" Raw Receipt JSON: {product.receipt}");

            try
            {
                // Step 1: 최상위 Payload JSON 파싱
                ReceiptWrapper payloadData = JsonUtility.FromJson<ReceiptWrapper>(product.receipt);
                if (payloadData != null && !string.IsNullOrEmpty(payloadData.Payload))
                {
                    // Step 2: Payload 내부의 JSON 문자열을 다시 파싱
                    PurchaseData purchaseData = JsonUtility.FromJson<PurchaseData>(payloadData.Payload);
                    if (purchaseData != null && !string.IsNullOrEmpty(purchaseData.json))
                    {
                        // Step 3: 최종적으로 purchaseToken을 추출
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
                Debug.LogError($" JSON 파싱 실패! Error: {e.Message}");
            }
        }

        Debug.Log($" packageName: {packageName}");
        Debug.Log($" productId: {productId}");
        Debug.Log($" Extracted purchaseToken: {purchaseToken}");


        // 구매 검증 실행
        if (!string.IsNullOrEmpty(purchaseToken))
        {
            Debug.Log(" Firebase Functions에 구매 검증 요청 중...");
            Task.Run(() => DataManager.Instance.VerifyPurchase(packageName, productId, purchaseToken, _boxData));
        }
        else
        {
            Debug.LogError(" 구매 토큰이 없습니다. 검증할 수 없음.");
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogError($"구매 실패: {failureDescription.reason}, 세부 사항: {failureDescription.message}");
        // 사용자에게 실패 메시지 표시 로직 추가
    }

    private bool VerifyReceipt(Product product)
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            try
            {
                // Google 및 Apple 스토어의 난독화된 키 데이터를 불러옴
                var validator = new CrossPlatformValidator(
                    GooglePlayTangle.Data(),   // Google Play 난독화 키
                    null,        // Apple App Store 난독화 키
                    Application.identifier     // 현재 앱의 패키지명
                );

                // 영수증 검증
                var result = validator.Validate(product.receipt);

                foreach (IPurchaseReceipt receipt in result)
                {
                    Debug.Log($"상품 ID: {receipt.productID}");
                    Debug.Log($"구매 날짜: {receipt.purchaseDate}");
                    Debug.Log($"트랜잭션 ID: {receipt.transactionID}");
                }

                return true;  // 검증 성공
            }
            catch (IAPSecurityException ex)
            {
                Debug.LogError("영수증 검증 실패: " + ex.Message);
                return false; // 검증 실패
            }
        }

        // 에디터 테스트 시 항상 true 반환
        Debug.LogWarning("에디터 환경에서 검증 스킵");
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