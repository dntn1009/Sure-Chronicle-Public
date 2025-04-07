using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;

public class MessageData
{
    public string _key;
    public string _title;
    public string _body;
    public MessageType _type;
    public int _value;

    public MessageData(string key, string title, string body, MessageType type, int value)
    {
        _key = key;
        _title = title;
        _body = body;
        _type = type;
        _value = value;
    }

    public MessageData()
    {
        _key = string.Empty;
        _title = string.Empty;
        _body = string.Empty;
        _type = MessageType.Gold;
        _value = 0;
    }

    public override bool Equals(object obj)
    {
        if (obj is MessageData other)
        {
            // ������ _key�� �������� ���� ���� �Ǵ�
            return _key == other._key;
        }
        return false;
    }

    // GetHashCode ������
    public override int GetHashCode()
    {
        return _key.GetHashCode();
    }

}
