using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class CardBase : MonoBehaviour
{
    private bool _isAlreadyOnField;
    public int fieldIndex;
    public int ID;
    
    public bool isAlreadyOnField
    {
        get { return _isAlreadyOnField; }
        set { _isAlreadyOnField = value; PlayerHand.Instance.RemoveCardFromHand(this); }
    } //when setting on field first time logically remove from hand!

    public bool isStartCard;
    [SerializeField] protected string presetTag;
    [Header("Card Details")]
    [SerializeField] string nameText;
    [SerializeField] Sprite picture,background,border;
    [SerializeField] TextMeshProUGUI cardName,cardDescription;
    [SerializeField] Image cardPicture, cardBackground, cardBorder;

    [Header("Stats")]
    //change numOfAttacks into a 'status effect'
    [SerializeField] protected int numOfAttacksGive, attackGive, healthGive, timerGive;
    [SerializeField] protected int shieldGive, snowGive, fireGive, crystalGive, poisonGive, pepperGive, curseGive, reflectGive, hazeGive, bombGive, inkGive, demonizeGive;
    [SerializeField] protected bool hasBarrage, hasBuffAttack, hasSmackback; //special ability flags

    
    protected virtual void Awake()
    {
        cardPicture.sprite = picture;
        cardBackground.sprite = background;
        cardBorder.sprite = border;
        cardName.text = nameText;

        cardDescription.text = CreateCardDescription();
    }
    public virtual string CreateCardDescription()
    {
        return "not implemented yet"; //somehow have to figure out how to create 'emojis' of the sprites
    }
    public virtual bool TryUse(UnitCard unitToUseOn)
    {
        return false; //so Can't use it
    }

    public virtual bool TryPlaceOnField(int atIndex, bool isPlayerCard = false, UnitCard cardAlreadyThere = null) 
    {
        return false;
    }
    public virtual bool TryDiscard()
    {
        return false;
    }

    public virtual void View()
    {
        
    }
}
