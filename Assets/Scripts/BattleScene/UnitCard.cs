using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UnitCard :CardBase
{
    [SerializeField] OffensiveStats offStats;
    public StatsData statsData;
    [SerializeField] Transform parentTransform; //so is it in hand or on field somewhere
    




    public bool isDead, isBoss,hasCrown;
   
    [Header("Stats")]
    public int currentAtkTimer, currentHealth;
    [SerializeField] int maxAtkTimer, maxHealth;
    //possible add in 'start' with so has start values separate to On which is in-game!
    [SerializeField] StatusEffect shieldOn,snowOn, fireOn, crystalOn, poisonOn, pepperOn, curseOn,reflectOn, hazeOn,bombOn,inkOn,demonizeOn;
                                                                                                           
   
    [Header("Stat visuals")]
    [SerializeField] TextMeshProUGUI cAttackTimerText;
    [SerializeField] TextMeshProUGUI cHealthText;
    [SerializeField] Slider currentHealthSlider;
    //shield = ice (block dmg/fire damage, snow = stop enemy timer (reduced by globalFireLvl) , fire = damage/turn, crystal = total atk block , poison = damage/turn ignoring shield, pepper = bonus atk/turn, curse = -ve bonus atk/turn
    protected override void Awake()
    {
        base.Awake();
        tag = presetTag; //set it's tag!
        if (ID == -1) { return; } //don't set anything else if it's empty!
        currentAtkTimer = maxAtkTimer;
        currentHealth = maxHealth;
        currentHealthSlider.maxValue = maxHealth;
       

        ChangeStatus();
       
      

    }
  

    public override CardSaveData CreateCardSaveData()
    {
        return new CardSaveData
        {
            baseID = ID,
            //runtimeID = CardI
            cardType = "UnitCard",
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
            attack = this.statsData.attack,
            numOfAttacks = this.statsData.numOfAttacks,
            maxAtkTimer = this.maxAtkTimer,
            maxHealth = this.maxHealth,
            isBoss = this.isBoss,
            hasCrown = this.hasCrown,

            //+ve effects
            shieldOn = this.shieldOn.value,
            crystalOn = this.crystalOn.value,
            pepperOn = this.pepperOn.value,

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
        this.statsData.attack = cardSaveData.attack;
        this.statsData.numOfAttacks = cardSaveData.numOfAttacks;
        this.maxAtkTimer = cardSaveData.maxAtkTimer;
        this.maxHealth = cardSaveData.maxHealth;
        this.isBoss = cardSaveData.isBoss;
        this.hasCrown = cardSaveData.hasCrown;

        // Apply positive effects
        this.shieldOn.Add(cardSaveData.shieldOn);  // Assuming `shieldOn` is some kind of reference to a bool or value
        this.crystalOn.Add(cardSaveData.crystalOn);
        this.pepperOn.Add(cardSaveData.pepperOn);

        //Setup rest
        offStats = GetComponent<OffensiveStats>();
        offStats.Setup(statsData);
        cAttackTimerText.text = $"{currentAtkTimer}x{offStats.currentNumOfAttacks}";
        currentAtkTimer = maxAtkTimer;
        currentHealth = maxHealth;
        currentHealthSlider.maxValue = maxHealth;
        ChangeStatus();
        


    }
    //Card Auto logic methods
    public void ReduceTimer()
    {
        //ANIMATIONS HERE!
        if(snowOn.value > 0)
        {
            snowOn.Add(-1);
        }
        else
        {
            currentAtkTimer--;
            
        }
        CheckAtkTimerAtZero();
        
        DoStatusEffects();
    }
    void CheckAtkTimerAtZero()
    {
        if (currentAtkTimer <= 0)
        {
            for (int i = 0; i < offStats.currentNumOfAttacks; i++)
            {
                Attack();
            }

            currentAtkTimer = maxAtkTimer;
        }
        cAttackTimerText.text = $"{currentAtkTimer}x{offStats.currentNumOfAttacks}";
        //Have certain status effects change here (the ones that update after attacking if there is)
    }
    
    void DoStatusEffects()
    {
        //End turn status change
        //fire, poison, pepper, curse
        if (fireOn.value > 0)
        {
            TakeDamage(fireOn.value, false,true); //ignores crystal but not shield
            fireOn.Add(-1);
        }
        if(poisonOn.value > 0)
        {
            TakeDamage(poisonOn.value,true, false); //ignores shield but not crystal
            poisonOn.Add(-1);
        }
        if(pepperOn.value > 0)
        {
            pepperOn.Add(-1);
        }
        if(curseOn.value > 0)
        {
            curseOn.Add(-1);
            offStats.ChangeOffStats(curseOn.value);
        }
    }
    //--COMBAT--
    //Just basic attack row enemy
    void Attack()
    {
      
        
        bool isPlayer = fieldIndex < 6;
        //Get whether it's player, then find row and closest enemy in row
       
         var enemy =  FindNearestEnemy(fieldIndex % 2,isPlayer); //top row if it's 0, bottom if it's 1
        if(enemy != null)
        {
            int reflectValue = enemy.reflectOn.value;
            Debug.Log($"{name} attacked {enemy.name} for {offStats.currentAttack+pepperOn.value-curseOn.value} damage");
            enemy.TakeDamage(offStats.currentAttack+pepperOn.value-curseOn.value,false,false, 
                healthGive,attackGive,numOfAttacksGive,timerGive,
                snowGive,poisonGive,fireGive,curseGive,shieldGive,
                reflectGive, hazeGive,inkGive,bombGive,demonizeGive);

            curseOn.Add(-curseOn.value); //remove all curse after using it!
            if (reflectValue > 0) //take reflected damage even if kill enemy
            {
                TakeDamage(reflectValue);
            }
        }
       
        
    }

    //Maybe put buff/dmg together so can have like heal that adds pepper? (got to play around with this!)
    public void ChangeStatus(int healthAdded = 0,int attackAdded = 0, int numAttksAdded = 0, int timerAdded = 0, 
        int snowAdded = 0, int poisonAdded = 0, int fireAdded = 0, int curseAdded = 0, int shieldAdded = 0, 
        int reflectAdded = 0, int hazeAdded = 0, int inkAdded = 0, int bombAdded = 0, int demonizeAdded = 0,
        int pepperAdded = 0, int crystalAdded = 0)
    {
       
        //-ve
        snowOn.Add(snowAdded);
        poisonOn.Add(poisonAdded);
        fireOn.Add(fireAdded);
        hazeOn.Add(hazeAdded);
        inkOn.Add(inkAdded);
        bombOn.Add(bombAdded);
        demonizeOn.Add(demonizeAdded);
        curseOn.Add(curseAdded);
        

        //+ve
        shieldOn.Add(shieldAdded);
        reflectOn.Add(reflectAdded);
        pepperOn.Add(pepperAdded);
        crystalOn.Add(crystalAdded);
        Heal(healthAdded);
        //offenseStatSpecific
        offStats.ChangeOffStats(curseOn.value,attackAdded,numAttksAdded);
        currentAtkTimer += timerAdded;
        CheckAtkTimerAtZero();
    }
    public void TakeDamage(int damage=0,bool ignoreShield = false,bool ignoreCrystal = false, 
        int healthAdded = 0, int attackAdded = 0, int numAttksAdded = 0, int timerAdded = 0,
        int snowAdded = 0, int poisonAdded = 0, int fireAdded = 0, int curseAdded = 0, int shieldAdded = 0,
        int reflectAdded = 0, int hazeAdded = 0, int inkAdded = 0, int bombAdded = 0, int demonizeAdded = 0,
        int pepperAdded = 0, int crystalAdded = 0)
    {
        damage += bombOn.value; //add bomb value to damage

        //if demonize is > 0 then double damage and decrease demonize
        if(demonizeOn.value > 0)
        {
            damage *= 2;
            demonizeOn.Add(-1);
        }
        //only check shield/crystal if it actually deals any damage (keep it like that for now)
        if(damage > 0)
        {
            if (!ignoreCrystal && crystalOn.value > 0) //if not ignoring crystal then reduce crystal by 1 and make damage 0
            {
                crystalOn.Add(-1);
                damage = 0;
            }
            if (!ignoreShield && shieldOn.value > 0) //if not ignoring shield then reduce shield by damage then make damage 0 if enough shield
            {
                int newDamage = damage - shieldOn.value;
                if (newDamage <= 0)
                {
                    shieldOn.Add(-damage);
                    damage = 0;
                }
                else
                {
                    shieldOn.Add(-shieldOn.value); //make it 0
                    damage = newDamage;
                }
            }
        }
        
       
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            Die();
            return;
        }
       

       
        ChangeStatus(healthAdded,attackAdded,numAttksAdded,timerAdded,
            snowAdded,poisonAdded,fireAdded,curseAdded,shieldAdded,
            reflectAdded,hazeAdded,inkAdded,bombAdded,demonizeAdded,
            pepperAdded,crystalAdded);
    }

    void Heal(int healthAdded)
    {
        currentHealth = Mathf.Clamp(healthAdded + currentHealth, 0, maxHealth);
        currentHealthSlider.value = currentHealth;
        cHealthText.text = $"{currentHealth}";
    }
    void Die()
    {
        Debug.Log($"{name} died");
        isDead = true;
        GameManager.Instance.EmptyOutField(fieldIndex, fieldIndex < 6);
        //For animation: give it a rigidbody (now has gravity), impulse force a little up
        GetComponent<Collider2D>().enabled = false;
        var rb = gameObject.AddComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(0.25f, 1f), ForceMode2D.Impulse);
        
    }
    void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }

    UnitCard FindNearestEnemy(int row,bool isPlayer)
    {
        //if it's player += 2 until hit max in enemyField, else in playerfield
        UnitCard[] fieldToSearch = isPlayer ? GameManager.Instance.enemyField : GameManager.Instance.playerField;
        for(int i = row; i < 6; i += 2) //go through own row
        {
            if (fieldToSearch[i].ID != -1) //card in there
            {
                return fieldToSearch[i];
            }
        }
        row ^= 1;
        for (int i = row; i < 6; i += 2) //go through own row
        {
            if (fieldToSearch[i].ID != -1) //card in there
            {
                return fieldToSearch[i];
            }
        }
        return null; //should only return this if nothing to hit

    }
    //
    public int GetFieldIndex()
    {
        return fieldIndex >= 0 ? fieldIndex : -1;
    }

    //card actions (when selecting, placing on, deselecting)

    public void ReturnToPosition()
    {

    }
    public override void View()
    {
        //Place in viewing spot in UI
    }


    //Good enough for now -> needs to be done via hovering insted of on-click
    public override bool TryPlaceOnField(int atIndex, bool isPlayerCard = false, UnitCard cardAlreadyThere = null)
    {
        if (cardAlreadyThere != null && cardAlreadyThere.ID != -1) //so not an empty
        {
            //Debug.Log($"there is a card already here: {cardAlreadyThere.ID}");
            
            //get next index in row
            int newIndex = atIndex + 2; //Also need to check it other way (so if i try place on 3rd and 2nd has free spot!
            if (newIndex >= (isPlayerCard ? 6 : 12))
            {
                //Debug.Log("reached left most index"); //after hitting left most index
                return false;
            }
            //if it hasn't gone too far then try place the card that's already there on the next spot 
            else
            {
                //Debug.Log($"Trying to place next card in row at index {newIndex}");
                if (cardAlreadyThere.TryPlaceOnField(newIndex,isPlayerCard,GameManager.Instance.playerField[newIndex]))
                {
                    GameManager.Instance.PlayerPlaceCardOnFullField(atIndex, this);
                    return true;
                }
                return false;
            }
            
        }
        else
        {
            if (isPlayerCard && atIndex <= 5 || !isPlayerCard && atIndex >= 6)
            {
               
                GameManager.Instance.PlayerPlaceCardOnEmptyField(atIndex, this);
                return true;
            }
            return false;
           
        }
        
    }
    

    public override bool TryDiscard()
    {
        if (isBoss) { return false; } //so can't discard leader
        //HAVE IT PLACED INTO DISCARD
        PlayerHand.Instance.AddToDiscard(this, false); //got to change this so can't just discard any card
        GameManager.Instance.selectedCard = null;//just remove it since it should become missing
        Heal(5);// heal it and remove all status effects
        ClearAllNegativeStatusEffects();
        return true;
    }
   
    void ClearAllNegativeStatusEffects()
    {


        snowOn.Add(-snowOn.value);
        fireOn.Add(-fireOn.value);
        poisonOn.Add(-poisonOn.value);
        curseOn.Add(-curseOn.value);
        hazeOn.Add(-hazeOn.value);
        inkOn.Add(-inkOn.value);
        demonizeOn.Add(-demonizeOn.value);
        bombOn.Add(-bombOn.value);
        

    }

}

[Serializable]
public class StatusEffect
{

    public int value;
    public TextMeshProUGUI textObject;

    public void Add(int amount)
    {
        value += amount;
        textObject.text = value.ToString();
        textObject.transform.parent.gameObject.SetActive(value > 0);
    }
}
