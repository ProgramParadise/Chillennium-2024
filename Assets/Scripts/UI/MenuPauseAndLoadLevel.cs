using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Include so we can load new scenes

public class MenuPauseAndLoadLevel : MonoBehaviour
{
    public string levelToLoad;
    public float delay = 2f;

    // Use invoke to wait for a delay then call LoadLevel
    void Update()
    {
        Invoke("LoadLevel", delay);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            delay = 0;
        }
    }

    // Load the specified level
    void LoadLevel()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
