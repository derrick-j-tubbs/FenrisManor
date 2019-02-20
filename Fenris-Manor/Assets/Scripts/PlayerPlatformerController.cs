using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject
{
    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            }
        }

        bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < 0.01f));

        if (flipSprite) {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
        targetVelocity = move * maxSpeed;

    }
}
