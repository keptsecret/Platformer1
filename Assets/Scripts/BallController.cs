using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    Rigidbody2D rb;
    public float acceleration;
    public float jumpPower;
    public float maxSpeed;
    bool isGrounded = true;
    bool jumpPressed = false;
    bool facingRight;
    Vector3 pos;
    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        facingRight = true;
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        //float jump = Input.GetAxisRaw("Jump");
        Vector2 movement;

        movement = new Vector2(moveHorizontal * acceleration, 0);
        animator.SetFloat("Speed", Mathf.Abs(moveHorizontal));

        if (jumpPressed & isGrounded)
        {
            float moveVertical = jumpPower;
            animator.SetBool("isJumping", true);
            movement.y = moveVertical;
        }

        if (!Input.GetKey(KeyCode.A) & !Input.GetKey(KeyCode.D))
        {
            rb.velocity *= new Vector2(0.9f, 1.0f);
        }
        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);
        }

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

        jumpPressed = false;
        rb.AddForce(movement);
    }

    void Update()
    {
        pos = Camera.main.WorldToViewportPoint(transform.position);
        if (pos.y < 0.0f)
        {
            transform.position = new Vector3(0f, 0f);
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
        //jumpPressed = Input.GetButtonDown("Jump");
        //Debug.Log(jumpPressed);
    }

    // Collision with ground
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isGrounded)
        {
            if (collision.gameObject.layer == 8)
            {
                isGrounded = true;
                Debug.Log("On ground");
            }
            if (collision.gameObject.layer == 9)
            {
                if (collision.gameObject.transform.position.y < transform.position.y)
                {
                    isGrounded = true;
                    Debug.Log("On ground");
                }
            }
        }
        
        animator.SetBool("isJumping", false);
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (isGrounded)
        {
            if (collision.gameObject.layer == 8)
            {
                isGrounded = false;
                Debug.Log("In air");
            }
            if (collision.gameObject.layer == 9)
            {
                if (collision.gameObject.transform.position.y < transform.position.y)
                {
                    isGrounded = false;
                    Debug.Log("In air");
                }
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
