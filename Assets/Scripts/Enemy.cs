using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public BoxCollider2D groundCheck;
    
    [SerializeField] public LayerMask groundMask;
    public Transform yoyoTransform;
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject yoyo;
    public bool grounded;
    public BoxCollider2D col;
    public Rigidbody2D body;
    [SerializeField] public bool grabbed;
    [Range(0f,1f)] [SerializeField] public float groundDecay; // Friction coefficient
    [SerializeField] public LayerMask yoyoLayer;

    [SerializeField] float touching;
    [SerializeField] float minDistance;
    public Vector2 vel;
    [SerializeField] float returnSpeed;
    public Vector2 pos;
    [SerializeField] public float gravity;
    [SerializeField] public float maxPull;
    public float flail;
    public bool lmb, rmb;
    public Vector3 mouseposition;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        lmb = Input.GetMouseButton(0);
        rmb = Input.GetMouseButton(1);
        CheckMousePosition();
        CheckGrabbed();
        ApplyFriction();
        vel = body.velocity;
    }

    public void CheckMousePosition() {
        mouseposition = Input.mousePosition;
        mouseposition.z = Camera.main.nearClipPlane;
        mouseposition = Camera.main.ScreenToWorldPoint(mouseposition);
    }
    public void CheckGrabbed() {
        Collider2D[] yoyoCol = Physics2D.OverlapAreaAll(col.bounds.min, col.bounds.max, yoyoLayer);
        if(yoyoCol.Length > 0) {
            pos = player.transform.position;
            if (lmb)
            {
                Vector2 direction = mouseposition - transform.position;
                Vector2 newvector = direction.normalized * flail;
                
                if(grabbed) {
                    if(body.velocity.magnitude < maxPull) {
                        body.velocity += newvector;
                    }
                    
                } else {
                    body.velocity = newvector;
                }
            }
            else
            {
                Vector2 direction = player.transform.position - transform.position;
                Vector2 newvector = direction.normalized;
                
                if(grabbed) {
                    if(body.velocity.magnitude < maxPull) {
                        body.velocity += newvector;
                    }
                    
                } else {
                    body.velocity = newvector;
                }
            }
            
            grabbed = true;
            body.gravityScale = 0;
        } else {
            grabbed = false;
            body.gravityScale = gravity;
        }
    }

    public void ApplyFriction() {
        grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
        if(grounded && !grabbed) {
            body.velocity = new Vector2(body.velocity.x * groundDecay, body.velocity.y);
        }
    }
}