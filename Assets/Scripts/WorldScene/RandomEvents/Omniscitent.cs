using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Omniscitent : MonoBehaviour
{
    [SerializeField] GameObject seeAllEventsOutcome,seeAllBattlesOutcome,seeDeckOutcome,RefuseOutcome;
    [SerializeField] Button saeButton, sabButton, sdButton;
    private void OnEnable()
    {
        if (WorldManager.Instance.omnisciChaos)
        {
            sdButton.interactable = false;
        }
        if (WorldManager.Instance.omnisciForesight)
        {
            sabButton.interactable = false;
        }
        if (WorldManager.Instance.omnisciJudgement)
        {
            saeButton.interactable = false;
        }
    }
    public void SeeAllEvents()
    {
        //Go through each node in worldManager nodeList and find ones with  EncounterType.Event then change sprite to actual based on event specific
        WorldManager.Instance.omnisciJudgement = true;
        seeAllEventsOutcome.SetActive(true);
    }

    public void SeeAllBattles()
    {
        //Go through each node in worldManager nodeList and find ones with  EncounterType.Battle or Elite then change sprite to actual based on event specific

        WorldManager.Instance.omnisciForesight = true;
        seeAllBattlesOutcome.SetActive(true);
    }
    public void SeeDeck()
    {
        //Be able to see deck in battle BUT chance for card drawn to go immediately into discard AND low chance of plyed cards getting consumed
        //(very evil)
        WorldManager.Instance.omnisciChaos = true;
        seeDeckOutcome.SetActive(true);
    }

    public void Refuse()
    {
        RefuseOutcome.SetActive(true);
    }
    public void Continue()
    {
        GameObject buttonGO = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        buttonGO.transform.parent.gameObject.SetActive(false); //sets parent of button (which is outcome gameObject) false
        gameObject.SetActive(false);
        if(WorldManager.Instance.omnisciChaos && WorldManager.Instance.omnisciForesight && WorldManager.Instance.omnisciJudgement)
        {
            WorldManager.Instance.omnisciBlessing = true;
        }
    }
}
