using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Move : NetworkBehaviour
{

    [SerializeField] Rigidbody2D rb;
    [SerializeField] float moveSpeed;
    [SerializeField] public Animator an;
    public float jumpForce;
    private float moveInput;
    bool canDash = true;

    bool isDashing; 
    private bool isGrounded;
    [NonSerialized] public bool isMoving;
    [NonSerialized] public bool isStunned;
    [NonSerialized] public bool isStanding;
    [NonSerialized] public bool isKnockedback;
    [NonSerialized] public bool isAttacking;
    [NonSerialized] public bool isInvisible;


    [SerializeField] float dashingPower; 
    [SerializeField] float dashingTime;
    [SerializeField] float dashingCooldown;
    float lastClickTime;    
    private const float DOUBE_TIME = .2f; // what counts as a double input
    [SerializeField] Transform feetPos;
    [SerializeField] float checkRadius;
    [SerializeField] LayerMask whatIsGround;
    float jumpTime;
    [SerializeField] float jumpStartTime;
    bool isJumping;
    [SerializeField] Attack attackScript;
    [NonSerialized]public bool facingRight;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(7, 7);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) enabled =false;
    }
    
    
    void Update()
    { 
        if (!attackScript.isCoolDown) {

        if (isDashing)
        {
            return; // this is so the player is locked in a dash and cant fumble around while dashing 
        }
        
        moveInput = Input.GetAxisRaw("Horizontal");
        FaceMoveDirection();
        Jump();
        if (Input.GetButtonDown("Horizontal") && moveInput == 1 && canDash == true)
        { // DASH RIGHT
            float timeSinceLastClick = Time.time - lastClickTime;
            if (timeSinceLastClick <= DOUBE_TIME && moveInput == 1)
            {
                Debug.Log("IDASH RIGHT");
                StartCoroutine(Dash());
            }

            //Debug.Log(timeSinceLastClick);
            lastClickTime = Time.time;
        }
        else if (Input.GetButtonDown("Horizontal") && moveInput == -1 && canDash == true)
        {
            // DASH LEFT
            float timeSinceLastClick = Time.time - lastClickTime;
            if (timeSinceLastClick <= DOUBE_TIME && moveInput == -1)
            {
                Debug.Log("IDASH LEFT");
                StartCoroutine(Dash());
            }
            //Debug.Log(timeSinceLastClick);
            lastClickTime = Time.time;

        }
        else if (Input.GetButtonDown("Jump") && canDash == true)
        {
            float timeSinceLastClick = Time.time - lastClickTime;
            if (timeSinceLastClick <= DOUBE_TIME)
            {
                Debug.Log("IDASH WITH THE JUMP KEY");
                StartCoroutine(Dash());
            }
            //Debug.Log(timeSinceLastClick);
            lastClickTime = Time.time;

        }
        }
    }
    private void FixedUpdate()
    {
        if (!attackScript.isCoolDown)
        {
            if (isDashing)
            {
                
                return; // this is so the player is locked in a dash and cant fumble around while dashing 
            }
            isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
            //Debug.Log("pls " + isGrounded);
            if (canDash == false && isGrounded == true)
            {
                StartCoroutine(DashCooldown());
            }
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            
            if(moveInput != 0)
            {
                an.SetBool("isMove", true);
            }
            else
            {
                an.SetBool("isMove", false);
            }
        }
    }
    void Jump()
    {

       // isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround); 

        if (isGrounded == true && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            
            jumpTime = jumpStartTime;
            rb.velocity = Vector2.up * jumpForce;
        }
        
        
        if (isJumping == true && Input.GetButton("Jump"))
        {
            if (jumpTime> 0) {
                an.SetBool("isJump", true);
                rb.velocity = Vector2.up * jumpForce;
                jumpTime -= Time.deltaTime ;
                //Debug.Log(jumpTime);
            }
            else { 
                isJumping=false;
                
                //Debug.Log("BIIG JUMp NO ELSE");
            }
            
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
            an.SetBool("isJump", false);
            //Debug.Log("BIIG JUMp NO GETUP");
        }
        if (isGrounded)
        {
            an.SetBool("isJump", false);
        }
    }


    private IEnumerator Dash()
    {
        canDash = false;
        //Debug.Log("cant dash dipshit");
        isDashing = true;
        an.SetBool("isDash", true);
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(moveInput* dashingPower, 0f);
        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originalGravity;
        isDashing=false;
        an.SetBool("isDash", false);

    }

    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
        //Debug.Log("can dash dipshit");

    }


    

    void Flip()
    {
        Vector2 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        facingRight = !facingRight;

    }






    void FaceMoveDirection()
    {
        

        if (moveInput > 0 && facingRight)
        {
            Flip();
          
        }
        else if (moveInput < 0 && !facingRight)
        {
            
            Flip();
            foreach (Normals boxes in attackScript.allNor)
            {
                //boxes.HitBox.transform.rotation = -1f;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(feetPos.position, checkRadius);
    }
}
