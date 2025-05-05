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
    public Sprite battleSprite, randomSprite, campSprite, eliteSprite, shopSprite, relicSprite, bossSprite;   
    Dictionary<int, (EncounterType encounter, string specifics)> nodeData = new Dictionary<int, (EncounterType encounter,string specifics)>();

    [Header("Resets on load")]
    List<Node> nodeList = new List<Node>(); //list of all nodes
    public Node currentNode; //have this set at start
    Node startNode;
    NodeVisualConnectorDrawer nodeConnector;
    Transform connectorBoss;
    GameObject playerToken;
    

    [Header("Randomizer")]
    [SerializeField] List<string> battleList, randomList, eliteList;
    [SerializeField] List<EncounterType> randomizedEncounters;
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
        if(scene.buildIndex == 1) { return; } //don't do any of this with battle scene
        Debug.Log("scene loaded");
        ReattachReferences();
        if (WorldPlayer.gameHasStarted)
        {
           
            RecreateScene();
        }
        else
        {
            StartGame();        
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
        var nodeTuple = (encounter: node.encounter, specifics: node.specifics);
        nodeData[node.ID] = nodeTuple;
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
        node.specifics = data.specifics;
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
        List<string> currentBattleList = new List<string>(battleList);
        List<string> currentRandomList = new List<string>(randomList);
        List<string> currentEliteList = new List<string>(eliteList);
        List<EncounterType> currentRandomizedEncounters = new List<EncounterType>(randomizedEncounters);
        SaveNodes(); //save after making them!
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
                case EncounterType.Random:
                    GetRandomElement(currentRandomList, node);
                    break;
                case EncounterType.Camp:
                    break;
                case EncounterType.Shop:
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

    }
    
    void GetRandomElement(List<string> currentList, Node node)
    {
        if(currentList.Count <= 0)
        {
            Debug.Log("cant get an element for that node!- need more encounters!!");
            return;
        }
        string specific = currentList[Random.Range(0, currentList.Count)];
        currentList.Remove(specific);
        node.specifics = specific; 
    }



}



public enum EncounterType
{
    Battle,
    Random,
    Camp,
    Shop,
    Elite,
    Relic,
    Boss
}