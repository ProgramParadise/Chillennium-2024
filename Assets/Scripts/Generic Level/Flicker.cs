using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    public bool cheap = true;

    public float minAlpha = 0.5f;
    public float maxAlpha = 0.9f;

    public float speed = 0.5f;

    [HideInInspector]
    private float randomStart = 0;

    private void Awake()
    {
        randomStart = Random.Range(0, 2 * Mathf.PI);

        gameObject.GetComponent<SpriteRenderer>().color = new Color(
            1f,
            1f,
            1f,
            Mathf.Lerp(
                minAlpha,
                maxAlpha,
                Mathf.PingPong(Time.time * speed + randomStart, maxAlpha - minAlpha)
            )
        );
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(
            1f,
            1f,
            1f,
            Mathf.Lerp(
                minAlpha,
                maxAlpha,
                Mathf.PingPong(Time.time * speed + randomStart, maxAlpha - minAlpha)
            )
        );

        if (cheap && Time.time % Mathf.Round(Random.Range(5, 15)) <= 1 && Time.time > 5)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(
                1f,
                1f,
                1f,
                Mathf.Lerp(
                    minAlpha,
                    maxAlpha,
                    Mathf.PingPong(
                        Time.time * speed + randomStart + Mathf.PI / 6,
                        maxAlpha - minAlpha
                    )
                )
            );
        }
    }
}
