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
    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        bool rightPressed = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        bool leftPressed = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);

        // Set animation state
        bool isWalking = rightPressed || leftPressed;
        _animator.SetBool("IsWalking", isWalking);

        // Calculate movement
        float moveDirection = 0f;
        if (rightPressed) moveDirection = 1f;
        if (leftPressed) moveDirection = -1f;

        // Move both player and camera
        Vector3 movement = new Vector3(moveDirection * moveSpeed * Time.deltaTime, 0f, 0f);
        transform.position += movement;
        mainCamera.transform.position += movement;

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
