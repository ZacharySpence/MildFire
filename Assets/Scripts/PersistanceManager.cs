using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class PersistanceManager 
{
    public static List<int> unlockedPlayerGroups = new List<int>();
    public static List<(int, int)> skullsOnID = new List<(int, int)>();
    public static List<NodeData> currentGameNodeData = new List<NodeData>(); //ADD IN SAVING/LOADING NODES FOR SAVE/LOAD FEATURES!
    //Add in skullCharms in storage, currentSkullCount, cardID's & bools for isDying.
    const string SaveKey = "UnlockedData";

    

    public static void SavePersistence()
    {
        UnlockedData data = new UnlockedData();
        data.unlockedPlayerGroups = unlockedPlayerGroups;

        foreach (var tuple in skullsOnID)
        {
            data.skullsOnID.Add(new SkullPersistentData(tuple.Item1, tuple.Item2));
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public static void LoadPersistence()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            unlockedPlayerGroups.Add(200); //so if haven't got any saved, auto add leader 200
            return;
        }

        string json = PlayerPrefs.GetString(SaveKey);
        UnlockedData data = JsonUtility.FromJson<UnlockedData>(json);

        // Load data into static lists
        unlockedPlayerGroups = data.unlockedPlayerGroups;
        skullsOnID = new List<(int, int)>();
        foreach (var skull in data.skullsOnID)
        {
            skullsOnID.Add((skull.skullID, skull.cardID));
        }
    }

}

[System.Serializable]
public class NodeData
{
    public int nodeID;
    public string encounterType;
    public EncounterData encounterData;
}
[System.Serializable]
public class UnlockedData
{
    //everything persistant across runs
    public List<int> unlockedPlayerGroups = new List<int>();
    public List<SkullPersistentData> skullsOnID = new List<SkullPersistentData>(); //skullID,cardID
}

[System.Serializable]
public class SkullPersistentData
{
    public int skullID;
    public int cardID;

    public SkullPersistentData(int skullID, int cardID)
    {
        this.skullID = skullID;
        this.cardID = cardID;
    }
}
