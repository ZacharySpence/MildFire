using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [SerializeField] bool isBusy; //just ot test
    public bool isGameStarted, cardFullyFinished;

    [Header("OnField")]
    public UnitCard[] playerField = new UnitCard[6]; //always 6 items! //actually goes down in COLUMNS!
    public UnitCard[] enemyField= new UnitCard[6]; //always 6 items!
    public List<Transform> fields = new List<Transform>(); //first 6 is player, second 6 is enemy
    
    [Header("EnemySpecifics")]
    [SerializeField] public List<EnemyCards> enemyDeck = new List<EnemyCards>(); //because dont just take x amount, try take whole list, remaining goes to next list
    [SerializeField] int waveTimer, currentWaveTimer;
    bool bossHasSpawned;
    bool hasOmnisciForesight;

    [Header("PlayerSpecifics")]
    public CardBase selectedCard;
    public GameObject cursorOn;
    [SerializeField] int bellTimer, currentBellTimer;
    public bool willStartGame;

    [Header("BattleSpecifics")]
    public int fireLevel = 0;
    int roundNumber = 1;
    int currentBossID;
    bool skullFight;
    int skullIDInFight;
   
    [Header("References")]
    [SerializeField]PlayerHand playerHand;
    [SerializeField]UnitCard emptyCard;
    [SerializeField] GameObject thanksForPlayingPanel;

    [Header("Hud Visuals")]
    [SerializeField] TextMeshPro waveTimerText, redrawBellText;

    [Header("UI Visuals")]
   public bool discardViewing, deckViewing,consumeViewing;
    public GameObject viewPanel;
    public UnitCard viewCard, uiViewCard;

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

    private void OnLevelWasLoaded(int level)
    {
        if(level == 2)
        {
           
            Setup();

        }
        else
        {
            isBusy = true;
        }
        
    }
    private void Update()
    {
        if (isBusy)
        {
            return;
        }
        if(selectedCard != null)
        {
            SelectedCardFollowCursor();
        }
      
        //on left click select, or use, on right click deselect
        if (Input.GetMouseButtonDown(0))
        {
            //if viewing and left click then go back
            if (viewPanel.activeSelf)
            {
                viewPanel.SetActive(false);
                if (deckViewing)
                {
                    deckViewing = false;
                    playerHand.StopViewingDraw();
                }
                else if (discardViewing)
                {
                    discardViewing = false;
                    playerHand.StopViewingDiscard();
                }
                else if (consumeViewing)
                {
                    consumeViewing = false;
                    playerHand.StopViewingConsume();    
                }
                else
                {
                    OnDeselect(true);
                }
                return;
            }
          
            // Get the mouse position in screen space
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Cast a ray at the mouse position
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);  // Vector2.zero means no direction, just a point

            // Check if the ray hit something
            if (hit.collider != null)
            {
                cursorOn = hit.collider.gameObject;
                if (selectedCard != null)
                {
                    if (selectedCard.TryGetComponent<UnitCard>(out var unitCard))
                    {
                        UseUnitSelectedCard();
                    }
                    else
                    {
                        UseUseSelectedCard();
                    }
                }
                else
                {
                    DoSomethingBasedOnWhatClicked();
                }
            }
              
        }
        else if (Input.GetMouseButtonDown(1) && selectedCard != null)
        {
            OnDeselect(true);
        }
       
    }
   
    void Setup()
    {
      
        //fill up enemy and player field with 'empty' card prefab
        for (int i = 0; i < playerField.Length; i++)
        {
            UnitCard emptyP = Instantiate(emptyCard, fields[i]);
            emptyP.fieldIndex = i;
            playerField[i] = emptyP;
            UnitCard emptyE = Instantiate(emptyCard, fields[i + 6]);
            emptyE.fieldIndex = i + 6;
            enemyField[i] = emptyE;
        }
        enemyDeck = DeckHolder.enemyDeck;
        AddInNewEnemies();
        isBusy = false;

    }
    void SelectedCardFollowCursor()
    {
        // Get the mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Set the z-position to 0 to keep it on the 2D plane
        mousePosition.z = 0f;

        // Move the GameObject to the mouse position
        selectedCard.transform.position = mousePosition;
    }


    public void StartGame()
    {
        Debug.Log("starting game!");
        //Called automatically when playerHand is empty at start!
        playerHand.PressBell();
        waveTimerText.transform.parent.gameObject.SetActive(true);
        redrawBellText.transform.parent.gameObject.SetActive(true);
        isGameStarted = true;
        
       
        
    }
    public IEnumerator WaitForConditionWithTimeout()
    {
        float startTime = Time.time;  // Track the start time

        // Wait until 'isReady' becomes true or timeout occurs
        while (!cardFullyFinished)
        {
            // Check if the timeout has passed
            if (Time.time - startTime > 5f)
            {
                Debug.Log("Timeout reached. Condition not met.");
                yield break;  // Stop the coroutine if timeout is reached
            }

            yield return null;  // Wait for the next frame
        }
       // yield return new WaitForSeconds(0.5f); //to give some time BEFORE next animation!
        cardFullyFinished = false; //reset it for next time!

        // Continue once 'isReady' is true
        Debug.Log("Condition is met, continuing...");
    }

    //Auto-Game
    public IEnumerator EndPlayerTurn() //called by pressing button (refresh bell) OR after playing a card
    {
        yield return StartCoroutine(WaitForConditionWithTimeout()); //wait for current card to finish doing its thing 
            //-> maybe instead make an ActionQueue and then wait until action queue is empty?
        
        
        UnitCard[] merged = enemyField.Concat(playerField).ToArray();
       
        //Count down -> go through all cards in fields (enemies first!)
        foreach (UnitCard card in merged)
        {
            if(card.ID == -1) { continue; } //ignore blanks
            if (card.isDead) { Destroy(card); continue; } //if it doesn't work i'ts because destroy is removing from list too early!
            if (roundNumber % 3 == 0) //every 3 rounds, add 1 fire to all cards on field
            {
              
                card.ChangeStatus(fireAdded: 1); //so actually taking them out of combat means safe from fire?
            }
            if(card.maxAtkTimer == 0) //doesn't have an attack timer (so smackback/reaction only)
            {
                continue;
            }
            yield return StartCoroutine(card.ReduceTimer()); //Do combat one by one
            
        }

        //do bell timer /new enemies arriving
        if(enemyDeck.Count > 0)
        {
            ReduceWaveTimer();
        }
        if (bossHasSpawned)
        {
            if (CheckVictory())
            {
                Victory();
            }
        }
        roundNumber++;
        
        StartNewTurn();
    }

    void StartNewTurn()
    {
        isBusy = false; //allow 'cursor' movement
        ReduceBellTimer();
        
    }
    public void RestartBellTimer()
    {
        if(currentBellTimer > 0 && isGameStarted)
        {
            isBusy = true;
            cardFullyFinished = true; //so no waiting!
            StartCoroutine(EndPlayerTurn());
        }
        currentBellTimer = bellTimer;
        redrawBellText.text = $"{currentBellTimer}";
    }
    void ReduceBellTimer()
    {
        currentBellTimer--;
        if (currentBellTimer <= 0)
        {
            redrawBellText.text = "~"; //to show it's ready!
        }
        else
        {
            redrawBellText.text = $"{currentBellTimer}";  
        }
       
        
    }
    void ReduceWaveTimer()
    {
        currentWaveTimer--;
        if (currentWaveTimer <= 0)
        {
            //Add in new enemies
            AddInNewEnemies();
            currentWaveTimer = waveTimer;
            
        }
        waveTimerText.text = $"{currentWaveTimer}";
    }
    void CallInEnemiesEarly()
    {
        AddInNewEnemies();
        currentWaveTimer = waveTimer;
        waveTimerText.text = $"{currentWaveTimer}";
        

    }
    void AddInNewEnemies()
    {
        if (enemyDeck.Count <= 0)
        {
            Debug.Log("No enemies Left");
            waveTimerText.transform.parent.gameObject.SetActive(false); //get rid of wave timer and don't add in any enemies
            return;
        }
        //pop the list
        var enemiesToAdd = enemyDeck[0];
        enemyDeck.RemoveAt(0); 

        //add in new enemies (in rows? top row then btm row where can unless has given spot)
        for (int i = 0; i < 6; i++)
        {
            if (enemiesToAdd.cards.Count == 0) { return; }
            if (enemyField[i].ID == -1 ) //so if it's empty
            {
                var card = IDLookupTable.instance.GetCardByID(enemiesToAdd.cards[0]) as UnitCard;
               
                
                if (card.isBoss)
                {
                    foreach(var skull in PersistanceManager.skullsOnID)
                    {
                        //ALSO check if persistantSkullsID holds id+100 if so then add that skull (to make it skull battle!)
                        if (skull.Item2+100 == card.ID)
                        {
                            var saveData = card.CreateCardSaveData();
                            saveData = IDLookupTable.instance.skullCharmList.Find(x => x.ID == skull.Item1).ChangeCard(saveData);
                            card.SetupUsingCardSaveData(saveData);
                            skullIDInFight = skull.Item1;
                            skullFight = true;
                        }
                    }
                   
                    currentBossID = card.ID; //store the boss ID
                }
                PlaceEnemyCardOnEmptyField(i, card);
                enemiesToAdd.cards.RemoveAt(0); //pop the card
               
            }
        }

        
        //if there's still another deck, add remaining cards to front of next deck
        if(enemyDeck.Count > 0)
        {
            enemiesToAdd.cards.AddRange(enemyDeck[0].cards); //any remaining add to 0th element of enemyDeck (at front so first taken)
            enemyDeck[0].cards = enemiesToAdd.cards;
        }
        else
        {
            //else nothing left in deck so just need to add whats remaining
            enemyDeck.Add(enemiesToAdd);
        }

        if (enemyDeck.Count <= 0)
        {
            waveTimerText.transform.parent.gameObject.SetActive(false); //get rid of wave timer and don't add in any enemies (do AGAIN to remove afterwards)
            return;
        }




    }

    public void AddInNewSpawns(List<int> spawnsToAdd, bool isPlayer)
    {      
        //add in new enemies (in rows? top row then btm row where can unless has given spot)
        for (int i = 0; i < 6; i++)
        {
            if (spawnsToAdd.Count == 0) { return; }
            if (isPlayer) //so if it's empty
            {
               if(playerField[i].ID == -1)
                {
                    PlayerPlaceCardOnEmptyField(i, IDLookupTable.instance.GetCardByID(spawnsToAdd[0]) as UnitCard);
                    spawnsToAdd.RemoveAt(0); //pop the card
                }              
            }
            else
            {
                if (enemyField[i].ID == -1)
                {
                    PlaceEnemyCardOnEmptyField(i, IDLookupTable.instance.GetCardByID(spawnsToAdd[0]) as UnitCard);
                    spawnsToAdd.RemoveAt(0); //pop the card
                }               
            }
        }
    }
    bool CheckVictory()
    {
        //check if there is at least 1 card left that is a boss!
        foreach (var card in enemyField)
        {
            if (card.isBoss)
            {
                return false;
            }
        }
        PersistanceManager.unlockedPlayerGroups.Add(currentBossID - 100); //player equiv is -100
        PersistanceManager.hasClicked = false; //so don't go back into battle upon load
        PersistanceManager.SavePersistence();
        if (skullFight)
        {
            PersistanceManager.skullFightsWon.Add(skullIDInFight);
            //also then remove skullID from the persistance skulls used so card doesn't get skull next game? (IMPLEMENT)
        }
        return true;
    }
    void Defeat()
    {
        Debug.Log("YOU LOST");
        //do global flag resets!?
        WorldPlayer.gameHasStarted = false;
        thanksForPlayingPanel.SetActive(true);
        string path = Path.Combine(Application.persistentDataPath, "GameSaveFile.json");
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save file deleted.");
        }

    }
    void Victory()
    {
        Debug.Log("YOU WON!");
        
        SceneManager.LoadScene(1);//back to travel scene!
        
    }
    
    //Placement Logic
    void PlaceEnemyCardOnEmptyField(int index, UnitCard cardToPlace)
    {
        UnitCard card = Instantiate(cardToPlace, fields[index+6].transform); //actually have to instantiate the card!
        card.SetupUsingCardSaveData(card.CreateCardSaveData()); //set it up using its own data
      //  Debug.Log($"placing {card.name} at index: {index+6}");
        enemyField[index].gameObject.SetActive(false); //hide the empty card (so can just re-enable it later
        enemyField[index] = card; //logically place card
        card.fieldIndex = index+6;

        if (card.isBoss) { bossHasSpawned = true; }
        //card.transform.position = fields[index+6].position; //physically place card
    }
    public void EmptyOutField(int index, bool isPlayerCard, bool justReset = false)
    {
      //  Debug.Log($"Index is {index} for {isPlayerCard}");
        fields[index].GetChild(0).gameObject.SetActive(true);
        
        if (isPlayerCard)
        {
            if (playerField[index].isBoss && !justReset) { Defeat(); return; } //so if the unit at that field that dies is the player boss (leader, then defeat)
            playerField[index] = fields[index].GetChild(0).GetComponent<UnitCard>();
        }
        else
        {            
            enemyField[index-6] = fields[index].GetChild(0).GetComponent<UnitCard>();
        }
    }
    public void PlayerPlaceCardOnEmptyField(int index, UnitCard cardToPlace, bool endCase = true)
    {


         Debug.Log($"placing {cardToPlace.name} at index: {index}");
        if (playerField[index].GetComponent<CardBase>().ID == -1)
        {
            playerField[index].gameObject.SetActive(false); //hide the empty card (so can just re-enable it later
        }

        playerField[index] = cardToPlace; //logically place card
        cardToPlace.fieldIndex = index;
        cardToPlace.transform.position = fields[index].position; //physically place card
        //only automove if this is the last card to place
        if (endCase)
        {
            AutoMovePlayerCardsAfterPlacing(index % 2); //always will be the last card to place 
        }
    }
       
    
    public void PlayerPlaceCardOnFullField(int index, UnitCard cardToPlace, bool endCase = true)
    {
        Debug.Log($"placing {cardToPlace.name} at index: {index}");
        playerField[index] = cardToPlace; //overwrite and logically place card
        cardToPlace.fieldIndex = index;
        cardToPlace.transform.position = fields[index].position; //physically place card
        
    }
        
    public void MoveCard(int newIndex, bool isPlayer, UnitCard cardToMove)
    {
        if (isPlayer)
        {
            int index = cardToMove.fieldIndex;
            fields[index].GetChild(0).gameObject.SetActive(true);
            playerField[index] = fields[index].GetChild(0).GetComponent<UnitCard>();
            playerField[newIndex] = cardToMove;
            cardToMove.fieldIndex = newIndex;
            cardToMove.transform.position = fields[newIndex].position;

        }
        else
        {
            int index = cardToMove.fieldIndex;
            Debug.Log("EnemyFieldIndex:" + (cardToMove.fieldIndex - 6));
            fields[index].GetChild(0).gameObject.SetActive(true);
            enemyField[index - 6] = fields[index].GetChild(0).GetComponent<UnitCard>();
            enemyField[newIndex] = cardToMove;
            cardToMove.fieldIndex = newIndex + 6;
            cardToMove.transform.position = fields[newIndex + 6].position;

        }

    }

    public void AutoMovePlayerCardsAfterPlacing(int row)
    {
        //After placing card ->
        //1. move each card to rightmost spots
        for (int i = row+2; i < 6; i +=2)
        {
            //if field has card in it AND field right of it is empty, then move it to said field
            if (playerField[i].ID != -1 && playerField[i-2].ID == -1)
            {
                MoveCard(i - 2, true, playerField[i]);
            }
        }
      
    }
    //Card/Click actions




    void SelectCard(CardBase cardToSelect)
    {
        selectedCard = cardToSelect;
        int index = selectedCard.fieldIndex; //Haven't reset this so if i deselect then place at it's fieldIndex!
        if (index != -1) //so grabbing off field
        {  
            
            playerField[index] = fields[index].GetChild(0).GetComponent<UnitCard>();//get the empty child card
            playerField[index].gameObject.SetActive(true); //set it active;
            playerField[index].fieldIndex = index; //logically set it as the empty card
            AutoMovePlayerCardsAfterPlacing(index % 2);
        }
        selectedCard.GetComponent<Collider2D>().enabled = false; //disable it so cursor no longer hits it
        //lock to cursor
    }

    public void UseUnitSelectedCard()
    {
        //selected card is a unit card

        //check what cursor is currently selected then do something based on it
        bool hasPlayed = false;
        switch (cursorOn.tag)
        {
            case "playerCard":
                //Debug.Log("place this card here and move previous card before IF IT CAN (if not chuck it off left edge which destroys card");
                hasPlayed = selectedCard.TryPlaceOnField(cursorOn.GetComponent<UnitCard>().GetFieldIndex(),true,cursorOn.GetComponent<UnitCard>()); 
                if(hasPlayed && !selectedCard.isAlreadyOnField)
                {
                    
                    selectedCard.isAlreadyOnField = true;
                }
                else 
                { 
                    hasPlayed = false;
                    OnDeselect();
                } //if already on field just moving it (so don't end turn!)
                break;
            case "enemyCard":                    
                //Debug.Log("try place card on enemy (shouldnt work!)");
                break;
            case "discard":
                if(selectedCard.TryGetComponent<UnitCard>(out var unitCard))
                {
                    selectedCard.TryDiscard();
                }                 
                //Debug.Log("move card back into discard (only for unitCards");
                break;
            case "emptyCard":
                //Debug.Log("place card in empty spot");
                hasPlayed = selectedCard.TryPlaceOnField(cursorOn.GetComponent<UnitCard>().GetFieldIndex(),true);
                if (hasPlayed && !selectedCard.isAlreadyOnField) 
                {
                   // Debug.Log("Placing on field first time");
                    selectedCard.isAlreadyOnField = true;
                }
                else 
                {
                    //Debug.Log("Card already on field!");
                    hasPlayed = false;
                    OnDeselect();
                } //if already on field just moving it (so don't end turn!)
                break;
            
        }
        if (hasPlayed && isGameStarted ) 
        {
            
            OnDeselect();
            isBusy = true; //stop player pressing stuff
            StartCoroutine(EndPlayerTurn());
        }
        else if (!isGameStarted && willStartGame)
        {
            OnDeselect();
            StartGame();
        }
    }
    public void UseUseSelectedCard()
    {
        //check what cursor is currently selected then do something based on it
        bool hasPlayed = false;
        switch (cursorOn.tag)
        {
            case "playerCard":
                //Debug.Log("try use card on player card");
                hasPlayed = selectedCard.TryUse(cursorOn.GetComponent<UnitCard>());
                if (hasPlayed)
                {
                    selectedCard.TryDiscard();//After using it discard it!
                }
                //if already on field just moving it (so don't end turn!)
                break;
            case "enemyCard":
                //Debug.Log("try use card on enemy");
                hasPlayed = selectedCard.TryUse(cursorOn.GetComponent<UnitCard>());
                if (hasPlayed)
                {
                    selectedCard.TryDiscard(); //After using it discard it!
                }

                break;
            case "discard":
               // selectedCard.TryDiscard();
               Debug.Log("can't move use card back into discard (only for unitCards)");
                break;
            case "emptyCard":
                //Debug.Log("try using card in empty spot (shouldn't work!");
               
                break;
            default:
                Debug.Log("Trying to use card on something that cant be used on!");
                break;

        }
        if (hasPlayed && isGameStarted)
        {
            if (selectedCard != null)
            {
                OnDeselect();
            }
            isBusy = true; //stop player pressing stuff
            StartCoroutine(EndPlayerTurn());
        }
    }
    public void DoSomethingBasedOnWhatClicked()
    {
        //check what cursor is currently selected then do something based on it
        switch (cursorOn.tag)
        {
            case "playerCard":
               SelectCard(cursorOn.GetComponent<CardBase>());
               // Debug.Log("CARD selected. could be Unit or Use card");
                break;
            case "enemyCard":
                //don't select the card!
                viewCard = cursorOn.GetComponent<UnitCard>();
                viewCard.SelectedView();
               // Debug.Log("Enemy unit card selected to view");
                break;
            case "bell":
                playerHand.PressBell();
               // Debug.Log("playerHand bell");
                break;
            case "waveTimer":
                CallInEnemiesEarly(); //dont forget to make waveTimer disappear once no more enemies to add!
              //  Debug.Log("waveTimer");
                break;
            case "deck":
                playerHand.ViewDeck();
               // Debug.Log("playerHand deck");
                break;
            case "discard":
                playerHand.ViewDiscard();
               // Debug.Log("playerHand discard");
                break;
            case "consume":
                playerHand.ViewConsume();
                break;
            
        }

    }

    //when deselecting the card (so can only do this when have a card selected!)
    public void OnDeselect(bool returnToPosition = false)
    {
       
        if(viewCard != null)
        {
            selectedCard = viewCard;
            viewCard = null;
        }
        selectedCard.GetComponent<Collider2D>().enabled =true;
        if (returnToPosition)
        {
            switch (selectedCard.tag)
            {
                case "playerCard":
                    if(selectedCard.fieldIndex != -1)
                    {
                        Debug.Log("Place back on field");
                        //if card has automoved onto original spot then gotta do the whole placement
                        if (playerField[selectedCard.fieldIndex].ID != -1)
                        {
                            selectedCard.TryPlaceOnField(selectedCard.fieldIndex, true, playerField[selectedCard.fieldIndex]);
                        }
                        //else just place back onto original spot (since kept fieldIndex!
                        else
                        {
                            selectedCard.transform.position = fields[selectedCard.fieldIndex].position; 
                        }
                      
                        //PlayerPlaceCardOnEmptyField(selectedCard.fieldIndex, selectedCard as UnitCard);
                    }
                    else
                    {
                        Debug.Log("put card back in hand/position");
                        playerHand.LayoutChildren(); //good enough for now!
                    }
                   
                    break;
                case "enemyCard":
                    Debug.Log("de-view enemy card");
                    selectedCard.transform.parent = null;
                    selectedCard.transform.position = fields[selectedCard.fieldIndex].position; //physically place card
                    Destroy(uiViewCard.gameObject); //feels ineffective 
                    break;
            }
        }
        
        selectedCard = null;
    }

}
[Serializable]
public class EnemyCards
{
    public List<int> cards;
    
}
