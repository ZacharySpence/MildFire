using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WorldPlayer : MonoBehaviour
{
    static bool gameHasStarted; //persistent across scenes ->> need to save this to then load if loading game
    [SerializeField] Transform startingPanel;
    [SerializeField] GameObject startingButtonPrefab;
    [SerializeField] List<int> allLeaderID = new List<int>();
    private void Start()
    {
        if (gameHasStarted)
        {
            CreateRewardChoice();
        }
        else
        {
            CreateLeaderChoice();
        }
        
        
    }
    void AddToPlayerDeck(int id)
    {
       
       
        var card =  IDLookupTable.instance.GetCardByID(id); //get template of card (reference it)
        Debug.Log(card.name);
        var saveData = card.CreateCardSaveData();

        IDLookupTable.instance.playerDeck.Add(saveData);
    }

    public void CreateNewDeck(int leaderID)
    {
        //use DeckHolder to find a deck called 'Leader+num' (i.e Leader33) then load that
        var deck = DeckHolder.LoadDeck("Leader", leaderID);
        foreach(var cardID in deck)
        {
            AddToPlayerDeck(cardID);
        }
        startingPanel.gameObject.SetActive(false);
        gameHasStarted = true;
    }

    
    void CreateLeaderChoice()
    {
        List<int> copyList = new List<int>(allLeaderID);
        int amount = Math.Min(3, copyList.Count);              
        for (int i = 0; i < amount; i++) //so either 3 or the count whichever is smaller
        {
            GameObject button = Instantiate(startingButtonPrefab, startingPanel);
            int randomId = copyList[Random.Range(0, copyList.Count)];
            copyList.Remove(randomId);
            Debug.Log(randomId);
            UnitCard card = Instantiate(IDLookupTable.instance.GetCardByID(randomId), button.transform) as UnitCard; //actually have to instantiate the card!
            
            
            card.SetupUsingCardSaveData(card.CreateCardSaveData()); //set it up using its own data
            card.transform.localScale = new Vector2(200f, 300f);
            card.transform.position = Vector2.zero;
           
            card.transform.parent.GetComponent<Button>().onClick.AddListener(() => CreateNewDeck(card.ID));
        }
    }

    void CreateRewardChoice()
    {
        CreateReward(DeckHolder.reserveRewardCards);
        CreateReward(DeckHolder.itemRewardCards);
        CreateReward(DeckHolder.companionRewardCards);  
    }
    void CreateReward(List<int> rewardList)
    {
        int reward = rewardList[Random.Range(0, rewardList.Count)];
        GameObject button = Instantiate(startingButtonPrefab, startingPanel);
        CardBase card = Instantiate(IDLookupTable.instance.GetCardByID(reward), button.transform);
        card.SetupUsingCardSaveData(card.CreateCardSaveData()); //set it up using its own data
        card.transform.localScale = new Vector2(200f, 300f);
        card.transform.position = Vector2.zero;
        card.transform.parent.GetComponent<Button>().onClick.AddListener(() => ChooseReward(card.ID));
    }

    void ChooseReward(int id)
    {
        AddToPlayerDeck(id);
        startingPanel.gameObject.SetActive(false);
        //+enable next options on board!
    }
}
