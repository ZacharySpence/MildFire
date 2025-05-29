using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenu : MonoBehaviour
{
    public void Settings()
    {
        //open settings panel
    }
    public void SaveAndExit()
    {
        PersistanceManager.SavePersistence();
        SceneManager.LoadScene(0);
    }
}
