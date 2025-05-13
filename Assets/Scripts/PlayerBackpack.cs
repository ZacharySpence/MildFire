using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBackpack : MonoBehaviour
{
    [SerializeField] GameObject playerDeckPanel, content;
    [SerializeField] GameObject baseUIUnitCard, baseUIUseCard, baseUIUseOffCard, charmUI; //so i am already using the base cards and then adding data so 
    [SerializeField] Transform charmTransform;
    bool hasMadeVisualDeck;

    [Header("Interaction")]
    [SerializeField] public SkullCharm selectedCharm;
    public void OpenPlayerDeck()
    {
        playerDeckPanel.SetActive(true);
        if (!hasMadeVisualDeck)
        {
            CreatePlayerVisualDeck();
            CreateVisualCharms();
        }
        
    }

    private void Update()
    {
        if (selectedCharm != null)
        {
            SelectedCharmFollowCursor();
        }
       
    }
    void SelectedCharmFollowCursor()
    {
        // Get the mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Set the z-position to 0 to keep it on the 2D plane
        mousePosition.z = 0f;

        // Move the GameObject to the mouse position
        selectedCharm.transform.position = mousePosition;
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
                    card.GetComponentInChildren<Button>().onClick.AddListener(() => OnClickCard(cardSaveData));
                    tempDeck.Add(card.GetComponentInChildren<UnitCard>());
                    break;
                case "UseCard":
                    card = Instantiate(baseUIUseCard, content.transform);
                    card.GetComponentInChildren<UseCard>().SetupUsingCardSaveData(cardSaveData);
                    card.GetComponentInChildren<Button>().onClick.AddListener(() => OnClickCard(cardSaveData));
                    tempDeck.Add(card.GetComponentInChildren<UseCard>());
                    break;
                case "UseOffCard":
                    card = Instantiate(baseUIUseOffCard, content.transform);
                    card.GetComponentInChildren<UseOffCard>().SetupUsingCardSaveData(cardSaveData);
                    card.GetComponentInChildren<Button>().onClick.AddListener(() => OnClickCard(cardSaveData));
                    tempDeck.Add(card.GetComponentInChildren<UseOffCard>());
                    break;
            }
        }
        hasMadeVisualDeck = true;
        playerDeckPanel.SetActive(false);
    }

    void CreateVisualCharms()
    {
        foreach(var charmSaveData in IDLookupTable.instance.charmsInPlayerStorage)
        {
            var charm = Instantiate(charmUI, charmTransform);
            OrderCharms();
            var skullCharm = charm.GetComponent<SkullCharm>();
            skullCharm.SetupUsingCharmSaveData(charmSaveData);
            skullCharm.DisableTooltips();
            charm.GetComponent<Button>().onClick.AddListener(() => OnClickCharm(skullCharm));
           
        }
    }


  

    void AttachCharmToCard(CardSaveData card)
    {
        //remove from storage
        IDLookupTable.instance.charmsInPlayerStorage.Remove(IDLookupTable.instance.charmsInPlayerStorage.Find(x => x.ID == selectedCharm.ID));
        var cardToChange = IDLookupTable.instance.playerDeck.Find(x => x == card);
        cardToChange.currentCharmAmount += 1;
        cardToChange = selectedCharm.ChangeCard(cardToChange);
        //visually remove charm too!
    }

    void OrderCharms()
    {
        //order the current children of charmsTransform (maybe in a pyramid style?)
    }
    //For buttons
    public void CloseBackpack()
    {
        OnClickCharm(null); //so just puts charm back where it was
    }
    //For the cards/charms when made
    void OnClickCard(CardSaveData card)
    {
        if (selectedCharm != null && card.currentCharmAmount < 3)
        {
            if(card.cardType == "UseCard" && selectedCharm.healthChange != 0 || selectedCharm.attackChange != 0)
            {
                return; //can't attach certain charms to use cards!
            }
            AttachCharmToCard(card);
        }
    }
    void OnClickCharm(SkullCharm charm)
    {
        if (selectedCharm != null)
        {

            selectedCharm.transform.localPosition = Vector2.zero;//back to parent
            selectedCharm.button.enabled = true;
            selectedCharm = null;
            OrderCharms();
        }
        selectedCharm = charm;
        selectedCharm.button.enabled = false; //so when clicking doesnt click on it
    }
}
