using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor.Animations;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI.Table;
using Random = UnityEngine.Random;
public class UnitCard :CardBase
{
    [SerializeField] OffensiveStats offStats;
    public StatsData statsData;
    [SerializeField] Transform parentTransform; //so is it in hand or on field somewhere
    




    public bool isDead, isBoss,hasCrown, hasDoneTimer, hasDied;
   
    [Header("Stats")]
    public int currentAtkTimer, currentHealth;
    [SerializeField] public int maxAtkTimer, maxHealth;
    //possible add in 'start' with so has start values separate to On which is in-game!
    [SerializeField] StatusEffect shieldOn,snowOn, fireOn, crystalOn, poisonOn, pepperOn, curseOn,reflectOn, hazeOn,bombOn,inkOn,demonizeOn;
    [Header("UnitSpecific Specials")]
    public bool hasEverburnResistance;
    public bool hasPoisonResistance, hasSmackback, hasLongshot, hasSpawnOnDeath;
    public Targeting healthTarget, attackTarget, numOfAtkTarget, timerTarget,
        shieldTarget, snowTarget, fireTarget, crystalTarget, poisonTarget, pepperTarget,
        curseTarget, reflectTarget, hazeTarget, bombTarget, inkTarget, demonizeTarget;
    

    [Header("Stat visuals")]
    [SerializeField] TextMeshProUGUI cAttackTimerText   ;
    [SerializeField] TextMeshProUGUI cHealthText;
    [SerializeField] Slider currentHealthSlider;
    [SerializeField] GameObject poisonRes, everburnRes, smackback;
    //shield = ice (block dmg/fire damage, snow = stop enemy timer (reduced by globalFireLvl) , fire = damage/turn, crystal = total atk block , poison = damage/turn ignoring shield, pepper = bonus atk/turn, curse = -ve bonus atk/turn

    [Header("Animators")]
    [SerializeField] Animator timerAnimator;
    [SerializeField] AnimatorController timerController;

    [Header("Flags")]
    bool hideTimer;
    bool hasAttacked;
    
