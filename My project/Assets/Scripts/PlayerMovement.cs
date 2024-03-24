using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private BoxCollider2D groundCheck;
    [SerializeField] private BoxCollider2D wallCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private GameObject grappler;
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private float acceleration;
    [Range(0f,1f)]
    [SerializeField] private float groundDecay;
    [SerializeField] private float maxGroundSpeed;
    [SerializeField] private float maxFallSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float fallAcceleration;
    [SerializeField] private float swingAcceleration;
    [SerializeField] private float maxSwingSpeed;
    [SerializeField] private float airSpeed;
    [SerializeField] private Sprite[] spriteArray;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private bool grounded;
    private bool moved;

    private float xInput;
    private float yInput;
    [SerializeField] private DistanceJoint2D ropeJoint;
    [SerializeField] private Rigidbody2D anchor;

    [SerializeField] private float wallSlidingSpeed;
    [SerializeField] private bool isWallSliding;

    private bool hasWallJumped;

    // Update is called once per frame
    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ropeJoint.enabled = false;
    }
    private void Update()
    {
        CheckInput();
        HandleJump();
        HandleSprite();
    }

    private void FixedUpdate() {
        CheckGround();
        HandleXMovement();
        ApplyFriction();
        Pull();
        HandleWallSlide();
        Swing();
        
    }

    private void CheckInput() {
        if(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            xInput = 0;
        } else
        {
            xInput = Input.GetAxis("Horizontal");
        }
        
        yInput = Input.GetAxis("Vertical");
    }

    private void HandleXMovement(){
        if(grounded) {
            moved = true;
            if(Input.GetKey(KeyCode.LeftShift)) {
               body.velocity = new Vector2(xInput * maxGroundSpeed * 2f, body.velocity.y); 
            } else {
                body.velocity = new Vector2(xInput * maxGroundSpeed, body.velocity.y); 
            }
        } else if (moved && !isWallSliding){
            body.velocity = new Vector2(xInput * maxGroundSpeed, body.velocity.y);
        } else {
            body.AddForce(transform.right * xInput * airSpeed);
        }
        if(Input.GetMouseButton(0))
        {
            moved = false;
        }
    }

    private void HandleJump() {
        if(grounded) {
            hasWallJumped = false;
        }
        if(yInput > 0 && grounded) {
            body.velocity = new Vector2(body.velocity.x,  jumpSpeed);
        }
        if(yInput > 0 && isWallSliding && !hasWallJumped) {
            body.velocity = new Vector2(400f * Mathf.Sign(transform.localScale.x),600f);
            moved = false;
            hasWallJumped = true;
        }
        if(yInput < 0 && !grounded)
        {
            body.AddForce(transform.up*yInput * fallAcceleration);
        }
    }

    private void CheckGround() {
        grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    }

    private void ApplyFriction() {
        if(grounded && xInput == 0 && yInput == 0 && !Input.GetMouseButton(0)) {
            body.velocity *= groundDecay;
            body.velocity = new Vector2(body.velocity.x * groundDecay, body.velocity.y);
        }
    }

    private void Pull()
    {
        Vector2 direction = new Vector2(0,0);
        Vector2 newvector = new Vector2(0,0);
        if (Input.GetMouseButton(0) && Physics2D.OverlapAreaAll(circleCollider.bounds.min, circleCollider.bounds.max, groundMask).Length > 0)
        {
            direction = grappler.transform.position - transform.position;
            newvector = direction.normalized * swingAcceleration;
            body.AddForce(newvector);
            
        }
        body.velocity = Vector2.ClampMagnitude(body.velocity, maxSwingSpeed);
    }
    
    private void Swing() {

        if (Input.GetMouseButton(1) && Physics2D.OverlapAreaAll(circleCollider.bounds.min, circleCollider.bounds.max, groundMask).Length > 0)
        {
            anchor.transform.position = grappler.transform.position;
            if(!ropeJoint.enabled)ropeJoint.distance = Vector2.Distance(transform.position, grappler.transform.position);
            ropeJoint.enabled = true;
           
            
        } else {
            ropeJoint.enabled = false;
        }
    }

    private void HandleWallSlide() {
        if(Physics2D.OverlapAreaAll(wallCheck.bounds.min, wallCheck.bounds.max, groundMask).Length > 0 && !grounded && xInput != 0f) {
            body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, -wallSlidingSpeed, float.MaxValue));
            isWallSliding = true;
        } else {
            isWallSliding = false;
        }
    }
    private void HandleSprite()
    {
        if (Input.GetMouseButton(0))
        {
            spriteRenderer.sprite = spriteArray[1];
        } else if (grounded)
        {
            spriteRenderer.sprite = spriteArray[0];
        } else if (body.velocity.y > 0)
        {
            spriteRenderer.sprite = spriteArray[2];
        } else
        {
            spriteRenderer.sprite = spriteArray[3];
        }
        if(Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            Vector3 diff = grappler.transform.position - transform.position;
            diff.Normalize();
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
                
            float direction = Mathf.Sign(diff.x);
            transform.localScale = new Vector3(direction * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        } else
        {
            transform.rotation = Quaternion.Euler(0, 0f, 0f);
            if (Mathf.Abs(xInput) > 0)
            {
                float direction;
                if(isWallSliding) {
                    direction = -Mathf.Sign(xInput);
                } else {
                    direction = Mathf.Sign(xInput);
                }
                
                transform.localScale = new Vector3(direction * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }
}
