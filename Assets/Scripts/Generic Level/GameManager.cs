using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool restart = true;

    // Static reference to game manager so can be called from other scripts directly (not just through gameobject component)
    public static GameManager gm;

    // Levels to move to on after victory and lose
    public string levelAfterVictory;
    public string levelAfterGameOver;

    [Header("Game performance")]
    public int score = 0;
    public int highscore = 0;

    [Header("UI")]
    public Text UIScore;
    public Text UIHighScore;
    public Text UILevel;
    public GameObject[] UIExtraLives;
    public GameObject UIGamePaused;
    public GameObject UIGamePaused2;

    GameObject _player;
    float playerInsantiateDelay = 2f; // How long to check again if the player object exists after it was not found the first time
    Vector3 _spawnLocation;

    [HideInInspector]
    public float stainRoll;

    [HideInInspector]
    public Scene _scene;

    // Set things up here
    void Awake()
    {
        //set up bullet dictionary
        EnemyProjectileManager.LoadDictionary();

        stainRoll = Random.value;

        // Setup reference to game manager
        if (gm == null)
            gm = this.GetComponent<GameManager>();

        // Setup all the variables, the UI, and provide errors if things not setup properly.
        setupDefaults();
    }

    // Game loop
    void Update()
    {
        // If RETURN pressed then return to menu
        if (Time.timeScale == 0f)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("MainMenu");
            }
        }
        // If ESC pressed then pause the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale > 0f)
            {
                UIGamePaused.SetActive(true); // This brings up the pause UI
                UIGamePaused2.SetActive(true); // This brings up the pause UI2
                Time.timeScale = 0f; // This pauses the game action
            }
            else
            {
                Time.timeScale = 1f; // This unpauses the game action (ie. back to normal)
                UIGamePaused.SetActive(false); // Remove the pause UI
                UIGamePaused2.SetActive(false); // Remove the pause UI2
            }
        }
    }

    // Setup all the variables, the UI, and provide errors if things not setup properly.
    void setupDefaults()
    {
        // Setup reference to player
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }

        if (_player == null)
        {
            Debug.LogError("Player not found in GameManager. Attempting to find player again in " + playerInsantiateDelay + " seconds");
            StartCoroutine(GetPlayer());
        }

        // Get current scene
        _scene = SceneManager.GetActiveScene();

        // Get initial _spawnLocation based on initial position of player
        _spawnLocation = _player.transform.position;

        // If levels not specified, default to current level
        if (levelAfterVictory == "")
        {
            Debug.LogWarning("levelAfterVictory not specified, defaulted to current level");
            levelAfterVictory = _scene.name;
        }

        if (levelAfterGameOver == "")
        {
            Debug.LogWarning("levelAfterGameOver not specified, defaulted to current level");
            levelAfterGameOver = _scene.name;
        }

        // Friendly error messages
        if (UIScore == null)
            Debug.LogError("Need to set UIScore on Game Manager.");

        if (UIHighScore == null)
            Debug.LogError("Need to set UIHighScore on Game Manager.");

        if (UILevel == null)
            Debug.LogError("Need to set UILevel on Game Manager.");

        if (UIGamePaused == null)
            Debug.LogError("Need to set UIGamePaused on Game Manager.");

        if (UIGamePaused2 == null)
            Debug.LogError("Need to set UIGamePaused2 on Game Manager.");

        // Get stored player prefs
        refreshPlayerState();

        // Get the UI ready for the game
        refreshGUI();
    }

    // Get stored Player Prefs if they exist, otherwise go with defaults set on gameObject
    void refreshPlayerState()
    {
        score = PlayerPrefManager.GetScore();
        highscore = PlayerPrefManager.GetHighscore();

        // Save that this level has been accessed so the MainMenu can enable it
        PlayerPrefManager.UnlockLevel();
    }

    // Refresh all the GUI elements
    void refreshGUI()
    {
        // Set the text elements of the UI
        UIScore.text = "Score: " + score.ToString();
        UIHighScore.text = "Highscore: " + highscore.ToString();
        UILevel.text = _scene.name;
    }

    // Public function to add points and update the gui and highscore player prefs accordingly
    public void AddPoints(int amount)
    {
        // Increase score
        score += amount;

        // Update UI
        UIScore.text = "Score: " + score.ToString();

        // If score > highscore then update the highscore UI too
        if (score > highscore)
        {
            highscore = score;
            UIHighScore.text = "Highscore: " + score.ToString();
        }
    }

    // Public function to reset game accordingly
    public void ResetGame()
    {
        // Update GUI
        refreshGUI();

        if (restart)
        {
            // Save the current player prefs before going to GameOver
            PlayerPrefManager.SavePlayerState(score, highscore, "", "");

            // Load the gameOver screen
            SceneManager.LoadScene(levelAfterGameOver);
        }
        // Tell the player to respawn
        else
        {
            // _player.GetComponent<Player_Movement>().Respawn(_spawnLocation);
        }
    }

    // Public function for level complete
    public void LevelCompete()
    {
        // Save the current player prefs before moving to the next level
        PlayerPrefManager.SavePlayerState(score, highscore, "", "");

        // Use a coroutine to allow the player to get fanfare before moving to next level
        StartCoroutine(LoadNextLevel());
    }

    // Load the nextLevel after delay
    public IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(0f);
        SceneManager.LoadScene(levelAfterVictory);
    }

    // Load specific level
    public IEnumerator LoadLevel(string level)
    {
        yield return new WaitForSeconds(0f);
        SceneManager.LoadScene(level);
    }

    public GameObject DrawObject(GameObject[] gameObjects, float x, float y, Transform objectTransform, bool addColorTint = true)
    {
        // Choose random gameObject through noise if provided a list of gameObjects
        int index = (int)(Mathf.PerlinNoise(x * 0.5f + stainRoll, y * 0.5f + stainRoll) * gameObjects.Length) % gameObjects.Length;

        GameObject obj = Instantiate(gameObjects[index], new Vector3(x, y, 0f), Quaternion.identity, objectTransform);

        // Change color tint with noise to create light and dark variation
        if (addColorTint)
        {
            float randomStain = Mathf.PerlinNoise(x * 0.1f + stainRoll, y * 0.1f + stainRoll) / 2 + 0.45f;
            obj.GetComponent<SpriteRenderer>().color = new Color(randomStain, randomStain, randomStain);

            if (obj.GetComponentsInChildren<SpriteRenderer>().Length > 0)
            {
                foreach (SpriteRenderer sr in obj.GetComponentsInChildren<SpriteRenderer>())
                {
                    sr.color = new Color(randomStain, randomStain, randomStain);
                }
            }
        }

        return obj;
    }

    // Make sure to access player only when it is instantiated
    IEnumerator GetPlayer()
    {
        yield return new WaitForSeconds(2f);

        // Setup reference to player
        if (_player == null)
            _player = GameObject.FindGameObjectWithTag("Player");

        if (_player == null)
            Debug.LogError("Player not found in Game Manager");
    }
}
