using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [SerializeField] bool isBusy; //just ot test
    public bool isGameStarted;

    [Header("OnField")]
    public UnitCard[] playerField = new UnitCard[6]; //always 6 items! //actually goes down in COLUMNS!
    public UnitCard[] enemyField= new UnitCard[6]; //always 6 items!
    public List<Transform> fields = new List<Transform>(); //first 6 is player, second 6 is enemy
    
    [Header("EnemySpecifics")]
    [SerializeField] public List<EnemyCards> enemyDeck = new List<EnemyCards>(); //because dont just take x amount, try take whole list, remaining goes to next list
    [SerializeField] int waveTimer, currentWaveTimer;
    bool bossHasSpawned;

    [Header("PlayerSpecifics")]
    public CardBase selectedCard;
    public GameObject cursorOn;
    [SerializeField] int bellTimer, currentBellTimer;
    public bool willStartGame;
   
    [Header("References")]
    [SerializeField]PlayerHand playerHand;
    [SerializeField]UnitCard emptyCard;

    [Header("Hud Visuals")]
    [SerializeField] TextMeshPro waveTimerText, redrawBellText;

    [Header("UI Visuals")]
   public bool discardViewing, deckViewing;
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
        if(level == 1)
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
                else
                {
                    OnDeselect(true);
                }
                return;
            }
            Debug.Log("Lclick");
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
    //Auto-Game
    public IEnumerator EndPlayerTurn() //called by pressing button (refresh bell) OR after playing a card
    {

        isBusy = true; //stop player pressing stuff
        UnitCard[] merged = enemyField.Concat(playerField).ToArray();
       
        //Count down -> go through all cards in fields (enemies first!)
        foreach (UnitCard card in merged)
        {
            if(card.ID == -1) { continue; } //ignore blanks
            if (card.isDead) { Destroy(card); continue; } //if it doesn't work is cause destroy is removing from list too early!
            card.ReduceTimer(); //Do combat one by one
            yield return new WaitForSeconds(.1f);
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
                PlaceEnemyCardOnEmptyField(i, IDLookupTable.instance.GetCardByID(enemiesToAdd.cards[0]) as UnitCard);
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
        return true;
    }
    void Defeat()
    {
        Debug.Log("YOU LOST");
    }
    void Victory()
    {
        Debug.Log("YOU WON!");
    }
    
    //Placement Logic
    void PlaceEnemyCardOnEmptyField(int index, UnitCard cardToPlace)
    {
        UnitCard card = Instantiate(cardToPlace, fields[index+6].transform); //actually have to instantiate the card!
        Debug.Log($"placing {card.name} at index: {index+6}");
        enemyField[index].gameObject.SetActive(false); //hide the empty card (so can just re-enable it later
        enemyField[index] = card; //logically place card
        card.fieldIndex = index+6;

        if (card.isBoss) { bossHasSpawned = true; }
        //card.transform.position = fields[index+6].position; //physically place card
    }
    public void EmptyOutField(int index, bool isPlayerCard)
    {
        Debug.Log($"Index is {index} for {isPlayerCard}");
        fields[index].GetChild(0).gameObject.SetActive(true);
        if (isPlayerCard)
        {
            if (fields[index].GetComponent<UnitCard>().isBoss) { Defeat(); } //so if the unit at that field that dies is the player boss (leader, then defeat)
            playerField[index] = fields[index].GetChild(0).GetComponent<UnitCard>();
        }
        else
        {            
            enemyField[index-6] = fields[index].GetChild(0).GetComponent<UnitCard>();
        }
    }
    public void PlayerPlaceCardOnEmptyField(int index, UnitCard cardToPlace)
    {
        Debug.Log($"placing {cardToPlace.name} at index: {index}");
        if (playerField[index].GetComponent<CardBase>().ID == -1)
        {
            playerField[index].gameObject.SetActive(false); //hide the empty card (so can just re-enable it later
        }

        playerField[index] = cardToPlace; //logically place card
        cardToPlace.fieldIndex = index;
        cardToPlace.transform.position = fields[index].position; //physically place card
    }
    public void PlayerPlaceCardOnFullField(int index, UnitCard cardToPlace)
    {
        Debug.Log($"placing {cardToPlace.name} at index: {index}");
        playerField[index] = cardToPlace; //overwrite and logically place card
        cardToPlace.fieldIndex = index;
        cardToPlace.transform.position = fields[index].position; //physically place card
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
                Debug.Log("try place card on enemy (shouldnt work!)");
                break;
            case "discard":
                selectedCard.TryDiscard();               
                Debug.Log("move card back into discard (only for unitCards");
                break;
            case "emptyCard":
                //Debug.Log("place card in empty spot");
                hasPlayed = selectedCard.TryPlaceOnField(cursorOn.GetComponent<UnitCard>().GetFieldIndex(),true);
                if (hasPlayed && !selectedCard.isAlreadyOnField) 
                {
                    Debug.Log("Placing on field first time");
                    selectedCard.isAlreadyOnField = true;
                }
                else 
                {
                    Debug.Log("Card already on field!");
                    hasPlayed = false;
                    OnDeselect();
                } //if already on field just moving it (so don't end turn!)
                break;
            
        }
        if (hasPlayed && isGameStarted ) 
        {
            
            OnDeselect();
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
                Debug.Log("try use card on player card");
                hasPlayed = selectedCard.TryUse(cursorOn.GetComponent<UnitCard>());
                if (hasPlayed)
                {
                    selectedCard.TryDiscard();//After using it discard it!
                }
                //if already on field just moving it (so don't end turn!)
                break;
            case "enemyCard":
                Debug.Log("try use card on enemy");
                hasPlayed = selectedCard.TryUse(cursorOn.GetComponent<UnitCard>());
                if (hasPlayed)
                {
                    selectedCard.TryDiscard(); //After using it discard it!
                }

                break;
            case "discard":
                selectedCard.TryDiscard();
                Debug.Log("move card back into discard (only for unitCards");
                break;
            case "emptyCard":
                Debug.Log("try using card in empty spot (shouldn't work!");
               
                break;

        }
        if (hasPlayed && isGameStarted)
        {
            if (selectedCard != null)
            {
                OnDeselect();
            }
            
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
                Debug.Log("CARD selected. could be Unit or Use card");
                break;
            case "enemyCard":
                //don't select the card!
                viewCard = cursorOn.GetComponent<UnitCard>();
                viewCard.View();
                Debug.Log("Enemy unit card selected to view");
                break;
            case "bell":
                playerHand.PressBell();
                Debug.Log("playerHand bell");
                break;
            case "waveTimer":
                CallInEnemiesEarly(); //dont forget to make waveTimer disappear once no more enemies to add!
                Debug.Log("waveTimer");
                break;
            case "deck":
                playerHand.ViewDeck();
                Debug.Log("playerHand deck");
                break;
            case "discard":
                playerHand.ViewDiscard();
                Debug.Log("playerHand discard");
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
                        selectedCard.transform.position = fields[selectedCard.fieldIndex].position; //physically place card
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
