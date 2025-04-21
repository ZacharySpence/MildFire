using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseOffCard : CardBase
{
    [SerializeField] OffensiveStats offStats;

    protected override void Awake()
    {
        base.Awake();
        tag = presetTag; //set it's tag!

    }
    private void Start()
    {
        offStats.Setup();
    }
    public override bool TryUse(UnitCard cardToUseOn)
    {

        //have unit take damage (and get effects of use card)
        cardToUseOn.TakeDamage(offStats.currentAttack, false, false, 
            healthGive, attackGive, numOfAttacksGive, timerGive,
        snowGive, poisonGive, fireGive, curseGive, shieldGive,
        reflectGive, hazeGive, inkGive, bombGive, demonizeGive,
        pepperGive,crystalGive);
        return true;
    }
    public override bool TryDiscard()
    {
        //HAVE IT PLACED INTO DISCARD
        PlayerHand.Instance.AddToDiscard(this, false); //got to change this so can't just discard any card
        GameManager.Instance.selectedCard = null;//just remove it since it should become missing
        return true;
    }
}
