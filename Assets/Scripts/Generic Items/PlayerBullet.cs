using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 20;
    public float lifetime = 5f;
    public float damage = 0.15f;
    public GameObject vfx_prefab;
    public GameObject wallDamageExplosion;
    public GameObject tableDamageExplosion;
    public GameObject damageExplosion;
    public AudioClip damageSFX;
    AudioSource _audio;
    SpriteRenderer _spriteRenderer;

    void Awake()
    {
        if (vfx_prefab != null)
            vfx_prefab = Instantiate(vfx_prefab, transform.localPosition, new Quaternion(0, 0, 0, 0), transform);

        _audio = GetComponent<AudioSource>();
        if (_audio == null)
        { // if AudioSource is missing
            Debug.LogWarning("AudioSource component missing from this gameobject. Adding one.");
            // add the AudioSource component dynamically
            _audio = gameObject.AddComponent<AudioSource>();
        }

        _spriteRenderer = GetComponent<SpriteRenderer>();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += transform.right * Time.deltaTime * speed;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Table") || collision.CompareTag("Enemy"))
        {
            // Play damage sound
            if (damageSFX)
            {
                PlaySound(damageSFX);
            }

            // Make bullet different color after richochet
            _spriteRenderer.color = new Color(0f, 0f, 0f, 0.1f);

            // If wall damage explosion prefab is provided, then instantiate it
            if (collision.CompareTag("Wall") && wallDamageExplosion)
            {
                Instantiate(wallDamageExplosion, transform.position, transform.rotation);
            }

            // If table damage explosion prefab is provided, then instantiate it
            if (collision.CompareTag("Table") && tableDamageExplosion)
            {
                Instantiate(tableDamageExplosion, transform.position, transform.rotation);
            }

            // If damage explosion prefab is provided, then instantiate it
            if (collision.CompareTag("Enemy") && damageExplosion)
            {
                Instantiate(damageExplosion, transform.position, transform.rotation);
            }

            DestroyBullet();
        }
    }

    void PlaySound(AudioClip clip)
    {
        _audio.PlayOneShot(clip);
    }

    //We destroy some things immediately, and then later we kill the entire game object, after Sound effect is done
    void DestroyBullet()
    {
        if (vfx_prefab != null)
            Destroy(vfx_prefab);
        Destroy(gameObject, damageSFX.length);
        GetComponent<PolygonCollider2D>().enabled = false; 
        this.enabled = false;
    }
}
