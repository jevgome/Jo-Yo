using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private BoxCollider2D groundCheck;
    
    [SerializeField] private LayerMask groundMask;
    private Transform yoyoTransform;
    [SerializeField] private GameObject player;
    private bool grounded;
    private BoxCollider2D collider;
    private Rigidbody2D body;
    [SerializeField] private bool grabbed;
    [Range(0f,1f)] [SerializeField] private float groundDecay; // Friction coefficient
    private bool rmb;
    [SerializeField] private LayerMask yoyoLayer;

    [SerializeField] float touching;
    [SerializeField] float minDistance;


    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
        CheckInput();
    }

    void FixedUpdate() {
        CheckGrabbed();
        ApplyFriction();
        CheckCollision();
        touching = Physics2D.OverlapAreaAll(collider.bounds.min, collider.bounds.max, yoyoLayer).Length;
    }

    private void CheckInput() {
        rmb = Input.GetMouseButton(1);
    }
    public bool Grounded() {
        return Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    }

    private void CheckGround() {
        grounded = Grounded();
    }

    private void CheckGrabbed() {
        if(grabbed && Vector2.Distance(transform.position, player.transform.position) > minDistance) {
            Collider2D[] yoyoCollide = Physics2D.OverlapAreaAll(collider.bounds.min, collider.bounds.max, yoyoLayer);
            if(yoyoCollide.Length > 0 && yoyoTransform == null) {
                Collider2D yoo = Physics2D.OverlapAreaAll(collider.bounds.min, collider.bounds.max, yoyoLayer)[0];
                yoyoTransform = yoo.GetComponent<Transform>();
            }
            transform.position = yoyoTransform.position;
            
        }
    }

    private void CheckObjects() {

    }
    private void CheckCollision() {
        // if(Physics2D.OverlapAreaAll(collider.bounds.min, collider.bounds.max, yoyoLayer).Length > 0 && rmb) {
        //     grabbed = true;
        // } else if(!rmb) {
        //     grabbed = false;
        // }

        grabbed = Physics2D.OverlapAreaAll(collider.bounds.min, collider.bounds.max, yoyoLayer).Length > 0 && rmb;
    }

    private void ApplyFriction() {
        if(grounded && !grabbed) {
            body.velocity *= groundDecay;
            body.velocity = new Vector2(body.velocity.x * groundDecay, body.velocity.y);
        }
    }
}

