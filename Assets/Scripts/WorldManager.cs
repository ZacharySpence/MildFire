using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour
{
    public static int travelNode;
    static List<Node> nodeList = new List<Node>(); //needs to be a linked list (1 directional) of nodes connected to
    public static Node currentNode; //have this set at start
    public Node startNode;
    static string nodeName;
    public Sprite bSprite, rSprite, cSprite, eSprite;
    public static Sprite battleSprite, randomSprite, campSprite, eliteSprite;
    public NodeVisualConnectorDrawer nConnector;
    public static NodeVisualConnectorDrawer nodeConnector;
    public Transform cBoss;
    public static Transform connectorBoss;
    public GameObject pToken;
    public static GameObject playerToken;

    private void Awake()
    {
        if(battleSprite == null)
        {
            battleSprite = bSprite;
            randomSprite = rSprite;
            campSprite = cSprite;
            eliteSprite = eSprite;
            nodeConnector = nConnector;
            
        }
        playerToken = pToken;
        connectorBoss = cBoss;
    }
    private void Start()
    {
        if (!WorldPlayer.gameHasStarted)
        {
            currentNode = startNode;
            nodeName = currentNode.name;
            currentNode.button.interactable = true;
           
        }
        else
        {
            currentNode = GameObject.Find(nodeName).GetComponent<Node>();
            playerToken.transform.position = currentNode.transform.position;
        }
        
        UpdateNodes();

    }
    public static void UpdateNodes()
    {

        //1. Enable the buttons on each node linked to current
        
        foreach( var node in currentNode.linkedNodes)
        {
            node.button.interactable = true;
        }
        

    }
    public static void MoveNode(Node nodeToMoveTo)
    {
       
        foreach (var node in currentNode.linkedNodes)
        {
            node.button.interactable = false; //make all the ones we COULD have clicked unclickable
        }
        currentNode = nodeToMoveTo; //make this the current node
        UpdateNodes();
        nodeName = currentNode.name;
        playerToken.transform.position = currentNode.transform.position;
        //2. Move the 'figure' to the current node
    }

    public void RandomiseNodes()
    {

    }


    
}



public enum EncounterType
{
    Battle,
    Random,
    Camp,
    Elite
}