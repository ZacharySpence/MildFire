using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OffensiveStats : MonoBehaviour
{
    [Header("OffensiveStats")]
    public int currentNumOfAttacks, currentAttack;
    [HideInInspector] public int  attack,numOfAttacks;
    [Header("OffensiveStatVisual")]
    [SerializeField] public TextMeshProUGUI cAttackText,cAttackAmountText;

    public void Setup(StatsData data)
    {
        
        attack = data.attack;
        numOfAttacks = data.numOfAttacks;
        
        currentAttack = attack;
        cAttackText.text = $"{currentAttack}";
        currentNumOfAttacks = numOfAttacks;
        cAttackAmountText.text = $"x{currentNumOfAttacks}";
        if(currentNumOfAttacks > 1)
        {
            cAttackAmountText.transform.parent.gameObject.SetActive(true);
        }
    }

    public void ChangeOffStats(int curseOn=0,int attackAdded=0,int numAttksAdded=0)
    {
        //curse specific
        attack += attackAdded; //perma change (within battle)
        currentAttack = attack - curseOn;  //temp change
        cAttackText.text = $"{currentAttack}";
        currentNumOfAttacks += numAttksAdded;       
        cAttackAmountText.text = $"x{currentNumOfAttacks}";
        if (currentNumOfAttacks > 1)
        {
            cAttackAmountText.transform.parent.gameObject.SetActive(true);
        }
    }
  
}
[System.Serializable]
public class StatsData
{
    public int attack;
    public int numOfAttacks;
}
