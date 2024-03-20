using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D body;
    public BoxCollider2D groundCheck;
    public LayerMask groundMask;

    public float acceleration;
    [Range(0f,1f)]
    public float groundDecay;
    public float maxGroundSpeed;
    public float jumpSpeed;

    public bool grounded;

    float xInput;
    float yInput;

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        HandleJump();
    }

    void FixedUpdate() {
        CheckGround();
        HandleXMovement();
        // ApplyFriction();
    }

    void CheckInput() {
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
    }

    void HandleXMovement(){
       if(Mathf.Abs(xInput) > 0) {
            // float increment = xInput * acceleration;
            // float newSpeed = Mathf.Clamp(body.velocity.x + increment, -maxGroundSpeed, maxGroundSpeed);
            // body.velocity = new Vector2(newSpeed, body.velocity.y);
          float direction = Mathf.Sign(xInput);
          transform.localScale = new Vector3(direction,1,1);

       }

       body.velocity = new Vector2(xInput * maxGroundSpeed, body.velocity.y);

    }

    void HandleJump() {
        if(yInput > 0 && grounded) {
            body.velocity = new Vector2(body.velocity.x,  jumpSpeed);
        }
    }

    void CheckGround() {
        grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    }

    void ApplyFriction() {
        if(grounded && xInput == 0 && yInput == 0) {
            // body.velocity *= groundDecay;
            body.velocity = new Vector2(body.velocity.x * groundDecay, body.velocity.y);
        }
    }

}
