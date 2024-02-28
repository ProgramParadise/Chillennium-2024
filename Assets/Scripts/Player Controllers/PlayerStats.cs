using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    public float health = 1f;
    public float regenRate = 0.05f;
    public float speed = 10;

    [Header("Healthbar")]
    public Image healthbar;
    public Image healthbarBackground;
    float healthbarAlpha;
    float healthbarBackgroundAlpha;
    public float fadeOutWaitTime = 5f;
    public float fadeInTime = 0.1f;
    public float fadeOutTime = 1f;
    bool fadedOut = false;

    [Header("Sound effects")]
    AudioSource _audio;

    void Awake()
    {
        _audio = GetComponent<AudioSource>();
        if (_audio == null)
        { // if AudioSource is missing
            Debug.LogWarning("AudioSource component missing from this gameobject. Adding one.");
            // add the AudioSource component dynamically
            _audio = gameObject.AddComponent<AudioSource>();
        }

        healthbar = GameObject.Find("Healthbar").GetComponent<Image>();
        GameObject hbb = GameObject.Find("Healthbar Background");
        if (hbb != null)
        {
            healthbarBackground = hbb.GetComponent<Image>();
            healthbarAlpha = healthbar.color.a;
            healthbarBackgroundAlpha = healthbarBackground.color.a;
        }

    }

    private void Start()
    {
    }

    void Update()
    {
        if (health < 0)
        {
            if (SceneManager.GetActiveScene().name == "Intro")
            {
                StartCoroutine(GameManager.gm.LoadLevel("Intro"));
            }
            else
            {
                StartCoroutine(GameManager.gm.LoadLevel("Level 1"));
            }
        }

        // Fade out healthbar when not taking damage and at full health
        if (health >= 1 && !fadedOut)
        {
            health = 1;
            if (fadeOutWaitTime > 0)
            {
                fadeOutWaitTime -= Time.deltaTime;
            }
            else
            {
                fadeOutWaitTime = 0;
                healthbar.CrossFadeAlpha(0, fadeOutTime, false);
                healthbarBackground.CrossFadeAlpha(0, fadeOutTime, false);
                fadedOut = true;
            }
        }
        else if (health > 0 && health < 1)
        {
            float regen = regenRate * Time.deltaTime;

            health += regen;
            healthbar.fillAmount += regen;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy Bullet"))
        {
            modifyHealth(collision.GetComponent<EnemyBullet>().damage);

            // Make enemy bullet no longer deal damage after richochet
            collision.GetComponent<EnemyBullet>().damage = 0;
        }
    }

    // Could either be used for dealing damage or healing
    public void modifyHealth(float damage)
    {
        // Healthbar fade in
        fadedOut = false;
        fadeOutWaitTime = 5f;
        if(healthbar != null)
            healthbar.CrossFadeAlpha(healthbarAlpha, fadeInTime, false);
        if (healthbarBackground != null)
            healthbarBackground.CrossFadeAlpha(healthbarBackgroundAlpha, fadeInTime, false);

        // Apply damage
        health -= damage;
        if (healthbar != null)
            healthbar.fillAmount -= damage;
    }
}
