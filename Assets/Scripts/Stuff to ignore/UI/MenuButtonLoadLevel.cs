using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Include so we can load new scenes

public class MenuButtonLoadLevel : MonoBehaviour
{
    public void loadLevel(string levelToLoad)
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
