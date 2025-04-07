using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserInfo
{
    public int _level;
    public int _star;
    public int _count;
    public int _equipIndex;

    public int CountMax { get { return _star * 5; } }
    public UserInfo(UserCardData userdata)
    {
        _level = userdata._level;
        _star = userdata._star;
        _count = userdata._count;
        _equipIndex = userdata._equipIndex;
    }

}
