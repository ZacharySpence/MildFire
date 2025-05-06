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
            case EncounterType.Event:
                button.onClick.AddListener(() => OnEventClick());
                GetComponent<Image>().sprite = WorldManager.Instance.eventSprite;
                break;
            case EncounterType.Camp:
                button.onClick.AddListener(() => OnCampClick());
                GetComponent<Image>().sprite = WorldManager.Instance.campSprite;
                break;
            case EncounterType.Shop:
                button.onClick.AddListener(() => OnShopClick());
                GetComponent<Image>().sprite = WorldManager.Instance.shopSprite;
                break;
            case EncounterType.Relic:
                button.onClick.AddListener(() => OnRelicClick());
                GetComponent<Image>().sprite = WorldManager.Instance.relicSprite;
                break;
            case EncounterType.Elite:
                button.onClick.AddListener(() => OnEliteBattleClick());
                GetComponent<Image>().sprite = WorldManager.Instance.eliteSprite;
                break;
            case EncounterType.Boss:
                button.onClick.AddListener(() => OnBossBattleClick());
                GetComponent<Image>().sprite = WorldManager.Instance.bossSprite;
                break;
           
        }
       
    }

   
    public void OnBossBattleClick()
    {
        Debug.Log("Boss battle");
        WorldManager.Instance.MoveNode(this);
    }
    public void OnRelicClick()
    {
        Debug.Log("relic");
        WorldManager.Instance.MoveNode(this);
    }
    public void OnShopClick()
    {
        Debug.Log("Shop!");
        WorldManager.Instance.MoveNode(this);
    }
    public void OnCampClick()
    {
        Debug.Log("Camp!");
        UIManager.instance.campfire.SetActive(true);
        WorldManager.Instance.MoveNode(this);

    }
    public void OnEliteBattleClick()
    {
        Debug.Log("elite battle!");
        WorldManager.Instance.MoveNode(this);
    }
    public void OnEventClick()
    {
        Debug.Log("random event!");
        RandomEventManager.instance.LoadEvent(specifics);
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
