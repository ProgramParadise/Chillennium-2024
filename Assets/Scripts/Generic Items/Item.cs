using System.Collections;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{

    Vector3 mousePos;
    Vector3 objectPos;

    ItemPickup _itemPickup;
    public Transform muzzle;
    float angle;

    // Damage
    [Header("Damage")]
    public float damage;

    // Fire cooldown
    [Header("Cooldown")]
    public float cooldown;
    float cooldownTimer = 0;

    // Reloading and Bullets
    [Header("Reloading and Bullets")]
    public GameObject bullet;
    public AudioClip shootSFX;
    public GameObject muzzleFlash;
    public int bulletMaximum;
    int bulletsLeft;
    public int cartridgeSize;
    int cartridgeCurrent;
    public float reloadTime;
    float reloading = -1;

    // Burst and Scatter
    [Header("Burst and Scatter")]
    public int burstSize = 1;
    public float burstScatter = 0.0f;
    public float burstSpacing = 0;

    // UI elements
    [SerializeField]
    InventoryUI inventoryUI;

    AudioSource _audio;

    void Start()
    {
        Debug.Log("Yea it started");
        _audio = GetComponent<AudioSource>();
        if (_audio == null)
        { // if AudioSource is missing
            Debug.LogWarning("AudioSource component missing from this gameobject. Adding one.");
            // add the AudioSource component dynamically
            _audio = gameObject.AddComponent<AudioSource>();
        }

        inventoryUI = GameObject.Find("Inventory UI").GetComponent<InventoryUI>();
        cartridgeCurrent = cartridgeSize;
        bulletsLeft = bulletMaximum;

        // get a reference to the components we are going to be changing and store a reference for efficiency purposes
        _itemPickup = GetComponent<ItemPickup>();

        // if Transform is missing
        if (_itemPickup == null)
        {
            Debug.LogError("ItemPickup component missing from this gameobject (" + gameObject.name + "). Adding one.");

            // add the Transform component dynamically
            _itemPickup = gameObject.AddComponent<ItemPickup>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_itemPickup.pickedUp)
        {
            mousePos = Input.mousePosition;
            mousePos.z = 5.23f; //The distance between the camera and object
            objectPos = Camera.main.WorldToScreenPoint(transform.position);
            mousePos.x = mousePos.x - objectPos.x;
            mousePos.y = mousePos.y - objectPos.y;
            angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

            // Flips the gun
            if (mousePos.x < 0)
            {
                transform.rotation = Quaternion.Euler(new Vector3(180, 0, -angle));
            }
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }

            // Firing action
            if (Input.GetMouseButton(0) && cooldownTimer < 0 && reloading < 0 && bulletsLeft / cartridgeSize >= 0)
            {
                StartCoroutine(Fire());
                cooldownTimer = cooldown;
            }

            // UI Updates
            if (reloading < 0)
            {
                inventoryUI.chamber = cartridgeCurrent.ToString();
            }
            else
            {
                inventoryUI.chamber = "RELOADING";
            }

            if (bulletsLeft / cartridgeSize >= 0)
            {
                inventoryUI.totalAmmo = (bulletsLeft / cartridgeSize).ToString();
            }
            else
            {
                inventoryUI.chamber = "0";
                inventoryUI.totalAmmo = "0";
            }

            cooldownTimer -= 1;
            reloading -= 1;
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }

    IEnumerator Fire()
    {
        for (int i = 0; i < burstSize; i++)
        {
            if (shootSFX)
            {
                PlaySound(shootSFX);
            }
            float randomAngle = Random.Range(-burstScatter, burstScatter);
            Instantiate(bullet, muzzle.position, Quaternion.Euler(new Vector3(0, 0, angle + randomAngle)));
            Instantiate(muzzleFlash, muzzle.position, Quaternion.Euler(new Vector3(0, 0, angle + randomAngle)));
            cartridgeCurrent -= 1;
            if (cartridgeCurrent == 0)
            {
                reloading = reloadTime;
                bulletsLeft -= cartridgeSize;
                cartridgeCurrent = cartridgeSize;
            }
            yield return new WaitForSeconds(burstSpacing);
        }
    }

    void PlaySound(AudioClip clip)
    {
        _audio.PlayOneShot(clip);
    }
}
