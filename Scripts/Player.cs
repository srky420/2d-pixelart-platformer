using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  
    [Header("Horizontal Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float dashActiveTime = 0.2f;
    [SerializeField] private float dashCooldown = 2f;
    private float dashCooldownTimer;
    private float dashTimer;
    private Vector2 direction;
    private bool facingRight = true;
    private bool canDash;

    [Header("Vertical Movement")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float jumpDelay = 0.25f;
    [SerializeField] private float coyoteJumpDelay = 0.2f;
    [SerializeField] private bool onWall;
    private Vector2 wallJumpDirection = Vector2.right;
    [SerializeField] private float wallSlideSpeed = 1f;
    private float coyoteTimer;
    private float jumpTimer;
    

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject characterHolder;
    [SerializeField] private Animator anim;
    [SerializeField] private ParticleSystem dashEffectPS;
    [SerializeField] private ParticleSystem dustEffectPS;
    [SerializeField] private ParticleSystem slideDustEffectPS;
    public CameraShake cameraShake;


    [Header("Physics")]
    [SerializeField] private float maxSpeed = 7f;
    [SerializeField] private float linearDrag = 4f;
    [SerializeField] private float gravity = 1f;
    [SerializeField] private float fallMultiplier = 5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    
    [Header("Collision")]
    [SerializeField] private bool onGround = false;
    [SerializeField] private float groundLength = 0.6f;
    [SerializeField] private Vector3 colliderOffset;
    [SerializeField] private Vector3 wallColliderOffset;
    [SerializeField] private float wallColliderRadius;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Local variable for Landing Squash
        bool wasOnGround = onGround;

        //Ground check raycast
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) 
        || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);

        onWall = facingRight? Physics2D.OverlapCircle(transform.position + wallColliderOffset, wallColliderRadius, groundLayer)
        : Physics2D.OverlapCircle(transform.position - wallColliderOffset, wallColliderRadius, groundLayer);
        
        anim.SetBool("onGround", onGround);
        anim.SetBool("onWall", onWall);
        //Direction Input Logic
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //Character Landing Squash
        if(!wasOnGround && onGround)
        {
            dustEffectPS.Play();
            StartCoroutine(JumpSqueeze(1.25f, 0.6f, 0.08f));
        }

        //Jump Input Logic
        if(Input.GetButtonDown("Jump"))
        {
            jumpTimer = Time.time + jumpDelay;
        }

        //Coyote Jump Input Logic
        if(onGround || onWall)
        {
            coyoteTimer = Time.time + coyoteJumpDelay;
        }

        //Dash Input Logic
        if((Input.GetKeyDown(KeyCode.LeftShift)) && (dashCooldownTimer <= 0))
        {
            dashTimer = dashActiveTime;
            dashCooldownTimer = dashCooldown;
        }
        else
        {
            dashEffectPS.Stop();
            dashTimer -= Time.deltaTime;
            dashCooldownTimer -= Time.deltaTime;
        }

    }

    private void FixedUpdate()
    {
        //Dash function call, return, no other movement/physics applied
        if(dashTimer > 0)
        {
            Dash(direction.x);
            return;
        }

        //Character Move Function call
        moveCharacter(direction.x);

        //Character Jump Function call, condition
        if(jumpTimer > Time.time && coyoteTimer > Time.time)
        {
            Jump();
        }
        WallSlide();
        //Physics Logic function call
        ModifyPhysics();

    }//FixedUpdate
    
    private void moveCharacter(float horizontal)
    {
        rb.AddForce(Vector2.right * horizontal * moveSpeed);
                
        if(Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }

        if(((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight)) && !onWall ) Flip();
    
        anim.SetFloat("horizontal", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("vertical", rb.velocity.y);

    }//moveCharacter


    private void Jump()
    {
        if(onWall && !onGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            Flip();
            rb.AddForce((Vector2.up + wallJumpDirection) * jumpForce, ForceMode2D.Impulse);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        
        jumpTimer = 0;

        StartCoroutine(JumpSqueeze(0.7f, 1.20f, 0.2f));
    }//Jump

    private void Dash(float horizontal)
    {   
        if(horizontal != 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.right * horizontal * dashForce, ForceMode2D.Impulse);
            rb.gravityScale = 0;

            anim.SetTrigger("dashing");
            dashEffectPS.Play();
            StartCoroutine(cameraShake.DoCameraShake(0.05f, 0.1f));
            StartCoroutine(JumpSqueeze(1.25f, 0.6f, 0.1f));
        }
    }//Dash

    private void Flip()
    {
        wallJumpDirection *= -1;
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler( 0, facingRight ? 0 : 180, 0);
    
    }//Flip

    private void ModifyPhysics()
    {
        bool changingDirection = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);

        if(onGround)
        {
            if(Mathf.Abs(direction.x) < 0.4f || changingDirection)
            {
                rb.drag = linearDrag;
            }
            else
            {
                rb.drag = 0f;
            }
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = gravity;
            rb.drag = linearDrag * 0.05f;
            if(rb.velocity.y < 0)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }
            else if(rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = gravity * (fallMultiplier / 2);
            }
        }
        
    }//ModifyPhysics

    private void WallSlide()
    {
        if(onWall && !onGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
            slideDustEffectPS.Play();
        }
        else
        {
            slideDustEffectPS.Stop();
        }
    }
    IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds) {
        Vector3 originalSize = Vector3.one;
        Vector3 newSize = new Vector3(xSqueeze, ySqueeze, originalSize.z);
        float t = 0f;
        while (t <= 1.0) {
            t += Time.deltaTime / seconds;
            characterHolder.transform.localScale = Vector3.Lerp(originalSize, newSize, t);
            yield return null;
        }
        t = 0f;
        while (t <= 1.0) {
            t += Time.deltaTime / seconds;
            characterHolder.transform.localScale = Vector3.Lerp(newSize, originalSize, t);
            yield return null;
        }

    }//IENumerator JumpSqueeze
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
        if(facingRight)
        {
            Gizmos.DrawWireSphere(transform.position + wallColliderOffset, wallColliderRadius);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position - wallColliderOffset, wallColliderRadius);
        }
    }//Gizmos



}//Player
