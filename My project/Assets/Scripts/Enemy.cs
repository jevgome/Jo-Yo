using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private BoxCollider2D groundCheck;
    
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private GameObject yoyo;
    [SerializeField] private GameObject player;
    [SerializeField] private float pullAcceleration;
    private bool grounded;
    private BoxCollider2D collider;
    private Rigidbody2D body;
    [SerializeField] private bool grabbed;
    [Range(0f,1f)] [SerializeField] private float groundDecay; // Friction coefficient


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
        CheckCollision();
    }

    void FixedUpdate() {
        CheckGrabbed();
        ApplyFriction();
    }

    public bool Grounded() {
        return Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    }

    private void CheckGround() {
        grounded = Grounded();
    }

    private void CheckGrabbed() {
        if(grabbed) {
            
            Vector2 direction = player.transform.position - transform.position;
            Vector2 newvector = direction.normalized * pullAcceleration;
            // body.AddForce(newvector);
            body.velocity = newvector;
        }
    }

    private void CheckCollision() {
        grabbed = Physics2D.OverlapAreaAll(collider.bounds.min, collider.bounds.max, yoyo.layer).Length > 0;
    }

    private void ApplyFriction() {
        if(grounded && !grabbed) {
            body.velocity *= groundDecay;
            body.velocity = new Vector2(body.velocity.x * groundDecay, body.velocity.y);
        }
    }
}

