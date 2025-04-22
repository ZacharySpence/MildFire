using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OffensiveStats : MonoBehaviour
{
    [Header("OffensiveStats")]
    [HideInInspector] public int currentNumOfAttacks, currentAttack;
    [SerializeField] public StatsData statData;
    [Header("OffensiveStatVisual")]
    [SerializeField] protected TextMeshProUGUI cAttackText,cAttackAmountText;

    public void Setup(StatsData data)
    {
        statData = new StatsData
        {
            attack = data.attack,
            numOfAttacks = data.numOfAttacks
        };
        currentAttack = statData.attack;
        Debug.Log("setting current attack");
        cAttackText.text = $"{currentAttack}";
        currentNumOfAttacks = statData.numOfAttacks;
        cAttackAmountText.text = $"{currentNumOfAttacks}";
    }

    public void ChangeOffStats(int curseOn=0,int attackAdded=0,int numAttksAdded=0)
    {
        //curse specific
        currentAttack = statData.attack - curseOn;
        cAttackText.text = $"{currentAttack}";
        currentAttack += attackAdded;
        currentNumOfAttacks += numAttksAdded;
    }
  
}
[System.Serializable]
public class StatsData
{
    public int attack;
    public int numOfAttacks;
}
