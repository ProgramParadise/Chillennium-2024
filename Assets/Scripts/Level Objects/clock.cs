using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class clock : MonoBehaviour
{
    public float targetTime = 60.0f;
    private int hour = 1;
    public TMP_Text textObj;
    public string loadSceneName = "Bad Ending";
    private string timeOClock = "9:00";
    
    //8 minute timer
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        targetTime -= Time.deltaTime;
        //Debug.Log("Time:" + targetTime);
        if (targetTime <= 0.0f) { timerEnded(); }
    }

    void timerEnded()
    {
        if (hour == 1) timeOClock = "10:00";
        else if (hour == 2) timeOClock = "11:00";
        else if (hour == 3) timeOClock = "12:00";
        else if (hour == 4) timeOClock = "01:00";
        else if (hour == 5) timeOClock = "02:00";
        else if (hour == 6) timeOClock = "03:00";
        else if (hour == 7) timeOClock = "04:00";
        else if (hour == 8) timeOClock = "05:00";
        else if (hour == 9)
        {
            SceneManager.LoadScene(loadSceneName, LoadSceneMode.Single);
        }
        //Debug.Log(textObj.text);
        textObj.text = timeOClock;
        hour += 1;
        targetTime = 60.0f;
    }
}