    protected override void Awake()
    {
        base.Awake();
        tag = presetTag; //set it's tag!
        if (ID == -1) { return; } //don't set anything else if it's empty!
        currentAtkTimer = maxAtkTimer;
        currentHealth = maxHealth;
        currentHealthSlider.maxValue = maxHealth;
        ChangeStatus();
        //specifics needing to turn off!
        snowOn.Add(0);
        fireOn.Add(0);
        if(timerAnimator.runtimeAnimatorController == null)
        {
            timerAnimator.runtimeAnimatorController = timerController;
        }
    }
 //--SETUP--
    public override CardSaveData CreateCardSaveData()
    {
        return new CardSaveData
        {
            baseID = ID,
            //runtimeID = CardI
            cardType = "UnitCard",
            manualDescription = this.manualDescription,
            manualDescriptionText = this.manualDescriptionText,

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
            hasDied = this.hasDied,

            //+ve effects
            shieldOn = this.shieldOn.value,
            crystalOn = this.crystalOn.value,
            pepperOn = this.pepperOn.value,

            //Stat targets
            healthTarget = this.healthTarget,
            attackTarget = this.attackTarget,
            numOfAtkTarget = this.numOfAtkTarget,
            timerTarget = this.timerTarget,
            shieldTarget = this.shieldTarget,
            snowTarget = this.snowTarget,
            fireTarget = this.fireTarget,
            crystalTarget = this.crystalTarget,
            poisonTarget = this.poisonTarget,
            pepperTarget = this.pepperTarget,
            curseTarget = this.curseTarget,
            reflectTarget = this.reflectTarget,
            hazeTarget = this.hazeTarget,
            bombTarget = this.bombTarget,
            inkTarget = this.inkTarget,
            demonizeTarget = this.demonizeTarget,

            //specialAbilities
            hasLifesteal = this.hasLifesteal,
            hasSpawnOnDeath = this.hasSpawnOnDeath,
            spawnsOnDeath = this.spawnsOnDeath,
            hasEverburnResistance = this.hasEverburnResistance,
            hasPoisonResistance = this.hasPoisonResistance,
            hasBarrage = this.hasBarrage,
            hasSmackback = this.hasSmackback,
           
            hasLongshot = this.hasLongshot,
            hasAimless = this.hasAimless,
            hasBuffFriendly = this.hasBuffFriendly
        };


    }

    
    public override void SetupUsingCardSaveData(CardSaveData cardSaveData)
    {// Set all the fields from the save data back to the card

        base.SetupUsingCardSaveData(cardSaveData);
        this.manualDescription = cardSaveData.manualDescription;
        this.manualDescriptionText = cardSaveData.manualDescriptionText;


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
        this.hasDied = cardSaveData.hasDied;

        //Apply positive effects
        this.shieldOn.value = cardSaveData.shieldOn;
        this.crystalOn.value = cardSaveData.crystalOn;
        this.reflectOn.value = cardSaveData.reflectOn;
        this.pepperOn.value = cardSaveData.pepperOn;
        
        //Apply negative effects
        this.poisonOn.value = cardSaveData.poisonOn;
        this.snowOn.value = cardSaveData.snowOn;
        this.fireOn.value = cardSaveData.fireOn;
        this.curseOn.value = cardSaveData.curseOn;
        this.hazeOn.value = cardSaveData.hazeOn;
        this.bombOn.value = cardSaveData.bombOn;
        this.inkOn.value = cardSaveData.inkOn;
        this.demonizeOn.value = cardSaveData.demonizeOn;

        //ApplyStatTargets
        healthTarget = cardSaveData.healthTarget;
        attackTarget = cardSaveData.attackTarget;
        numOfAtkTarget = cardSaveData.numOfAtkTarget;
        timerTarget = cardSaveData.timerTarget;
        shieldTarget = cardSaveData.shieldTarget;
        snowTarget = cardSaveData.snowTarget;
        fireTarget = cardSaveData.fireTarget;
        crystalTarget = cardSaveData.crystalTarget;
        poisonTarget = cardSaveData.poisonTarget;
        pepperTarget = cardSaveData.pepperTarget;
        curseTarget = cardSaveData.curseTarget;
        reflectTarget = cardSaveData.reflectTarget;
        hazeTarget = cardSaveData.hazeTarget;
        bombTarget = cardSaveData.bombTarget;
        inkTarget = cardSaveData.inkTarget;
        demonizeTarget = cardSaveData.demonizeTarget;

        //Apply special effects
        this.hasLifesteal = cardSaveData.hasLifesteal;
        this.hasSpawnOnDeath = cardSaveData.hasSpawnOnDeath;
        this.spawnsOnDeath = cardSaveData.spawnsOnDeath;
        this.hasEverburnResistance = cardSaveData.hasEverburnResistance;
        this.hasPoisonResistance = cardSaveData.hasPoisonResistance;
        this.hasBarrage = cardSaveData.hasBarrage;
        this.hasSmackback = cardSaveData.hasSmackback;
        
        this.hasLongshot = cardSaveData.hasLongshot;
        this.hasAimless = cardSaveData.hasAimless;
        this.hasBuffFriendly = cardSaveData.hasBuffFriendly;

        //Setup rest
        name = nameText;
        offStats = GetComponent<OffensiveStats>();
        offStats.Setup(statsData);
        cAttackTimerText.text = $"{currentAtkTimer}";
       
        currentAtkTimer = maxAtkTimer;
        currentHealth = hasDied?maxHealth/2 :maxHealth; //so if have died before then make it half health
        currentHealthSlider.maxValue = maxHealth;
        if (hasPoisonResistance)
        {
            poisonRes.SetActive(true);
        }
        if (hasEverburnResistance)
        {
            everburnRes.SetActive(true);
        }
        if (hasSmackback)
        {
            smackback.SetActive(true);
        }
        if(maxAtkTimer == 0) //so no attack timer
        {
            cAttackTimerText.transform.parent.gameObject.SetActive(false);
        }
        ChangeStatus();
        if (!manualDescription)
        {
            CreateCardDescription(); //make one from scratch
        }
        else
        {
           //rework the manual description to include sprites
            var textList = manualDescriptionText.Split(" ").ToList();
            cardDescription.text = desc.CreateDescription(textList);
        }
        

        hideTimer = WorldManager.Instance.omnisciBlessing ? false : WorldManager.Instance.omnisciForesight;


    }

