using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Imported Objects and Components
    [SerializeField] public BoxCollider2D groundCheck;
    [SerializeField] public BoxCollider2D wallCheck;
    [SerializeField] public LayerMask groundMask;
    [SerializeField] public LayerMask wallMask;
    [SerializeField] public LayerMask enemyMask;
    [SerializeField] public GameObject grappler;
    [SerializeField] public CircleCollider2D circleCollider;
    [SerializeField] public Sprite[] spriteArray;
    [SerializeField] public Rigidbody2D anchor;

    // Physics stuff
    
    [Range(0f,1f)] [SerializeField] public float groundDecay; // Friction coefficient
    [SerializeField] public float maxGroundSpeed; // Speed on ground
    [SerializeField] public float maxFallSpeed; // Max speed when falling faster
    [SerializeField] public float jumpSpeed; // Height of jump
    [SerializeField] public float fallAcceleration; // Force of faster falling
    [SerializeField] public float swingAcceleration; // Pulling force of swing
    [SerializeField] public float maxSwingSpeed; // Max pulling force
    [SerializeField] public float airSpeed; // XMovement arial speed after swinging
    [SerializeField] public float wallSlidingSpeed; // Gravity when wall-sliding
    [SerializeField] public float sprintingSpeed; // Speed when sprinting
    [SerializeField] public float minRopeDistance; // Distance when rope becomes rigid
    
    // Internal vars
    public bool grounded; // If on ground
    public bool moved; // When to keep x-acceleration
    public bool isWallSliding;
    public bool hasWallJumped;
    public float xInput;
    public float yInput;
    public bool lmb;
    public bool rmb;
    public Collider2D[] grabbedEnemies;
    public Collider2D[] grabbedGround;
    
    // Internal Components
    public Rigidbody2D body;
    public SpriteRenderer spriteRenderer;
    public DistanceJoint2D ropeJoint;

    public void Start() {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ropeJoint = GetComponent<DistanceJoint2D>();

        grabbedGround = Physics2D.OverlapAreaAll(circleCollider.bounds.min, circleCollider.bounds.max, groundMask);
        grabbedEnemies = Physics2D.OverlapAreaAll(circleCollider.bounds.min, circleCollider.bounds.max, enemyMask);

        ropeJoint.enabled = false;
    }
    public void Update()
    {
        CheckYoYoCollision();
        CheckInput();
        HandleJump();
        HandleSprite();
        HandlePlayerRotation();
    }

    public void FixedUpdate() {
        CheckGround();
        HandleXMovement();
        ApplyFriction();
        HandleSwing();
        HandleFasterFalling();
        HandleEnemyGrab();
        HandleWallSlide();
    }

    public void CheckInput() {
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
    
    public void CheckYoYoCollision() {
        grabbedGround = Physics2D.OverlapAreaAll(circleCollider.bounds.min, circleCollider.bounds.max, groundMask);
        grabbedEnemies = Physics2D.OverlapAreaAll(circleCollider.bounds.min, circleCollider.bounds.max, enemyMask);
    }

    public void HandleXMovement(){
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

    public void HandleJump() {
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

    public void HandleFasterFalling() {
        if(yInput < 0 && !grounded)
        {
            body.AddForce(Vector2.up*yInput * fallAcceleration);
        }
    }

    public void CheckGround() {
        grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    }

    public void ApplyFriction() {
        if(grounded && xInput == 0 && yInput == 0 && !rmb) {
            body.velocity *= groundDecay;
            body.velocity = new Vector2(body.velocity.x * groundDecay, body.velocity.y);
        }
    }

    public void HandleSwing()
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

    public void HandleEnemyGrab() {
        // if (rmb && grabbedEnemies.Length > 0)
        // {
        //     if(grounded && grabbedEnemies[0].GetComponent<Enemy>().Grounded()) { 
        //         Vector2 direction = grappler.transform.position - transform.position;
        //         Vector2 newvector = -direction.normalized * swingAcceleration;
        //         grabbedEnemies[0].GetComponent<Rigidbody2D>().AddForce(newvector);
        //     }
        //     Vector2 direction = grappler.transform.position - transform.position;
        //     Vector2 newvector = direction.normalized * swingAcceleration;
        //     body.AddForce(newvector);
        // }
    }

    public void HandleWallSlide() {
        if(grabbedGround.Length > 0 && !grounded && xInput != 0f) {
            body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, -wallSlidingSpeed, float.MaxValue));
            isWallSliding = true;
        } else {
            isWallSliding = false;
        }
    }
    public void HandleSprite()
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

    public void HandlePlayerRotation() {
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
