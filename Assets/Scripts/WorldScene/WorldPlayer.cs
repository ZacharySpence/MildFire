using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldPlayer : MonoBehaviour
{
    static int nextID = 400; //persistent across scenes ->> need to save this to the nload if loading game
    private void Start()
    {
        CreateNewDeck(100);
    }
    void AddToPlayerDeck(int id)
    {
        Debug.Log(id);
        //1. get card by id
      var card =  IDLookupTable.instance.GetCardByID(id);
        //2. make it the next static ID
        if(card != null)
        {
            Debug.Log(card.name);
        }
        else
        {
            Debug.Log("for some reason we're still returning null?");
        }
        card.ID = nextID;
        nextID++;
        
        IDLookupTable.instance.playerDeck.Add(card);
    }

    void CreateNewDeck(int leaderID)
    {
        //use DeckHolder to find a deck called 'Leader+num' (i.e Leader33) then load that
        var deck = DeckHolder.LoadDeck("Leader", leaderID);
        foreach(var cardID in deck)
        {
            AddToPlayerDeck(cardID);
        }
    }
}