    public override void CreateCardDescription()
    {
        text.Clear(); //so empty it out!
        //left tooltips
        if (shieldGive > 0)
        {
       
            text.Add($"Apply {shieldGive} shield {(shieldTarget.other && hasBuffFriendly ?"to an ally":" ")}{(shieldTarget.self?"to self":"")}");
            tooltipHandler.hasShield = true;
        }
        if (crystalGive > 0)
            {
                text.Add($"Apply {crystalGive} crystal {(crystalTarget.other && hasBuffFriendly ?"to an ally":" ")}{(crystalTarget.self?"to self":"")}");
            tooltipHandler.hasCrystal = true;
        }
        if (bombGive > 0)
        {
            text.Add($"Apply {bombGive} bomb {(bombTarget.other && hasBuffFriendly ? "to an ally" :" ")}{(bombTarget.self ? "to self" : "")}");
            tooltipHandler.hasBomb = true;
        }
        if (inkGive > 0)
        {
            text.Add($"Apply {inkGive} ink {(inkTarget.other && hasBuffFriendly ? "to an ally" :" ")}{(inkTarget.self ? "to self" : "")}");
            tooltipHandler.hasInk = true;
        }
       
        //right tooltips
        if (snowGive > 0)
        {
            text.Add($"Apply {snowGive} snow {(snowTarget.other && hasBuffFriendly ? "to an ally" : " ")}{(snowTarget.self ? "to self" : "")}");
            tooltipHandler.hasSnow = true;
        }
        if (fireGive > 0)
        {
            text.Add($"Apply {fireGive} fire {(fireTarget.other && hasBuffFriendly ? "to an ally" : " ")}{(fireTarget.self ? "to self" : "")}");
            tooltipHandler.hasFire = true;
        }
        if (poisonGive > 0)
        {
            text.Add($"Apply {poisonGive} poison {(poisonTarget.other && hasBuffFriendly ? "to an ally" : " ")}{(poisonTarget.self ? "to self" : "")}");
            tooltipHandler.hasPoison = true;
        }
        if (pepperGive > 0)
        {
            text.Add($"Apply {pepperGive} pepper");
            tooltipHandler.hasPepper = true;
        }
        if (curseGive > 0)
        {
            text.Add($"Apply {curseGive} curse {(curseTarget.other && hasBuffFriendly ? "to an ally" : " ")}{(curseTarget.self ? "to self" : "")}");
            tooltipHandler.hasCurse = true; }
        if (demonizeGive > 0)
        {
            text.Add($"Apply {demonizeGive} demonize {(demonizeTarget.other && hasBuffFriendly ? "to an ally" : " ")}{(demonizeTarget.self ? "to self" : "")}");
            tooltipHandler.hasDemonize = true;
        }
        if (reflectGive > 0)
        {
            text.Add($"Apply {reflectGive} reflect {(reflectTarget.other && hasBuffFriendly ? "to an ally" : " ")}{(reflectTarget.self ? "to self" : "")}");
            tooltipHandler.hasReflect = true;
        }
        if (hazeGive > 0)
        {
            text.Add($"Apply {hazeGive} haze {(hazeTarget.other && hasBuffFriendly ? "to an ally" : " ")}{(hazeTarget.self ? "to self" : "")}");
            tooltipHandler.hasHaze = true;
        }
        if (numOfAttacksGive > 0)
        {
            text.Add($"Increase {(numOfAtkTarget.other ? (hasBuffFriendly ? "ally" : "target") : "")}{(numOfAtkTarget.self ? "own" : "")} frenzy by {numOfAttacksGive}");
            tooltipHandler.hasFrenzy = true;
        }

       

        if (attackGive > 0) { text.Add($"Increase {(attackTarget.other ? (hasBuffFriendly?"ally": "target") : "")}{(attackTarget.self ? "own" : "")} attack by {attackGive} "); }
        if (healthGive > 0) { text.Add($"Heal {(healthTarget.other ? (hasBuffFriendly ? "ally" : "target") : "")}{(healthTarget.self ? "" : "")} {healthGive} health"); }
        if (timerGive < 0) { text.Add($"Reduce {(timerTarget.other ? (hasBuffFriendly ? "ally" : "target") : "")}{(timerTarget.self ? "own" : "")} timer by {timerGive}"); } //Specifically make timer negative!

        //Special Abilities
        if(offStats.currentNumOfAttacks > 1)
        {
            text.Add($"<color=yellow>Frenzy</color>");
            tooltipHandler.hasFrenzy = true;
        }
        if (hasBarrage) { 
            text.Add($"<color=yellow>Barrage</color>");
            tooltipHandler.hasBarrage = true;
        }
        if (hasLifesteal) { text.Add($"<color=yellow>Lifesteal</color>");
            tooltipHandler.hasLifesteal = true;
        }
        if (hasAimless) {
            text.Add($"<color=yellow>Aimless</color>");
            tooltipHandler.hasAimless = true;
        }
        if (hasConsume) {
            text.Add($"<color=yellow>Consume</color>");
            tooltipHandler.hasConsume = true;
        }
        if (hasEverburnResistance) {
            text.Add($"<color=yellow>Everburn Resistance</color>");
            tooltipHandler.hasEverburnResistance = true; }
        if (hasLongshot)
        {
            text.Add($"<color=yellow>Longshot</color>");
            tooltipHandler.hasLongshot = true;
        }
        if (hasPoisonResistance)
        {
            text.Add($"<color=yellow>Poison Resistance</color>");
            tooltipHandler.hasPoisonResistance = true; }
        if (hasSmackback)
        {
            text.Add($"<color=yellow>Smackback</color>");
            tooltipHandler.hasSmackback = true;
        }
        if (hasSpawnOnDeath)
        {
            text.Add($"<color=yellow>Spawn</color>");
            tooltipHandler.hasSpawnOnDeath = true;
        }
        DisableTooltips();
        cardDescription.text = string.Join(" ", text);
        var textList = cardDescription.text.Split(" ").ToList();
        cardDescription.text = desc.CreateDescription(textList);
        
    }

