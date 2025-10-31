using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementInicio : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Rigidbody2D rb;
    private Vector2 movement;
    public Animator _animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();  // Initialize SpriteRenderer
    }

    void Update()
    {
        bool rightPressed = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        bool leftPressed = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);

        // Set animation state
        bool isWalking = rightPressed || leftPressed;
        _animator.SetBool("IsWalking", isWalking);

        // Flip sprite based on key press
        if (rightPressed)
        {
            spriteRenderer.flipX = false;
        }
        else if (leftPressed)
        {
            spriteRenderer.flipX = true;
        }
    }
}
