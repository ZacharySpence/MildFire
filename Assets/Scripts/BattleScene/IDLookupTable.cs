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
     public List<CardSaveData> playerDeck = new List<CardSaveData>(); //random numbers 400+ -> this is the players deck!
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
        if (ID < 200)
        {
            return unitCards.FirstOrDefault(card => card.ID == ID);
        }
        else if (ID < 300)
        {
            return useCards.FirstOrDefault(card => card.ID == ID);
        }
        else if (ID < 400)
        {
            return enemyCards.FirstOrDefault(card => card.ID == ID);
        }

        // IDs 400+ are runtime instances — not stored as templates
        Debug.LogWarning($"No template card found for ID: {ID}");
        return null;

    }
    
}