    //Card Auto logic methods
    public IEnumerator ReduceTimer()
    {
        yield return new WaitForSeconds(.3f); //wait a second before starting?
        if(fieldIndex < 6)
        {
            hideTimer = false; //so never hide timer for player! Or maybe do who knows
            hideTimer = false; //so never hide timer for player! Or maybe do who knowsc
        }
        hasDoneTimer = false;
        //ANIMATIONS HERE!
        if(snowOn.value > 0)
        {
            snowOn.Add(-1);
        }
        else
        {
            timerAnimator.Play("countdown");
            //Animate here!
            currentAtkTimer--;
            
        }
        yield return StartCoroutine(CheckTimerAtZeroCoroutine());
        
        DoEndTurnStatusEffectChange();
    }
    IEnumerator CheckTimerAtZeroCoroutine()
    {
        timerAnimator.SetBool("atOne", false);
        if (currentAtkTimer <= 0)
        {
            for (int i = 0; i < offStats.currentNumOfAttacks; i++)
            {
                yield return StartCoroutine(TryAttack());
            }

            currentAtkTimer += maxAtkTimer;
            cAttackTimerText.transform.parent.GetComponent<Image>().color = Color.white;
            yield return StartCoroutine(CheckTimerAtZeroCoroutine()); //so if i decrease it enough to have it do 2 attacks! (o
            
        }
        else if(currentAtkTimer == 1)
        {
            //colour it red and animate to go faster
            if (!hideTimer)
            {
                cAttackTimerText.transform.parent.GetComponent<Image>().color = Color.red;
                timerAnimator.SetBool("atOne", true);
            }
            
            BattleManager.Instance.cardFullyFinished = true;
        }
        else
        {
            BattleManager.Instance.cardFullyFinished = true; //always gets to here or the == 1 thanks to the recursive the top!
        }
        if (!hideTimer)
        {
            cAttackTimerText.text = $"{currentAtkTimer}";
            offStats.cAttackAmountText.text = $"x{offStats.currentNumOfAttacks}";
        }
       
    }
  

    void DoEndTurnStatusEffectChange()
    {
        //End turn status change
        //fire, poison, ink
        if (fireOn.value > 0)
        {
            TakeDamage(fireOn.value, false,true); //ignores crystal but not shield          
        }
        if(poisonOn.value > 0)
        {
            Debug.Log("poison changing");
            TakeDamage(poisonOn.value,true, false); //ignores shield but not crystal
            poisonOn.Add(-1);
        }
        if(inkOn.value > 0)
        {
            inkOn.Add(-1);
        }
     
        //Status Change only after attacking
        if (hasAttacked)
        {
          
            curseOn.Add(-curseOn.value); //remove all curse after using it!
            pepperOn.Add(-pepperOn.value); //remove all pepper after using it
            offStats.ChangeOffStats(curseOn.value); //change attack back to what it was before adding pepper/curse

            if (hazeOn.value > 0)
            {
                hazeOn.value -= 1;
            }
            hasAttacked = false;
          
        }
        
    }
    #region Combat
    //Just basic attack row enemy

