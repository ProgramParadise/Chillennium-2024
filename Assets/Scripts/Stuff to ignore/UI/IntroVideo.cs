using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class IntroVideo : MonoBehaviour
{
    VideoPlayer video;
    public GameObject UI;
    public float volume = 0.3f;

    // How long the intro is in seconds where you can't skip ahead    
    public float introTime = 15.95f;

    // What time in seconds to skip to
    public float skipTime = 140.5f;

    // How long the video is
    public float videoLength = 148f;

    void Start()
    {
        video = gameObject.GetComponent<VideoPlayer>();
        video.GetComponent<AudioSource>().volume = volume;
    }

    void Update()
    {
        if (video.GetComponent<AudioSource>().volume != volume)
        {
            video.GetComponent<AudioSource>().volume = volume;
        }

        // Skip to outro if not in intro
        if (Input.anyKeyDown && video.time < skipTime && video.time >= introTime)
        {
            video.time = skipTime;
        }

        // Reach end of video
        if (video.time > videoLength || Input.GetKeyDown(KeyCode.Escape))
        {
            UI.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
