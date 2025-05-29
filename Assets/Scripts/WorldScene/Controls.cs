using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    [SerializeField] GameObject escapeMenu;    
    
    // Update is called once per frame
    void Update()
    {
        if (!WorldPlayer.startingPanelOpen && Input.GetKeyDown(KeyCode.Escape)) //so can't escape mid reward/leader choice
        {
            escapeMenu.SetActive(!escapeMenu.activeSelf);
        }
    }
}
