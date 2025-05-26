using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardSaveData 
{
    // --- Identity ---
    public int baseID;       // ID from the original card
    public int runtimeID;    // Unique per-session ID (optional)
    public string cardType;  // e.g., "UnitCard", "UseCard", etc.
    public bool manualDescription;
    public string manualDescriptionText;
    // --- Core CardBase Data ---
    public int fieldIndex;
    public string presetTag;
    public string nameText;
    public Sprite picture;
    public Sprite background;
    public Sprite border;
    public int currentCharmAmount;

    // --- Give Stats (Powerups) ---
    public int numOfAttacksGive;
    public int attackGive;
    public int healthGive;
    public int timerGive;

    public int shieldGive;
    public int snowGive;
    public int fireGive;
    public int crystalGive;
    public int poisonGive;
    public int pepperGive;
    public int curseGive;
    public int reflectGive;
    public int hazeGive;
    public int bombGive;
    public int inkGive;
    public int demonizeGive;

    // --- UnitCard-Specific ---
    public int maxAtkTimer;
    public int maxHealth;
    public bool isBoss;
    public bool hasCrown;
    public bool hasDied;

    // --- Runtime Status Flags ---
    public int shieldOn;
    public int snowOn;
    public int fireOn;
    public int crystalOn;
    public int pepperOn;
    public int poisonOn;
    public int curseOn;
    public int reflectOn;
    public int hazeOn;
    public int bombOn;
    public int inkOn;
    public int demonizeOn;

    // ---Off stats ---
    public int attack;
    public int numOfAttacks;

    //---STATS targets
    public Targeting healthTarget;
    public Targeting attackTarget;
    public Targeting numOfAtkTarget;
    public Targeting timerTarget;
    public Targeting shieldTarget;
    public Targeting snowTarget;
    public Targeting fireTarget;
    public Targeting crystalTarget;
    public Targeting poisonTarget;
    public Targeting pepperTarget;
    public Targeting curseTarget;
    public Targeting reflectTarget;
    public Targeting hazeTarget;
    public Targeting bombTarget;
    public Targeting inkTarget;
    public Targeting demonizeTarget;
    //---Specials--
    public bool hasLifesteal;
    public bool hasSpawnOnDeath;
    public List<int> spawnsOnDeath;
    public bool hasSelfTargetPosEffects;
    public bool hasEverburnResistance;
    public bool hasPoisonResistance;
    public bool hasBarrage;
    public bool hasSmackback;
    public bool hasDieReaction;
    public bool hasAttackReaction;
    public bool hasLongshot;
    public bool hasAimless;
    public bool hasBuffFriendly;
    public bool hasConsume;
}
