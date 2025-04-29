using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBackpack : MonoBehaviour
{
    [SerializeField] GameObject playerDeckPanel, content;
    [SerializeField] GameObject baseUIUnitCard, baseUIUseCard, baseUIUseOffCard; //so i am already using the base cards and then adding data so 
    bool hasMadeVisualDeck;
    public void OpenPlayerDeck()
    {
        playerDeckPanel.SetActive(true);
        if (!hasMadeVisualDeck)
        {
            CreatePlayerVisualDeck();
        }
        
    }
    public void CreatePlayerVisualDeck()
    {
        playerDeckPanel.SetActive(true);
        List<CardBase> tempDeck = new List<CardBase>();
        foreach (var cardSaveData in IDLookupTable.instance.playerDeck)
        {


            switch (cardSaveData.cardType)
            {
                case "UnitCard":
                    var card = Instantiate(baseUIUnitCard, content.transform);
                    card.GetComponentInChildren<UnitCard>().SetupUsingCardSaveData(cardSaveData);
                    tempDeck.Add(card.GetComponentInChildren<UnitCard>());
                    break;
                case "UseCard":
                    card = Instantiate(baseUIUseCard, content.transform);
                    card.GetComponentInChildren<UseCard>().SetupUsingCardSaveData(cardSaveData);
                    tempDeck.Add(card.GetComponentInChildren<UseCard>());
                    break;
                case "UseOffCard":
                    card = Instantiate(baseUIUseOffCard, content.transform);
                    card.GetComponentInChildren<UseOffCard>().SetupUsingCardSaveData(cardSaveData);
                    tempDeck.Add(card.GetComponentInChildren<UseOffCard>());
                    break;
            }
        }
        hasMadeVisualDeck = true;
        playerDeckPanel.SetActive(false);
    }
}
