using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics.Contracts;
public class IDLookupTable : MonoBehaviour
{
    public static IDLookupTable instance;

    [SerializeField] List<CardBase> unitCards;//100-199
    [SerializeField] List<CardBase> useCards; //200-299
    [SerializeField] List<CardBase> enemyCards; //300-399
     public List<CardBase> playerDeck = new List<CardBase>(); //random numbers 400+ -> this is the players deck!
    Dictionary<int, CardBase> cardLookup;
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


    public CardBase GetPlayerCardsByID(int ID)
    {
       return playerDeck.FirstOrDefault(card => card.ID == ID)?.Clone();
    }
   /* public CardBase GetCardByID(int ID)
    {

     
        else
        {
            Debug.Log("Card found: " + match.name);
        }
        return cardLookup.TryGetValue(ID, out var card) ? card.Clone() : null;
    }
    */
    public CardBase GetCardByID(int ID)
    {
        foreach (var card in unitCards)
        {
            Debug.Log($"Card in unitCards: ID={card.ID}, Name={card.name}");
        }
        if (ID < 200)
        {
            var card = unitCards.FirstOrDefault(card => card.ID == ID);
            if(card != null)
            {
                Debug.Log(card.name + ":" + card.ID);
            }
            return card.Clone();
        }
        else if(ID < 300)
        {
            return useCards.FirstOrDefault(card => card.ID == ID)?.Clone();
        }
        else if (ID < 400)
        {
            return enemyCards.FirstOrDefault(card => card.ID == ID)?.Clone();
        }
        else
        {
            return null;
        }
       
    }
    
}
