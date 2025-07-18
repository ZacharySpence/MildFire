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
    public static WorldPlayer Instance;
    public static bool gameHasStarted; //persistent across scenes ->> need to save this to then load if loading game
    public static bool startingPanelOpen;
    public Transform startingPanel;
    [SerializeField] GameObject startingButtonPrefab;
    [SerializeField] PlayerBackpack playerBackpack;
   
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        
    }

    
    //gotta change to save my data separate to a prefab!

    void AddToPlayerDeck(int id)
    {
       
       
        var card =  IDLookupTable.instance.GetCardByID(id); //get template of card (reference it)
        
        var saveData = card.CreateCardSaveData();

        //gives them training when adding to deck!
        if (PersistanceManager.nessyTrainedCompanionsID.Contains(id))
        {
            
            saveData.hasBeenTrained = true;
        }

        IDLookupTable.instance.playerDeck.Add(saveData);
    }

    public void CreateNewDeck(int leaderID)
    {
        IDLookupTable.instance.playerDeck.Clear(); //empty it out when making a new deck!
        //use DeckHolder to find a deck called 'Leader+num' (i.e Leader33) then load that
        var deck = DeckHolder.LoadDeck("Leader", leaderID);
        foreach(var cardID in deck)
        {
            AddToPlayerDeck(cardID);
        }
        
        gameHasStarted = true;

        //FOR TESTING:
        //WorldManager.Instance.currentNode.OnCampClick();
        //WorldManager.Instance.currentNode.specifics = "SkullGondola";
        //WorldManager.Instance.currentNode.OnEventClick();
        //startingPanel.gameObject.SetActive(false); //don't need to since going straight into battle!
        WorldManager.Instance.currentNode.specifics = "Tester";
        //FOR ACTUAL:
        startingPanelOpen = false;
        WorldManager.Instance.currentNode.OnClick(); //forcibly start 1st battle!
    }

    
    public void CreateLeaderChoice()
    {
        startingPanelOpen = true;
        List<int> copyList = new List<int>(PersistanceManager.unlockedPlayerGroups);
        int amount = Math.Min(3, copyList.Count);              
        for (int i = 0; i < amount; i++) //so either 3 or the count whichever is smaller
        {
            GameObject button = Instantiate(startingButtonPrefab, startingPanel);
            int randomId = copyList[Random.Range(0, copyList.Count)];
            copyList.Remove(randomId);
           
            UnitCard card = Instantiate(IDLookupTable.instance.GetCardByID(randomId), button.transform) as UnitCard; //actually have to instantiate the card!
            
            
            card.SetupUsingCardSaveData(card.CreateCardSaveData()); //set it up using its own data
            card.transform.localScale = new Vector2(200f, 300f);
            card.transform.position = Vector2.zero;
           
            card.transform.parent.GetComponent<Button>().onClick.AddListener(() => CreateNewDeck(card.ID));
        }
    }

    
    public void CreateRewardChoice()
    {
        startingPanelOpen = true;
        Debug.Log("reserves");
        CreateReward(DeckHolder.reserveRewardCards);
        Debug.Log("items");
        CreateReward(DeckHolder.itemRewardCards);
        Debug.Log("companions");
        CreateReward(DeckHolder.companionRewardCards,WorldManager.Instance.nessyCurse);
        if (WorldManager.Instance.nessyBlessing)
        {
            CreateReward(DeckHolder.companionRewardCards);
        }
    }
    void CreateReward(List<int> rewardList, bool nessyCurse = false)
    {

        int reward = rewardList[Random.Range(0, rewardList.Count)];
        GameObject button = Instantiate(startingButtonPrefab, startingPanel);
        CardBase card = Instantiate(IDLookupTable.instance.GetCardByID(reward), button.transform);
        card.SetupUsingCardSaveData(card.CreateCardSaveData()); //set it up using its own data
        card.transform.localScale = new Vector2(200f, 300f);
        card.transform.position = Vector2.zero;

        if (nessyCurse)
        {
            if (Random.Range(1, 101) > 50)
            {
                card.transform.parent.GetComponent<Button>().onClick.AddListener(() => NessySteal(card.gameObject));
            }
        }
        else
        {
            card.transform.parent.GetComponent<Button>().onClick.AddListener(() => ChooseReward(card.ID));
        }
    }
    void ChooseReward(int id)
    {
        AddToPlayerDeck(id);
        startingPanel.gameObject.SetActive(false);
        startingPanelOpen = false;
        //+enable next options on board!
        WorldManager.Instance.UpdateNodes();
    }

    void NessySteal(GameObject card)
    {
        Debug.Log("STOLEN BY NESSY");
        Destroy(card);
        //play sounds
        startingPanel.gameObject.SetActive(false);
        WorldManager.Instance.UpdateNodes();
    }


}
