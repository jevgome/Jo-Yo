using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Imported Objects and Components
    [SerializeField] private BoxCollider2D groundCheck;
    [SerializeField] private BoxCollider2D wallCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject grappler;
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private Sprite[] spriteArray;
    [SerializeField] private Rigidbody2D anchor;

    // Physics stuff
    
    [Range(0f,1f)] [SerializeField] private float groundDecay; // Friction coefficient
    [SerializeField] private float maxGroundSpeed; // Speed on ground
    [SerializeField] private float maxFallSpeed; // Max speed when falling faster
    [SerializeField] private float jumpSpeed; // Height of jump
    [SerializeField] private float fallAcceleration; // Force of faster falling
    [SerializeField] private float swingAcceleration; // Pulling force of swing
    [SerializeField] private float maxSwingSpeed; // Max pulling force
    [SerializeField] private float airSpeed; // XMovement arial speed after swinging
    [SerializeField] private float wallSlidingSpeed; // Gravity when wall-sliding
    [SerializeField] private float sprintingSpeed; // Speed when sprinting
    [SerializeField] private float minRopeDistance; // Distance when rope becomes rigid
    
    // Internal vars
    private bool grounded; // If on ground
    private bool moved; // When to keep x-acceleration
    private bool isWallSliding;
    private bool hasWallJumped;
    private float xInput;
    private float yInput;
    private bool lmb;
    private bool rmb;
    private Collider2D[] grabbedEnemies;
    private Collider2D[] grabbedGround;
    
    // Internal Components
    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private DistanceJoint2D ropeJoint;

    private void Start() {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ropeJoint = GetComponent<DistanceJoint2D>();

        grabbedGround = Physics2D.OverlapAreaAll(circleCollider.bounds.min, circleCollider.bounds.max, groundMask);
        grabbedEnemies = Physics2D.OverlapAreaAll(circleCollider.bounds.min, circleCollider.bounds.max, enemyMask);

        ropeJoint.enabled = false;
    }
    private void Update()
    {
        CheckYoYoCollision();
        CheckInput();
        HandleJump();
        HandleSprite();
        HandlePlayerRotation();
    }

    private void FixedUpdate() {
        CheckGround();
        HandleXMovement();
        ApplyFriction();
        HandleSwing();
        HandleFasterFalling();
        HandleEnemyGrab();
        HandleWallSlide();
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
        lmb = Input.GetMouseButton(0);
        rmb = Input.GetMouseButton(1);
    }
    
    private void CheckYoYoCollision() {
        grabbedGround = Physics2D.OverlapAreaAll(circleCollider.bounds.min, circleCollider.bounds.max, groundMask);
        grabbedEnemies = Physics2D.OverlapAreaAll(circleCollider.bounds.min, circleCollider.bounds.max, enemyMask);
    }

    private void HandleXMovement(){
        if(grounded) {
            moved = true;
            if(Input.GetKey(KeyCode.LeftShift)) {
               body.velocity = new Vector2(xInput * sprintingSpeed, body.velocity.y); 
            } else {
                body.velocity = new Vector2(xInput * maxGroundSpeed, body.velocity.y); 
            }
        } else if (moved && !isWallSliding){
            body.velocity = new Vector2(xInput * maxGroundSpeed, body.velocity.y);
        } else {
            body.AddForce(transform.right * xInput * airSpeed);
        }
        if(rmb && grabbedGround.Length > 0)
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
        
    }

    private void HandleFasterFalling() {
        if(yInput < 0 && !grounded)
        {
            body.AddForce(Vector2.up*yInput * fallAcceleration);
        }
    }

    private void CheckGround() {
        grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    }

    private void ApplyFriction() {
        if(grounded && xInput == 0 && yInput == 0 && !rmb) {
            body.velocity *= groundDecay;
            body.velocity = new Vector2(body.velocity.x * groundDecay, body.velocity.y);
        }
    }

    private void HandleSwing()
    {
        if (rmb && grabbedGround.Length > 0)
        {
            Vector2 direction = grappler.transform.position - transform.position;
            Vector2 newvector = direction.normalized * swingAcceleration;
            
            anchor.transform.position = grappler.transform.position;
            if(!ropeJoint.enabled)ropeJoint.distance = Vector2.Distance(transform.position, grappler.transform.position);
            if(ropeJoint.distance < minRopeDistance && xInput == 0 && yInput == 0) {
                ropeJoint.enabled = true;
            } else {
                ropeJoint.enabled = false;
                body.AddForce(newvector);
            }
        } else {
            ropeJoint.enabled = false;
        }
        body.velocity = Vector2.ClampMagnitude(body.velocity, maxSwingSpeed);
    }

    private void HandleEnemyGrab() {
        if (rmb && grabbedEnemies.Length > 0)
        {
            if(grounded && grabbedEnemies[0].GetComponent<Enemy>().Grounded()) { 
                // Vector2 direction = grappler.transform.position - transform.position;
                // Vector2 newvector = -direction.normalized * swingAcceleration;
                // grabbedEnemies[0].GetComponent<Rigidbody2D>().AddForce(newvector);
            }
            // Vector2 direction = grappler.transform.position - transform.position;
            // Vector2 newvector = direction.normalized * swingAcceleration;
            // body.AddForce(newvector);
        }
    }

    private void HandleWallSlide() {
        if(grabbedGround.Length > 0 && !grounded && xInput != 0f) {
            body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, -wallSlidingSpeed, float.MaxValue));
            isWallSliding = true;
        } else {
            isWallSliding = false;
        }
    }
    private void HandleSprite()
    {
        if (rmb && grabbedGround.Length > 0)
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
    }

    private void HandlePlayerRotation() {
        if(rmb && grabbedGround.Length > 0)
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
