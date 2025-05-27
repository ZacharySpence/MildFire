using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBackpack : MonoBehaviour
{
    [SerializeField] GameObject playerDeckPanel, content;
    [SerializeField] GameObject baseUIUnitCard, baseUIUseCard, baseUIUseOffCard, charmUI; //so i am already using the base cards and then adding data so 
    [SerializeField] Transform charmTransform;
    [SerializeField] Canvas canvas;
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
            if (Input.GetMouseButtonDown(1))
            {
                DeselectCharm();
            }
        }
       
    }
    void SelectedCharmFollowCursor()
    {
        Vector2 localPoint;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            null,
            out localPoint))
        {
            selectedCharm.GetComponent<RectTransform>().localPosition = localPoint;
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
                    card.GetComponentInChildren<Button>().onClick.AddListener(() => OnClickCard(card,cardSaveData));
                    tempDeck.Add(card.GetComponentInChildren<UnitCard>());
                    break;
                case "UseCard":
                    card = Instantiate(baseUIUseCard, content.transform);
                    card.GetComponentInChildren<UseCard>().SetupUsingCardSaveData(cardSaveData);
                    card.GetComponentInChildren<Button>().onClick.AddListener(() => OnClickCard(card,cardSaveData));
                    tempDeck.Add(card.GetComponentInChildren<UseCard>());
                    break;
                case "UseOffCard":
                    card = Instantiate(baseUIUseOffCard, content.transform);
                    card.GetComponentInChildren<UseOffCard>().SetupUsingCardSaveData(cardSaveData);
                    card.GetComponentInChildren<Button>().onClick.AddListener(() => OnClickCard(card,cardSaveData));
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


  

    void AttachCharmToCard(GameObject card,CardSaveData cardSaveData)
    {
        //remove from storage
        IDLookupTable.instance.charmsInPlayerStorage.Remove(IDLookupTable.instance.charmsInPlayerStorage.Find(x => x.ID == selectedCharm.ID));
        var cardToChange = IDLookupTable.instance.playerDeck.Find(x => x == cardSaveData);
        cardToChange.currentCharmAmount += 1;
        cardToChange = selectedCharm.ChangeCard(cardToChange);
        //re-set up the card data!
        switch (cardToChange.cardType)
        {
            case "UnitCard":
                Debug.Log(card.transform.GetChild(0).name);
                card.transform.GetChild(0).GetComponent<UnitCard>().SetupUsingCardSaveData(cardToChange);
                break;
            case "UseOffCard":
                card.GetComponentInChildren<UseOffCard>().SetupUsingCardSaveData(cardToChange);
                break;
            case "UseCard":
                card.GetComponentInChildren<UseCard>().SetupUsingCardSaveData(cardToChange);
                break;
        }

        //Add to persistance manager & save
        PersistanceManager.skullsOnID.Add((selectedCharm.ID, cardToChange.baseID));
        PersistanceManager.SavePersistence();

        //Also then find the actual card in backpack!

        //visually remove charm too!
        Destroy(selectedCharm.gameObject);
        selectedCharm = null;

       
        
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
    void OnClickCard(GameObject card, CardSaveData cardSaveData)
    {
        Debug.Log("clicked backpack card");
        if (selectedCharm != null && cardSaveData.currentCharmAmount < 3)
        {
            //Make it so only can do on unitCards
            if(cardSaveData.cardType != "UnitCard")
            {
                return;
            }
           /* if(cardSaveData.cardType == "UseCard" && (selectedCharm.healthChange != 0 || selectedCharm.attackChange != 0))
            {
                return; //can't attach certain charms to use cards!
            }*/
            AttachCharmToCard(card,cardSaveData);
        }
    }
    void OnClickCharm(SkullCharm charm)
    {
        Debug.Log("clicked skullCharm");
        if (selectedCharm != null)
        {
            DeselectCharm();
        }
        selectedCharm = charm;
        selectedCharm.button.enabled = false; //so when clicking doesnt click on it
        selectedCharm.GetComponent<Image>().raycastTarget = false;
        // Re-parent to the canvas
        selectedCharm.transform.SetParent(canvas.transform, worldPositionStays: false);
        selectedCharm.transform.SetAsLastSibling();
    }

    void DeselectCharm()
    {
        selectedCharm.transform.SetParent(charmTransform, worldPositionStays: false);
        selectedCharm.transform.localPosition = Vector2.zero;//back to parent
        selectedCharm.button.enabled = true;
        selectedCharm.GetComponent<Image>().raycastTarget = true;
        selectedCharm = null;
        OrderCharms();
    }
}
