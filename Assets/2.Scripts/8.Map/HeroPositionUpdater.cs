using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPositionUpdater : SingletonMonobehaviour<HeroPositionUpdater>
{
    public Transform hero; // 히어로의 Transform
    public Material tileMaterial; // 석상 타일에 적용된 Material



    void Update()
    {
        if (hero != null && tileMaterial != null)
        {
            // 히어로의 월드 위치를 HeroPosition으로 전달
            Vector3 heroWorldPosition = hero.position;
            tileMaterial.SetVector("_HeroPosition", heroWorldPosition);
        }
    }

    public void SetHeroPosition(Transform heroPos)
    {
        hero = heroPos;
    }
}
