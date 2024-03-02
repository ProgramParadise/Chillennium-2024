using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStats : MonoBehaviour
{
    [Header("Enemy Stats")]

    [HideInInspector]
    public bool alive = true;
    public float max_health;
    float health = 1f;
    public float regenRate = 0f;
    public float speed = 5f;

    [Header("Enemy particle effects")]
    public GameObject deathExplosion;

    [Header("Enemy sound effects")]
    AudioSource _audio;
    public AudioClip deathSFX;

    [Header("Healthbar")]
    public GameObject healthbarObject;
    public GameObject healthbarBackgroundObject;
    Transform healthbarTransform;
    SpriteRenderer healthbar;
    SpriteRenderer healthbarBackground;
    float healthbarAlpha;
    float healthbarBackgroundAlpha;
    public float fadeOutWaitTime = 5f;
    public float fadeInTime = 0.1f;
    public float fadeOutTime = 1f;
    bool fadedOut = false;

    // Component references
    EnemyProjectileManager _enemyController;
    Animator _animator;

    void Awake()
    {
        health = max_health;
        _audio = GetComponent<AudioSource>();
        if (_audio == null)
        { // if AudioSource is missing
            Debug.LogWarning("AudioSource component missing from this gameobject. Adding one.");
            // add the AudioSource component dynamically
            _audio = gameObject.AddComponent<AudioSource>();
        }

        healthbarTransform = healthbarObject.GetComponent<Transform>();
        healthbar = healthbarObject.GetComponent<SpriteRenderer>();
        healthbarBackground = healthbarBackgroundObject.GetComponent<SpriteRenderer>();

        healthbarBackground.GetComponent<Transform>().localScale = new Vector3(max_health, healthbarBackground.GetComponent<Transform>().localScale.y, healthbarBackground.GetComponent<Transform>().localScale.z);
        healthbarTransform.localScale = new Vector3(health, healthbarTransform.localScale.y, healthbarTransform.localScale.z);

        healthbarAlpha = healthbar.color.a;
        healthbarBackgroundAlpha = healthbarBackground.color.a;

        _enemyController = GetComponent<EnemyProjectileManager>();

        _animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Fade out healthbar when not taking damage and at full health
        if (health >= max_health && !fadedOut)
        {
            health = max_health;
            if (fadeOutWaitTime > 0)
            {
                fadeOutWaitTime -= Time.deltaTime;
            }
            else
            {
                fadeOutWaitTime = 0;
                StartCoroutine(SpriteCrossFadeAlpha(healthbar, 0, fadeOutTime));
                StartCoroutine(SpriteCrossFadeAlpha(healthbarBackground, 0, fadeOutTime));
                fadedOut = true;
            }
        }
        else if (health > 0 && health < max_health)
        {
            float regen = regenRate * Time.deltaTime;

            health += regen;

            // Change healthbar scale
            healthbarTransform.localScale = new Vector3(health, healthbarTransform.localScale.y, healthbarTransform.localScale.z);

            // Align left
            healthbarTransform.position = new Vector3(healthbarTransform.position.x - regen / 2f, healthbarTransform.position.y, healthbarTransform.position.z);
        }
    }

    void PlaySound(AudioClip clip)
    {
        _audio.PlayOneShot(clip);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player Bullet"))
        {
            modifyHealth(collision.GetComponent<PlayerBullet>().damage);

            // Make player bullet no longer deal damage after richochet
            collision.GetComponent<PlayerBullet>().damage = 0;
        }
    }

    public void Die()
    {
        if (alive)
        {
            // Can die only once
            alive = false;

            //    _enemyController.StopMoving();
            _animator.SetTrigger("Die");

            // Set healthbar to empty
            healthbarTransform.localScale = new Vector3(0, healthbarTransform.localScale.y, healthbarTransform.localScale.z);

            // Align left
            healthbarTransform.position = new Vector3(healthbarTransform.position.x - max_health / 2f, healthbarTransform.position.y, healthbarTransform.position.z);

            // Play enemy death sound
            if (deathSFX)
            {
                PlaySound(deathSFX);
            }

            // Death particle effect
            if (deathExplosion)
            {
                Instantiate(deathExplosion, transform.position, transform.rotation);
            }

            Destroy(gameObject, deathSFX.length);
        }

    }

    public void CheckHealth(float damage = 0)
    {
        if (health > 0)
        {
            // Healthbar fade in
            fadedOut = false;
            fadeOutWaitTime = 5f;
            StartCoroutine(SpriteCrossFadeAlpha(healthbar, healthbarAlpha, fadeInTime));
            StartCoroutine(SpriteCrossFadeAlpha(healthbarBackground, healthbarBackgroundAlpha, fadeInTime));

            health -= damage;

            // Change healthbar scale
            healthbarTransform.localScale = new Vector3(health, healthbarTransform.localScale.y, healthbarTransform.localScale.z);

            // Align left
            healthbarTransform.position = new Vector3(healthbarTransform.position.x - damage / 2f, healthbarTransform.position.y, healthbarTransform.position.z);
        }
        else
        {
            Die();
        }
    }

    public void modifyHealth(float damage)
    {
        if (alive)
        {
            CheckHealth(damage);
        }

        CheckHealth();
    }

    public static IEnumerator SpriteCrossFadeAlpha(SpriteRenderer target, float targetAlpha, float duration)
    {
        float currentTime = 0;

        float r = target.color.r;
        float g = target.color.g;
        float b = target.color.b;

        float start = target.color.a;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            target.color = new Color(r, g, b, Mathf.Lerp(start, targetAlpha, currentTime / duration));
            yield return null;
        }
        yield break;
    }
}
