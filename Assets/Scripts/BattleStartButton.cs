using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleStartButton : MonoBehaviour
{
    [SerializeField] string enemyDeckName;
 

    public void OnClick()
    {
       DeckHolder.LoadEnemyDeck(enemyDeckName);
       SceneManager.LoadScene(1);

    }

    
}
