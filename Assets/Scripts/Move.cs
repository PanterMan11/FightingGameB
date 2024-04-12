using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{

    [SerializeField] Rigidbody2D rb;
    [SerializeField] float moveSpeed;
    [SerializeField] SpriteRenderer sr;
    public float jumpForce;
    private float moveInput;
    private bool isGrounded;
    bool canDash = true;
    bool isDashing ;
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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
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
            //Debug.Log("BIIG JUMp NO GETUP");
        }
    }


    private IEnumerator Dash()
    {
        canDash = false;
        Debug.Log("cant dash dipshit");
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(moveInput* dashingPower, 0f);
        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originalGravity;
        isDashing=false;

        
    }

    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
        Debug.Log("can dash dipshit");

    }


    








    void FaceMoveDirection()
    {

        if (moveInput > 0)
        {
            sr.flipX = false;
          
        }
        else if (moveInput < 0)
        {
            sr.flipX = true;
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
