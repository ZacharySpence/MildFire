using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TooltipHandler : MonoBehaviour
{
    public bool hasShield
    {
        set { tooltipPrefabs.First(obj => obj.name == "Shield").SetActive(value); }
    }
    public bool hasSnow
    {
        set { tooltipPrefabs.First(obj => obj.name == "Snow").SetActive(value); }
    }
    public bool hasFire
    {
        set { tooltipPrefabs.First(obj => obj.name == "Everburn").SetActive(value); }
    }
    public bool hasPoison
    {
        set { tooltipPrefabs.First(obj => obj.name == "Poison").SetActive(value); }
    }
    public bool hasCrystal
    {
        set { tooltipPrefabs.First(obj => obj.name == "Crystal").SetActive(value); }
    }
    public bool hasPepper
    {
        set { tooltipPrefabs.First(obj => obj.name == "Pepper").SetActive(value); }
    }
    public bool hasCurse
    {
        set { tooltipPrefabs.First(obj => obj.name == "Curse").SetActive(value); }
    }
    public bool hasReflect
    {
        set { tooltipPrefabs.First(obj => obj.name == "Reflect").SetActive(value); }
    }
    public bool hasHaze
    {
        set { tooltipPrefabs.First(obj => obj.name == "Haze").SetActive(value); }
    }
    public bool hasBomb
    {
        set { tooltipPrefabs.First(obj => obj.name == "Bomb").SetActive(value); }
    }
    public bool hasInk
    {
        set { tooltipPrefabs.First(obj => obj.name == "Ink").SetActive(value); }
    }
    public bool hasDemonize
    {
        set { tooltipPrefabs.First(obj => obj.name == "Demonize").SetActive(value); }
    }
    public bool hasEverburnResistance
    {
        set { tooltipPrefabs.First(obj => obj.name == "EverburnResistance").SetActive(value); }
    }
    public bool hasPoisonResistance
    {
        set { tooltipPrefabs.First(obj => obj.name == "PoisonResistance").SetActive(value); }
    }
    public bool hasSmackback
    {
        set { tooltipPrefabs.First(obj => obj.name == "Smackback").SetActive(value); }
    }
    public bool hasLongshot
    {
        set { tooltipPrefabs.First(obj => obj.name == "Longshot").SetActive(value); }
    }
    public bool hasSpawnOnDeath
    {
        set { tooltipPrefabs.First(obj => obj.name == "SpawnOnDeath").SetActive(value); }
    }
    public bool hasLifesteal
    {
        set { tooltipPrefabs.First(obj => obj.name == "Lifesteal").SetActive(value); }
    }
    public bool hasConsume
    {
        set { tooltipPrefabs.First(obj => obj.name == "Consume").SetActive(value); }
    }
    public bool hasAimless
    {
        set { tooltipPrefabs.First(obj => obj.name == "Aimless").SetActive(value); }
    }
    public bool hasBarrage
    {
        set { tooltipPrefabs.First(obj => obj.name == "Barrage").SetActive(value); }
    }
    public bool hasFrenzy
    {
        set { tooltipPrefabs.First(obj => obj.name == "Frenzy").SetActive(value); }
    }

    [SerializeField] GameObject[] tooltipPrefabs;

}
