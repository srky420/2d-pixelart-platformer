using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpMultiplier;
    [SerializeField] private LayerMask ground;
    [SerializeField, Range(0, 1)] private float xDamping;
    private float g;
    private float movementDirection;
    private Rigidbody2D rb;
    private bool jumpPressed;
    private BoxCollider2D col;
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        g = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        float xVelocity = rb.velocity.x;
        xVelocity += Input.GetAxisRaw("Horizontal");
        xVelocity *= Mathf.Pow(1f - xDamping, Time.deltaTime * 10f);
        rb.velocity = new Vector2(xVelocity, rb.velocity.y);
        

        if(isGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if(jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

            jumpBufferCounter = 0f;
        }

        if(Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            coyoteTimeCounter = 0f;
        }
       
    
    }

    void FixedUpdate()
    {
        if(rb.velocity.y < 0f)
        {
            rb.gravityScale = fallMultiplier;
        }
        else if(rb.velocity.y > 0f && !Input.GetButton("Jump"))
        {
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = g;
        }
    }

    private bool isGrounded()
    {
        return Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0, Vector2.down, 0.1f, ground);
    }
    
}
