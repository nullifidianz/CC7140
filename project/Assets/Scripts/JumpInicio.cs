using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpInicio : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private float playerHalfHeight;

    void Start()
    {
        playerHalfHeight = spriteRenderer.bounds.extents.y;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && GetIsGrounded())
        {
            Jump();
        }

        Debug.DrawRay(transform.position, Vector2.down * (playerHalfHeight + 0.1f), Color.red);
    }

    private bool GetIsGrounded(){
        return Physics2D.Raycast(transform.position, Vector2.down, playerHalfHeight + 0.1f, LayerMask.GetMask("Ground"));
    }

    private void Jump(){
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
