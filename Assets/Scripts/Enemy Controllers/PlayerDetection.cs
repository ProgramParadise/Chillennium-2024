using UnityEngine;
using System.Collections;
using System;
public class PlayerDetection : MonoBehaviour
{
    public Transform origin, end;
    public float max_tracking_dist;       //radius at which an enemy will disengage from a player (must be >= radar_dist)
    public float radar_dist;              //radius at which an enemy searches for players
    public float aggro_enable_dist;       //radius at which an enemy will aggro onto a player (must be <= max_tracking_dist)
    public float aggro_disable_dist;      //radius at which an enemy will de-aggro from a player (must be >= aggro_enable_dist)


    [HideInInspector]
    public Transform playerTransform;
    float playerInsantiateDelay = 2f; // How long to check again if the player object exists after it was not found the first time

    public float radarSpd;
    public bool playerDetected;
    public bool playerAggroed;

    private int playerLayer;
    private Rigidbody2D enemyRb;
    public Vector3 facePlayer;

    private void Start()
    {
        end.position = origin.position + new Vector3(radar_dist, 0, 0);
        playerLayer = LayerMask.GetMask("Player");

        // Setup reference to player transform
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }

        if (playerTransform == null)
        {
            Debug.LogError("Player not found in PlayerDetection. Attempting to find player again in " + playerInsantiateDelay + " seconds");
            StartCoroutine(GetPlayer());
        }
        enemyRb = GetComponentInParent<Rigidbody2D>();
    }

    private void Update()
    {
        PlayerDetector();
        if (playerDetected == false)
        {
            Radar();
        }
        else //player is detected
        {
            TrackPlayer();
        }

    }

    //raycast from player to enemy
    void PlayerDetector()
    {
        Debug.DrawLine(origin.position, end.position, Color.red);
        playerDetected = Physics2D.Linecast(origin.position, end.position, playerLayer);
    }

    public bool PlayerIsDetected()
    {
        return playerDetected;
    }

    void Radar()
    {
        end.RotateAround(origin.position, Vector3.forward, radarSpd * Time.deltaTime);
    }

    void PlayersPosition()
    {
        facePlayer = playerTransform.position - enemyRb.transform.GetChild(0).GetChild(0).position;

        // Rotate enemy to face player
        // enemyRb.transform.GetChild(0).GetChild(0).up = -facePlayer;
    }

    void TrackPlayer()
    {
        end.position = playerTransform.position;
        float player_distance = Vector3.Distance(origin.position, end.position);

        if (player_distance > max_tracking_dist)
        {
            playerDetected = false;
            end.position = origin.position + new Vector3(radar_dist, 0, 0);
            return;
        }
        if (player_distance > aggro_disable_dist)
        {
            playerAggroed = false;
        }
        if (player_distance <= aggro_enable_dist)
        {
            playerAggroed = true;
        }

        PlayersPosition();
    }

    // Make sure to access player only when it is instantiated
    IEnumerator GetPlayer()
    {
        yield return new WaitForSeconds(playerInsantiateDelay);

        // Setup reference to player transform
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        if (playerTransform == null)
            Debug.LogError("Player not found in PlayerDetection");
    }
}
