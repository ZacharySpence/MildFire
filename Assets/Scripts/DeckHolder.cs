using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DeckHolder : MonoBehaviour
{
    //Stores all decks in JSON
    public static List<EnemyCards> enemyDeck = new List<EnemyCards>();
    public static List<int> LoadDeck(string name, int id)
    {
        string json = File.ReadAllText(Application.persistentDataPath + $"/{name}{id}.json");
        IntListWrapper wrapper = JsonUtility.FromJson<IntListWrapper>(json);
        List<int> loadedInts = wrapper.values;
        return loadedInts;
        
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
        /*//Testing
        foreach(var cards in enemyDeck)
        {
           // Debug.Log("EDeck:");
            foreach(var card in cards.cards)
            {
                Debug.Log(card);
            }
        }*/


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

