using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public bool loop = true;
    public AudioClip[] tracks;
    public int currentTrackIndex = 0;
    AudioSource _audio;
    public float fadeInTime = 5f;
    public float fadeOutTime = 5f;

    bool fadingOut = false;

    // Start is called before the first frame update
    void Start()
    {
        _audio = GetComponent<AudioSource>();
        if (_audio == null)
        { // if AudioSource is missing
            Debug.LogWarning("AudioSource component missing from this gameobject. Adding one.");
            // add the AudioSource component dynamically
            _audio = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (!_audio.isPlaying && currentTrackIndex < tracks.Length)
        {
            PlayNextTrack();
        }

        // Fade out when track is about to end
        if (!fadingOut && _audio.clip.length - _audio.time < fadeOutTime)
        {
            fadingOut = true;
            StartCoroutine(StartFade(_audio, fadeOutTime, 0));
        }
    }

    public void PlayNextTrack()
    {
        // Set volume to 0 to fade in volume
        _audio.volume = 0;

        // Play next track
        PlaySound(tracks[currentTrackIndex]);

        // Fade in
        StartCoroutine(StartFade(_audio, fadeInTime, 1));

        if (loop)
        {
            currentTrackIndex = (currentTrackIndex + 1) % tracks.Length;
        }
        else
        {
            currentTrackIndex++;
        }

        fadingOut = false;
    }

    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    void PlaySound(AudioClip clip)
    {
        _audio.PlayOneShot(clip);
    }
}
