using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics.Contracts;
public class IDLookupTable : MonoBehaviour
{
    public static IDLookupTable instance;

    [SerializeField] public List<CardBase> unitCards;
    [SerializeField] List<CardBase> useCards; 
    [SerializeField] List<CardBase> enemyCards;
    
    public List<CardSaveData> playerDeck = new List<CardSaveData>();
    public List<CharmSaveData> charmsInPlayerStorage = new List<CharmSaveData>();
    Dictionary<int, CardBase> cardLookup;

    [Header("For Events")]
    public List<SkullCharm> skullCharmList;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);

        cardLookup = new Dictionary<int, CardBase>();
        foreach (var card in unitCards)
            cardLookup[card.ID] = card;

        foreach (var card in useCards)
            cardLookup[card.ID] = card;

        foreach (var card in enemyCards)
            cardLookup[card.ID] = card;
    }


   
    public CardBase GetCardByID(int ID)
    {
        if (ID < 200 || (ID >= 200 && (ID / 100) % 2 == 0))
        {
            // Unit/Use cards
            return unitCards.FirstOrDefault(card => card.ID == ID) ?? useCards.FirstOrDefault(c => c.ID == ID);
        }
        else if (ID >= 300 && (ID / 100) % 2 == 1)
        {
            // Enemy cards
            return enemyCards.FirstOrDefault(card => card.ID == ID);
        }

        Debug.LogWarning($"No template card found for ID: {ID}");
        return null;

    }

    public UnitCard GetCompanionCard(int id = 0)
    {
        if(id != 0)
        {
           return  unitCards.Find(x => x.ID == id) as UnitCard;
        }
        else
        {
            var companions = unitCards.OfType<UnitCard>().Where(x => !x.isBoss && !playerDeck.Any(y => y.baseID == x.ID)).OrderBy(x => Random.value).ToList();
            return companions[0];//just the first one
        }
        
    }
    
}
