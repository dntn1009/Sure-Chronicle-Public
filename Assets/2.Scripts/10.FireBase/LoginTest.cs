using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginTest : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _firebaseText;

    FirebaseAuth _auth;

    // Start is called before the first frame update
    void Start()
    {
        //Firebase 초기화
        _auth = FirebaseAuth.DefaultInstance;
        //GPGS 활성화
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        //Google 로그인 시도
        if (_auth.CurrentUser == null)
        {
            SignIn();

        }



    }
    public void realtimeDatabaseWrite(string userid)
    {
        List<UserCardData> _userCardDatas = new List<UserCardData>();
        _userCardDatas.Add(new UserCardData(0, Random.Range(1, 30), Random.Range(1, 5), Random.Range(1, 20), 0));
       // UserData _userData = new UserData(9999999, 20000, new TicketData(1, 1), new StageData(1, 5), new SharedEnforceData(1, 1, 1, 1), false, new Vector3(1, 0, 1), _userCardDatas);
        //string jsonData = JsonUtility.ToJson(_userData);
    /*    FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(userid).SetRawJsonValueAsync(jsonData)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("User data saved successfully.");
                    _firebaseText.text = "Data Saved Successfully!";
                }
                else
                {
                    Debug.LogError($"Failed to save user data: {task.Exception}");
                    _firebaseText.text = "Failed to Save Data.";
                }
            });*/
    }

    public void SignIn()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // 서버 인증 코드 요청
            PlayGamesPlatform.Instance.RequestServerSideAccess(false, (authCode) =>
            {
                if (!string.IsNullOrEmpty(authCode))
                {
                    Debug.Log($"Server Auth Code: {authCode}");
                    StartCoroutine(TryFirebaseLogin(authCode));
                }
                else
                    Debug.LogError("Failed to retrieve Server Auth Code. Check Web Client ID and SHA-1 settings.");
            });
        }
        else
            Debug.LogError($"Google Sign-In Failed: {status}");

    }


    IEnumerator TryFirebaseLogin(string authCode)
    {
        yield return null;
        _firebaseText.text = "Trying Firebase Login...";
        Credential credential = PlayGamesAuthProvider.GetCredential(authCode);

        _auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                _firebaseText.text = "Firebase Cancled";
            }
            else if (task.IsFaulted)
            {
                _firebaseText.text = "Firebase Faulted";
            }
            else
            {
                _firebaseText.text = "Firebase Success";
                FirebaseUser user = task.Result;
                string userId = user.UserId;
                realtimeDatabaseWrite(userId);
            }
        });

    }
}
