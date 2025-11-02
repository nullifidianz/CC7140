using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Latters : MonoBehaviour
{
    private float vertical;
    private bool isLadder;
    private bool isClimbing;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float climbingSpeed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private float playerHalfHeight;
    void Start()
    {
        playerHalfHeight = spriteRenderer.bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {
        vertical = Input.GetAxis("Vertical");
        if(isLadder && Mathf.Abs(vertical) > 0f)
        {
            isClimbing = true;
        }
    }

    private void FixedUpdate(){
        if(isClimbing)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(rb.velocity.x, vertical * climbingSpeed);
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Escada"))
        {
            isLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Escada"))
        {
            isLadder = false;
            isClimbing = false;
        }
    }
}
