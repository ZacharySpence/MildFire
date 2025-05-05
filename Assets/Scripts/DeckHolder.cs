using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DeckHolder : MonoBehaviour
{
    //Stores all decks in JSON
    public static List<EnemyCards> enemyDeck = new List<EnemyCards>();
    public static List<int> reserveRewardCards = new List<int>();
    public static List<int> itemRewardCards = new List<int>();
    public static List<int> companionRewardCards = new List<int>();
    public static List<int> LoadDeck(string name, int id)
    {
        string json = File.ReadAllText(Application.persistentDataPath + $"/{name}{id}.json");
        IntListWrapper wrapper = JsonUtility.FromJson<IntListWrapper>(json);
        List<int> loadedInts = wrapper.values;
        
        CreateReserves(name);
       

        return loadedInts;
        
    }
    public static void CreateReserves(string name = "Leader")
    {
        string json = "";
        string filePath = Application.persistentDataPath + $"/{name}Group.json";
        if (File.Exists(filePath))
        {
            json = File.ReadAllText(filePath);
           
        }
        else
        {
            json = File.ReadAllText(Application.persistentDataPath + $"/LeaderGroup.json");
        }
        GroupListWrapper reserves = JsonUtility.FromJson<GroupListWrapper>(json);
        reserveRewardCards = reserves.items;
        reserveRewardCards.AddRange(reserves.companions);
    }
    public static void LoadEnemyDeck(string name)
    {
        string path = Path.Combine(Application.persistentDataPath, $"{name}.json");
        string json = File.ReadAllText(path);

        IntListListWrapper wrapper = JsonUtility.FromJson<IntListListWrapper>(json);
        if (wrapper == null || wrapper.values == null)
        {
            Debug.LogError("Failed to load or parse enemy deck JSON.");
            return;
        }

        enemyDeck.Clear();
        foreach (var intList in wrapper.values)
        {
            EnemyCards enemyCards = new EnemyCards();
            enemyCards.cards = intList.values;
           // Debug.Log(enemyCards.cards.Count + ": count");
            enemyDeck.Add(enemyCards);
        }
        CreateRewardDecks(name);
    }
    public static void CreateRewardDecks(string name)
    {
        Debug.Log("making reward decks");
        string json = File.ReadAllText(Application.persistentDataPath + $"/{name}Group.json");
        GroupListWrapper wrapper = JsonUtility.FromJson<GroupListWrapper>(json);
        itemRewardCards = wrapper.items;
        companionRewardCards = wrapper.companions;

    }

}
[System.Serializable]
public class IntListWrapper
{
    public List<int> values;

    public IntListWrapper(List<int> values)
    {
        this.values = values;
    }
}

[Serializable]
public class IntListListWrapper
{
    public List<IntListWrapper> values;

    public IntListListWrapper(List<IntListWrapper> values)
    {
        this.values = values;
    }
}

[Serializable]
public class GroupListWrapper
{
    public List<int> items;
    public List<int> companions;

    public GroupListWrapper(List<int> items, List<int> companions)
    {
        this.items = items;
        this.companions = companions;
    }
}

