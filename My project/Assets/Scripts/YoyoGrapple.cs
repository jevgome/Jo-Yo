using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoyoGrapple : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask enemyMask;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D body;
    private CircleCollider2D collider;
    [SerializeField] private float throwSpeed;
    [SerializeField] private float returnSpeed;
    private bool grappling = false;
    [SerializeField] private bool grounded;
    [SerializeField] private bool enemied;
    private bool lmb;
    private bool rmb;
    private Vector3 mouseposition;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<CircleCollider2D>();
        spriteRenderer.enabled = false;
        body = GetComponent<Rigidbody2D>();
    }
    
    private void Update() {
        CheckInput();
        CheckMousePosition();
    }

    private void CheckInput() {
        lmb = Input.GetMouseButton(0);
        rmb = Input.GetMouseButton(1);
    }
    private void FixedUpdate()
    {
        CheckCollision();
        CheckGrabbed();
    }

    private void CheckMousePosition() {
        mouseposition = Input.mousePosition;
        mouseposition.z = Camera.main.nearClipPlane;
        mouseposition = Camera.main.ScreenToWorldPoint(mouseposition);
    }
    private void CheckGrabbed() {
        Collider2D[] grabbedEnemies = Physics2D.OverlapAreaAll(collider.bounds.min, collider.bounds.max, enemyMask);
        grounded = Physics2D.OverlapAreaAll(collider.bounds.min, collider.bounds.max, groundMask).Length > 0;
        // enemied = grabbedEnemies.Length > 0;
        if(grabbedEnemies.Length > 0 && rmb) {
            enemied= true;
        } else if(!rmb) {
            enemied = false;
        }
    }

    private void CheckCollision() {
        
        Vector2 direction = new Vector2(0,0);
        Vector2 newvector = new Vector2(0,0);
        if(rmb) {
            spriteRenderer.enabled = true;
            if(!grappling) {
                direction = mouseposition - transform.position;
                newvector = direction.normalized * throwSpeed;
                grappling = true;
            }
                
            if(enemied){
                if(Vector2.Distance(player.transform.position, transform.position) > 2) {
                    direction = player.transform.position - transform.position;
                    newvector = direction.normalized * returnSpeed;
                    body.AddForce(newvector);
                } else {
                    spriteRenderer.enabled = false;
                    transform.position = player.transform.position;
                    grappling = false;
                    enemied = false;
                }
                
            } else if(grounded) {
                body.velocity = Vector2.zero;
            } else {
                body.AddForce(newvector);
            }
        } else {
            spriteRenderer.enabled = false;
            transform.position = player.transform.position;
            body.velocity = Vector2.zero;
            grappling = false;
        }
    }

}
