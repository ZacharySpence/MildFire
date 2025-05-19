using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] List<CardBase> consume = new List<CardBase>();
    [SerializeField] int cardDrawAmount;

    [SerializeField] CardBase baseUnitCard, baseUseCard, baseUseOffCard;
    //Instantiate cards in hand panel in UI
    //cards have tags

    [SerializeField] float spacing = 2.1f;
    public bool hasBadChaos,hasGoodChaos;
    public int chanceForConsume;
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
        
        hasGoodChaos = WorldManager.Instance.omnisciBlessing;
        if (hasGoodChaos)
        {
            hasBadChaos = false;
        }
        else
        {
            hasBadChaos = WorldManager.Instance.omnisciChaos;
            if (WorldManager.Instance.omnisciJudgement && hasGoodChaos)
            {
                chanceForConsume = 100 - chanceForConsume; //flip it round
            }
        }
        List<CardBase> tempDeck = new List<CardBase>();
        bool crystalBuff = WorldManager.Instance.noogleBeefBuff;
        bool poisonDebuff = WorldManager.Instance.noogleBeefDebuff || WorldManager.Instance.noogleCurse;
        foreach (var cardData in IDLookupTable.instance.playerDeck.ToList()) //so take copy so can change the card data!
        {
            
            switch (cardData.cardType)
            {
                case "UnitCard":
                    var card = Instantiate(baseUnitCard,transform);
                    if (crystalBuff)
                    {
                        cardData.crystalOn += 1;
                    }
                    else if (poisonDebuff)
                    {
                        cardData.poisonOn += Random.Range(1, 4); //1-3 poison on start
                    }
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
                    AddToDiscard(card, true); //with chaos chance to consume at start too! (i like it!)
                }
            }
            else
            {
                AddToDiscard(card, true); //with chaos chance to consume at start too! (i like it!)
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
        
        if (hasBadChaos || hasGoodChaos)
        {
            //keep in order if chaotic either way
        }
        else
        {
            // Shuffle using Fisher-Yates
            for (int i = copy.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                var temp = copy[i];
                copy[i] = copy[j];
                copy[j] = temp;
            }
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
    public void ViewConsume()
    {
        BattleManager.Instance.consumeViewing = true;
        GameObject viewPanel = BattleManager.Instance.viewPanel;
        viewPanel.SetActive(true);
        foreach (var card in consume)
        {

            card.gameObject.SetActive(true);
            card.transform.parent = viewPanel.transform;
        }
        viewPanel.GetComponent<LayoutHelper>().LayoutCards();
    }
    public void StopViewingConsume()
    {
        foreach(var card in consume)
        {
            card.gameObject.SetActive(false);
            card.transform.parent = null;
        }
    }

    public void AddToDiscard(CardBase card, bool inHand) //only things that aren't destroyed get to discard
    {
        if (hasBadChaos && Random.Range(1,100) < chanceForConsume)
        {
            //only consume card if within chance and got bad chaos 
            AddToConsume(card, inHand);
            return;
        }
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
    public void AddToConsume(CardBase card, bool inHand)
    {
        //not readded to cardpool so gone forever!
        consume.Add(card);
        card.GetComponent<Collider2D>().enabled = true;
        card.gameObject.SetActive(false);
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
        Shuffle(discard);
        deck = new Queue<CardBase>(discard); //currently just place back on top ADD IN A SHUFFLE!
        discard.Clear();
    }
    // Fisher-Yates Shuffle Algorithm
    public void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;

        // Loop through the list and swap elements
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