    public IEnumerator TryAttack()
    {
      
        
        bool isPlayer = fieldIndex < 6;
        //Get whether it's player, then find row and closest enemy in row
        int row = fieldIndex % 2;

        if (hasBuffFriendly || hazeOn.value > 0) //so target allies 
        {
            if (hasBarrage && inkOn.value == 0)
            {
                DoBarrageAnim();
                var allies = Barrage(row, !isPlayer);
                if(allies.Count > 0)
                {
                    hasAttacked = true;
                    foreach (var ally in allies)
                    {
                        if (ally)
                            if (hazeOn.value > 0)
                            {
                                hazeOn.value = Math.Clamp(hazeOn.value - 1, 0, 100);
                                DoAttack(ally);
                            }
                            else
                            {
                                DoBuff(ally); //do a different coroutine for this one! (maybe barrage attack specific
                            }
                    }
                }
                Debug.Log("changing status post attack");
                //so anything self-targeting happens after entire barrage
                ChangeStatus(healthTarget.self ? healthGive : 0, attackTarget.self ? attackGive : 0, numOfAtkTarget.self ? numOfAttacksGive : 0,
                timerTarget.self ? timerGive : 0, snowTarget.self ? snowGive : 0, poisonTarget.self ? poisonGive : 0, fireTarget.self ? fireGive : 0,
                curseTarget.self ? curseGive : 0, shieldTarget.self ? shieldGive : 0, reflectTarget.self ? reflectGive : 0, hazeTarget.self ? hazeGive : 0,
                inkTarget.self ? inkGive : 0, bombTarget.self ? bombGive : 0, demonizeTarget.self ? demonizeGive : 0, pepperTarget.self ? pepperGive : 0,
                crystalTarget.self ? crystalGive : 0);
                yield return null;
            }
            else
            {
                var ally = FindNearestEnemy(row, !isPlayer); //basically get nearest ally -> if at front then do on self
                if (ally != null)
                {
                    hasAttacked = true;
                    if(hazeOn.value > 0)
                    {
                        yield return StartCoroutine(DoAttackAnim(ally));
                    }
                    else
                    {
                        yield return StartCoroutine(DoBuffAnim(ally));
                    }
                   
                }
            }
            if (hasAttacked)
            {
                Debug.Log("changing status post attack");
                //so anything self-targeting happens after entire barrage
                ChangeStatus(healthTarget.self ? healthGive : 0, attackTarget.self ? attackGive : 0, numOfAtkTarget.self ? numOfAttacksGive : 0,
                timerTarget.self ? timerGive : 0, snowTarget.self ? snowGive : 0, poisonTarget.self ? poisonGive : 0, fireTarget.self ? fireGive : 0,
                curseTarget.self ? curseGive : 0, shieldTarget.self ? shieldGive : 0, reflectTarget.self ? reflectGive : 0, hazeTarget.self ? hazeGive : 0,
                inkTarget.self ? inkGive : 0, bombTarget.self ? bombGive : 0, demonizeTarget.self ? demonizeGive : 0, pepperTarget.self ? pepperGive : 0,
                crystalTarget.self ? crystalGive : 0);
            }
        }
        else
        {
            if (hasBarrage && inkOn.value == 0)
            {
                DoBarrageAnim();
                var enemies = Barrage(row, isPlayer);
                if(enemies.Count > 0)
                {
                    hasAttacked = true;
                    foreach (var enemy in enemies)
                    {

                        DoAttack(enemy); //do a different coroutine for this one! (maybe barrage attack specific
                        

                    }
                }
                Debug.Log("changing status post attack");
                //so anything self-targeting happens after entire barrage
                ChangeStatus(healthTarget.self ? healthGive : 0, attackTarget.self ? attackGive : 0, numOfAtkTarget.self ? numOfAttacksGive : 0,
                timerTarget.self ? timerGive : 0, snowTarget.self ? snowGive : 0, poisonTarget.self ? poisonGive : 0, fireTarget.self ? fireGive : 0,
                curseTarget.self ? curseGive : 0, shieldTarget.self ? shieldGive : 0, reflectTarget.self ? reflectGive : 0, hazeTarget.self ? hazeGive : 0,
                inkTarget.self ? inkGive : 0, bombTarget.self ? bombGive : 0, demonizeTarget.self ? demonizeGive : 0, pepperTarget.self ? pepperGive : 0,
                crystalTarget.self ? crystalGive : 0);
                yield return null;

            }
            else if(hasAimless && inkOn.value == 0)
            {
                var enemy = ChooseAimlessTarget(isPlayer);
                if(enemy != null)
                {
                    hasAttacked = true;
                    yield return StartCoroutine(DoAttackAnim(enemy));
                }
            }
            else if(hasLongshot && inkOn.value == 0)
            {
                var enemy = FindFurthestEnemy(row, isPlayer); //top row if it's 0, bottom if it's 1
                if (enemy != null)
                {
                    hasAttacked = true;
                    yield return StartCoroutine(DoAttackAnim(enemy));
                }
            }
            else
            {
                var enemy = FindNearestEnemy(row, isPlayer); //top row if it's 0, bottom if it's 1
                if (enemy != null)
                {
                    hasAttacked = true;
                    yield return StartCoroutine(DoAttackAnim(enemy));
                }

            }
            
        }
      
        
       
        
    }

   
    void DoBarrageAnim()
    {
        Debug.Log("doing barrage Animation");
    }
    IEnumerator DoAttackAnim(UnitCard enemy)
    {
        Debug.Log("doing attack anim");
        Transform target = enemy.transform;
        // Store original position
        Vector3 origin = transform.position;
        float moveDuration = 0.2f;  // Duration for quick move towards and back
        // Calculate the distance to target and speed for quick movement
        float journeyLength = Vector3.Distance(origin, target.position);
        float moveSpeed = journeyLength / moveDuration;
        float distanceCovered = 0f;

        offStats.ChangeOffStats(pepperOn.value - curseOn.value);//change to reflect pepper addition to attack both logically and visually!
        yield return new WaitForSeconds(0.5f); //smalle delay BEFORE attacking
        // Move towards the target (stop 10% before target)
        while (distanceCovered < journeyLength * 0.75f)
        {
            distanceCovered += moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        DoAttack(enemy);
        // Move back to the original position (stop 10% before origin)
        distanceCovered = 0f;
        while (distanceCovered < journeyLength * 0.9f)
        {
            distanceCovered += moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, origin, moveSpeed * Time.deltaTime);
            yield return null;
        }
        

    }

