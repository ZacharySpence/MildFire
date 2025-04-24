using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class CardBase : MonoBehaviour
{
    private bool _isAlreadyOnField;
    public int fieldIndex;
    
    
    public bool isAlreadyOnField
    {
        get { return _isAlreadyOnField; }
        set { _isAlreadyOnField = value; PlayerHand.Instance.RemoveCardFromHand(this); }
    } //when setting on field first time logically remove from hand!

    public int ID;
    [SerializeField] protected string presetTag;
    [Header("Card Details")]
    [SerializeField] protected string nameText;
    [SerializeField] protected Sprite picture,background,border;
    [SerializeField] TextMeshProUGUI cardName,cardDescription;
    [SerializeField] Image cardPicture, cardBackground, cardBorder;

    [Header("Stats")]
    //change numOfAttacks into a 'status effect'
    [SerializeField] protected int numOfAttacksGive, attackGive, healthGive, timerGive;
    [SerializeField] protected int shieldGive, snowGive, fireGive, crystalGive, poisonGive, pepperGive, curseGive, reflectGive, 
        hazeGive, bombGive, inkGive, demonizeGive;
    [SerializeField] protected bool hasBarrage, hasBuffAttack, hasSmackback; //special ability flags

    public abstract CardSaveData CreateCardSaveData();

    public virtual void SetupUsingCardSaveData(CardSaveData cardSaveData)
    {
        this.ID = cardSaveData.baseID;  // Set base ID -> or make it runtimeID to make it unique?
        this.fieldIndex = cardSaveData.fieldIndex;
        this.presetTag = cardSaveData.presetTag;
        this.nameText = cardSaveData.nameText;
        this.picture = cardSaveData.picture;
        this.border = cardSaveData.border;
        this.background = cardSaveData.background;
       
    }
    protected virtual void Awake()
    {
        if(ID == -1) { return; }
      
        
    }
    [SerializeField] List<string> text = new List<string>();
    public virtual void CreateCardDescription()
    {
        Debug.Log("CREATING CARD DESCRIPTION FOR"+ gameObject.name);
        //Got to make the description in the first place!
        if (shieldGive > 0){text.Add($"Apply {shieldGive} shield");}
        if (snowGive > 0) { text.Add($"Apply {snowGive} snow"); }
        if (fireGive > 0) { text.Add($"Apply {fireGive} fire"); }
        if (crystalGive > 0) { text.Add($"Apply {crystalGive} crystal"); }
        if (poisonGive > 0) { text.Add($"Apply {poisonGive} poison"); }
        if (pepperGive > 0) { text.Add($"Apply {pepperGive} pepper"); }
        if (curseGive > 0) { text.Add($"Apply {curseGive} curse"); }
        if (reflectGive > 0) { text.Add($"Apply {reflectGive} reflect"); }
        if (hazeGive > 0) { text.Add($"Apply {hazeGive} haze"); }
        if (bombGive > 0) { text.Add($"Apply {bombGive} bomb"); }
        if (inkGive > 0) { text.Add($"Apply {inkGive} ink"); }
        if (demonizeGive > 0) { text.Add($"Apply {demonizeGive} demonize"); }
        if (numOfAttacksGive > 0) { text.Add($"Get frenzy {numOfAttacksGive}"); }
        if (attackGive > 0) { text.Add($"Increase attack by {attackGive}"); }
        if (healthGive > 0) {text.Add($"Heal {healthGive} health"); }
        if (timerGive < 0) { text.Add($"Reduce timer by {timerGive}"); }
        cardDescription.text = string.Join(" ", text);
        var textList = cardDescription.text.Split(" ").ToList();
        cardDescription.text = cardDescription.GetComponent<DescriptionCreator>().CreateDescription(textList);
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
