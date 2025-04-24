using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldPlayer : MonoBehaviour
{
    static int nextID = 400; //persistent across scenes ->> need to save this to the nload if loading game
    private void Start()
    {
        CreateNewDeck(100);
        
    }
    void AddToPlayerDeck(int id)
    {
       // Debug.Log(id);
       
      var card =  IDLookupTable.instance.GetCardByID(id); //get template of card (reference it)

        var saveData = card.CreateCardSaveData();

        IDLookupTable.instance.playerDeck.Add(saveData);
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
