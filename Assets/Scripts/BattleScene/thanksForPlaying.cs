using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class thanksForPlaying : MonoBehaviour
{
    public void Restart()
    {
        SceneManager.LoadScene(0);//back to travelScene
    }
}
