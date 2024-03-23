using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D body;
    BoxCollider2D groundCheck;
    public LayerMask groundMask;
    public GameObject grappler;
    public CircleCollider2D circleCollider;
    public float acceleration;
    [Range(0f,1f)]
    public float groundDecay;
    public float maxGroundSpeed;
    public float maxFallSpeed;
    public float jumpSpeed;
    public float fallAcceleration;
    public float swingAcceleration;
    public float maxSwingSpeed;
    public float airSpeed;
    public Sprite[] spriteArray;
    SpriteRenderer spriteRenderer;

    public bool grounded;
    public bool moved;

    float xInput;
    float yInput;



    // Update is called once per frame
    void Start() {
        groundCheck = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        CheckInput();
        HandleJump();
        HandleSprite();
    }

    void FixedUpdate() {
        CheckGround();
        HandleXMovement();
        ApplyFriction();
        HandleSwing();
        
    }

    void CheckInput() {
        if(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            xInput = 0;
        } else
        {
            xInput = Input.GetAxis("Horizontal");
        }
        
        yInput = Input.GetAxis("Vertical");
    }

    void HandleXMovement(){
        if(grounded) {
            moved = true;
        } 
        if(Input.GetMouseButton(0))
        {
            moved = false;
        }
        if(moved)
        {
            body.velocity = new Vector2(xInput * maxGroundSpeed, body.velocity.y);
        } else
        {
            body.AddForce(transform.right * xInput * airSpeed);
        }
    }

    void HandleJump() {
        if(yInput > 0 && grounded) {
            body.velocity = new Vector2(body.velocity.x,  jumpSpeed);
        }
        if(yInput < 0 && !grounded)
        {
            body.AddForce(transform.up*yInput * fallAcceleration);
        }
    }

    void CheckGround() {
        grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    }

    void ApplyFriction() {
        if(grounded && xInput == 0 && yInput == 0 && !Input.GetMouseButton(0)) {
            body.velocity *= groundDecay;
            body.velocity = new Vector2(body.velocity.x * groundDecay, body.velocity.y);
        }
    }

    void HandleSwing()
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

    void HandleSprite()
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
        if(Input.GetMouseButton(0))
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
                float direction = Mathf.Sign(xInput);
                transform.localScale = new Vector3(direction * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }
}
