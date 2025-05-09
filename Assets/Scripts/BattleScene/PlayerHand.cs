using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class PlayerHand : MonoBehaviour
{
    public static PlayerHand Instance;
    public List<CardBase> hand;
    [SerializeField] Transform physicalHand;
    Queue<CardBase> deck;
    [SerializeField] List<CardBase> cardPool = new List<CardBase>(); //cards should only be in card pool IF been discarded and haven't yet been drawn again
    [SerializeField] List<CardBase> discard = new List<CardBase>();
    [SerializeField] int cardDrawAmount;

    [SerializeField] CardBase baseUnitCard, baseUseCard, baseUseOffCard;
    //Instantiate cards in hand panel in UI
    //cards have tags

    [SerializeField] float spacing = 2.1f;
    [Header("Flags")]
    
    bool setupComplete = false;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }
    private void Start()
    {
        Setup();
    }


    void Setup()
    {
        List<CardBase> tempDeck = new List<CardBase>();
        foreach (var cardData in IDLookupTable.instance.playerDeck)
        {
            
            switch (cardData.cardType)
            {
                case "UnitCard":
                    var card = Instantiate(baseUnitCard,transform);
                    card.SetupUsingCardSaveData(cardData);
                    tempDeck.Add(card);
                    break;
                case "UseCard":
                    card = Instantiate(baseUseCard,transform);
                    card.SetupUsingCardSaveData(cardData);
                    tempDeck.Add(card);
                    break;
                case "UseOffCard":
                    card = Instantiate(baseUseOffCard, transform);
                    card.SetupUsingCardSaveData(cardData);
                    tempDeck.Add(card);
                    break;
            }
           
        }

        //Make starting hand by grabbing all crown cards/the leader or discarding rest of cards
        for (int i = tempDeck.Count - 1; i >= 0; i--)
        {
            var card = tempDeck[i];
            if (card is UnitCard unitCard)
            {
                if (unitCard.hasCrown || unitCard.isBoss)
                {
                    tempDeck.RemoveAt(i); // Safely remove from the list
                }
                else
                {
                    AddToDiscard(card, true);
                }
            }
            else
            {
                AddToDiscard(card, true);
            }
        }
        //add the rest to a deck
        Reshuffle();
        LayoutChildren();
        setupComplete = true;
    }
    void OnHandSizeChanged()
    {
        if (!setupComplete) { return; }
        LayoutChildren();
        if (!BattleManager.Instance.isGameStarted && hand.Count == 0)
        {
            BattleManager.Instance.willStartGame = true;
        }
    }
    //INSTEAD -> I want to make a scriptable of each card (using ID) then on start instantiate each card. that way can make cards easier!
    public void ViewDeck()
    {
        BattleManager.Instance.deckViewing = true;
        //Shuffle and view deck
        Debug.Log("SelectedView Deck");
        GameObject viewPanel = BattleManager.Instance.viewPanel;
        viewPanel.SetActive(true);
        // Deep copy each item 
        List<CardBase> copy = new List<CardBase>();
        foreach (var card in deck)
        {
            copy.Add(card);
        }
        // Shuffle using Fisher-Yates
        for (int i = copy.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = copy[i];
            copy[i] = copy[j];
            copy[j] = temp;
        }

        foreach (var card in copy)
        {
          
            card.gameObject.SetActive(true);
            card.transform.parent = viewPanel.transform;
        }
        viewPanel.GetComponent<LayoutHelper>().LayoutCards();

    }
    public void StopViewingDraw()
    {
        foreach (var card in deck)
        {
            
            card.gameObject.SetActive(false);
            card.transform.parent = null;
        }
    }
    public void ViewDiscard()
    {
        BattleManager.Instance.discardViewing = true;
        //Just view discard (order doesn't matter)
        Debug.Log("SelectedView Discard");
        GameObject viewPanel = BattleManager.Instance.viewPanel;
        viewPanel.SetActive(true);
        foreach(var card in discard)
        {
            
            card.gameObject.SetActive(true);
            card.transform.parent = viewPanel.transform;
        }
        viewPanel.GetComponent<LayoutHelper>().LayoutCards();
    }
    public void StopViewingDiscard()
    {
        foreach (var card in discard)
        {
           
            card.gameObject.SetActive(false);
            card.transform.parent = null;
        }
       
    }

    public void AddToDiscard(CardBase card, bool inHand) //only things that aren't destroyed get to discard
    {
        card.GetComponent<Collider2D>().enabled = true; //re-enable the collider!
        cardPool.Add(card); //add to card pool
        discard.Add(card); //add also to discard
        //For later -> if(isUnitCard) -> healToMax(); (so heal unit cards when placed in discard!
        card.gameObject.SetActive(false); //turn off
        if (inHand)
        {
            RemoveCardFromHand(card);
        }
        
    }
    public void PressBell()
    {
        //throw hand into discard
        foreach(var card in hand)
        {
            AddToDiscard(card, false); //ironically don't want to call it being in hand cause it changes the hand list!
        }
        hand.Clear();
        
        //just draw from deck NO reshuffle (unless empty)
        for (int i = 0; i < cardDrawAmount; i++)
        {
            if(deck.TryDequeue(out CardBase card))
            {
                AddCardToHand(card);
            }
            else
            {
                Reshuffle();
                AddCardToHand(deck.Dequeue());
            }
            
        }
        Debug.Log($"Drew {cardDrawAmount} cards");
        BattleManager.Instance.RestartBellTimer();
        LayoutChildren();

    }
     
    public void RemoveCardFromHand(CardBase card)
    {
        
        hand.Remove(card);
        card.transform.parent = null; //so isn't laid out
        
        OnHandSizeChanged(); 
    }
    void AddCardToHand(CardBase card)
    {
        
        //if card was already made before, then grab from pool (which it should!)
        int possIndex = cardPool.IndexOf(card);
        if (possIndex != -1)
        {
            cardPool[possIndex].gameObject.SetActive(true);
            cardPool.RemoveAt(possIndex);
            //Add logically to hand
            hand.Add(card);
            card.transform.parent = transform;
        }
        //else create card
       else
        {
            Debug.Log("Card not found in card pool!!!");
            /*
            CardBase newCard = Instantiate(card, physicalHand); //will assume each card has it's own ID's (even copies -> when making in world deck add to their ID so it's always different!)
    
            hand.Add(newCard);
            newCard.transform.parent = transform;*/
        }
        
        
        
    }
    public void LayoutChildren() //will need to change this so it's nicely visual
    {
        float currentX = -4f;
        foreach(Transform child in transform)
        {
            if (!child.gameObject.activeSelf)
            {
                continue;
            }
            
            child.localPosition = new Vector3(currentX, 0f, 0f);
            currentX += spacing;

        }

    }

    void Reshuffle()
    {
        //Debug.Log("Put all discard into deck and reshuffle into new deck queue");
        deck = new Queue<CardBase>(discard); //currently just place back on top ADD IN A SHUFFLE!
        discard.Clear();
    }
}
