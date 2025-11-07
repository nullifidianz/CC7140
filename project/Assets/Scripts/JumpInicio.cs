using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpInicio : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Vector2 WallJumpForce = new Vector2(2f,4f);
    private float playerHalfHeight;
    private float playerHalfWidth;

    void Start()
    {
        playerHalfHeight = spriteRenderer.bounds.extents.y;
        playerHalfWidth = spriteRenderer.bounds.extents.x;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            CheckJumpType();
        }

        Debug.DrawRay(transform.position, Vector2.down * (playerHalfHeight + 0.1f), Color.red);
    }

    private void CheckJumpType(){
        if(GetIsGrounded()){
            Jump();
        }
        else{
            int wallJumpDirection = GetWallJumpDirection();
            if(wallJumpDirection != 0){
                WallJump(wallJumpDirection);
            }
        }
    }
    private void WallJump(int wallJumpDirection){
        Vector2 force = WallJumpForce;
        force.x *= wallJumpDirection;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D other){
        GetIsGrounded();
    }

    private int GetWallJumpDirection(){
        // Verifica parede à direita - só permite wall jump se estiver se movendo para a direita
        if(Physics2D.Raycast(transform.position, Vector2.right, playerHalfWidth + 0.1f, LayerMask.GetMask("Wall"))){
            // Só permite wall jump se estiver se movendo em direção à parede (ou parado)
            if(rb.velocity.x >= -0.1f){
                return -1; // Pula para a esquerda
            }
        }
        // Verifica parede à esquerda - só permite wall jump se estiver se movendo para a esquerda
        else if(Physics2D.Raycast(transform.position, Vector2.left, playerHalfWidth + 0.1f, LayerMask.GetMask("Wall"))){
            // Só permite wall jump se estiver se movendo em direção à parede (ou parado)
            if(rb.velocity.x <= 0.1f){
                return 1; // Pula para a direita
            }
        }
        return 0;
    }

    private bool GetIsGrounded(){
        return Physics2D.Raycast(transform.position, Vector2.down, playerHalfHeight + 0.1f, LayerMask.GetMask("Ground"));
    }

    private void Jump(){
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
