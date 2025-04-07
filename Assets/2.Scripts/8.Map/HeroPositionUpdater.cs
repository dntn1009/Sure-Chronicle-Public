using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPositionUpdater : SingletonMonobehaviour<HeroPositionUpdater>
{
    public Transform hero; // ������� Transform
    public Material tileMaterial; // ���� Ÿ�Ͽ� ����� Material



    void Update()
    {
        if (hero != null && tileMaterial != null)
        {
            // ������� ���� ��ġ�� HeroPosition���� ����
            Vector3 heroWorldPosition = hero.position;
            tileMaterial.SetVector("_HeroPosition", heroWorldPosition);
        }
    }

    public void SetHeroPosition(Transform heroPos)
    {
        hero = heroPos;
    }
}
