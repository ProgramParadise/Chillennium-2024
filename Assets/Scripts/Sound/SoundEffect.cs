using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public AudioClip sound;
    AudioSource _audio;

    public float volume;

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
        _audio.volume = volume;
    }

    void Update()
    {
        if (_audio.volume != volume)
        {
            _audio.volume = volume;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlaySound(sound);
        }
    }

    void PlaySound(AudioClip clip)
    {
        _audio.PlayOneShot(clip);
    }
}
