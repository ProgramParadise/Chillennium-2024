using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public bool playerCanMove = true;
    private Rigidbody2D rb;
    private PlayerStats stats;
    public Animator animator;
    float moveHorizontal;
    float moveVertical;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        stats = gameObject.GetComponent<PlayerStats>();
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


        animator.SetFloat("Speed", rb.velocity.magnitude);

    }

    private void FixedUpdate()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        rb.velocity = new Vector2(moveHorizontal * stats.speed, moveVertical * stats.speed);
        if (rb.velocity.magnitude > stats.speed)
        {
            rb.velocity = rb.velocity * (stats.speed / rb.velocity.magnitude);
        }
    }
}