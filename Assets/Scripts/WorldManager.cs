using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;
   
    [Header("Stays on load")]
    int currentNodeID;
    public Sprite battleSprite, eventSprite, campSprite, eliteSprite, shopSprite, relicSprite, bossSprite;   
    public Dictionary<int, (EncounterType encounter, EncounterData eData)> nodeData = new Dictionary<int, (EncounterType encounter,EncounterData eData)>();
    public bool skullCollectionUnlocked;
    public int skullAmount, moneyAmount;

    [Header("Resets on load")]
    List<Node> nodeList = new List<Node>(); //list of all nodes
    public Node currentNode; //have this set at start
    Node startNode;
    NodeVisualConnectorDrawer nodeConnector;
    Transform connectorBoss;
    GameObject playerToken;
    

    [Header("Randomizer")]
    [SerializeField] List<EncounterData> battleList, randomList, eliteList;
    [SerializeField] List<EncounterType> randomizedEncounters;

    [Header("WorldEffects")]
    public bool noogleBeefBuff, noogleBeefDebuff, noogleChickenBad, noogleVeggieBad, noogleBlessing, noogleCurse, noogleVeggieGood, noogleChickenGood,
        nessyBlessing, nessyCurse, omnisciJudgement, omnisciForesight, omnisciChaos, omnisciBlessing, skullBlessing;
    
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
       
    }
    private void Start()
    {
      //  OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 1){ return; } //don't do any of this with battle scene
        Debug.Log("scene loaded");
        ReattachReferences();
        if (PersistanceManager.loadedGame)
        {
            LoadGame();
            
        }
        else if (WorldPlayer.gameHasStarted)
        {
           
            RecreateScene();
        }
        else
        {
            StartGame();        
        }
    }

    void LoadGame()
    {
        WorldPlayer.gameHasStarted = true;
        PersistanceManager.LoadPersistence();
        //Add skulls back to playerHand
        var skullDict = PersistanceManager.skullsOnID.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
        foreach (var skullCharm in IDLookupTable.instance.skullCharmList)
        {
            if (skullDict.TryGetValue(skullCharm.ID, out int cardID))
            {
                skullCharm.ChangeCard(IDLookupTable.instance.playerDeck.Find(x => x.baseID == cardID));

            }
        }
        //Recreate the map
        nodeList.Clear();
        var nodes = GameObject.FindGameObjectsWithTag("Node"); //collect all the nodes!
        foreach (var node in nodes)
        {
            var nodeComp = node.GetComponent<Node>();
            
            var currentNodeData = PersistanceManager.currentGameNodeData.Find(x => x.nodeID == nodeComp.ID);
            EncounterType encounterType = (EncounterType)Enum.Parse(typeof(EncounterType), currentNodeData.encounterType);
           
            nodeData[nodeComp.ID] = (encounterType,
                new EncounterData { specific = currentNodeData.specific, specificSprite = LoadSprite(encounterType, currentNodeData.spriteName) });

            nodeList.Add(nodeComp);
        }
        //--PersistanceManager Load in NodeData & currentNodeID -> need to store sprites as resources so can find via names/ID rather than store in nodeData!
        new Dictionary<int, (EncounterType encounter, EncounterData eData)>();
        LoadNodes();
        currentNode = GetNode(currentNodeID);
        playerToken.transform.position = currentNode.transform.position;
        UpdateNodes();

        WorldPlayer.Instance.startingPanel.gameObject.SetActive(false); //forcibly get rid of starting panel
          
        

    }
    Sprite LoadSprite(EncounterType encounterType, string sprite)
    {
        switch (encounterType)
        {
            case EncounterType.Battle:
                return Resources.Load<Sprite>($"Characters/{sprite}");
            default:
                return Resources.Load<Sprite>($"WorldSprites/{sprite}");
        }
    }
    void ReattachReferences()
    {  
      
        connectorBoss=GameObject.FindWithTag("NodeConnectorBoss").transform;
        nodeConnector = connectorBoss.GetComponent<NodeVisualConnectorDrawer>();
        playerToken = GameObject.FindWithTag("PlayerToken") ;
    }

    public Node GetNode(int id)
    {
        return nodeList.Find(x => x.ID == id);
    }
    public void SaveNodes()
    {
        Debug.Log("saving nodes");
        foreach(var node in nodeList)
        {
            SaveNode(node);
        }
    }
    public void SaveNode(Node node)
    {
        var nodeTuple = (encounter: node.encounter, eData: 
            new EncounterData { specific = node.specifics,
                                specificSprite = node.specificSprite }) ;
        nodeData[node.ID] = nodeTuple;
        Debug.Log("Data:"+nodeData[node.ID].eData.specific);
    }
    public void LoadNodes()
    {
        foreach(var node in nodeList)
        {
            LoadNode(node);
            foreach (var linked in node.linkedNodes)
            {
                nodeConnector.DrawConnection(
                    node.GetComponent<RectTransform>(),
                    linked.GetComponent<RectTransform>());
            }
        }
    }
    public void LoadNode(Node node)
    {
        var data = nodeData[node.ID];
        Debug.Log("Loaded data:" + data.eData.specific);
        node.specifics = data.eData.specific;
        node.specificSprite = data.eData.specificSprite;
        node.encounter = data.encounter;
        node.Setup();
    }
    void RecreateScene()
    {
        //Recreate the map
        nodeList.Clear();
        var nodes=  GameObject.FindGameObjectsWithTag("Node"); //collect all the nodes!
        foreach (var node in nodes)
        {
            nodeList.Add(node.GetComponent<Node>());
        }
        LoadNodes();
        currentNode = GetNode(currentNodeID);
        playerToken.transform.position = currentNode.transform.position;
        UpdateNodes();
        //Give them a choice
        WorldPlayer.Instance.CreateRewardChoice();

        //Remove temp buffs/debuffs
        noogleBeefBuff = false;
        noogleBeefDebuff = false;
    }
    public void StartGame()
    {
        Debug.Log("starting game");
        //Create the map
        nodeList.Clear();
        var nodes = GameObject.FindGameObjectsWithTag("Node"); //collect all the nodes!
        foreach (var node in nodes)
        {
            nodeList.Add(node.GetComponent<Node>());
        }
        RandomiseNodes(); //only do once at very start
        startNode = GetNode(0); //ID == 0 is always start Node!
        MoveNode(startNode);
        currentNode.button.interactable = true;
        //give them first choice
        Debug.Log("doing player choice");
        PersistanceManager.LoadPersistence(); //just to make the file in the beginning!
        WorldPlayer.Instance.CreateLeaderChoice();
    }
    public  void UpdateNodes()
    {

        //1. Enable the buttons on each node linked to current
        
        foreach( var node in currentNode.linkedNodes)
        {
            node.button.interactable = true;
        }
        

    }
    public void MoveNode(Node nodeToMoveTo)
    {
       if(currentNode != null)
        {
            foreach (var node in currentNode.linkedNodes)
            {
                node.button.interactable = false; //make all the ones we COULD have clicked unclickable
            }
        }
       
        currentNode = nodeToMoveTo; //make this the current node
        UpdateNodes();
        currentNodeID = currentNode.ID;
        playerToken.transform.position = currentNode.transform.position;
        //2. Move the 'figure' to the current node
    }

    public void RandomiseNodes()
    {
        List<EncounterData> currentBattleList = new List<EncounterData>(battleList);
        List<EncounterData> currentRandomList = new List<EncounterData>(randomList);
        List<EncounterData> currentEliteList = new List<EncounterData>(eliteList);
        List<EncounterType> currentRandomizedEncounters = new List<EncounterType>(randomizedEncounters);
       
        foreach(var node in nodeList)
        {
            if (node.randomize)
            {
                node.encounter = currentRandomizedEncounters[Random.Range(0,currentRandomizedEncounters.Count)];
                currentRandomizedEncounters.Remove(node.encounter);

            }
            switch (node.encounter)
            {
                case EncounterType.Battle:
                    GetRandomElement(currentBattleList,node);                
                    break;
                case EncounterType.Event:
                    GetRandomElement(currentRandomList, node);
                    break;
                case EncounterType.Camp:
                    //don't need this
                    break;
                case EncounterType.Shop:
                    //don't need this
                    break;
                case EncounterType.Relic:
                    //Get from relicList and add the relic into specifics?
                    break;
                case EncounterType.Elite:
                     GetRandomElement(currentEliteList,node);
                    break;
            }
            
            node.Setup();
            foreach (var linked in node.linkedNodes)
            {
                nodeConnector.DrawConnection(
                    node.GetComponent<RectTransform>(),
                    linked.GetComponent<RectTransform>());
            }
        }
        SaveNodes(); //save after making them!

    }
    
    void GetRandomElement(List<EncounterData> currentList, Node node)
    {
        if(currentList.Count <= 0)
        {
            Debug.Log("cant get an element for that node!- need more encounters!!");
            return;
        }
        EncounterData eData= currentList[Random.Range(0, currentList.Count)];
        currentList.Remove(eData);
        node.specifics = eData.specific;
        node.specificSprite = eData.specificSprite;
    }

    //-FOR EVENTS
   public void GetOmnisciJudgment()
    {
        omnisciJudgement = true;
        foreach(var node in nodeList)
        {
            if(node.encounter == EncounterType.Event)
            {
                node.UpdateSprite();
            }
        }
    }
    public void GetOmnisciForesight()
    {
        omnisciForesight = true;
        foreach(var node in nodeList){
            if(node.encounter == EncounterType.Battle || node.encounter == EncounterType.Elite)
            {
                node.UpdateSprite();
            }
        }
    }

}



public enum EncounterType
{
    Battle,
    Event,
    Camp,
    Shop,
    Elite,
    Relic,
    Boss,

}

[Serializable]
public class EncounterData
{
    public string specific;
    public Sprite specificSprite;
}