using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class thanksForPlaying : MonoBehaviour
{
    public void Restart()
    {
        SceneManager.LoadScene(1);//back to travelScene
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
