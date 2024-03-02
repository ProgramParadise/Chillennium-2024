using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeTracks : MonoBehaviour
{
    public GameObject _audio;
    AudioSource[] AudioSrcArr;
    public AudioClip optionalAudio;
    public float volume;
    public float transitionAmount;
    public int transitionDelay;

    int iterations = 0;

    bool shouldTransition;
    string transitionDirection;

    bool triggered;
    public bool forStart;
    bool startUp;

    float prevVolume;

    void Start()
    {
        _audio = GameObject.Find("Audio Source");
        AudioSrcArr = _audio.GetComponents<AudioSource>();
        AudioSrcArr[0].Play();
        prevVolume = volume;
    }

    void Update()
    {
        if (startUp)
        {
            if (!AudioSrcArr[0].isPlaying)
            {
                AudioSrcArr[0].Play();
            }
            Debug.Log("StartUp");
            Debug.Log(AudioSrcArr[0].volume);
            if (AudioSrcArr[0].volume < volume)
            {
                Debug.Log("<");
                if (iterations % transitionDelay == 0)
                {
                    FadeInOutAudio(AudioSrcArr[0], "in");
                }
            }
            else if (AudioSrcArr[0].volume >= volume)
            {
                startUp = false;
            }
        }
        else if (shouldTransition)
        {
            
            if (AudioSrcArr[0].volume > 0.0 || AudioSrcArr[1].volume < volume)
            {
                if (iterations % transitionDelay == 0)
                {
                    if (transitionDirection == "out")
                    {
                        FadeInOutAudio(AudioSrcArr[0], "out");
                    }
                    else
                    {
                        FadeInOutAudio(AudioSrcArr[1], "in");
                    }
                }
            }
            else if (AudioSrcArr[1].volume >= volume)
            {
                triggered = false;
            }
        }
        else if (volume != prevVolume)
        {
            if (AudioSrcArr[0].volume > 0)
            {
                AudioSrcArr[0].volume = volume;
                prevVolume = volume;
            }
            else
            {
                AudioSrcArr[1].volume = volume;
                prevVolume = volume;
            }
        }

        if (triggered)
        {
            TransitionSound(AudioSrcArr, optionalAudio);
        }
        iterations++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!forStart)
            {
                triggered = true;
                TransitionSound(AudioSrcArr, optionalAudio);
            }
            else
            {
                startUp = true;
            }
        }
    }

    private void FadeInOutAudio(AudioSource audio, string fadeDirection = "out")
    {
        if (fadeDirection.ToLower() == "out")
        {
            if (audio.volume > 0.0)
            {
                Debug.Log("Lowering Volume");
                audio.volume -= transitionAmount;
            }
        }
        else
        {
            if (audio.volume < volume)
            {
                audio.volume += transitionAmount;
            }
        }
    }

    private void SecondClip()
    {
        AudioSrcArr[1].volume = 0;
        AudioSrcArr[1].Play();
        shouldTransition = true;
        transitionDirection = "in";
    }

    private void TransitionSound(AudioSource[] audioSources, AudioClip transitionClip)
    {
        if (!audioSources[1].isPlaying)
        {
            if (audioSources[0].clip != transitionClip)
            {
                if (audioSources[0].volume > 0)
                {
                    shouldTransition = true;
                    transitionDirection = "out";
                }
                else
                {
                    shouldTransition = false;
                    transitionDirection = "";
                }
            }
            
            if (transitionClip != null && !shouldTransition)
            {
                if (transitionClip.length > 0)
                {
                    if (audioSources[0].clip != transitionClip)
                    {
                        Debug.Log("playing soundfx");
                        audioSources[0].clip = transitionClip;
                        audioSources[0].volume = volume;
                        audioSources[0].loop = false;
                        audioSources[0].Play();
                        Invoke("SecondClip", transitionClip.length);
                    }
                }
            }
            
            if (transitionClip == null)
            {
                SecondClip();
            }
        }
    }
}
