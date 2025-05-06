using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Seductress : MonoBehaviour
{
    [SerializeField] GameObject TradeOneOutcome,GiveThreeOutcome,RefuseOutcome;

    List<CardSaveData> companions;
    [SerializeField] Button tradeButton,giveButton;
    [SerializeField] UnitCard companionToTrade;
    [SerializeField] Transform toyTransform;
    [SerializeField] TextMeshProUGUI greetingText;

    CardSaveData sacrifice;
    private void OnEnable()
    {
        companions = IDLookupTable.instance.playerDeck.Where(x => x.cardType == "UnitCard" && x.isBoss == false).OrderBy(x => Random.value).ToList();
        sacrifice = companions.Take(1).FirstOrDefault();

        if (companions.Count() < 1)
        {
            tradeButton.interactable = false;
            giveButton.interactable = false;
        }
        else if (companions.Count() < 3)
        {
            tradeButton.interactable = true;
            giveButton.interactable = false;
        }
        else
        {
            tradeButton.interactable = true;
            giveButton.interactable = true;
        }

        if (sacrifice != null)
        {
            var newText = "";
            if (WorldManager.Instance.omnisciJudgement)
            {
                newText = greetingText.text.Replace("companion", "companion");
            }
            else
            {
                 newText = greetingText.text.Replace("companion", sacrifice.nameText);
            }
           
            greetingText.text = newText;
            companions.Remove(sacrifice);
        }
        else
        {
            greetingText.text = "Well tut tut, why would you rock up here with nothing of value? so silly of you";
        }
    
        
        CreateCompanionCard();
       
    }

    void CreateCompanionCard()
    {
        companionToTrade = Instantiate(IDLookupTable.instance.GetCompanionCard(), toyTransform); //actually have to instantiate the card!

        companionToTrade.hasDied = true;
        //buff companion so it's better than normal (basically trained 4 times?)
        companionToTrade.maxHealth += 6;
        companionToTrade.statsData.attack += 2;
        companionToTrade.SetupUsingCardSaveData(companionToTrade.CreateCardSaveData()); //set it up using its own data
        companionToTrade.transform.localScale = new Vector2(200f, 300f);
        companionToTrade.transform.localPosition = Vector2.zero;
    }
    public void TradeOne()
    {       
        TradeOneOutcome.GetComponentInChildren<TextMeshProUGUI>().text = TradeOneOutcome.GetComponentInChildren<TextMeshProUGUI>().text.Replace("companion",sacrifice.nameText);
        IDLookupTable.instance.playerDeck.Remove(sacrifice);
        //add seductressCompanion
        companionToTrade.hasDied = true;
        IDLookupTable.instance.playerDeck.Add(companionToTrade.CreateCardSaveData());
        TradeOneOutcome.SetActive(true);
    }
    public void GiveThree()
    {
        int amount = 2;
        if (WorldManager.Instance.omnisciJudgement)
        {
            //so can be from 2 - max (so give all your companions away!
            amount = Random.Range(2, companions.Count());
        }
        var sacrifices = companions.Take(amount).ToList();
        var textComp = GiveThreeOutcome.GetComponentInChildren<TextMeshProUGUI>();
        textComp.text = textComp.text.Replace("companion0", sacrifice.nameText);
        IDLookupTable.instance.playerDeck.Remove(sacrifice);
        for (int i=1;i<amount+1;i++)
        {
            var text =textComp.text;
            Debug.Log($"removing {sacrifices[i-1].nameText}");
            IDLookupTable.instance.playerDeck.Remove(sacrifices[i-1]);
            if (text.Contains($"companion{i}"))
            {
                textComp.text = text.Replace($"companion{i}", sacrifices[i - 1].nameText);
            }
            else
            {
                textComp.text = text + $" and {sacrifices[i - 1].nameText}";
            }
        }
        
        
        if (WorldManager.Instance.omnisciJudgement)
        {
            textComp.text += " oh I'll make space for all of you";
        }
        WorldManager.Instance.nessyBlessing = true;
        GiveThreeOutcome.SetActive(true);
    }

    public void Refuse()
    {
        WorldManager.Instance.nessyCurse = true;
        RefuseOutcome.SetActive(true);
    }

    public void Continue()
    {
        GameObject buttonGO = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        buttonGO.transform.parent.gameObject.SetActive(false); //sets parent of button (which is outcome gameObject) false
        gameObject.SetActive(false);
    }
}
