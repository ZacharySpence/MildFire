using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class NoogleShop : MonoBehaviour
{
    [SerializeField] GameObject beefPosOutcome, beefNegOutcome, chickenPosOutcome, chickenNegOutcome, vegPosOutcome, vegNegOutcome;
    [SerializeField] Button chickenButton;

    int posBeefChance = 70;
    int posChickenChance = 80;
    int posVeggieChance = 50;
    private void OnEnable()
    {
        var companions = IDLookupTable.instance.playerDeck.Where(x => x.cardType == "UnitCard" && x.isBoss == false);

        if (companions.Count() < 1)
        {
            chickenButton.interactable = false;
        }
        else
        {
            chickenButton.interactable = true;
        }

        if (WorldManager.Instance.omnisciJudgement)
        {
            //flip the chances!
            posBeefChance = 30;
            posChickenChance = 20;
            posVeggieChance = 50;
        }
    }
    public void OrderSpicyBeefNoogles()
    {
        //75% crystal buff (+1 crystal to start)
        //25% poison debuff (+1-3 poison to all at start) -> put in battleManager
        var rand = Random.Range(1, 101);
        if(rand <=  posBeefChance) 
        {
            WorldManager.Instance.noogleBeefBuff = true;
            beefPosOutcome.SetActive(true);
        }
        else
        {
            WorldManager.Instance.noogleBeefDebuff = true;
            beefNegOutcome.SetActive(true);
        }
       
    }
    public void OrderChickenNoogles()
    {

        //90% heal all
        //10% lose a companion
        var rand = Random.Range(1,101);
        var companions = IDLookupTable.instance.playerDeck.Where(x => x.cardType == "UnitCard" && x.isBoss == false);
        if (rand <= posChickenChance)
        {
         
            foreach(var companion in companions)
            {
                companion.hasDied = false;
            }
            WorldManager.Instance.noogleChickenGood = true;
            chickenPosOutcome.SetActive(true);
           

        }
        else
        {
            companions.OrderBy(x => Random.value);
            var sacrifice = companions.Take(1).FirstOrDefault();
            
            if (sacrifice != null)
            {
                var input = chickenNegOutcome.GetComponentInChildren<TextMeshProUGUI>().text;
                input.Replace("companion", sacrifice.nameText);
                chickenNegOutcome.GetComponentInChildren<TextMeshProUGUI>().text = input;
                IDLookupTable.instance.playerDeck.Remove(sacrifice);
            }
            WorldManager.Instance.noogleChickenBad = true;
            chickenNegOutcome.SetActive(true);
        }
        
    }
    public void OrderVegetableNoodles()
    {
        var rand = Random.Range(0, 4);
        var companions = IDLookupTable.instance.playerDeck.Where(x => x.cardType == "UnitCard");
        if(rand <= posVeggieChance)
        {
            foreach(var companion in companions)
            {
                companion.maxHealth += 1;
            }
            WorldManager.Instance.noogleVeggieGood = true;
            vegPosOutcome.SetActive(true);           
        }
        else
        {
            foreach(var companion in companions)
            {
                companion.maxHealth -= 1;
            }
            vegNegOutcome.SetActive(true);
            WorldManager.Instance.noogleVeggieBad = true;
        }
        //50% +1 health max
        //50% -1 health max     
    }

    public void Continue()
    {
        //got all 3 plagues (poison, death, famine)
        if(!WorldManager.Instance.noogleBlessing && 
            WorldManager.Instance.noogleBeefDebuff && WorldManager.Instance.noogleChickenBad && WorldManager.Instance.noogleVeggieBad)
        {
            WorldManager.Instance.noogleBlessing = true;
            foreach( var card in IDLookupTable.instance.playerDeck.OfType<UnitCard>())
            {
                card.hasPoisonResistance = true;
            }
        }
        else if(WorldManager.Instance.noogleCurse &&
            WorldManager.Instance.noogleBeefBuff && WorldManager.Instance.noogleChickenGood && WorldManager.Instance.noogleVeggieGood)
        {
            WorldManager.Instance.noogleCurse = true; //also perma gives poison debuff!
            foreach(var card in IDLookupTable.instance.playerDeck.OfType<UnitCard>())
            {
                card.poisonGive += 1; //everything gives poison now
                
            }
        }
        GameObject buttonGO = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        buttonGO.transform.parent.gameObject.SetActive(false); //sets parent of button (which is outcome gameObject) false
        gameObject.SetActive(false);
    }
}
