using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStairController : MonoBehaviour
{
    public float transitionSpeed = 3f;
    private float fraction = 0;

    public float animationDelay = 6f / 60f;

    private bool canMove = true;
    private bool playerFalling = false;

    private Animator animator;
    private PlayerController playerController;
    private GameObject player;
    private PlayerPlatformerController platformerController;
    
    private StairController.STAIR_DIRECTION stairDirection;
    private GameObject leftEndStep;
    private GameObject rightEndStep;
    private string directionLeft;

    private void Awake() {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        animator = playerController.PlayerAnimator;
        platformerController = player.GetComponent<PlayerPlatformerController>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Stairs" || playerController.getIsClimbing())
            return;
        GameObject stairs = collision.gameObject;
        StairController stairController = stairs.GetComponentInParent<StairController>();
        leftEndStep = stairController.leftEndStep;
        rightEndStep = stairController.rightEndStep;

        stairDirection = stairController.getStairDirection();
        float climb = Input.GetAxisRaw("Climb");
        if ((climb == 1 || climb== -1) && playerController.getPlayerStairState() != PlayerController.STAIR_STATE.on_stair)
        {
            platformerController.enabled = false;
            playerController.setIsClimbing(true);
            //Debug.Log("Control taken from platformer controller");
            // move player to base of stairs if grounded
            if (platformerController.PlayerGrounded())
            {
                //Debug.Log("Closest End: " + closestEndStep.name);
                fraction = 0;
                StartCoroutine(MovePlayerToStairs(player, stairController));
            } else {
                StartCoroutine(SnapPlayerToStairs(player, stairController));
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.tag != "Stairs" || playerController.getIsClimbing())
            return;
        GameObject stairs = collision.gameObject;
        StairController stairController = stairs.GetComponentInParent<StairController>();
        leftEndStep = stairController.leftEndStep;
        rightEndStep = stairController.rightEndStep;

        float climb = Input.GetAxisRaw("Climb");
        if ((climb == 1 || climb == -1) && playerController.getPlayerStairState() != PlayerController.STAIR_STATE.on_stair) {
            platformerController.enabled = false;
            playerController.setIsClimbing(true);
            if (platformerController.PlayerGrounded()) {
                fraction = 0;
                StartCoroutine(MovePlayerToStairs(player, stairController));
            } else {
                StartCoroutine(SnapPlayerToStairs(player, stairController));
            }
        }
    }

    private void Update() {
        if (playerController.getPlayerStairState() == PlayerController.STAIR_STATE.on_stair && canMove && playerController.getIsClimbing()) {
            float move = Input.GetAxisRaw("Horizontal");
            float climb = Input.GetAxisRaw("Climb");
            if (Input.GetButton("Jump") && climb == -1 && !playerFalling) {
                StartCoroutine(FallOffStairs());
            } else {
                if (stairDirection == StairController.STAIR_DIRECTION.Up && !playerFalling) {
                    if (move == 1 || climb == 1) {
                        canMove = false;
                        StartCoroutine(MoveRightOnStairs(player));
                    } else if (move == -1 || climb == -1) {
                        canMove = false;
                        StartCoroutine(MoveLeftOnStairs(player));
                    }
                } else if (stairDirection == StairController.STAIR_DIRECTION.Down) {
                    if (move == 1 || climb == -1) {
                        canMove = false;
                        StartCoroutine(MoveRightOnStairs(player));
                    } else if (move == -1 || climb == 1) {
                        canMove = false;
                        StartCoroutine(MoveLeftOnStairs(player));
                    }
                }
            }
        }
    }

    GameObject FindClosestEnd(GameObject player, StairController stairController) {
        float leftDifference = Mathf.Abs(stairController.leftEndStep.transform.position.x - player.transform.position.x);
        float rightDifference = Mathf.Abs(stairController.rightEndStep.transform.position.x - player.transform.position.x);
        if (leftDifference < rightDifference)
            return stairController.leftEndStep;
        return stairController.rightEndStep;
    }

    Vector2 FindClosestStep(GameObject player, StairController stairController, GameObject closestEndStep) {
        float playerX = player.transform.position.x;
        playerX = Mathf.Round(playerX * 2f) * 0.5f;
        float playerY = 0;
        if (stairDirection == StairController.STAIR_DIRECTION.Up){
            if (closestEndStep.transform.position.x > playerX) {
                //Debug.Log("right is closer");
                playerY = closestEndStep.transform.position.y - (closestEndStep.transform.position.x - playerX) + 1.5f;
            } else {
                //Debug.Log("left is closer");
                playerY = closestEndStep.transform.position.y + (playerX - closestEndStep.transform.position.x) + 1f;
            }
        } else if (stairDirection == StairController.STAIR_DIRECTION.Down) {
            if (closestEndStep.transform.position.x > playerX) {
                //Debug.Log("right is closer");
                //Debug.Log("Initial: " + closestEndStep.transform.position.y);
                //Debug.Log("+/-: " + (playerX - closestEndStep.transform.position.x));
                // not entirely clear why this one needs to be + 0.5f instead of + 1 same issue as right is closer above, likely something to do with my placement of the end points. As long as this is consistent I don't really care. Even if it does secretly bother me.
                playerY = closestEndStep.transform.position.y + (closestEndStep.transform.position.x - playerX) + 0.5f;
            } else {
                //Debug.Log(playerX - closestEndStep.transform.position.x + 1);
                //Debug.Log(closestEndStep.transform.position.y + 1);
                //Debug.Log("left is closer");
                playerY = closestEndStep.transform.position.y - (playerX - closestEndStep.transform.position.x) + 1f;
            }
        }
        playerX += 0.25f;
        return new Vector2(playerX, playerY);
    }

    IEnumerator SnapPlayerToStairs(GameObject player, StairController stairController) {
        GameObject closestEndStep = FindClosestEnd(player, stairController);

        Vector2 closestStep = FindClosestStep(player, stairController, closestEndStep);
        Vector2 posPlayer = new Vector2(player.transform.position.x, closestStep.y);
        playerController.setIsClimbing(true);

        this.enabled = false;
        player.transform.position = Vector2.Lerp(posPlayer, closestStep, 1);
        animator.Play("PlayerStairsIdle");
        playerController.setPlayerStairState(PlayerController.STAIR_STATE.on_stair);
        platformerController.SetPlayerGrounded(false);
        yield return new WaitForSeconds(animationDelay);
        this.enabled = true;
    }

    IEnumerator MovePlayerToStairs(GameObject player, StairController stairController) {
        GameObject closestEndStep = FindClosestEnd(player, stairController);
        Vector2 posPlayer = new Vector2(player.transform.position.x, closestEndStep.transform.position.y + 1);
        Vector2 posStep = Vector2.zero;
        playerController.setIsClimbing(true);

        if (stairDirection == StairController.STAIR_DIRECTION.Up) {
            if (closestEndStep.transform.position.x == stairController.leftEndStep.transform.position.x) {
                FlipSprite(PlayerController.PLAYER_FACING.right);
                posStep = new Vector2(closestEndStep.transform.position.x + 0.25f, closestEndStep.transform.position.y + 1);
            } else if (closestEndStep.transform.position.x == stairController.rightEndStep.transform.position.x) {
                FlipSprite(PlayerController.PLAYER_FACING.left);
                posStep = new Vector2(closestEndStep.transform.position.x - 0.25f, closestEndStep.transform.position.y + 1);
            }
        } else if (stairDirection == StairController.STAIR_DIRECTION.Down) {
            if (closestEndStep.transform.position.x == stairController.leftEndStep.transform.position.x) {
                FlipSprite(PlayerController.PLAYER_FACING.right);
                posStep = new Vector2(closestEndStep.transform.position.x + 0.25f, closestEndStep.transform.position.y + 1);
            } else if (closestEndStep.transform.position.x == stairController.rightEndStep.transform.position.x) {
                FlipSprite(PlayerController.PLAYER_FACING.left);
                posStep = new Vector2(closestEndStep.transform.position.x - 0.25f, closestEndStep.transform.position.y + 1);
            }
        }

        this.enabled = false;
        while (fraction < 1) {
            fraction += Time.deltaTime * transitionSpeed * 2;
            if (fraction > 1)
                fraction = 1;
            player.transform.position = Vector2.Lerp(posPlayer, posStep, fraction);
            yield return new WaitForEndOfFrame();
        }
        animator.Play("PlayerStairsIdle");
        platformerController.SetPlayerGrounded(false);
        playerController.setPlayerStairState(PlayerController.STAIR_STATE.on_stair);
        this.enabled = true;
    }

    IEnumerator MoveLeftOnStairs(GameObject player) {
        Vector2 moveTo = Vector2.zero;
        FlipSprite(PlayerController.PLAYER_FACING.left);

        animator.Play("PlayerStairsIdle");
        yield return new WaitForSeconds(animationDelay / 2);

        if (stairDirection == StairController.STAIR_DIRECTION.Up) {
            float newX = player.transform.position.x - 0.5f;
            float newY = player.transform.position.y - 0.5f;
            moveTo = new Vector2(newX, newY);
            animator.Play("PlayerDownStairs");
        } else if (stairDirection == StairController.STAIR_DIRECTION.Down) {
            float newX = player.transform.position.x - 0.5f;
            float newY = player.transform.position.y + 0.5f;
            moveTo = new Vector2(newX, newY);
            animator.Play("PlayerUpStairs");
        } 
        if (!CheckLeftBound(moveTo)) {
            //Debug.Log("Moving Left on Stairs");
            player.transform.position = Vector2.Lerp(player.transform.position, moveTo, 1);
            yield return new WaitForSeconds(animationDelay);
            animator.Play("PlayerStairsIdle");
            yield return new WaitForSeconds(animationDelay / 2);
            canMove = true;
        } else {
            fraction = 0;
            Debug.Log("Moving Player Off Stairs");
            StartCoroutine(MovePlayerOffStairs());
        }
    }

    bool CheckLeftBound(Vector2 moveTo) {
        if (moveTo.x < leftEndStep.transform.position.x) {
            directionLeft = "left";
            return true;
        }
        return false;
    }

    IEnumerator MoveRightOnStairs(GameObject player) {
        Vector2 moveTo = Vector2.zero;
        FlipSprite(PlayerController.PLAYER_FACING.right);

        animator.Play("PlayerStairsIdle");
        yield return new WaitForSeconds(animationDelay / 2);

        if (stairDirection == StairController.STAIR_DIRECTION.Up) {
            
            float newX = player.transform.position.x + 0.5f;
            float newY = player.transform.position.y + 0.5f;
            moveTo = new Vector2(newX, newY);
            animator.Play("PlayerUpStairs");

        } else if (stairDirection == StairController.STAIR_DIRECTION.Down) {
            float newX = player.transform.position.x + 0.5f;
            float newY = player.transform.position.y - 0.5f;
            moveTo = new Vector2(newX, newY);
            animator.Play("PlayerDownStairs");    
        } 
        if (!CheckRightBound(moveTo)){
            player.transform.position = Vector2.Lerp(player.transform.position, moveTo, 1);
            yield return new WaitForSeconds(animationDelay);
            animator.Play("PlayerStairsIdle");
            yield return new WaitForSeconds(animationDelay / 2);
            canMove = true;
        } else {
            fraction = 0;
            Debug.Log("Moving Player Off Stairs");
            StartCoroutine(MovePlayerOffStairs());
        }
    }

    bool CheckRightBound(Vector2 moveTo) {
        if (moveTo.x > rightEndStep.transform.position.x) {
            directionLeft = "right";
            return true;
        }
        return false;
    }

    IEnumerator MovePlayerOffStairs() {
        Vector2 posPlayer = player.transform.position;
        Vector2 moveTo = Vector2.zero;
        if (directionLeft == "left") {
            moveTo = new Vector2(player.transform.position.x - 1.5f, player.transform.position.y);
        } else if (directionLeft == "right") {
            moveTo = new Vector2(player.transform.position.x + 1.5f, player.transform.position.y );
        }

        this.enabled = false;
        animator.Play("PlayerWalk");
        playerController.setIsClimbing(false);
        while (fraction < 1) {
            fraction += Time.deltaTime * transitionSpeed * 4;
            if (fraction > 1)
                fraction = 1;
            player.transform.position = Vector2.Lerp(posPlayer, moveTo, fraction);
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Player off stairs");
        playerController.setPlayerStairState(PlayerController.STAIR_STATE.off_stair);
        platformerController.enabled = true;
        canMove = true;
    }

    IEnumerator FallOffStairs() {        
        Debug.Log("Player jumping off stairs");
        platformerController.enabled = true;
        playerFalling = true;
        playerController.setIsClimbing(false);
        while (!platformerController.PlayerGrounded()) {
            animator.Play("PlayerJumpDown");
            yield return new WaitForEndOfFrame();
        }
        playerController.setPlayerStairState(PlayerController.STAIR_STATE.off_stair);
        canMove = true;
        playerFalling = false;
    }

    void FlipSprite(PlayerController.PLAYER_FACING playerFacing) {
        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
        bool flipSprite = (spriteRenderer.flipX ? (playerFacing == PlayerController.PLAYER_FACING.right) : (playerFacing == PlayerController.PLAYER_FACING.left));

            if (flipSprite) {
                spriteRenderer.flipX = !spriteRenderer.flipX;
                if (playerController.getPlayerFacing() == PlayerController.PLAYER_FACING.right) {
                    playerController.setPlayerFacing(PlayerController.PLAYER_FACING.left);
                } else {
                    playerController.setPlayerFacing(PlayerController.PLAYER_FACING.right);
                }
            }
    }
}
