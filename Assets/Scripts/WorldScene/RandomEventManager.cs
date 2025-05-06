using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventManager : MonoBehaviour
{
    public static RandomEventManager instance;
    [SerializeField] List<GameObject> randomEventUIs;

    GameObject chosenEvent;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        chosenEvent.SetActive(false);
        chosenEvent = null;
    }
    public void LoadEvent(string specifics)
    {
        //Find event within the prefabs,
         chosenEvent = randomEventUIs.Find(x => x.name == specifics);
        if(chosenEvent != null)
        {
            chosenEvent.SetActive(true);
        }
        else
        {
            Debug.LogError("CAN'T FIND CHOSEN EVENT!");
        }
        
    }

}
