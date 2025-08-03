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
        enemied = grabbedEnemies.Length > 0 && (lmb || rmb);
    }

    public void CheckCollision() {
        if((lmb || rmb) && (transform.position != player.transform.position || !grappling) ) {
            spriteRenderer.enabled = true;
            if(!grappling) {
                Vector2 direction = mouseposition - transform.position;
                direction = direction.normalized;
                Vector2 dir2 = direction;
                Vector2 pos = new Vector2(player.transform.position.x + dir2.x, player.transform.position.y + dir2.y);
                RaycastHit2D hit = Physics2D.Raycast(pos, direction);
                if(hit) {
                    transform.position = hit.point;
                }
                grappling = true;
            }

            if(enemied){
                transform.position = grabbedEnemies[0].GetComponent<Transform>().position;
                if(lmb)body.velocity = grabbedEnemies[0].GetComponent<Rigidbody2D>().velocity;
            } else if(grounded) {
                body.velocity = Vector2.zero;
            }
        } else {
            spriteRenderer.enabled = false;
            transform.position = player.transform.position;
            body.velocity = Vector2.zero;
            grappling = false;
        }
    }
}
