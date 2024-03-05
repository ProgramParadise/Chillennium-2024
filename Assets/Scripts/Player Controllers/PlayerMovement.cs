using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public bool playerCanMove = true;
    public bool playerCanMoveVertical = true;
    public bool playerCanJump = false;
    public KeyCode userKey = KeyCode.Space;
    public string axesNumber = "1";
    public Rigidbody2D rb;
    private PlayerStats stats;
    public Animator animator;
    float moveHorizontal;
    float moveVertical;
    public float buttonTime = 0.5f;
    public float jumpHeight = 5;
    public float cancelRate = 100;
    float jumpTime;
    bool jumping;
    bool jumpCancelled;
    bool hasJumped = false;
    private float waitTime = 0;
    public float raycastShiftX;
    public float raycastShiftY;

    public LayerMask groundLayer;

    bool IsGrounded()
    {
        Vector2 position = transform.position;
        Vector2 position2 = new Vector2(transform.position.x + raycastShiftX, transform.position.y + raycastShiftY);
        Vector2 direction = Vector2.down;
        float distance = 1.0f;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(position2, direction, distance, groundLayer);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.tag == "Wall")
            {
                return true;
            }
        }
        if (hit2.collider != null)
        {
            if (hit2.collider.gameObject.tag == "Wall")
            {
                return true;
            }
        }
        return false;
    }

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        stats = gameObject.GetComponent<PlayerStats>();
        if (playerCanJump) playerCanMoveVertical = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!animator.GetBool("Jump") && !animator.GetBool("pole") && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || (!animator.GetBool("Jump") && !animator.GetBool("pole")) && Mathf.Abs(Input.GetAxis("Horizontal1")) > 0))
        {
            animator.SetFloat("runSpeed", (Mathf.Abs(rb.velocity.x) / 7.5f));
            animator.SetFloat("Speed", 1);
        }
        else if (!animator.GetBool("Jump") && !animator.GetBool("pole"))
        {
            animator.SetFloat("Speed", 0);
        }
        if (hasJumped && IsGrounded() && !gameObject.GetComponent<PoleSwinging>().isTouchingPole && waitTime == 0)
        {
            Debug.Log("Should end jump anim");
            animator.SetBool("Jump", false);
            hasJumped = false;
        }else if (hasJumped && IsGrounded() && !gameObject.GetComponent<PoleSwinging>().isTouchingPole)
        {
            Debug.Log("Wait: " + waitTime);
            waitTime--;
        }
        if (moveHorizontal < -0.01 && !gameObject.GetComponent<PoleSwinging>().isTouchingPole)
        {
            gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        if (moveHorizontal > 0.01 && !gameObject.GetComponent<PoleSwinging>().isTouchingPole)
        {
            gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (playerCanJump)
        {
            if (Input.GetKeyDown(userKey) && IsGrounded())
            {
                animator.SetBool("Jump", true);
                float jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rb.gravityScale));
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

                jumping = true;
                jumpCancelled = false;
                jumpTime = 0;
                
                Debug.Log("Hello ther");
                hasJumped = true;
                waitTime = 10;
            }

            if (jumping)
            {
                jumpTime += Time.deltaTime;
                if (Input.GetKeyUp(userKey))
                {
                    jumpCancelled = true;
                    
                }

                if (jumpTime > buttonTime)
                {
                    jumping = false;
                }
            }
        }

        if (gameObject.GetComponent<PoleSwinging>().isTouchingPole)
        {
            animator.SetBool("pole", true);
        }
        else
        {
            animator.SetBool("pole", false);
            //if (!animator.GetBool("Jump") && !animator.GetBool("pole")) animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        }
    }

    private void FixedUpdate()
    {
        moveHorizontal = Input.GetAxis("Horizontal" + axesNumber);
        if (playerCanMoveVertical)
        {
            moveVertical = Input.GetAxis("Vertical" + axesNumber);
        }
        else moveVertical = 0f;

        if (playerCanJump)
        {
            if (jumpCancelled && jumping && rb.velocity.y > 0)
            {
                rb.AddForce(Vector2.down * cancelRate);
            }
        }

        if (IsGrounded())
        {
            if (playerCanMoveVertical)
            {
                rb.velocity = new Vector2(moveHorizontal * stats.speed, moveVertical * stats.speed);
                if (rb.velocity.magnitude > stats.speed)
                {
                    rb.velocity = rb.velocity * (stats.speed / rb.velocity.magnitude);
                }
            }
            else
            {
                rb.velocity = new Vector2(moveHorizontal * stats.speed, rb.velocity.y);
            }
        }
        
    }
}