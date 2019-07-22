using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject
{
    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected override void AnimateActor(){
        if(velocity.y != 0) {
            if (velocity.y > 0) {
                animator.Play("PlayerJumpUp");
            }
            else if(velocity.y < 0) {
                animator.Play("PlayerJumpDown");
            }
        } else {
            if (velocity.x != 0) {
                animator.Play("PlayerWalk");
                bool flipSprite = (spriteRenderer.flipX ? (velocity.x > 0.01f) : (velocity.x < 0.01f));

                if (flipSprite) {
                    spriteRenderer.flipX = !spriteRenderer.flipX;
                }
            } else {
                animator.Play("PlayerIdle");
            }
        }
    }

    protected override void ComputeVelocity(){
        //  Reset the move vector every time this function is called
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded) {
            velocity.y = jumpTakeOffSpeed;
        } else if (Input.GetButtonUp("Jump")){
            if (velocity.y > 0) {
                velocity.y *= 0.5f;
                move.x = move.x * 0.5f;
            }
        }

        targetVelocity = move * maxSpeed;

    }
}
