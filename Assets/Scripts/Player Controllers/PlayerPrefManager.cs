using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System; // Include so we can manipulate SceneManager

public static class PlayerPrefManager
{
    public static string GetNotes()
    {
        if (PlayerPrefs.HasKey("Notes"))
        {
            return PlayerPrefs.GetString("Notes");
        }
        else
        {
            return "";
        }
    }

    public static void SetNotes(string note)
    {
        PlayerPrefs.SetString("Notes", note);
    }

    public static string GetInventory()
    {
        if (PlayerPrefs.HasKey("Inventory"))
        {
            return PlayerPrefs.GetString("Inventory");
        }
        else
        {
            return "";
        }
    }

    public static void SetInventory(string inventory)
    {
        PlayerPrefs.SetString("Inventory", inventory);
    }

    public static int GetScore()
    {
        if (PlayerPrefs.HasKey("Score"))
        {
            return PlayerPrefs.GetInt("Score");
        }
        else
        {
            return 0;
        }
    }

    public static void SetScore(int score)
    {
        PlayerPrefs.SetInt("Score", score);
    }

    public static int GetHighscore()
    {
        if (PlayerPrefs.HasKey("Highscore"))
        {
            return PlayerPrefs.GetInt("Highscore");
        }
        else
        {
            return 0;
        }
    }

    public static void SetHighscore(int highscore)
    {
        PlayerPrefs.SetInt("Highscore", highscore);
    }

    // Store the current player state info into PlayerPrefs
    public static void SavePlayerState(int score, int highScore, string inventory, string notes)
    {
        // Save currentscore to PlayerPrefs for moving to next level
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.SetInt("Highscore", highScore);
        PlayerPrefs.SetString("Inventory", inventory);
        PlayerPrefs.SetString("Notes", notes);
    }

    // Reset stored player state and variables back to defaults
    public static void ResetPlayerState(bool resetHighscore)
    {
        Debug.Log("Player State reset.");
        PlayerPrefs.SetInt("Score", 0);

        if (resetHighscore)
            PlayerPrefs.SetInt("Highscore", 0);

        PlayerPrefs.SetString("Inventory", "");

        PlayerPrefs.SetString("Notes", "");
    }

    // Store a key for the name of the current level to indicate it is unlocked
    public static void UnlockLevel()
    {
        // Get current scene
        Scene scene = SceneManager.GetActiveScene();
        PlayerPrefs.SetInt(scene.name, 1);
    }

    // Determine if a levelname is currently unlocked (i.e., it has a key set)
    public static bool LevelIsUnlocked(string levelName)
    {
        return (PlayerPrefs.HasKey(levelName));
    }

    // Determine if a note is currently unlocked (i.e., it has a key set)
    public static bool NoteIsUnlocked(string noteName)
    {
        return (PlayerPrefs.HasKey(noteName));
    }

    // Output the defined Player Prefs to the console
    public static void ShowPlayerPrefs()
    {
        // Store the PlayerPref keys to output to the console
        string[] values = { "Score", "Highscore", "Inventory", "Notes" };

        // Loop over the values and output to the console
        foreach (string value in values)
        {
            if (PlayerPrefs.HasKey(value))
            {
                if (value == "Score" || value == "Highscore")
                {
                    Debug.Log(value + " = " + PlayerPrefs.GetInt(value));
                }
                else if (value == "Inventory" || value == "Notes")
                {
                    Debug.Log(value + " = " + PlayerPrefs.GetString(value));
                }
                else
                {
                    Debug.Log(value + " = " + PlayerPrefs.GetFloat(value));
                }
            }
            else
            {
                Debug.Log(value + " is not set.");
            }
        }
    }
}
