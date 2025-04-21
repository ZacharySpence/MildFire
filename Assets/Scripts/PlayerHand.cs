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
    List<CardBase> cardPool = new List<CardBase>(); //cards should only be in card pool IF been discarded and haven't yet been drawn again
    [SerializeField] List<CardBase> discard = new List<CardBase>();
    [SerializeField] int cardDrawAmount;
    //Instantiate cards in hand panel in UI
    //cards have tags

    [SerializeField] float spacing = 2.1f;
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
        deck =new Queue<CardBase>( GameManager.Instance.playerDeck);
        LayoutChildren();
    }

    //INSTEAD -> I want to make a scriptable of each card (using ID) then on start instantiate each card. that way can make cards easier!
    public void ViewDeck()
    {
        //Shuffle and view deck
        Debug.Log("View Deck");
    }
    public void ViewDiscard()
    {
        //Just view discard (order doesn't matter)
        Debug.Log("View Discard");
    }

    public void AddToDiscard(CardBase card, bool inHand) //only things that aren't destroyed get to discard
    {
      
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
        GameManager.Instance.RestartBellTimer();
        LayoutChildren();

    }
     
    public void RemoveCardFromHand(CardBase card)
    {
       
        hand.Remove(card);
        card.transform.parent = null; //so isn't laid out
        LayoutChildren();
    }
    void AddCardToHand(CardBase card)
    {
        
        //if card was already made before, then grab from pool
        int possIndex = cardPool.IndexOf(card);
        if (possIndex != -1)
        {
            cardPool[possIndex].gameObject.SetActive(true);
            cardPool.RemoveAt(possIndex);
            //Add logically to hand
            hand.Add(card);

        }
        //else create card
        else
        {
            CardBase newCard = Instantiate(card, physicalHand); //will assume each card has it's own ID's (even copies -> when making in world deck add to their ID so it's always different!)
    
            hand.Add(newCard);
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
        Debug.Log("Put all discard into deck and reshuffle into new deck queue");
        deck = new Queue<CardBase>(discard); //curently just place back on top ADD IN A SHUFFLE!
        discard.Clear();
    }
}
