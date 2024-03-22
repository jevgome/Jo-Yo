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

    public bool grounded;

    float xInput;
    float yInput;

    // Update is called once per frame
    void Start() {
        groundCheck = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        CheckInput();
        HandleJump();
    }

    void FixedUpdate() {
        CheckGround();
        HandleXMovement();
        ApplyFriction();
        HandleSwing();
    }

    void CheckInput() {
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
    }

    void HandleXMovement(){
       if(Mathf.Abs(xInput) > 0) {
          float direction = Mathf.Sign(xInput);
          transform.localScale = new Vector3(direction * Mathf.Abs(transform.localScale.x),transform.localScale.y,transform.localScale.z);
       }
        if(!Input.GetMouseButton(1)) {
            body.AddForce(new Vector2(xInput*maxGroundSpeed,body.velocity.y));
        }

    }

    void HandleJump() {
        if(yInput > 0 && grounded) {
            body.velocity = new Vector2(body.velocity.x,  jumpSpeed);
        }
        if(yInput < 0 && !grounded)
        {

            float increment = yInput * fallAcceleration;
            float newSpeed = Mathf.Clamp(body.velocity.y + increment, maxFallSpeed, 0);
            body.AddForce(transform.up*-newSpeed);
        }
    }

    void CheckGround() {
        grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    }

    void ApplyFriction() {
        if(grounded && xInput == 0 && yInput == 0) {
            body.velocity *= groundDecay;
            body.velocity = new Vector2(body.velocity.x * groundDecay, body.velocity.y);
        }
    }

    void HandleSwing()
    {
        Vector3 mouseposition = Input.mousePosition;
        mouseposition.z = Camera.main.nearClipPlane;
        mouseposition = Camera.main.ScreenToWorldPoint(mouseposition);
        Vector2 direction = new Vector2(0,0);
        Vector2 newvector = new Vector2(0,0);
        if (Input.GetMouseButton(1) && Physics2D.OverlapAreaAll(circleCollider.bounds.min, circleCollider.bounds.max, groundMask).Length > 0)
        {
            direction = grappler.transform.position - transform.position;
            newvector = direction.normalized * swingAcceleration * Time.fixedDeltaTime;
            

        }
        body.AddForce(newvector);
    }

}
