using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelOnVideoComplete : MonoBehaviour
{
    public string levelToLoad = "";
    private bool canLoadSoon = false;
    void Start()
    {
        
    }


    void Update()
    {
        if (gameObject.GetComponent<UnityEngine.Video.VideoPlayer>().isPlaying)
        {
            canLoadSoon = true;
        }
        if (canLoadSoon && gameObject.GetComponent<UnityEngine.Video.VideoPlayer>().isPlaying == false)
        {
            if (levelToLoad != "")
            {
                SceneManager.LoadScene(levelToLoad, LoadSceneMode.Single);
            }
            else Application.Quit();
        }
    }
}