    void DoAttack(UnitCard enemy)
    {       
        int reflectValue = enemy.reflectOn.value;

        
        Debug.Log($"{name} attacked {enemy.name} for {offStats.currentAttack} damage");

        enemy.TakeDamage(offStats.currentAttack, false, false,
            healthTarget.other ? healthGive : 0, attackTarget.other ? attackGive : 0, numOfAtkTarget.other ? numOfAttacksGive : 0,
            timerTarget.other ? timerGive : 0, snowTarget.other ? snowGive : 0, poisonTarget.other ? poisonGive : 0,
            fireTarget.other ? fireGive : 0, curseTarget.other ? curseGive : 0, shieldTarget.other ? shieldGive : 0,
            reflectTarget.other ? reflectGive : 0, hazeTarget.other ? hazeGive : 0, inkTarget.other ? inkGive : 0,
            bombTarget.other ? bombGive : 0, demonizeTarget.other ? demonizeGive:0, pepperTarget.other ? pepperGive:0,
            crystalTarget.other ? crystalGive:0);
        

        if (reflectValue > 0) //take reflected damage even if kill enemy
        {
            TakeDamage(reflectValue);
        }
        //special effects
        if (inkOn.value == 0) //only do special effects if have no ink on
        {
            if (hasLifesteal)
            {
                Heal(offStats.currentAttack);
            }
            if (enemy.hasSmackback && !enemy.isDead) //don't smackback if they're dead
            {
                enemy.Smackback(this);
            }
        }
        if (!hasBarrage)
        {
            //so after attacking get bonus
            ChangeStatus(healthTarget.self ? healthGive : 0, attackTarget.self ? attackGive : 0, numOfAtkTarget.self ? numOfAttacksGive : 0,
                    timerTarget.self ? timerGive : 0, snowTarget.self ? snowGive : 0, poisonTarget.self ? poisonGive : 0, fireTarget.self ? fireGive : 0,
                    curseTarget.self ? curseGive : 0, shieldTarget.self ? shieldGive : 0, reflectTarget.self ? reflectGive : 0, hazeTarget.self ? hazeGive : 0,
                    inkTarget.self ? inkGive : 0, bombTarget.self ? bombGive : 0, demonizeTarget.self ? demonizeGive : 0, pepperTarget.self ? pepperGive : 0,
                    crystalTarget.self ? crystalGive : 0);
        }


    }
    IEnumerator DoBuffAnim(UnitCard ally)
    {
        Debug.Log("doing Buff anim");


        DoBuff(ally);
        yield return null;
        //do a shake instead of move
      

    }
    void DoBuff(UnitCard ally)
    {
        ally.ChangeStatus(healthTarget.other ? healthGive : 0, attackTarget.other ? attackGive : 0, numOfAtkTarget.other ? numOfAttacksGive : 0,
            timerTarget.other ? timerGive : 0, snowTarget.other ? snowGive : 0, poisonTarget.other ? poisonGive : 0,
            fireTarget.other ? fireGive : 0, curseTarget.other ? curseGive : 0, shieldTarget.other ? shieldGive : 0,
            reflectTarget.other ? reflectGive : 0, hazeTarget.other ? hazeGive : 0, inkTarget.other ? inkGive : 0,
            bombTarget.other ? bombGive : 0, demonizeTarget.other ? demonizeGive : 0, pepperTarget.other ? pepperGive : 0,
            crystalTarget.other ? crystalGive : 0);
    }

