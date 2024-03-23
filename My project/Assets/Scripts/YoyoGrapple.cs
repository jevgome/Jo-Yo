using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoyoGrapple : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask groundMask;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D body;
    private CircleCollider2D collider;
    [SerializeField] private float speed;
    private bool grappling = false;
    [SerializeField] private bool collided;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<CircleCollider2D>();
        Physics.IgnoreLayerCollision(6,6);
        spriteRenderer.enabled = false;
        body = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector3 mouseposition = Input.mousePosition;
        mouseposition.z = Camera.main.nearClipPlane;
        mouseposition = Camera.main.ScreenToWorldPoint(mouseposition);
        collided = Physics2D.OverlapAreaAll(collider.bounds.min, collider.bounds.max, groundMask).Length > 0;
        Vector2 direction = new Vector2(0,0);
        Vector2 newvector = new Vector2(0,0);
        if(Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
            spriteRenderer.enabled = true;
            if(!grappling) {
                direction = mouseposition - transform.position;
                newvector = direction.normalized * speed * Time.fixedDeltaTime;
            }
            grappling = true;
            if(!collided) {
                body.AddForce(newvector);
            } else {
                body.velocity = Vector2.zero;
            }
        } else {
            spriteRenderer.enabled = false;
            transform.position = player.transform.position;
            grappling = false;
        }
    }
}
