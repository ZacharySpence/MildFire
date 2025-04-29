using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Node : MonoBehaviour
{
    public Button button;
    public string specifics;
    public EncounterType encounter;
    public List<Node> linkedNodes;
    public bool randomize;

    private void Start()
    {
        switch (encounter)
        {
            case EncounterType.Battle:
                button.onClick.AddListener(() => OnBattleClick());
                
                break;
            case EncounterType.Random:
                button.onClick.AddListener(() => OnRandomClick());
                GetComponent<Image>().sprite = WorldManager.randomSprite;
                break;
            case EncounterType.Camp:
                button.onClick.AddListener(() => OnCampClick());
                GetComponent<Image>().sprite = WorldManager.campSprite;
                break;
            case EncounterType.Elite:
                button.onClick.AddListener(() => OnEliteBattleClick());
                GetComponent<Image>().sprite = WorldManager.eliteSprite;
                break;
        }
        foreach(var node in linkedNodes)
        {
            Debug.Log("made a linker with" + node.name);
           var nodeConnector = Instantiate(WorldManager.nodeConnector, WorldManager.connectorBoss);
            if (node.GetComponent<RectTransform>() == null)
            {
                Debug.LogWarning($"{name} does not have a RectTransform!");
            }
            else
            {
                Debug.Log("RectTransform found!");
            }
            nodeConnector.Setup(GetComponent<RectTransform>(), node.GetComponent<RectTransform>());
        }
    }

    public void OnCampClick()
    {
        Debug.Log("Camp!");
        WorldManager.MoveNode(this);

    }
    public void OnEliteBattleClick()
    {
        Debug.Log("elite battle!");
        WorldManager.MoveNode(this);
    }
    public void OnRandomClick()
    {
        Debug.Log("random event!");
        WorldManager.MoveNode(this);    
    }
    public void OnBattleClick()
    {
        Debug.Log("normal battle!");
        DeckHolder.LoadEnemyDeck(specifics);
        WorldManager.MoveNode(this);
        SceneManager.LoadScene(1);


    }
}
