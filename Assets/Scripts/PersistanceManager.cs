using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;


public static class PersistanceManager 
{
    [Header("PlayerSpecific")]
    public static List<int> unlockedPlayerGroups = new List<int>();
    public static List<(int, int)> skullsOnID = new List<(int, int)>();
    public static int skullCount, moneyCount;
    public static List<(int,bool)> playerCurrentDeck = new List<(int,bool)>();
    public static List<int> skullsInPlayerStorage = new List<int>();
   
    //Add in skullCharms in storage.
   


    [Header("EndGameCounting")]
    public static int nessyCompanionsGiven;
    public static List<int> skullFightsWon = new List<int>();
    public static int poisonSpread;

    [Header("NessySpecific")]
    public static List<int> nessyTrainedCompanionsID = new List<int>(); //id,#oftimesTrained
    public static List<int> nessyCurrentTrainingCompanionsID = new List<int>(); //id, #oftimesTrained

    [Header("MapSpecific")]
    public static int currentNodeID;
    public static List<NodeData> currentGameNodeData = new List<NodeData>(); //ADD IN SAVING/LOADING NODES FOR SAVE/LOAD FEATURES!

    [Header("Save&LoadSpecific")]
    public static bool loadedGame;

    const string SaveFileName = "GameSaveFile.json";
    public static void SavePersistence()
    {
        //--Save PlayerSpecific data--
        SaveData data = new SaveData();
        data.unlockedPlayerGroups = unlockedPlayerGroups;

            //saving skulls used
        foreach (var tuple in skullsOnID)
        {
            data.skullsOnID.Add(new SkullPersistentData(tuple.Item1, tuple.Item2));
        }
            
            //saving player deck
        foreach(var card in IDLookupTable.instance.playerDeck)
        {
            data.playerCurrentDeck.Add(new CardData(card.baseID, card.hasDied));
        }

        data.skullCount = skullCount;
        data.moneyCount = moneyCount;
   
            //saving current skulls in player storage
        foreach(var skull in IDLookupTable.instance.charmsInPlayerStorage)
        {
            data.skullsInPlayerStorage.Add(skull.ID);
        }

        //--Save Map data--
        Debug.Log("Saving Map Data");
        foreach(var node in WorldManager.Instance.nodeData)
        {
            Debug.Log(node.Key);
            data.currentNodeData.Add(new NodeData
            {
                encounterType = node.Value.encounter.ToString(),
                nodeID = node.Key,
                specific = node.Value.eData.specific,
                spriteName = node.Value.eData.specificSprite.name,
            
                
            });
        }
        data.currentNodeID = WorldManager.Instance.currentNode.ID;

        //--Save EndGameCounting data--
        data.nessyCompanionsGiven = nessyCompanionsGiven;
        data.skullFightsWon = skullFightsWon;
        data.poisonSpread = poisonSpread;

        //--Save nessySpecific data--
        data.nessyTrainedCompanionsID = nessyTrainedCompanionsID;
        data.nessyCurrentTrainingCompanionsID = nessyCurrentTrainingCompanionsID;


        string json = JsonUtility.ToJson(data, prettyPrint: true);

        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        File.WriteAllText(path, json);

        Debug.Log($"Game saved to: {path}");
    }

    //--Only loads into main map (never battle)
    public static void LoadPersistence()
    {
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        if (!File.Exists(path))
        {
            unlockedPlayerGroups.Add(200); // default if no save exists
            Debug.LogWarning($"Save file not found at {path}, starting fresh.");
            return;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        //--Load PlayerSpecific data--
        unlockedPlayerGroups = data.unlockedPlayerGroups;

        
            //store already used skulls
        skullsOnID = new List<(int, int)>();
        foreach (var skull in data.skullsOnID)
        {
            skullsOnID.Add((skull.skullID, skull.cardID));

        }
            //recreate deck
        foreach(var card in data.playerCurrentDeck)
        {
            playerCurrentDeck.Add((card.cardID, card.isDead));
            //actually recreate deck!
        }
        skullCount = data.skullCount;
        moneyCount = data.moneyCount;
       
            //recreate skulls in storage
        foreach(var skullID in data.skullsInPlayerStorage)
        {
            var skullCharm = IDLookupTable.instance.skullCharmList.Find(x => x.ID == skullID);
            IDLookupTable.instance.charmsInPlayerStorage.Add(skullCharm.CreateCharmSaveData());
        }

        //--Load Map data--
        currentGameNodeData = data.currentNodeData;
        currentNodeID = data.currentNodeID;

        //--Load endSpecific data--
        nessyCompanionsGiven = data.nessyCompanionsGiven;
        skullFightsWon = data.skullFightsWon;
        poisonSpread = data.poisonSpread;

        //--Load nessySpecific data--
        nessyTrainedCompanionsID = data.nessyTrainedCompanionsID;
        nessyCurrentTrainingCompanionsID = data.nessyCurrentTrainingCompanionsID;

        Debug.Log($"Game loaded from: {path}");
    }

}


[System.Serializable]
public class SaveData
{
    [Header("PlayerSpecific")]
    //everything persistant across runs
    public List<int> unlockedPlayerGroups = new List<int>();
    public List<SkullPersistentData> skullsOnID = new List<SkullPersistentData>(); //skullID,cardID
    public int skullCount, moneyCount;
    public List<CardData> playerCurrentDeck = new List<CardData>();
    public List<int> skullsInPlayerStorage = new List<int>();
  
    
    [Header("EndGameCounting")]
    public int nessyCompanionsGiven;
    public List<int> skullFightsWon = new List<int>();
    public int poisonSpread;

    [Header("NessySpecific")]
    public List<int> nessyTrainedCompanionsID = new List<int>(); //id,#oftimesTrained
    public List<int> nessyCurrentTrainingCompanionsID = new List<int>(); //id, #oftimesTrained

    [Header("MapSpecific")]
    public List<NodeData> currentNodeData = new List<NodeData>();
    public int currentNodeID;

}

[System.Serializable]
public class NodeData
{
    public int nodeID;
    public string encounterType;
    public string specific;
    public string spriteName; 
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

[System.Serializable]
public class CardData
{
    public int cardID;
    public bool isDead;

    public CardData(int cardID, bool isDead)
    {
        this.cardID = cardID;
        this.isDead = isDead;
    }
}