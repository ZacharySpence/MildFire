using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField] Button loadButton;

    private void Start()
    {
        string path = Path.Combine(Application.persistentDataPath, "GameSaveFile.json");
        if (!File.Exists(path))
        {
            loadButton.interactable = false; //does that change the colour enough?
        }
    }
    public void StartGame()
    {
        PersistanceManager.loadedGame = false;
        SceneManager.LoadScene(1);
    }
    public void LoadGame()
    {
        PersistanceManager.loadedGame = true;
        SceneManager.LoadScene(1);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
