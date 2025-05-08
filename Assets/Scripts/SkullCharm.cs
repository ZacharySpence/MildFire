using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkullCharm : MonoBehaviour
{
    public int skullCost;
    public int ID;
    public int healthChange, attackChange;
    public Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
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
        return saveData;
    }

  
}
