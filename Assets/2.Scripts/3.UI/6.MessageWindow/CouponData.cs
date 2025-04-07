using DefineHelper;
using System.Collections.Generic;

[System.Serializable]
public class CouponData
{
    public List<string> _users;
    public string _title;
    public string _body;
    public MessageType _type;
    public int _value;

    public CouponData(List<string> users, string title, string body, MessageType type, int value)
    {
        _users = users;
        _title = title;
        _body = body;
        _type = type;
        _value = value;
    }

    public CouponData()
    {
        _users = new List<string>();
        _title = string.Empty;
        _body = string.Empty;
        _type = MessageType.Gold;
        _value = 0;
    }

    public bool userCheck(string userid)
    {
        // _users 리스트에 userid가 포함되어 있는지 확인
        return _users.Contains(userid);
    }
}
