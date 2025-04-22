using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseOffCard : CardBase
{
    [SerializeField] OffensiveStats offStats;
    public StatsData statsData;
    protected override void Awake()
    {
        base.Awake();
        tag = presetTag; //set it's tag!

    }
    private void Start()
    {
        offStats.Setup(statsData);
    }
    public override CardBase Clone()
    {  // Instantiate the GameObject (this will clone the whole GameObject)
        GameObject clonedGameObject = Instantiate(this.gameObject);
        // Get the UnitCard component from the cloned GameObject
        UseOffCard clonedCard = clonedGameObject.GetComponent<UseOffCard>();

        //cardbase specific
        clonedCard.fieldIndex = this.fieldIndex;
        clonedCard.ID = this.ID;
        clonedCard.presetTag = this.presetTag;
        clonedCard.nameText = this.nameText;
        clonedCard.picture = this.picture;
        clonedCard.border = this.border;
        clonedCard.background = this.background;
        clonedCard.numOfAttacksGive = this.numOfAttacksGive;
        clonedCard.attackGive = this.attackGive;
        clonedCard.healthGive = this.healthGive;
        clonedCard.timerGive = this.timerGive;
        clonedCard.shieldGive = this.shieldGive;
        clonedCard.snowGive = this.snowGive;
        clonedCard.fireGive = this.fireGive;
        clonedCard.crystalGive = this.crystalGive;
        clonedCard.poisonGive = this.poisonGive;
        clonedCard.pepperGive = this.pepperGive;
        clonedCard.curseGive = this.curseGive;
        clonedCard.reflectGive = this.reflectGive;
        clonedCard.hazeGive = this.hazeGive;
        clonedCard.bombGive = this.bombGive;
        clonedCard.inkGive = this.inkGive;
        clonedCard.demonizeGive = this.demonizeGive;

            //unit card specific
            //offStats specific
           clonedCard.statsData = new StatsData
           {
               attack = this.statsData.attack,
               numOfAttacks = this.statsData.numOfAttacks
           };
        return clonedCard;
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
