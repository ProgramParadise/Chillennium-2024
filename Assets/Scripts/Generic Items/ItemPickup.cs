using UnityEngine;
using System.Collections;
using System;

public class ItemPickup : MonoBehaviour
{

    public Vector3 handleOffset;
    private Vector3 handlePosition;
    private GameObject handPosition;
    public GameObject player;
    float playerInsantiateDelay = 2f;
    public bool pickedUp = false;

    void Start()
    {
        // Setup reference to player
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found in ItemPickup. Attempting to find player again in " + playerInsantiateDelay + " seconds");
            StartCoroutine(GetPlayer());
        }
        handlePosition = transform.position - handleOffset;
    }

    void Update()
    {
        if (pickedUp)
        {
            gameObject.transform.position = handPosition.transform.position;
        }
    }
    public void Pickup(GameObject hand)
    {
        handPosition = hand;
        pickedUp = true;
    }
    void OnDrawGizmos()
    {
        handlePosition = transform.position - handleOffset;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(handlePosition, 0.05f);
    }

    // Make sure to access player only when it is instantiated
    IEnumerator GetPlayer()
    {
        yield return new WaitForSeconds(2f);

        // Setup reference to player
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            Debug.LogError("Player not found in ItemPickup");
    }
}
