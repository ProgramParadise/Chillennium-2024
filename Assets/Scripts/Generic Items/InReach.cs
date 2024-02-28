using System.Collections.Generic;
using UnityEngine;

public class InReach : MonoBehaviour
{
    // What items are in reach to pick up
    public List<string> itemNameInReach;
    public List<GameObject> itemInReach;

    void OnTriggerEnter2D(Collider2D item)
    {
        if (item.CompareTag("Pickup"))
        {
            itemNameInReach.Add(item.name);
            itemInReach.Add(item.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D item)
    {
        if (item.CompareTag("Pickup"))
        {
            itemNameInReach.Remove(item.name);
            itemInReach.Remove(item.gameObject);
        }
    }

    public GameObject Grab(List<GameObject> inv)
    {
        GameObject closest = itemInReach[0];
        Vector3 hand = gameObject.transform.position;
        float distance = 3f;

        foreach (GameObject item in itemInReach)
        {
            int findClosest = inv.FindIndex(item => item.name == item.name);

            Debug.Log("Item (" + item.name + ") in range to grab");
            Debug.Log("Finding item (" + item.name + "), result: " + findClosest);
            Debug.Log("Checking if item (" + item.name + ") is closest");

            if (findClosest == -1)
            {
                float dist = (hand - item.transform.position).magnitude;
                if (dist < distance)
                {
                    closest = item;
                    distance = dist;
                }
            }
        }

        Debug.Log("Picking up item (" + closest.name + ")");
        return closest;
    }
}