    //Maybe put buff/dmg together so can have like heal that adds pepper? (got to play around with this!)
    public void ChangeStatus(int healthAdded = 0,int attackAdded = 0, int numAttksAdded = 0, 
        int timerAdded = 0, int snowAdded = 0, int poisonAdded = 0, 
        int fireAdded = 0, int curseAdded = 0, int shieldAdded = 0, 
        int reflectAdded = 0, int hazeAdded = 0, int inkAdded = 0, 
        int bombAdded = 0, int demonizeAdded = 0,int pepperAdded = 0, 
        int crystalAdded = 0)
    {
       
        //-ve
       
        hazeOn.Add(hazeAdded);
        inkOn.Add(inkAdded);
        bombOn.Add(bombAdded);
        demonizeOn.Add(demonizeAdded);
        curseOn.Add(curseAdded);

        if (hasPoisonResistance)
        {
            poisonOn.value = Math.Clamp(poisonOn.value + poisonAdded, 0, 1);
            poisonOn.Add(0);
        }
        else
        {
            poisonOn.Add(poisonAdded);
        }

        //Snow and fire
       
        if (snowAdded > 0 || fireAdded > 0) //1. check if adding fire or snow
        {
            //2. check if more fire than snow overall
            int fireRemaining = (fireOn.value+fireAdded) - (snowOn.value+snowAdded);
            Debug.Log("FIRE REMAINING:" + fireRemaining);
            if(fireRemaining > 0) //2.1 if more fire than snow overall then
            {
                //remove all current snow and add fire
                
                if (hasEverburnResistance)
                {
                    fireOn.value = Math.Clamp(fireOn.value + fireAdded, 0, 1);
                    fireOn.Add(0);
                }
                else
                {

                    fireOn.Add(fireAdded - (snowOn.value+snowAdded));
                }
                snowOn.Add(-snowOn.value);
            }
            else //2.2 more snow than fire
            {
                snowOn.Add(-fireOn.value+snowAdded); // reduce snow by current fire amount then add on extra
                fireOn.Add(-fireOn.value); //remove all current fire
            }
        }
       
        //
        //+ve
        shieldOn.Add(shieldAdded);
        reflectOn.Add(reflectAdded);
        pepperOn.Add(pepperAdded);
        crystalOn.Add(crystalAdded);
        Heal(healthAdded);
        //offenseStatSpecific
        offStats.ChangeOffStats(curseOn.value,attackAdded,numAttksAdded);
        currentAtkTimer += timerAdded;
        cAttackTimerText.text = $"{currentAtkTimer}"; //just to update the frenzy part!
        if (timerAdded < 0)
        {
            StartCoroutine(CheckTimerAtZeroCoroutine());
        }
       
    }
    public void TakeDamage(int damage=0,bool ignoreShield = false,bool ignoreCrystal = false, 
        int healthAdded = 0, int attackAdded = 0, int numAttksAdded = 0, 
        int timerAdded = 0,int snowAdded = 0, int poisonAdded = 0, 
        int fireAdded = 0, int curseAdded = 0, int shieldAdded = 0,
        int reflectAdded = 0, int hazeAdded = 0, int inkAdded = 0, 
        int bombAdded = 0, int demonizeAdded = 0,int pepperAdded = 0, 
        int crystalAdded = 0)
    {
        damage += bombOn.value; //add bomb value to damage

        //if demonize is > 0 then double damage and decrease demonize
        if(demonizeOn.value > 0)
        {
            damage *= 2;
            demonizeOn.Add(-1);
        }
        damage = Math.Clamp(damage, 0, 1000); //make sure damage is at least 0
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

    public void Heal(int healthAdded)
    {
        currentHealth = Mathf.Clamp(healthAdded + currentHealth, 0, maxHealth);
        currentHealthSlider.value = currentHealth;
        cHealthText.text = $"{currentHealth}";
    }
    void Die()
    {
        Debug.Log($"{name} died");
        isDead = true;
        
        bool isPlayer = fieldIndex < 6;
        bool movedCard = false;
        //Check if any allies to left of it, if so then for each ally move them -2 
        UnitCard[] fieldToSearch = isPlayer ? BattleManager.Instance.playerField : BattleManager.Instance.enemyField;

        int startIndex = fieldIndex + 2 - (isPlayer ? 0 : 6);
        for (int i = startIndex; i < 6; i += 2) //go through own row (AFTER itself
        {
                        
           
            if (fieldToSearch[i].ID != -1) //card in there
            {
                Debug.Log(fieldToSearch[i].fieldIndex + ":" + name);
                movedCard = true;
                BattleManager.Instance.MoveCard(i-2, isPlayer, fieldToSearch[i]); //move it back towards left!
            }
        }

        //only empty out if didn't move card at all (so no allies to take place!)
        if (!movedCard)
        {
            Debug.Log("NO ALLIES TO MOVE");
            BattleManager.Instance.EmptyOutField(fieldIndex,isPlayer);
        }

        //For animation: give it a rigidbody (now has gravity), impulse force a little up
        GetComponent<Collider2D>().enabled = false;
        if(gameObject.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.AddForce(new Vector2(0.25f, 1f), ForceMode2D.Impulse);
        }
        else
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.AddForce(new Vector2(0.25f, 1f), ForceMode2D.Impulse);
        }

   
      

        //special abilities
        if (hasSpawnOnDeath)
        {
            BattleManager.Instance.AddInNewSpawns(spawnsOnDeath, isPlayer); //if it's fieldIndex is less than 6 it's a player card!
        }

        if (isPlayer)
        {
            //update this on the player companion!
            var cardInDeck = IDLookupTable.instance.playerDeck.Find(x => x.baseID == ID);
            cardInDeck.hasDied = true;
        }

    }
    #endregion
    void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }


    //--SPECIAL ABILITY SPECIFICS
    List<UnitCard> Barrage(int row,bool isPlayer)
    {
        List<UnitCard> allUnitsToAttack = new List<UnitCard>();
        //if it's player += 2 until hit max in enemyField, else in playerfield
        UnitCard[] fieldToSearch = isPlayer ? BattleManager.Instance.enemyField : BattleManager.Instance.playerField;
        for (int i = row; i < 6; i += 2) //go through own row
        {
            if (fieldToSearch[i].ID != -1) //card in there
            {
                allUnitsToAttack.Add(fieldToSearch[i]);
            }
        }
        if(allUnitsToAttack.Count <= 0) //only if found none in a row then search next row
        {
            row ^= 1;
            for (int i = row; i < 6; i += 2) //go through other row
            {
                if (fieldToSearch[i].ID != -1) //card in there
                {
                    allUnitsToAttack.Add(fieldToSearch[i]);
                }
            }
        }
        return allUnitsToAttack;
       
    }
    public void Smackback(UnitCard enemy)
    {
        StartCoroutine(DoAttackAnim(enemy));
       
    }

    UnitCard ChooseAimlessTarget(bool isPlayer)
    {
       
        List<UnitCard> possibleTargets = new List<UnitCard>();
        foreach(var target in isPlayer?BattleManager.Instance.enemyField:BattleManager.Instance.playerField)
        {
            if(target.ID != -1)
            {
                possibleTargets.Add(target);
            }
        }
        if(possibleTargets.Count == 0)
        {
            return null;
        }
        else
        {
            return possibleTargets[Random.Range(0, possibleTargets.Count)]; //get a random target
        }
        
    }
    //--
    public UnitCard FindNearestEnemy(int row,bool isPlayer)
    {
        //if it's player += 2 until hit max in enemyField, else in playerfield
        UnitCard[] fieldToSearch = isPlayer ? BattleManager.Instance.enemyField : BattleManager.Instance.playerField;
        for(int i = row; i < 6; i += 2) //go through own row
        {
            if (fieldToSearch[i].ID != -1) //card in there
            {
                return fieldToSearch[i];
            }
        }
        row ^= 1;
        for (int i = row; i < 6; i += 2) //go through other row
        {
            if (fieldToSearch[i].ID != -1) //card in there
            {
                return fieldToSearch[i];
            }
        }
        return null; //should only return this if nothing to hit

    }
    public UnitCard FindFurthestEnemy(int row, bool isPlayer)
    {
        //if it's player += 2 until hit max in enemyField, else in playerfield
        UnitCard[] fieldToSearch = isPlayer ? BattleManager.Instance.enemyField : BattleManager.Instance.playerField;
        for (int i = 4+row; i >= 0; i -= 2) //go through own row starting at rightmost (so 4 for even, 5 for odd
        {
            if (fieldToSearch[i].ID != -1) //card in there
            {
                return fieldToSearch[i];
            }
        }
        row ^= 1;
        for (int i = 4+row; i >=0; i -= 2) //go through other row
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
    public override void SelectedView()
    {
        //Place in viewing spot in UI
        GameObject viewPanel = BattleManager.Instance.viewPanel;
        viewPanel.SetActive(true);

        UnitCard uCard =  Instantiate(this);
         BattleManager.Instance.uiViewCard = uCard;
        // uCard.transform.parent = viewPanel.transform;
        uCard.transform.position = Vector2.zero;
        uCard.GetComponentInChildren<Canvas>().sortingOrder = 5;
        uCard.transform.localScale *= 2f;
        uCard.EnableTooltips();

        
    }
    //--ON HOVER FEATURES (Move to separate script?)
    public void ViewOnHover()
    {
        EnableTooltips();
        transform.localScale *= 2f;
    }
    //

    


    //Good enough for now -> needs to be done via hovering insted of on-click
    public override bool TryPlaceOnField(int atIndex, bool isPlayerCard = false, UnitCard cardAlreadyThere = null, bool endCase = true)
    {
        if (cardAlreadyThere != null && cardAlreadyThere.ID != -1) //so not an empty
        {
            endCase = false;
            Debug.Log($"there is a card already here: {cardAlreadyThere.ID}");
            
            //get next index in row
            int newIndex = atIndex + 2;
            if (newIndex >= (isPlayerCard ? 6 : 12))
            {
                //Debug.Log("reached left most index"); //after hitting left most index
                return false;
            }
            //if it hasn't gone too far then try place the card that's already there on the next spot 
            else
            {
                Debug.Log($"Trying to place next card in row at index {newIndex}");
                if (cardAlreadyThere.TryPlaceOnField(newIndex,isPlayerCard,BattleManager.Instance.playerField[newIndex],endCase))
                {
                    if(atIndex < 2) //so indexes 0 or 1!
                    {
                        endCase = true;
                    }
                    Debug.Log("Placing on full field");
                    BattleManager.Instance.PlayerPlaceCardOnFullField(atIndex, this,endCase);
                   
                    return true;
                }
                return false;
            }
            
        }
        else
        {
            if (isPlayerCard && atIndex <= 5 || !isPlayerCard && atIndex >= 6)
            {
               
                BattleManager.Instance.PlayerPlaceCardOnEmptyField(atIndex, this, endCase);
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
        BattleManager.Instance.selectedCard = null;//just remove it since it should become missing
        Heal(5);// heal it and remove all status effects
        
        ClearAllNegativeStatusEffects();
        return true;
    }
   
    void ClearAllNegativeStatusEffects()
    {


        snowOn.Add(-snowOn.value);
        //fireOn.Add(-fireOn.value); //dont remove fire!
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
        value = Math.Clamp(amount+value,0,1000);
        textObject.text = value.ToString();
        textObject.transform.parent.gameObject.SetActive(value > 0);
    }
}

[Serializable]
public class Targeting
{
    public bool self;
    public bool other = true;
}
