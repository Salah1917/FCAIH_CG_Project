using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    int horizontal;
    float vertical;

    public bool isGrounded = false;
    public Vector2 boxSize;
    public float castDistance;
    public LayerMask groundLayer;

    public float speed = 3f;
    public float jumpForce = 3f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = rb.linearVelocity.y;
        rb.linearVelocity = new Vector2 (horizontal * speed, rb.linearVelocity.y);
        animator.SetInteger("Horizontal", horizontal);
        animator.SetFloat("Vertical",vertical);
        animator.SetBool("IsGrounded",isGrounded);
        if (horizontal < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (horizontal > 0) 
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if ( (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) ) && isGrounded)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up , castDistance, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
