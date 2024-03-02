using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    public float speed = 10;
    public bool isDead = false;

    private void Start()
    {
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "death")
        {
            isDead = true;
        }
    }
}
