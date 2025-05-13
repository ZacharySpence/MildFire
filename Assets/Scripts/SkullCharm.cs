using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkullCharm : MonoBehaviour
{
    public int skullCost;
    public int ID;
    
    public int healthChange, attackChange, timerChange, numOfAttackChange, shieldGiveChange, snowGiveChange, fireGiveChange, crystalGiveChange, poisonGiveChange,
        pepperGiveChange, curseGiveChange, reflectGiveChange, hazeGiveChange, bombGiveChange, inkGiveChange, demonizeGiveChange,
        shieldOnChange, snowOnChange, fireOnChange, crystalOnChange, poisonOnChange, pepperOnChange, curseOnChange, reflectOnChange,
        hazeOnChange, bombOnChange, inkOnChange, demonizeOnChange;
    public bool hasEverburnResistance, hasBarrage;
    public Button button;

    [Header("UISpecific")]
    [SerializeField] GameObject tooltipPrefab;
    [SerializeField] List<GameObject> rightTooltips, leftTooltips;
    [SerializeField] RectTransform rightTooltipPos, leftTooltipPos;
    [SerializeField] GameObject tooltip;
    List<string> text = new List<string>();
    private void Awake()
    {
        button = GetComponent<Button>();
       
        CreateCharmDescription();
    }
    public CharmSaveData CreateCharmSaveData()
    {
        return null;
    }
    public void SetupUsingCharmSaveData(CharmSaveData saveData) 
    { 

    }
    public CardSaveData ChangeCard(CardSaveData saveData)
    {
        //change the save data then return it!
        saveData.maxHealth += healthChange;
        saveData.attack += attackChange;

        saveData.snowGive += snowGiveChange;

        saveData.reflectOn += reflectOnChange;

        saveData.hasBarrage = hasBarrage ? true : saveData.hasBarrage;
        saveData.hasEverburnResistance = hasEverburnResistance? true:saveData.hasEverburnResistance; //change it if has or leave it as is!
        return saveData;
    }
    public void CreateCharmDescription()
    {

        //Give
        if (shieldGiveChange > 0)
        {
            text.Add($"Apply {shieldGiveChange} shield");     
        }
        if (crystalGiveChange > 0)
        {
            text.Add($"Apply {crystalGiveChange} crystal");
                 }
        if (bombGiveChange > 0)
        {
            text.Add($"Apply {bombGiveChange} bomb");
        }
        if (inkGiveChange > 0)
        {
            text.Add($"Apply {inkGiveChange} ink");
        }  
        if (snowGiveChange > 0)
        {
            text.Add($"Apply {snowGiveChange} snow");}
        if (fireGiveChange > 0)
        {
            text.Add($"Apply {fireGiveChange} fire");
        }
        if (poisonGiveChange > 0)
        {
            text.Add($"Apply {poisonGiveChange} poison");
        }
        if (pepperGiveChange > 0)
        {
            text.Add($"Apply {pepperGiveChange}  pepper");
        }
        if (curseGiveChange > 0)
        {
            text.Add($"Apply {curseGiveChange} curse");
        }
        if (demonizeGiveChange > 0)
        {
            text.Add($"Apply {demonizeGiveChange} demonize");
        }
        if (reflectGiveChange > 0)
        {
            text.Add($"Apply {reflectGiveChange} reflect");
        }
        if (hazeGiveChange > 0)
        {
            text.Add($"Apply {hazeGiveChange} haze");
        }
        if (numOfAttackChange > 0)
        {
            text.Add($"Get frenzy {numOfAttackChange}");       
        }

        //On
        //left tooltips
        if (shieldOnChange > 0)
        {
            text.Add($"+{shieldOnChange} shield");
        }
        if (crystalOnChange > 0)
        {
            text.Add($"+{crystalOnChange} crystal");
        }
        if (bombOnChange > 0)
        {
            text.Add($"+{bombOnChange} bomb");
        }
        if (inkOnChange > 0)
        {
            text.Add($"+{inkOnChange} ink");
        }
        if (snowOnChange > 0)
        {
            text.Add($"+{snowOnChange} snow");
        }
        if (fireOnChange > 0)
        {
            text.Add($"+{fireOnChange} fire");
        }
        if (poisonOnChange > 0)
        {
            text.Add($"+{poisonOnChange} poison");
        }
        if (pepperOnChange > 0)
        {
            text.Add($"+{pepperOnChange}  pepper");
        }
        if (curseOnChange > 0)
        {
            text.Add($"+{curseOnChange} curse");
        }
        if (demonizeOnChange > 0)
        {
            text.Add($"+{demonizeOnChange} demonize");
        }
        if (reflectOnChange > 0)
        {
            text.Add($"+{reflectOnChange} reflect");
        }
        if (hazeOnChange > 0)
        {
            text.Add($"+{hazeOnChange} haze");
        }

        if (numOfAttackChange > 0)
        {
            text.Add($"Get frenzy {numOfAttackChange}");
        }

        if (attackChange > 0) { text.Add($"+{attackChange} attack"); }
        if (healthChange >  0) { text.Add($"+{healthChange} health"); }
        if (timerChange < 0) { text.Add($"Reduce timer by {timerChange}"); } //Specifically make timer negative!

        //Special Abilities
        if (hasBarrage) { text.Add($"Gain <color=yellow>Barrage</color>"); }
        if(hasEverburnResistance) { text.Add($"Gain <color=yellow>Everburn Resistance</color>"); }
        //put it all onto one tooltip!
        if(tooltip == null)
        {
            tooltip = Instantiate(tooltipPrefab, rightTooltipPos);
            rightTooltips.Add(tooltip);
        }

       
        tooltip.GetComponentInChildren<TextMeshProUGUI>().text = tooltip.GetComponentInChildren<DescriptionCreator>().RewriteSentence(string.Join(" ", text)); ;

        //DisableTooltips(); //change so enable on hover!
    }
    public virtual void EnableTooltips()
    {
        foreach (var tooltip in rightTooltips)
        {
            tooltip.SetActive(true);
        }
        foreach (var tooltip in leftTooltips)
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

}
