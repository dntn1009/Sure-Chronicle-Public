using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    [SerializeField] int[] _monNums;
    [SerializeField] float _mRadius;
    [SerializeField] int[] _monCounts;
    
    public int[] _currentMCounts;

    public int[] MONNUMS { get { return _monNums; } }
    public float RADIUS { get { return _mRadius; } }
    public int[] COUNTS { get { return _monCounts; } }

    private void Awake()
    {
        _currentMCounts = new int[_monCounts.Length];
    }

    public Transform MTRANSFORM { get { return transform; } }

}
