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

    public LayerMask groundLayer;

    bool IsGrounded()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = 1.0f;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider != null)
        {
            Debug.Log("Grounded");
            return true;
        }
        Debug.Log("Not Grounded");
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
        if (moveHorizontal > 0.01)
        {
            gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        if (moveHorizontal < -0.01)
        {
            gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (playerCanJump)
        {
            if (Input.GetKeyDown(userKey) && IsGrounded())
            {
                float jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rb.gravityScale));
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

                jumping = true;
                jumpCancelled = false;
                jumpTime = 0;
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

        animator.SetFloat("Speed", rb.velocity.magnitude);
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