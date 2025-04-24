using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseCard : CardBase
{
    protected override void Awake()
    {
        base.Awake();
        tag = presetTag; //set it's tag!
        
    }
    public override CardSaveData CreateCardSaveData()
    {
        return new CardSaveData
        {
            baseID = ID,
            //runtimeID = CardI
            cardType = "UseCard",
            //cardbase fields
            fieldIndex = fieldIndex,
            presetTag = presetTag,
            nameText = this.nameText,
            picture = this.picture,
            border = this.border,
            background = this.background,

            attackGive = this.attackGive,
            healthGive = this.healthGive,
            timerGive = this.timerGive,
            shieldGive = this.shieldGive,
            snowGive = this.snowGive,
            fireGive = this.fireGive,
            crystalGive = this.crystalGive,
            poisonGive = this.poisonGive,
            pepperGive = this.pepperGive,
            curseGive = this.curseGive,
            reflectGive = this.reflectGive,
            hazeGive = this.hazeGive,
            bombGive = this.bombGive,
            inkGive = this.inkGive,
            demonizeGive = this.demonizeGive,
            numOfAttacksGive = this.numOfAttacksGive,
        };
    }

    public override void SetupUsingCardSaveData(CardSaveData cardSaveData)
    {// Set all the fields from the save data back to the card

        base.SetupUsingCardSaveData(cardSaveData);
        this.attackGive = cardSaveData.attackGive;
        this.healthGive = cardSaveData.healthGive;
        this.timerGive = cardSaveData.timerGive;
        this.shieldGive = cardSaveData.shieldGive;
        this.snowGive = cardSaveData.snowGive;
        this.fireGive = cardSaveData.fireGive;
        this.crystalGive = cardSaveData.crystalGive;
        this.poisonGive = cardSaveData.poisonGive;
        this.pepperGive = cardSaveData.pepperGive;
        this.curseGive = cardSaveData.curseGive;
        this.reflectGive = cardSaveData.reflectGive;
        this.hazeGive = cardSaveData.hazeGive;
        this.bombGive = cardSaveData.bombGive;
        this.inkGive = cardSaveData.inkGive;
        this.demonizeGive = cardSaveData.demonizeGive;
        this.numOfAttacksGive = cardSaveData.numOfAttacksGive;
        CreateCardDescription();

    }
    public override bool TryUse(UnitCard cardToUseOn)
    {
      
        cardToUseOn.ChangeStatus(healthGive,attackGive,numOfAttacksGive,timerGive, 
        snowGive, poisonGive, fireGive, curseGive, shieldGive, 
        reflectGive, hazeGive, inkGive, bombGive, demonizeGive,
        pepperGive,crystalGive);
        
       
        return true;
    }
    public override bool TryDiscard()
    {
        //HAVE IT PLACED INTO DISCARD
        PlayerHand.Instance.AddToDiscard(this, false); //got to change this so can't just discard any card
        BattleManager.Instance.selectedCard = null;//just remove it since it should become missing
        return true;
    }
}
