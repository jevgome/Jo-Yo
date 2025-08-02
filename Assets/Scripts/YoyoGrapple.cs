using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoyoGrapple : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] public LayerMask groundMask;
    [SerializeField] public LayerMask enemyMask;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D body;
    public CircleCollider2D col;
    [SerializeField] public float throwSpeed;
    [SerializeField] public float returnSpeed;
    public bool grappling = false;
    [SerializeField] public bool grounded;
    [SerializeField] public bool enemied;
    public bool thrown;
    public bool lmb;
    public bool rmb;
    public Vector3 mouseposition;
    public Collider2D[] grabbedEnemies;

    public void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<CircleCollider2D>();
        spriteRenderer.enabled = false;
        body = GetComponent<Rigidbody2D>();
    }
    
    public void Update() {
        CheckInput();
        CheckMousePosition();
    }

    public void CheckInput() {
        lmb = Input.GetMouseButton(0);
        rmb = Input.GetMouseButton(1);
    }
    public void FixedUpdate()
    {
        CheckGrabbed();
        CheckCollision();
    }

    public void CheckMousePosition() {
        mouseposition = Input.mousePosition;
        mouseposition.z = Camera.main.nearClipPlane;
        mouseposition = Camera.main.ScreenToWorldPoint(mouseposition);
    }
    public void CheckGrabbed() {
        grabbedEnemies = Physics2D.OverlapAreaAll(col.bounds.min, col.bounds.max, enemyMask);
        grounded = Physics2D.OverlapAreaAll(col.bounds.min, col.bounds.max, groundMask).Length > 0;
        enemied = grabbedEnemies.Length > 0 && rmb;
    }

    public void CheckCollision() {
        
        Vector2 direction = new Vector2(0,0);
        Vector2 newvector = new Vector2(0,0);
        if(rmb && (transform.position != player.transform.position || !thrown) ) {
            spriteRenderer.enabled = true;
            if(!grappling) {
                direction = mouseposition - transform.position;
                newvector = direction.normalized * throwSpeed;
                grappling = true;
            }

            if(Vector2.Distance(player.transform.position, transform.position) > 2) {
                thrown = true;
            }
                
            if(enemied){
                // if(Vector2.Distance(player.transform.position, transform.position) > 2) {
                //     direction = player.transform.position - transform.position;
                //     newvector = direction.normalized * returnSpeed;
                //     body.velocity = newvector;
                // } else {
                //     spriteRenderer.enabled = false;
                //     transform.position = player.transform.position;
                //     body.velocity = Vector2.zero;
                //     thrown = false;
                // }
                body.velocity = Vector2.zero;
                transform.position = grabbedEnemies[0].GetComponent<Transform>().position;
                
            } else if(grounded) {
                body.velocity = Vector2.zero;
            } else {
                body.velocity += newvector;
            }
        } else {
            spriteRenderer.enabled = false;
            transform.position = player.transform.position;
            body.velocity = Vector2.zero;
            grappling = false;
            thrown = false;
        }
    }
    // void OnTriggerStay (Collider other)
    // {
    //     body.velocity = Vector2.zero;
    //     other.GetComponent<Rigidbody>().velocity = Vector2.zero;
    // }

}
