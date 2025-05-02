using System;
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
    public int ID;
    public bool randomize;


   public void Setup()
    {
        switch (encounter)
        {
            case EncounterType.Battle:
                button.onClick.AddListener(() => OnBattleClick());

                break;
            case EncounterType.Random:
                button.onClick.AddListener(() => OnRandomClick());
                GetComponent<Image>().sprite = WorldManager.Instance.randomSprite;
                break;
            case EncounterType.Camp:
                button.onClick.AddListener(() => OnCampClick());
                GetComponent<Image>().sprite = WorldManager.Instance.campSprite;
                break;
            case EncounterType.Elite:
                button.onClick.AddListener(() => OnEliteBattleClick());
                GetComponent<Image>().sprite = WorldManager.Instance.eliteSprite;
                break;
        }
       
    }
    public void OnCampClick()
    {
        Debug.Log("Camp!");
        WorldManager.Instance.MoveNode(this);

    }
    public void OnEliteBattleClick()
    {
        Debug.Log("elite battle!");
        WorldManager.Instance.MoveNode(this);
    }
    public void OnRandomClick()
    {
        Debug.Log("random event!");
        WorldManager.Instance.MoveNode(this);    
    }
    public void OnBattleClick()
    {
        Debug.Log("normal battle!");
        DeckHolder.LoadEnemyDeck(specifics);
        WorldManager.Instance.MoveNode(this);
        SceneManager.LoadScene(1);


    }
}
