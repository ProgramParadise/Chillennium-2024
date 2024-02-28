using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour
{
    public GameObject hand;
    public InPickupReach reach;

    public int currentSlot = 0;

    public List<GameObject> inventory;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentSlot = 0;
            UpdateActive();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentSlot = 1;
            UpdateActive();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            try
            {
                GameObject obj = reach.Grab(inventory);
                obj.GetComponent<ItemPickup>().Pickup(hand);
                inventory.Add(obj);
            }
            catch
            {
                Debug.Log("No item in range to pickup");
            }
            UpdateActive();
        }
    }

    void UpdateActive()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (i == currentSlot)
            {
                inventory[i].SetActive(true);
            }
            else
            {
                inventory[i].SetActive(false);
            }
        }
    }
}
