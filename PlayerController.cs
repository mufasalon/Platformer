using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour

{
    public float speed;
    public float jumpForce;
    private float moveInput;

    private Rigidbody2D rb;

    private bool isGrounded;
    public Transform groundCheck;
    public float radiusCheck;
    public LayerMask whatGroundLayer;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;


    bool isTouchingFront;
    public Transform FrontCheck;
    bool wallSliding;
    public float WallSlidingSpeed;
    bool wallJumping;
    public float xWallForce;
    public float yWallForce;
    public float wallJumpTime;

    


    [SerializeField] private TrailRenderer tr;
   

    //private int extraJumps;
    //public int jumpValue;


    private bool facingRight = true;
   
    void Start()
    {
        //extraJumps = jumpValue;
        rb=GetComponent<Rigidbody2D>();
    }

   
    void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }


        isGrounded = Physics2D.OverlapCircle(groundCheck.position, radiusCheck , whatGroundLayer);
        moveInput = Input.GetAxis("Horizontal");
        Debug.Log(moveInput);
        rb.velocity=new Vector2(moveInput*speed, rb.velocity.y);


        if (facingRight == false && moveInput>0 )
        {
            Flip();
        }
        else if (facingRight == true && moveInput<0 )
        {
            Flip();
        }

        
    }

    private void Update()
    {
        /* if ( isGrounded == true ) 
         {
             extraJumps = 2;
         }*/
        float input = Input.GetAxisRaw("Horizontal");

        if ( isDashing ) 
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) )// && extraJumps>0)
        {
            rb.velocity = Vector2.up * jumpForce;
          // extraJumps--;
        }

       /*else if (Input.GetKeyDown(KeyCode.UpArrow) && extraJumps == 0 && isGrounded == true)
        {
            rb.velocity = Vector2.up * jumpForce;
             
        }*/


        if(Input.GetKeyDown(KeyCode.LeftShift) && canDash) 
        {
            StartCoroutine(Dash());
        }


        isTouchingFront = Physics2D.OverlapCircle(FrontCheck.position, radiusCheck, whatGroundLayer);
        if (isTouchingFront == true && isGrounded == false && input !=0 )
        {
            wallSliding = true;
        }
        else 
        {
            wallSliding = false;
        }
        if(wallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -WallSlidingSpeed, float.MaxValue));
        }
        if(Input.GetKeyDown(KeyCode.UpArrow) &&wallSliding == true)
        {
            wallJumping = true;
            Invoke("SetWallJumpingToFalse", wallJumpTime);

        }
        if(wallJumping == true)
        {
            rb.velocity = new Vector2(xWallForce * -input , yWallForce);
        }


        

    }

    void SetWallJumpingToFalse()
    { wallJumping = false; }
   


    

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;

    }


    private IEnumerator Dash() 
    {
        canDash = false;
        isDashing = true;
        float OriginalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale=OriginalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;

            
    }


}
