using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace since references UI Buttons directly
using UnityEngine.EventSystems; // include EventSystems namespace so can set initial input for controller support
using UnityEngine.SceneManagement;
using TMPro; // include so we can load new scenes

public class MainMenuManager : MonoBehaviour
{
    // references to Submenus
    public GameObject _MainMenu;
    public GameObject _NotesMenu;
    public GameObject _NotesInfoMenu;
    public GameObject _OptionsMenu;

    [HideInInspector]
    public bool MainMenuOpen = false;

    [HideInInspector]
    public bool OptionsOpen = false;

    // references to Button GameObjects
    public GameObject PlayButton;
    public GameObject OptionsButton;
    public GameObject QuitButton;

    // list the note names
    public string[] NoteNames;

    // reference to the NotesPanel gameObject where the buttons should be childed
    public GameObject NotesPanel;

    // reference to the default Note Button template
    public GameObject NoteButtonPrefab;

    // reference the menuTitleText so we can change it dynamically
    public TMP_Text menuTitleText;

    // init the menu
    void Awake()
    {
        // determine if Quit button should be shown
        displayQuitWhenAppropriate();
    }

    // loop through all the notes and set them to interactable 
    // based on if PlayerPref key is set for the note.
    void setNotesMenu()
    {
        // turn on notes menu while setting it up so no null refs
        _NotesMenu.SetActive(true);

        // loop through each noteName defined in the editor
        for (int i = 0; i < NoteNames.Length; i++)
        {
            // get the note name
            string notename = NoteNames[i];

            // dynamically create a button from the template
            GameObject noteButton = Instantiate(NoteButtonPrefab, Vector3.zero, Quaternion.identity);

            // name the game object
            noteButton.name = notename + " Button";

            // set the parent of the button as the notesPanel so it will be dynamically arrange based on the defined layout
            noteButton.transform.SetParent(NotesPanel.transform, false);

            // get the Button script attached to the button
            Button noteButtonScript = noteButton.GetComponent<Button>();

            // setup the listener to loadnote when clicked
            noteButtonScript.onClick.RemoveAllListeners();
            noteButtonScript.onClick.AddListener(() => _NotesInfoMenu.SetActive(true)); // PlayerPrefManager.GetNotes()

            // set the label of the button
            TMP_Text noteButtonLabel = noteButton.GetComponentInChildren<TMP_Text>();
            noteButtonLabel.text = notename;

            // determine if the button should be interactable based on if the note is unlocked
            if (PlayerPrefManager.NoteIsUnlocked(notename))
            {
                noteButtonScript.interactable = true;
            }
            else
            {
                noteButtonScript.interactable = false;
            }
        }
    }

    // determine if the QUIT button should be present based on what platform the game is running on
    void displayQuitWhenAppropriate()
    {
        switch (Application.platform)
        {
            // platforms that should have quit button
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.LinuxPlayer:
                QuitButton.SetActive(true);
                break;

            // platforms that should not have quit button
            // note: included just for demonstration purposed since
            // default will cover all of these.
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.WebGLPlayer:
                QuitButton.SetActive(false);
                break;

            // all other platforms default to no quit button
            default:
                QuitButton.SetActive(false);
                break;
        }
    }

    // Public functions below that are available via the UI Event Triggers, such as on Buttons.

    // Show the proper menu
    public void ShowMenu(string name)
    {
        // turn on desired menu and set default selected button for controller input
        switch (name)
        {
            case "Options":
                if (OptionsOpen)
                {
                    _OptionsMenu.SetActive(false);
                    OptionsOpen = false;

                    _MainMenu.SetActive(true);
                    MainMenuOpen = true;
                    menuTitleText.text = "";
                }
                else
                {
                    _OptionsMenu.SetActive(true);
                    OptionsOpen = true;

                    _MainMenu.SetActive(false);
                    MainMenuOpen = false;
                    menuTitleText.text = "Options";
                }
                break;
        }
    }

    // load the specified Unity level
    public void loadLevel(string levelToLoad)
    {
        // start new game so initialize player state
        PlayerPrefManager.ResetPlayerState(false);

        // load the specified level
        SceneManager.LoadScene(levelToLoad);
    }

    // quit the game
    public void QuitGame()
    {
        Application.Quit();
    }
}
