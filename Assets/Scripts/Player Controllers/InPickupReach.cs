using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InPickupReach : MonoBehaviour
{

    public List<string> itemName;
    public List<GameObject> itemInReach;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pickup") && !itemName.Contains(other.name))
        {
            itemName.Add(other.name);
            itemInReach.Add(other.gameObject);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            itemName.Remove(other.name);
            itemInReach.Remove(other.gameObject);
        }

    }

    public GameObject Grab(List<GameObject> inventory)
    {
        // Initialize variables
        GameObject closestItem = itemInReach[0];
        Vector3 handPosition = gameObject.transform.position;
        float closestDistance = 10000000000f;

        // Iterate through each item in reach
        foreach (GameObject itemObject in itemInReach)
        {
            // Check if item is in inventory
            int itemIndex = inventory.FindIndex(item => item.name == itemObject.name);

            // Debug information
            Debug.Log("Item (" + itemObject.name + ") in range to grab");
            Debug.Log("Finding item (" + itemObject.name + "), result: " + itemIndex);
            Debug.Log("Checking if item (" + itemObject.name + ") is closest");

            // If item is not in inventory, check if it's the closest
            if (itemIndex == -1)
            {
                float distance = (handPosition - itemObject.transform.position).magnitude;
                if (distance < closestDistance)
                {
                    closestItem = itemObject;
                    closestDistance = distance;
                }
            }
        }

        // Debug information
        Debug.Log("Picking up item (" + closestItem.name + ")");

        // Return closest item
        return closestItem;
    }
}
