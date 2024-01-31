using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    public string scenetoload;

    public void ChangeScene()
    {
        SceneManager.LoadScene(scenetoload);
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}

