using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    Rigidbody2D rb;
    Vector2 player2Mouse;
    float inputHor;
    bool jumpPressed;
    bool facingRight;
    bool isGrounded;
    bool isPulling;

    public Animator animator;
    public float acceleration = 12.5f;
    public float jumpPower = 170f;
    public float maxSpeed = 1.5f;
    public float friction = 0.8f;
    public float pullStrength = 25f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        facingRight = true;
        //Debug.Log(Screen.width + " " + Screen.height);
    }

    void Update()
    {
        // gets inputs
        inputHor = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            player2Mouse = Vector3.Normalize(mousePosition - (Vector2)transform.position);
            //Debug.Log("Click at: " + mousePosition);
            //Debug.Log("Character at: " + transform.position);
            Debug.Log(player2Mouse);
        }

        if (Input.GetMouseButton(0))
        {
            //Debug.Log("Mouse held");
            isPulling = true;
        } else
        {
            //Debug.Log("Mouse not held");
            isPulling = false;
        }
    }

    void FixedUpdate()
    {
        // turns around the sprite if facing the wrong way
        if (rb.velocity.x > 0.5f & !facingRight)
        {
            Flip();
            facingRight = true;
        }
        else if (rb.velocity.x < -0.5f & facingRight)
        {
            Flip();
            facingRight = false;
        }

        rb.AddForce(HandleMovement(inputHor, jumpPressed));
        rb.velocity += player2Mouse * pullStrength * Time.deltaTime;
        jumpPressed = false;
        if (!isPulling)
            player2Mouse = Vector2.zero;

        // adds friction and limits movement speed
        if (inputHor == 0f & isGrounded)
        {
            rb.velocity *= new Vector2(friction, 1.0f);
        }
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);
    }

    Vector2 HandleMovement(float horizontalMove, bool jump)
    {
        // checks moving left & right
        Vector2 movement = new Vector2(horizontalMove * acceleration, 0);
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        // checks jumps
        if (jump & isGrounded)
        {
            movement.y += jumpPower;
            animator.SetBool("isJumping", true);
        }

        return movement;
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // checks collisions if on ground
        if (!isGrounded)
        {
            if (collision.gameObject.layer == 8 | collision.gameObject.layer == 9)
            {
                isGrounded = true;
            }
        }
        animator.SetBool("isJumping", false);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // checks collisions if off ground
        if (isGrounded)
        {
            if (collision.gameObject.layer == 8 | collision.gameObject.layer == 9)
            {
                isGrounded = false;
            }
        }
    }
}
