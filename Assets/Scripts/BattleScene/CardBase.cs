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
    [SerializeField] protected TextMeshProUGUI cardName,cardDescription;
    [SerializeField] protected Image cardPicture, cardBackground, cardBorder;

    [Header("Stats")]
    //change numOfAttacks into a 'status effect'
    [SerializeField] protected int numOfAttacksGive;
    [SerializeField] protected int attackGive, healthGive, timerGive;
    [SerializeField] public int shieldGive, snowGive, fireGive, crystalGive, poisonGive, pepperGive, curseGive, reflectGive, 
        hazeGive, bombGive, inkGive, demonizeGive;
    [Header("Specials")]
    [SerializeField] protected bool hasBarrage;
    [SerializeField] protected bool hasLifesteal, hasConsume, hasBuffFriendly, hasAimless, hasSelfTargetPosEffects; //special ability flags
    [SerializeField] protected List<int> spawnsOnDeath = new List<int>();
    public int charmLimit, currentCharmAmount;

    [Header("UISpecific")]
    [SerializeField] protected GameObject tooltipPrefab;
    [SerializeField] protected List<GameObject> rightTooltips,leftTooltips;
    [SerializeField] protected RectTransform rightTooltipPos,leftTooltipPos;
    [SerializeField] protected bool manualDescription;
    [SerializeField] protected TooltipHandler tooltipHandler;
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

        cardName.text = nameText;
        cardPicture.sprite = picture;
        cardBackground.sprite = background;
        cardBorder.sprite = border;
        
        currentCharmAmount = cardSaveData.currentCharmAmount;
        
       
    }
    protected virtual void Awake()
    {
        if(ID == -1) { return; }
        desc = cardDescription.GetComponent<DescriptionCreator>();


    }
    
    protected List<string> text = new List<string>();
    protected DescriptionCreator desc;

    public virtual void CreateCardDescription()
    {
        
        text.Clear(); //so empty it out!
        //left tooltips
        if (shieldGive > 0)       {
            text.Add($"Apply {shieldGive} shield");
            tooltipHandler.hasShield = true;
        }
        if (crystalGive > 0) { 
            text.Add($"Apply {crystalGive} crystal");
            tooltipHandler.hasCrystal = true;
        }     
        if (bombGive > 0) { 
            text.Add($"Apply {bombGive} bomb");
            tooltipHandler.hasBomb = true;
        }
        if (inkGive > 0) { 
            text.Add($"Apply {inkGive} ink");
            tooltipHandler.hasInk = true;
        }

//right tooltips
        if (snowGive > 0)
        {
            text.Add($"Apply {snowGive} snow");
            tooltipHandler.hasSnow = true;  }
        if (fireGive > 0)
        {
            text.Add($"Apply {fireGive} fire");
            tooltipHandler.hasFire = true;
        }
        if (poisonGive > 0)
        {
            text.Add($"Apply {poisonGive} poison");
            tooltipHandler.hasPoison = true; }
        if (pepperGive > 0)
        {
            text.Add($"Apply {pepperGive} pepper");
            tooltipHandler.hasPepper = true; }
        if (curseGive > 0)
        {
            text.Add($"Apply {curseGive} curse");
            tooltipHandler.hasCurse = true; }
        if (demonizeGive > 0) { 
            text.Add($"Apply {demonizeGive} demonize");
            tooltipHandler.hasDemonize = true;
        }
        if (reflectGive > 0)
        {
            text.Add($"Apply {reflectGive} reflect");
            tooltipHandler.hasReflect = true;
        }
        if (hazeGive > 0)
        {
            text.Add($"Apply {hazeGive} haze");
            tooltipHandler.hasHaze = true;
        }
        if (numOfAttacksGive > 0) { 
            text.Add($"Get frenzy {numOfAttacksGive}");
            tooltipHandler.hasFrenzy = true;
        }

     
       
        if (attackGive > 0) { text.Add($"Increase attack by {attackGive}"); }
        if (healthGive > 0) { text.Add($"Heal {healthGive} health"); }
        if (timerGive < 0) { text.Add($"Reduce timer by {timerGive}"); } //Specifically make timer negative!

        //Special Abilities
        if (hasBarrage) { text.Add($"<color=yellow>Barrage</color>");
            tooltipHandler.hasBarrage = true;
        }
        if (hasLifesteal) { text.Add($"<color=yellow>Lifesteal</color>");
            tooltipHandler.hasLifesteal = true;
        }
        if (hasConsume)
        {
            text.Add($"<color=yellow>Consume</color>");
            tooltipHandler.hasConsume = true;
        }
        if (hasAimless)
        {
            text.Add($"<color=yellow>Aimless</color>");
            tooltipHandler.hasAimless = true;
        }
       
        DisableTooltips();
    }
    public virtual bool TryUse(UnitCard unitToUseOn)
    {
        return false; //so Can't use it
    }

    public virtual void EnableTooltips()
    {
        leftTooltipPos.gameObject.SetActive(true);
        rightTooltipPos.gameObject.SetActive(true);
       
    }
    public virtual void DisableTooltips()
    {
        leftTooltipPos.gameObject.SetActive(false);
        rightTooltipPos.gameObject.SetActive(false);
    }

    public virtual bool TryPlaceOnField(int atIndex, bool isPlayerCard = false, UnitCard cardAlreadyThere = null, bool endCase = true) 
    {
        return false;
    }
    public virtual bool TryDiscard()
    {
        return false;
    }

    public virtual void SelectedView()
    {
        
    }


}
