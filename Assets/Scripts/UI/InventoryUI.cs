using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System;
using System.Linq.Expressions;

public class InventoryUI : MonoBehaviour
{
    public GameObject player;
    float playerInsantiateDelay = 2f; // How long to check again if the player object exists after it was not found the first time

    public GameObject chamberTextMesh;
    public GameObject totalAmmoTextMesh;
    public Image currentSlotImage;
    public Image nextSlotImage;

    SpriteRenderer currentSlotSpriteRenderer;
    SpriteRenderer nextSlotSpriteRenderer;

    Sprite currentSlotSprite;
    Sprite nextSlotSprite;

    AspectRatioFitter currentSlotAspectRatioFitter;
    AspectRatioFitter nextSlotAspectRatioFitter;

    float currentSlotAspectRatio;
    float nextSlotAspectRatio;

    GameObject currentItem;
    GameObject nextItem;

    public string chamber;
    public string totalAmmo;

    TextMeshProUGUI chamberText;
    TextMeshProUGUI totalAmmoText;
    Inventory inventorySlots;
    List<GameObject> inv;
    int currentIndex;
    int nextIndex;
    int inventoryLength;

    // Start is called before the first frame update
    void Start()
    {
        chamberText = chamberTextMesh.GetComponent<TextMeshProUGUI>();
        totalAmmoText = totalAmmoTextMesh.GetComponent<TextMeshProUGUI>();
        currentSlotImage = currentSlotImage.GetComponent<Image>();
        nextSlotImage = nextSlotImage.GetComponent<Image>();

        // Setup reference to player
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player == null)
        {
            Debug.LogError("Player not found in InventoryUI. Attempting to find player again in " + playerInsantiateDelay + " seconds");
            StartCoroutine(GetPlayer());
        }
        else
        {
            inventorySlots = player.GetComponent<Inventory>();

            CheckInventory();
        }
    }

    void CheckNextSlot()
    {
        if (inventoryLength > 1)
        {
            nextItem = inv[nextIndex].transform.GetChild(0).gameObject;
            nextSlotSpriteRenderer = nextItem.GetComponent<SpriteRenderer>();
            nextSlotSprite = nextSlotSpriteRenderer.sprite;

            nextSlotAspectRatioFitter = transform.GetChild(1).GetChild(0).GetComponent<AspectRatioFitter>();
            nextSlotAspectRatio = nextSlotSprite.rect.width / nextSlotSprite.rect.height;
            nextSlotAspectRatioFitter.aspectRatio = nextSlotAspectRatio;
            nextSlotImage.color = new Color(1, 1, 1, 1);
        }
        else
        {
            nextSlotImage.color = new Color(1, 1, 1, 0);
        }
    }

    void CheckInventory()
    {
        // Update inv to match inventorySlots.inventory
        inv = new List<GameObject>();

        if (inventorySlots == null)
        {
            inventorySlots = player.GetComponent<Inventory>();
        }

        for (int i = 0; i < inventorySlots.inventory.Count; i++)
        {
            inv.Add(inventorySlots.inventory[i]);
        }

        currentIndex = inventorySlots.currentSlot;
        inventoryLength = inv.Count;

        if (currentIndex == 0)
        {
            nextIndex = currentIndex + 1;
        }
        else
        {
            nextIndex = currentIndex - 1;
        }

        if (inventoryLength > 0)
        {
            try
            {
                currentItem = inv[currentIndex].transform.GetChild(0).gameObject;
                currentSlotSpriteRenderer = currentItem.GetComponent<SpriteRenderer>();
                currentSlotSprite = currentSlotSpriteRenderer.sprite;

                currentSlotAspectRatioFitter = transform.GetChild(0).GetChild(0).GetComponent<AspectRatioFitter>();
                currentSlotAspectRatio = currentSlotSprite.rect.width / currentSlotSprite.rect.height;
                currentSlotAspectRatioFitter.aspectRatio = currentSlotAspectRatio;
                currentSlotImage.color = new Color(1, 1, 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                currentSlotImage.color = new Color(1, 1, 1, 0);
                currentSlotAspectRatioFitter.aspectRatio = 1;

                CheckNextSlot();
                return;
            }

            CheckNextSlot();
        }
    }

    // Update is called once per frame
    void Update()
    {
        chamberText.text = chamber;
        totalAmmoText.text = totalAmmo;

        CheckInventory();

        if (inventoryLength > 0)
        {
            currentSlotImage.sprite = currentSlotSprite;

            if (inventoryLength > 1)
            {
                nextSlotImage.sprite = nextSlotSprite;
            }
        }
    }

    // Make sure to access player only when it is instantiated
    IEnumerator GetPlayer()
    {
        yield return new WaitForSeconds(playerInsantiateDelay);

        // Setup reference to player transform
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            Debug.LogError("Player not found in InventoryUI");
        else
        {
            CheckInventory();
        }
    }
}
