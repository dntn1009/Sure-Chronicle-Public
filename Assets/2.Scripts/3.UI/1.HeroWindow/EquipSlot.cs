using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipSlot : MonoBehaviour, IPointerClickHandler
{

    [Header("Slot")]
    [SerializeField] EquipCard _equipCard;
    Card _card;

    HeroController _heroObj;

    public HeroController HeroObj { get { return _heroObj; } }
    public void Initialize(Card card = null)
    {
        if (card == null)
        {
            DeSpawnHero();
        }
        else
        {
            SpawnHero(card);
        }
    }

    public IEnumerator InitializeCoroutine(Card card = null)
    {
        if (card == null)
        {
            DeSpawnHero();
        }
        else
        {
            SpawnHero(card);
        }

        yield return null;
    }

    public void SetEquipCard(Card card)
    {
        if (_card != null)
        {
            DataManager.Instance.EquipCardToSlot(_card);
        }
        Initialize(card);
    }

    public void DeSpawnHero()
    {
        if (_card != null)
            Destroy(_heroObj.gameObject);

        _card = null;
        _equipCard.gameObject.SetActive(false);
    
    }


    public void SpawnHero(Card card)
    {
        Card before = _card;
        _card = card;
        _equipCard.gameObject.SetActive(true);
        _equipCard.Initialize(_card);
        HeroWindow hero = UIManager.Instance.GetHeroWindow();
        int index = hero.FindEquipIndex(this);
        //Debug.Log(index);

        if (index != -1)
        {
            //_heroObj = DataManager.Instance.GetHeroManager().ChangeHeroObjs(_card, index);

            StartCoroutine(DataManager.Instance.GetHeroManager().ChangeHeroObjs(_card, index, (heroObj) =>
            {
                if (heroObj != null)
                {
                    Debug.Log("HeroController 로드 완료!");
                    _heroObj = heroObj;  //정상적으로 로드된 경우 저장
                    DataManager.Instance.EquipCardToSlot(_card, index);
                }
                else
                {
                    Debug.LogError("HeroController 로드 실패!");
                }
            }));
        }

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        HeroWindow hero = UIManager.Instance.GetHeroWindow();
        bool equipCheck = hero.EquipCardClickCheck();

        Debug.Log(equipCheck);

        if (equipCheck)
        {
            Card equipCard = hero.EquipClickCard;
            SetEquipCard(equipCard);
            hero.FinishEquipClick();
        }
    }
}
