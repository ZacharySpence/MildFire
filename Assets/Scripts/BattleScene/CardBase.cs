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
    [SerializeField] protected int numOfAttacksGive, attackGive, healthGive, timerGive;
    [SerializeField] protected int shieldGive, snowGive, fireGive, crystalGive, poisonGive, pepperGive, curseGive, reflectGive, 
        hazeGive, bombGive, inkGive, demonizeGive;
    [Header("Specials")]
    [SerializeField] protected bool hasBarrage, hasBuffAttack, hasSmackback, hasLifesteal, hasSpawnOnDeath, hasSelfTargetPosEffects; //special ability flags
    [SerializeField] protected List<int> spawnsOnDeath = new List<int>();
    public int charmLimit, currentCharmAmount;

    [Header("UISpecific")]
    [SerializeField] GameObject tooltipPrefab;
    [SerializeField] List<GameObject> rightTooltips,leftTooltips;
    [SerializeField] RectTransform rightTooltipPos,leftTooltipPos;
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
        
        //left tooltips
        if (shieldGive > 0)       {
            text.Add($"Apply {shieldGive} shield");
            GameObject tooltip = Instantiate(tooltipPrefab, leftTooltipPos);
            leftTooltips.Add(tooltip);
            tooltip.GetComponentInChildren<TextMeshProUGUI>().text = desc.RewriteSentence($"Ice shield \n blocks damage");
        }
        if (crystalGive > 0) { 
            text.Add($"Apply {crystalGive} crystal");
            GameObject tooltip = Instantiate(tooltipPrefab, leftTooltipPos);
            leftTooltips.Add(tooltip);
            tooltip.GetComponentInChildren<TextMeshProUGUI>().text = desc.RewriteSentence($"Crystal crystal \n blocks entire instances of damage");
        }     
        if (bombGive > 0) { 
            text.Add($"Apply {bombGive} bomb");
            GameObject tooltip = Instantiate(tooltipPrefab, leftTooltipPos);
            leftTooltips.Add(tooltip);
            tooltip.GetComponentInChildren<TextMeshProUGUI>().text = desc.RewriteSentence($"Bom bomb \n Takes additional damage from all sources \n Does not count down!");

        }
        if (inkGive > 0) { 
            text.Add($"Apply {inkGive} ink");
            GameObject tooltip = Instantiate(tooltipPrefab, leftTooltipPos);
            leftTooltips.Add(tooltip);
            tooltip.GetComponentInChildren<TextMeshProUGUI>().text = desc.RewriteSentence($"Crystal crystal \n Silences effects \n Counts down every turn");
        }
        while (leftTooltips.Count < 3)
        {
            var tooltip = new GameObject($"EmptyTooltip {leftTooltips.Count} for {name}");

            tooltip.transform.parent = leftTooltipPos;
            leftTooltips.Add(tooltip);
        }
        //right tooltips
        if (snowGive > 0)
        {
            text.Add($"Apply {snowGive} snow");
            GameObject tooltip = Instantiate(tooltipPrefab, rightTooltipPos);
            rightTooltips.Add(tooltip);
            tooltip.GetComponentInChildren<TextMeshProUGUI>().text = desc.RewriteSentence($"Snow snow \n Freezes Counter timer & Reactions reaction \n Counts down every turn");
        }
        if (fireGive > 0)
        {
            text.Add($"Apply {fireGive} fire");
            GameObject tooltip = Instantiate(tooltipPrefab, rightTooltipPos);
            rightTooltips.Add(tooltip);
            tooltip.GetComponentInChildren<TextMeshProUGUI>().text = desc.RewriteSentence($"Fire fire \n Deals damage every turn \n reduced by snow"); //fire doesn't ever go down!

        }
        if (poisonGive > 0)
        {
            text.Add($"Apply {poisonGive} poison");
            GameObject tooltip = Instantiate(tooltipPrefab, rightTooltipPos);
            rightTooltips.Add(tooltip);
            tooltip.GetComponentInChildren<TextMeshProUGUI>().text = desc.RewriteSentence($"Poison poison \n Deals damage every turn \n Counts down every turn");
        }
        if (pepperGive > 0)
        {
            text.Add($"Apply {pepperGive} pepper");
            GameObject tooltip = Instantiate(tooltipPrefab, rightTooltipPos);
            rightTooltips.Add(tooltip);
            tooltip.GetComponentInChildren<TextMeshProUGUI>().text = desc.RewriteSentence($"Pepper pepper \n Temporarily increases attack \n Changes after triggering");
        }
        if (curseGive > 0)
        {
            text.Add($"Apply {curseGive} curse");
            GameObject tooltip = Instantiate(tooltipPrefab, rightTooltipPos);
            rightTooltips.Add(tooltip);
            tooltip.GetComponentInChildren<TextMeshProUGUI>().text = desc.RewriteSentence($"Curse curse \n Temporarily reduces attack \n Clears after triggering");
        }
        if (demonizeGive > 0) { 
            text.Add($"Apply {demonizeGive} demonize");
            GameObject tooltip = Instantiate(tooltipPrefab, rightTooltipPos);
            rightTooltips.Add(tooltip);
            tooltip.GetComponentInChildren<TextMeshProUGUI>().text = desc.RewriteSentence($"Demonize demonize \n Doubles damage taken \n Counts down after taking damage");
        }
        if (reflectGive > 0)
        {
            text.Add($"Apply {reflectGive} reflect");
            GameObject tooltip = Instantiate(tooltipPrefab, rightTooltipPos);
            rightTooltips.Add(tooltip);
            tooltip.GetComponentInChildren<TextMeshProUGUI>().text = desc.RewriteSentence($"Teeth reflect \n Deals damage to attackers");

        }
        if (hazeGive > 0)
        {
            text.Add($"Apply {hazeGive} haze");
            GameObject tooltip = Instantiate(tooltipPrefab, rightTooltipPos);
            rightTooltips.Add(tooltip);
            tooltip.GetComponentInChildren<TextMeshProUGUI>().text = desc.RewriteSentence($"Haze haze \n When attacking hit a random ally instead \n Counts down after each attack");

        }
        if (numOfAttacksGive > 0) { 
            text.Add($"Get frenzy {numOfAttacksGive}");
            GameObject tooltip = Instantiate(tooltipPrefab, rightTooltipPos);
            rightTooltips.Add(tooltip);
            tooltip.GetComponentInChildren<TextMeshProUGUI>().text = desc.RewriteSentence($"Frenzy numOfAttacks \n Triggers multiple times");

        }
        while (rightTooltips.Count < 3) 
        {
            var tooltip = new GameObject($"EmptyTooltip {rightTooltips.Count} for {name}");
            
            tooltip.transform.parent = rightTooltipPos;
            rightTooltips.Add(tooltip);
        }
        
        DisableTooltips();
       
        if (attackGive > 0) { text.Add($"Increase attack by {attackGive}"); }
        if (healthGive > 0) { text.Add($"Heal {healthGive} health"); }
        if (timerGive < 0) { text.Add($"Reduce timer by {timerGive}"); } //Specifically make timer negative!

        //Special Abilities
        if (hasBarrage) { text.Add($"<color=yellow>Barrage</color>"); }
    }
    public virtual bool TryUse(UnitCard unitToUseOn)
    {
        return false; //so Can't use it
    }

    public virtual void EnableTooltips()
    {
       foreach(var tooltip in rightTooltips )
        {
            tooltip.SetActive(true);
        }
       foreach(var tooltip in leftTooltips)
        {
            tooltip.SetActive(true);
        }
       
    }
    public virtual void DisableTooltips()
    {
        foreach (var tooltip in rightTooltips)
        {
            tooltip.SetActive(false);
        }
        foreach (var tooltip in leftTooltips)
        {
            tooltip.SetActive(false);
        }
    }

    public virtual bool TryPlaceOnField(int atIndex, bool isPlayerCard = false, UnitCard cardAlreadyThere = null) 
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
