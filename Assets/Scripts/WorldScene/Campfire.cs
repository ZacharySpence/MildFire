using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Campfire : MonoBehaviour
{
    //Could make it so this option is on top of each companion (get leader + 3 companions around campfire?
    [SerializeField] GameObject campUIUnitCard;
    [SerializeField] List<int> choices = new List<int>() { 0, 0, 0, 0 };
    [SerializeField] List<UnitCard> units = new List<UnitCard>();
    [SerializeField] List<Transform> premadePositions = new List<Transform>();
    private void OnEnable()
    {
        var companions = IDLookupTable.instance.playerDeck.Where(x => x.cardType == "UnitCard").OrderBy(x => Random.value).ToList();
        foreach( var companion in companions)
        {
            Debug.Log($"{companion.baseID} is in camp");
        }
        var Leader = companions.FirstOrDefault(x => x.isBoss == true);
        companions.Remove(Leader);
        companions = companions.Take(3).ToList();
        //1. Get list of companions
        //2. get leader
        //3. Create those 4 around campfire
        var card = Instantiate(campUIUnitCard, premadePositions[0].transform);
        card.GetComponentInChildren<UnitCard>().SetupUsingCardSaveData(Leader);
        units.Add(card.GetComponentInChildren<UnitCard>());
        card.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => OnRest(0));
        card.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => OnTrain(0));
        for (int i = 0; i < companions.Count; i++)
        {
            int index = i;
            card = Instantiate(campUIUnitCard, premadePositions[index + 1].transform);
            card.GetComponentInChildren<UnitCard>().SetupUsingCardSaveData(companions[i]);
            units.Add(card.GetComponentInChildren<UnitCard>());
            card.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => OnRest(index + 1));
            card.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => OnTrain(index + 1));
        }
        //4. create buttons on top of each with rest/train options
        //5. create confirm button on campfire
    }
    private void OnDisable()
    {
       foreach(var card in units)
        {
            Destroy(card.transform.parent.gameObject);
        }
        units.Clear();

    }
    public void OnRest(int index)
    {
        choices[index] = 0;
    }
    public void OnTrain(int index)
    {
        choices[index] = 1;
    }
    void Rest(int ID)
    {
       Debug.Log("resting");
       var companion = IDLookupTable.instance.playerDeck.Find(x => x.baseID == ID);
       companion.hasDied = false;
        
    }

    void Train(int ID)
    {
        Debug.Log("training");
        var companion = IDLookupTable.instance.playerDeck.Find(x => x.baseID == ID);
        var choice = Random.Range(0, 2);
        if(choice == 0)
        {
            companion.maxHealth += 3;
        }
        else
        {
            companion.attack += 1;
        }
    }

    public void OnConfirm()
    {
        for(int i = 0; i < 4; i++)
        {
            if (choices[i] == 0)
            {
                Rest(units[i].ID);
            }
            else
            {
                Train(units[i].ID);
            }
            
        }
        gameObject.SetActive(false);
    }
}
