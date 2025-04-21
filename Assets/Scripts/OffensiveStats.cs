using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OffensiveStats : MonoBehaviour
{
    [Header("OffensiveStats")]
    [HideInInspector] public int currentNumOfAttacks, currentAttack;
    [SerializeField] protected int numOfAttacks, attack;
    [Header("OffensiveStatVisual")]
    [SerializeField] protected TextMeshProUGUI cAttackText,cAttackAmountText;

    public void Setup()
    {
        currentAttack = attack;
        Debug.Log("setting current attack");
        cAttackText.text = $"{currentAttack}";
        currentNumOfAttacks = numOfAttacks;
        cAttackAmountText.text = $"{currentNumOfAttacks}";
    }

    public void ChangeOffStats(int curseOn=0,int attackAdded=0,int numAttksAdded=0)
    {
        //curse specific
        currentAttack = attack - curseOn;
        cAttackText.text = $"{currentAttack}";
        currentAttack += attackAdded;
        currentNumOfAttacks += numAttksAdded;
    }
  
}
