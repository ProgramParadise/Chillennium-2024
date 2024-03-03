using UnityEngine;
using System.Collections;
using System;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public bool moveX = true;
    public bool moveY = true;
    public float slowX = 1;
    public float slowY = 1;
    float playerInsantiateDelay = 2f; // How long to check again if the player object exists after it was not found the first time
    public Vector3 last_position;
    public bool playingCutscene = false;
    void Start()
    {
        // Setup reference to player
        /*if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player == null)
        {
            Debug.LogError("Player not found in CameraFollow. Attempting to find player again in " + playerInsantiateDelay + " seconds");
            StartCoroutine(GetPlayer());
        }*/

        last_position = gameObject.transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!playingCutscene)
        {
            Vector3 lookpoint = player.transform.position - last_position;
            Vector2 error = (lookpoint * 20) + player.transform.position - gameObject.transform.position;
            if (moveX && moveY)
            {
                gameObject.transform.Translate((error.x / 10) * slowX, (error.y / 10) * slowY, 0);
            } else if (moveX && !moveY)
            {
                gameObject.transform.Translate((error.x / 10) * slowX, 0, 0);
            } else
            {
                gameObject.transform.Translate(0, (error.y / 10) * slowY, 0);
            }
            last_position = player.transform.position;
        }
    }

    // Make sure to access player only when it is instantiated
    IEnumerator GetPlayer()
    {
        yield return new WaitForSeconds(playerInsantiateDelay);

        // Setup reference to player
        /*if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            Debug.LogError("Player not found in CameraFollow");*/
    }

    public IEnumerator PlayCutscene(GameObject target, float time)
    {
        playingCutscene = true;

        while (time > 0)
        {
            Vector3 lookpoint = target.transform.position - last_position;
            Vector2 error = (lookpoint * 20) + target.transform.position - gameObject.transform.position;
            gameObject.transform.Translate(error.x / 10, error.y / 10, 0);
            last_position = target.transform.position;

            time -= Time.deltaTime;
            yield return null;
        }

        playingCutscene = false;
    }
}
