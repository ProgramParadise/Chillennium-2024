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
    private float theta;
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

    void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Hello World");
        if (collision.gameObject.tag == "pole")
        {
            pole = collision.gameObject;
            point = pole.transform.position;
            Camera = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
            OldSlowX = Camera.slowX;
            OldSlowY = Camera.slowY;
            Camera.slowX = newSlowX;
            Camera.slowY = newSlowY;
            Debug.Log("CAMERA SHIT:" + Camera.slowX + ", " + Camera.slowY);
            theta = Mathf.Atan(Mathf.Abs(point.x - gameObject.transform.position.x)/Mathf.Abs(point.y - gameObject.transform.position.x)) * (180/Mathf.PI);
            if (point.x > gameObject.transform.position.x)
            {
                theta = 0 - theta;
            }
            Debug.Log(theta);
            player.rb.simulated = false;
            isTouchingPole = true;
            pole.GetComponent<Collider2D>().isTrigger = false;
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
        if (isTouchingPole)
        {
            dTheta = (Time.deltaTime * AngularVelocity);
            transform.RotateAround(point, axis, dTheta);
            if (AngularVelocity < Vmax && Input.GetAxis("Horizontal1") > 0.01)
            {
                AngularVelocity += Vstep;
            }
            else if (AngularVelocity > Vmin + Vstep && Input.GetAxis("Horizontal1") < 0.01)
            {
                AngularVelocity -= Vstep;
            }
        } 
        if (isTouchingPole && canUseTheta)
        {

            Debug.Log("Touching Pole");
            /*dTheta = (Time.deltaTime * AngularVelocity);*/
            theta += dTheta;
            if (theta + dTheta > 360)
            {
                theta = (theta + dTheta) - 360;
            }
            
            //animator.SetFloat("Speed", animationSpeed);
            
            Debug.Log(theta + ", " + Mathf.Cos(theta * (Mathf.PI / 180)));
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Camera.slowX = OldSlowX;
                Camera.slowY = OldSlowY;

                Debug.Log("Final: " + theta + ", " + Mathf.Cos(theta * (Mathf.PI / 180)));
                float VelocityX = (AngularVelocity) * Mathf.Cos(theta * (Mathf.PI / 180)) * VelocityXModifier;
                if (VelocityX > VelocityXMax) VelocityX = VelocityXMax;
                if (VelocityX < -1 * VelocityXMax) VelocityX = -1 * VelocityXMax;
                if ((theta > 90 && theta < 180) || (theta > 270 && theta < 360)) VelocityX *= -1;

                float VelocityY = ((AngularVelocity) * Mathf.Sin(theta * (Mathf.PI / 180)) * VelocityYModifier);
                if (VelocityY < -0.5f * VelocityYMax) VelocityY = -0.5f * VelocityYMax;
                if (VelocityY > VelocityYMax) VelocityY = VelocityYMax;
                player.rb.simulated = true;
                Debug.Log(VelocityX + ", " + VelocityY);
                player.rb.velocity = new Vector2(VelocityX, VelocityY);
                isTouchingPole = false;
                pole.GetComponent<Collider2D>().isTrigger = true;
            }
        }
    }
}
