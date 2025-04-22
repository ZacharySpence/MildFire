using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DeckHolder : MonoBehaviour
{
    //Stores all decks in JSON
    public static List<int> LoadDeck(string name, int id)
    {
        string json = File.ReadAllText(Application.persistentDataPath + $"/{name}{id}.json");
        IntListWrapper wrapper = JsonUtility.FromJson<IntListWrapper>(json);
        List<int> loadedInts = wrapper.values;
        return loadedInts;
        
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
