using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    private float checkpointPositionX = -1000f;
    private float checkpointPositionY;
    public GameObject player;
    private CameraFollow Camera;
    private GameObject wipeObject;
    private bool doTransition = false;
    private float OldSlowX;
    private float OldSlowY;
    public string loadSceneName;
    private GameObject CameraObj;
    private float fuckCounter = 0;

    public float translateAmount;

    void OnPlayerDeath()
    {
        Camera.slowX = 0.0f;
        Camera.slowY = 0.0f;
        Debug.Log(Camera.slowX);
        if (checkpointPositionX != -1000f) player.transform.position = new Vector3(checkpointPositionX, checkpointPositionY, 0);
        else SceneManager.LoadScene(loadSceneName, LoadSceneMode.Single);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            checkpointPositionX = player.transform.position.x;
            checkpointPositionY = player.transform.position.y;
        }
    }

    void Start()
    {
        //wipeObject = GameObject.Find("Wipe");
        CameraObj = GameObject.Find("Main Camera");
        Camera = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        OldSlowX = Camera.slowX;
        OldSlowY = Camera.slowY;
    }

    void Update()
    {
        if (player.GetComponent<PlayerStats>().isDead) fuckCounter += 1;
        /*if (doTransition)
        {
            wipeObject.transform.Translate(translateAmount, 0, 0);
        }*/
        if (fuckCounter >= 75 && player.GetComponent<PlayerStats>().isDead)
        {

            Camera.slowX = OldSlowX;
            Camera.slowY = OldSlowY;

            player.GetComponent<PlayerStats>().isDead = false;
            fuckCounter = 0;
            //wipeObject.SetActive(false);
        }
        if (player.GetComponent<PlayerStats>().isDead || Input.GetKeyDown(KeyCode.R))
        {
            //wipeObject.GetComponent<SpriteRenderer>().enabled = true;
            //wipeObject.transform.position = new Vector3(CameraObj.transform.position.x - 100, CameraObj.transform.position.y, -3);
            //doTransition = true;
            OnPlayerDeath();


            /*if (wipeObject.transform.position.x == CameraObj.transform.position.x)
            {
                
            }*/
            //CameraObj.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
            
        }
        //else if (doTransition && !player.GetComponent<PlayerStats>().isDead)
        //{
            
            
            /*f (wipeObject.transform.position.x != CameraObj.transform.position.x)
            {
                //wipeObject.transform.position = new Vector3(CameraObj.transform.position.x, CameraObj.transform.position.y, -3);
            }
            if (wipeObject.transform.position.x < CameraObj.transform.position.x + 100)
            {
                //wipeObject.transform.Translate(translateAmount, 0, 0);
            }else
            {
                //wipeObject.GetComponent<SpriteRenderer>().enabled = false;
                player.GetComponent<PlayerStats>().isDead = false;
                *//*Camera.slowX = OldSlowX;
                Camera.slowY = OldSlowY;*//*
            }*/
        //}
    }
}
