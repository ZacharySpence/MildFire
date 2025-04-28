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

    // --- Core CardBase Data ---
    public int fieldIndex;
    public string presetTag;
    public string nameText;
    public Sprite picture;
    public Sprite background;
    public Sprite border;

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

    // --- Runtime Status Flags ---
    public int shieldOn;
    public int crystalOn;
    public int pepperOn;

    // ---Off stats ---
    public int attack;
    public int numOfAttacks;

    //---Specials--
    public bool hasLifesteal;
    public bool hasSpawnOnDeath;
    public List<int> spawnsOnDeath;
    public bool hasSelfTargetPosEffects;
}
