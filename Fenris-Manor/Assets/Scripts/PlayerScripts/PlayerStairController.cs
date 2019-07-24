using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStairController : MonoBehaviour
{
    public float transitionSpeed = 3f;
    private float fraction = 0;

    public float animationDelay = 6f / 60f;

    private bool canMove = true;

    private Animator animator;
    private PlayerController playerController;
    private GameObject player;
    private PlayerController.STAIR_STATE playerStairState = PlayerController.STAIR_STATE.off_stair;
    private StairController.STAIR_DIRECTION stairDirection;

    private void Awake() {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        animator = playerController.PlayerAnimator;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Stairs")
            return;

        PlayerPlatformerController platformerController = player.GetComponent<PlayerPlatformerController>();
        
        GameObject stairs = collision.gameObject;
        StairController stairController = stairs.GetComponent<StairController>();

        GameObject closestEndStep;

        stairDirection = stairController.getStairDirection();
        float climb = Input.GetAxis("Climb");
        if (climb > 0.1f)
        {
            platformerController.enabled = false;
            //Debug.Log("Control taken from platformer controller");
            // move player to base of stairs if grounded [may require animation]
            if (platformerController.PlayerGrounded())
            {
                closestEndStep = FindClosestEnd(stairController, player);
                //Debug.Log("Closest End: " + closestEndStep.name);
                StartCoroutine(MovePlayerToStairs(player, closestEndStep, stairController));
                playerStairState = PlayerController.STAIR_STATE.on_stair;
            }
        }
    }

    private void Update() {
        if (playerStairState == PlayerController.STAIR_STATE.on_stair && canMove) {
            if (Input.GetKey("d")) {
                canMove = false;
                Debug.Log("Moving Right on Stairs");
                StartCoroutine(MoveRightOnStairs(player));
            } else if (Input.GetKey("a")) {
                canMove = false;
                Debug.Log("Moving Left on Stairs");
                StartCoroutine(MoveLeftOnStairs(player));
            }
        }
    }

    GameObject FindClosestEnd(StairController stairs, GameObject player) {
        float leftDifference = Mathf.Abs(stairs.leftEndStep.transform.position.x - player.transform.position.x);
        float rightDifference = Mathf.Abs(stairs.rightEndStep.transform.position.x - player.transform.position.x);
        if (leftDifference < rightDifference)
            return stairs.leftEndStep;
        return stairs.rightEndStep;
    }

    GameObject FindOppositeEnd(StairController stairs, GameObject closestEndStep) {
        if (stairs.rightEndStep.transform.position.x == closestEndStep.transform.position.x) {
            return stairs.leftEndStep;
        }
        return stairs.rightEndStep;
    }

    IEnumerator MovePlayerToStairs(GameObject player, GameObject closestEndStep, StairController stairController) {
        Vector2 posPlayer = new Vector2(player.transform.position.x, closestEndStep.transform.position.y + 1);
        Vector2 posStep = Vector2.zero;
        if (closestEndStep.transform.position.x == stairController.leftEndStep.transform.position.x) {
            posStep = new Vector2(closestEndStep.transform.position.x + 0.25f, closestEndStep.transform.position.y + 1);
        } else if (closestEndStep.transform.position.x == stairController.rightEndStep.transform.position.x) {
            posStep = new Vector2(closestEndStep.transform.position.x - 0.25f, closestEndStep.transform.position.y + 1);
        }

        this.enabled = false;
        while (fraction < 1) {
            fraction += Time.deltaTime * transitionSpeed;
            if (fraction > 1)
                fraction = 1;
            player.transform.position = Vector2.Lerp(posPlayer, posStep, fraction);
            yield return new WaitForEndOfFrame();
        }
        animator.Play("PlayerStairsIdle");
        this.enabled = true;
    }

    IEnumerator MoveLeftOnStairs(GameObject player) {
        Vector2 moveTo = Vector2.zero;
        float velocity = -1f;
        FlipSprite(velocity);

        animator.Play("PlayerStairsIdle");
        yield return new WaitForSeconds(animationDelay / 2);

        if (stairDirection == StairController.STAIR_DIRECTION.Up) {
            float newX = player.transform.position.x - 0.5f;
            float newY = player.transform.position.y - 0.5f;
            moveTo = new Vector2(newX, newY);
            animator.Play("PlayerDownStairs");
        } else if (stairDirection == StairController.STAIR_DIRECTION.Down) {
            float newX = player.transform.position.x + 0.5f;
            float newY = player.transform.position.y + 0.5f;
            moveTo = new Vector2(newX, newY);
            animator.Play("PlayerUpStairs");
        } 
        player.transform.position = Vector2.Lerp(player.transform.position, moveTo, 1);
        yield return new WaitForSeconds(animationDelay);
        animator.Play("PlayerStairsIdle");
        yield return new WaitForSeconds(animationDelay / 2);
        canMove = true;
    }

    IEnumerator MoveRightOnStairs(GameObject player) {
        Vector2 moveTo = Vector2.zero;
        float velocity = 1f;
        FlipSprite(velocity);

        animator.Play("PlayerStairsIdle");
        yield return new WaitForSeconds(animationDelay / 2);

        if (stairDirection == StairController.STAIR_DIRECTION.Up) {
            
            float newX = player.transform.position.x + 0.5f;
            float newY = player.transform.position.y + 0.5f;
            moveTo = new Vector2(newX, newY);
            animator.Play("PlayerUpStairs");

        } else if (stairDirection == StairController.STAIR_DIRECTION.Down) {
            float newX = player.transform.position.x - 0.5f;
            float newY = player.transform.position.y - 0.5f;
            moveTo = new Vector2(newX, newY);
            animator.Play("PlayerDownStairs");    
        } 
        player.transform.position = Vector2.Lerp(player.transform.position, moveTo, 1);
        yield return new WaitForSeconds(animationDelay);
        animator.Play("PlayerStairsIdle");
        yield return new WaitForSeconds(animationDelay / 2);
        canMove = true;
    }

    void FlipSprite(float velocity) {
        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
        bool flipSprite = (spriteRenderer.flipX ? (velocity > 0.01f) : (velocity < 0.01f));

                if (flipSprite) {
                    spriteRenderer.flipX = !spriteRenderer.flipX;
                }
    }
}
