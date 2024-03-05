using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleSwinging : MonoBehaviour
{
    private GameObject pole;
    private CameraFollow Camera;
    private Player_Movement player;
    public float Vmax;
    public float Vmin;
    public float Vstep;
    public bool isTouchingPole = false;
    //public Animator animator;
    public float AngularVelocity;
    public float animationSpeed;
    public float newSlowX;
    public float newSlowY;
    public float theta;
    private float OldSlowX;
    private float OldSlowY;
    private Vector3 point;
    private Vector3 axis;
    public float VelocityXModifier;
    public float VelocityYModifier;
    public float VelocityXMax;
    public float VelocityYMax;
    private float dTheta;
    private bool foundStart = false;
    private bool canUseTheta = true;
    public bool XVelocityAlwaysPositive; //this is only a temporary fix :)
    private float waitTime = 0;
    public float VelocityX;
    private bool canStart = true;
    public bool shouldUseD = false;
    private bool accel = false;

    void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Hello World");
        if (collision.gameObject.tag == "pole")
        {
            if (VelocityX != 0) VelocityX = 0;
            if (theta != 0) theta = 0;
            if (dTheta != 0) dTheta = 0;
            pole = collision.gameObject;
            point = pole.transform.position;
            player.transform.LookAt(transform.position + axis, pole.transform.position - player.transform.position);
            Camera = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
            OldSlowX = Camera.slowX;
            OldSlowY = Camera.slowY;
            Camera.slowX = newSlowX;
            Camera.slowY = newSlowY;
            Debug.Log("CAMERA SHIT:" + Camera.slowX + ", " + Camera.slowY);
            theta = Mathf.Atan(Mathf.Abs(point.x - gameObject.transform.position.x)/Mathf.Abs(point.y - gameObject.transform.position.x)) * (180/Mathf.PI);
            if (point.x > gameObject.transform.position.x && point.y > gameObject.transform.position.y)
            {
                theta = 0 - theta;
            }
            else if (point.x > gameObject.transform.position.x && point.y < gameObject.transform.position.y)
            {
                theta = -90 - (90 - theta);
                
            }
            else if (point.x < gameObject.transform.position.x && point.y < gameObject.transform.position.y)
            {
                theta = -180 - theta;
                
            }
            else if (point.x < gameObject.transform.position.x && point.y > gameObject.transform.position.y)
            {
                theta = -270 - (90 - theta);
                
            }
            else if (point.x == gameObject.transform.position.x && point.y > gameObject.transform.position.y)
            {
                theta = 0;
                
            }
            else if (point.x == gameObject.transform.position.x && point.y < gameObject.transform.position.y)
            {
                theta = -180;
                
            }
            else if (point.x < gameObject.transform.position.x && point.y == gameObject.transform.position.y)
            {
                theta = -270;
                
            }
            else if (point.x > gameObject.transform.position.x && point.y == gameObject.transform.position.y)
            {
                theta = -90;
                
            }

            Debug.Log(theta);
            player.rb.simulated = false;
            isTouchingPole = true;
            //pole.GetComponent<Collider2D>().isTrigger = false;
        }
    }

    void Start()
    {
        axis = new Vector3(0, 0, 1);
        AngularVelocity = Vmin;
        player = gameObject.GetComponent<Player_Movement>();
    }


    /* Detect initial collision between the pole Collider2D and the Player Collider
     * Use polePosition as the fixed point the player is rotated around
     * Get camera SlowX value and SlowY values, store
     * - Then set SlowX and SlowY to something far smaller (i.e. 0.1)
     * ALSO MAKE IT WOKR WITH ANIMATION FRAMES
     * When "D" is pushed, slowly accelerate from Vmin until the player reaches Vmax
     * - or lets go of "D", in which case, slow down the players velocity until Vmin
     * When the player is on left side, have bottom pole visible
     * - when the player is on right side, have top pole visible
     * When the player presses "SPACE":
     * - Acceleration = 0
     * - Calculate the x and y components of velocity
     * -- Vx = (angular velocity) * cos(theta)
     * -- Vy = (angular velocity) * sin(theta)
    */



    void Update()
    {
        /*if (!foundStart)
        {
            if (gameObject.transform.position.x > point.x - 0.2 && gameObject.transform.position.x < point.x + 0.2 && gameObject.transform.position.y < point.y)
            {
                foundStart = true;
                canUseTheta = true;
                theta = 0;
            }
        }*/
        /*if (theta > 0)
        {
            canStart = true;
        }*/
        if (shouldUseD)
        {
            if (Input.GetAxis("Horizontal1") > 0.01)
            {
                accel = true;
            }
            else accel = false;
        }
        else accel = true;
        if (theta < 0) canUseTheta = false;
        else canUseTheta = true;
        if (!isTouchingPole && waitTime > 0)
        {
            waitTime--;
            if (waitTime == 0)
            {
                pole.GetComponent<Collider2D>().isTrigger = false;
            }
        }
        if (isTouchingPole && canStart)
        {
            dTheta = (Time.deltaTime * AngularVelocity) * (180 / Mathf.PI);
            transform.RotateAround(point, axis, dTheta);
            if (AngularVelocity < Vmax && accel)
            {
                AngularVelocity += Vstep;
            }
            else if (AngularVelocity > Vmin + Vstep && !accel)
            {
                AngularVelocity -= Vstep;
            }
            theta += dTheta;
            if (theta + dTheta > 360)
            {
                theta = (theta + dTheta) - 360;
            }
            Debug.Log(theta);
        } 
        if (isTouchingPole && canUseTheta && canStart)
        {
            Debug.Log("Touching Pole");
            /*dTheta = (Time.deltaTime * AngularVelocity);*/
            

            //animator.SetFloat("Speed", animationSpeed);
            VelocityX = Mathf.Abs((1.19f) * (AngularVelocity) * Mathf.Cos(theta * (Mathf.PI / 180)) * VelocityXModifier);
            if (theta < 270 && theta > 160)
            {
                VelocityX = -1 * VelocityX;
            }
            Debug.Log(theta + ", " + VelocityX);
            //Debug.Log(theta + ", " + Mathf.Cos(theta * (Mathf.PI / 180)));
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Camera.slowX = OldSlowX;
                Camera.slowY = OldSlowY;

                //Debug.Log("Final: " + theta + ", " + Mathf.Cos(theta * (Mathf.PI / 180)));
                //VelocityX = (AngularVelocity) * Mathf.Cos(theta * (Mathf.PI / 180)) * VelocityXModifier;
                if (VelocityX > VelocityXMax) VelocityX = VelocityXMax;
                if (VelocityX < -1 * VelocityXMax) VelocityX = -1 * VelocityXMax;
                //if ((theta > 90 && theta < 270)) VelocityX = -1 * Mathf.Abs(VelocityX);

                float VelocityY = Mathf.Abs((AngularVelocity) * Mathf.Sin(theta * (Mathf.PI / 180)) * VelocityYModifier);
                //if (VelocityY < -0.5f * VelocityYMax) VelocityY = -0.5f * VelocityYMax;
                if (VelocityY > VelocityYMax) VelocityY = VelocityYMax;
                player.rb.simulated = true;
                Debug.Log(VelocityX + ", " + VelocityY + ", " + theta);
                player.rb.velocity = new Vector2(VelocityX, VelocityY);
                isTouchingPole = false;
                pole.GetComponent<Collider2D>().isTrigger = true;
                waitTime = 60;
            }
        }
    }
}
