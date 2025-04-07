using fsmStatePattern;
using DesignPattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class HeroManager : MonoBehaviour
{
    [SerializeField] HeroController[] _heroObjs;

    public IEnumerator ChangeHeroObjs(Card card, int index, Action<HeroController> onLoaded)
    {
        if(index != 0)
        {
            yield return new WaitUntil(() => _heroObjs[0] != null);
        }

        // ���� ����� ���� �� ��ġ ����
        Vector3 pos = DataManager.Instance.InitHeroPosition;

        if (_heroObjs[index] != null)
        {
            pos = _heroObjs[index].transform.position;
            Destroy(_heroObjs[index].gameObject);
        }
        else if (index != 0 && _heroObjs[index] == null)
        {
            pos = _heroObjs[0].transform.position;
        }
        
        // Addressable �ε�
        bool isLoaded = false;
        HeroController hero = null;

        DataManager.Instance.LoadAddressable<GameObject>(card.HeroPrefabs, (obj) =>
        {
            // ���ο� ����� ������Ʈ ����
            hero = Instantiate(obj).GetComponent<HeroController>();
            if(index == 0)
                hero.SetSpawn(index, card.MyStatus);
            else
                hero.SetSpawn(index, card.MyStatus, _heroObjs[0]);
            hero.transform.SetParent(transform);
            hero.transform.position = pos;
            _heroObjs[index] = hero;
            isLoaded = true;
        });

        // �ε� �Ϸ�� ������ ��ٸ���
        yield return new WaitUntil(() => isLoaded);

        ChangeFollowHeros();
        if (index == 0)
        {
            Camera.main.GetComponent<CameraController>().FirstSlotFollow(hero.transform);
            HeroPositionUpdater.Instance.SetHeroPosition(hero.transform);
        }

        // �Ϸ�� HeroController�� �ݹ����� ����
        onLoaded?.Invoke(hero);
    }

    /*   public HeroController ChangeHeroObjs(Card card, int index)
       {
           //��ġ ���� �� ����� ����
           Vector3 pos = DataManager.Instance.InitHeroPosition;

           if (_heroObjs[index] != null)
           {
               pos = _heroObjs[index].transform.position;
               Destroy(_heroObjs[index].gameObject);
           }
           else if (index != 0 && _heroObjs[index] == null)
           {
               pos = _heroObjs[0].transform.position;
           }

           DataManager.Instance.LoadAddressable<GameObject>(card.HeroPrefabs, (obj) =>
           {
               //���ο� ����� ������Ʈ ����
               var hobj = Instantiate(obj).GetComponent<HeroController>();
               hobj.SetSpawn(index, card.MyStatus);
               hobj.transform.SetParent(transform);
               hobj.transform.position = pos;
               _heroObjs[index] = hobj;
           });

       ChangeFollowHeros();
               if (index == 0)
               {
                   Camera.main.GetComponent<CameraController>().FirstSlotFollow(_heroObjs[index].transform);
                   HeroPositionUpdater.Instance.SetHeroPosition(_heroObjs[index].transform);
               }

           return _heroObjs[index];
       }*/


    public void HeroChangeState(IState<HeroController> state)
    {
        foreach(HeroController hero in _heroObjs)
        {
            if (hero == null || hero.IsDeath)
                continue;

            hero.ChangeAnyFromState(state);
        }
    }

    public void ChangeFollowHeros()
    {
        foreach (HeroController hero in _heroObjs)
        {
            if (hero == null)
                continue;

            hero.ChangeFollowHero(_heroObjs[0]);
        }
    }//slot0�� ���󰡵��� �����ϴ� �޼���
  
    public void SetWaitingHeros(bool Check)
    {
        foreach (HeroController hero in _heroObjs)
        {
            if (hero == null)
                continue;

            hero.WaitingCheck(Check);
        }
    }//�������� ���� ��, ��ٸ��� ���� �޼���

    public void HerosAllDeath()
    {
        foreach(HeroController hero in _heroObjs)
        {
            if (hero == null)
                continue;

            hero.StopRespawn();
        }
    }

    public void HerosDuringChapter(Transform transform = null)
    {
        foreach (HeroController hero in _heroObjs)
        {
            if (hero == null)
                continue;
            hero.ChapterSpawn(transform);
        }
    }
}
