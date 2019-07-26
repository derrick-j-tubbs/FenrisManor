using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStairController : MonoBehaviour
{
    public float transitionSpeed = 3f;
    private float fraction = 0;

    public float animationDelay = 6f / 60f;

    private bool canMove = true;
    private bool climbEnabled = true;

    private Animator animator;
    private PlayerController playerController;
    private GameObject player;
    private PlayerPlatformerController platformerController;
    
    private StairController.STAIR_DIRECTION stairDirection;
    private GameObject leftEndStep;
    private GameObject rightEndStep;
    private string directionLeft;
    private bool isClimbing;

    private void Awake() {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        animator = playerController.PlayerAnimator;
        platformerController = player.GetComponent<PlayerPlatformerController>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Stairs")
            return;
        if (!climbEnabled)
            return;   
        GameObject stairs = collision.gameObject;
        StairController stairController = stairs.GetComponentInParent<StairController>();
        leftEndStep = stairController.leftEndStep;
        rightEndStep = stairController.rightEndStep;

        GameObject closestEndStep;

        stairDirection = stairController.getStairDirection();
        float climb = Input.GetAxisRaw("Climb");
        if ((climb == 1 || climb== -1) && playerController.getPlayerStairState() != PlayerController.STAIR_STATE.on_stair)
        {
            platformerController.enabled = false;
            //Debug.Log("Control taken from platformer controller");
            // move player to base of stairs if grounded [may require animation]
            if (platformerController.PlayerGrounded())
            {
                closestEndStep = FindClosestEnd(stairController, player);
                //Debug.Log("Closest End: " + closestEndStep.name);
                fraction = 0;
                StartCoroutine(MovePlayerToStairs(player, closestEndStep, stairController));
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.tag != "Stairs")
            return;
        if (!climbEnabled)
            return;
        GameObject stairs = collision.gameObject;
        GameObject closestEndStep;
        StairController stairController = stairs.GetComponentInParent<StairController>();
        leftEndStep = stairController.leftEndStep;
        rightEndStep = stairController.rightEndStep;

        float climb = Input.GetAxisRaw("Climb");
        if ((climb == 1 || climb == -1) && playerController.getPlayerStairState() != PlayerController.STAIR_STATE.on_stair && platformerController.PlayerGrounded()) {
            platformerController.enabled = false;
            closestEndStep = FindClosestEnd(stairController, player);
            fraction = 0;
            StartCoroutine(MovePlayerToStairs(player, closestEndStep, stairController));
        }
    }

    private void Update() {
        if (playerController.getPlayerStairState() == PlayerController.STAIR_STATE.on_stair && canMove && climbEnabled) {
            float move = Input.GetAxisRaw("Horizontal");
            float climb = Input.GetAxisRaw("Climb");
            if (climb == -1 && Input.GetButton("Jump")) {
                FallOffStairs();
                return;
            }
            if (stairDirection == StairController.STAIR_DIRECTION.Up) {
                if (move == 1 || climb == 1) {
                    canMove = false;
                    Debug.Log("Moving Right on Stairs");
                    StartCoroutine(MoveRightOnStairs(player));
                } else if (move == -1 || climb == -1) {
                    canMove = false;
                    StartCoroutine(MoveLeftOnStairs(player));
                }
            } else if (stairDirection == StairController.STAIR_DIRECTION.Down) {
                if (move == 1 || climb == -1) {
                    canMove = false;
                    Debug.Log("Moving Right on Stairs");
                    StartCoroutine(MoveRightOnStairs(player));
                } else if (move == -1 || climb == 1) {
                    canMove = false;
                    StartCoroutine(MoveLeftOnStairs(player));
                }
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



    IEnumerator MovePlayerToStairs(GameObject player, GameObject closestEndStep, StairController stairController) {
        Vector2 posPlayer = new Vector2(player.transform.position.x, closestEndStep.transform.position.y + 1);
        Vector2 posStep = Vector2.zero;
        if (stairDirection == StairController.STAIR_DIRECTION.Up) {
            if (closestEndStep.transform.position.x == stairController.leftEndStep.transform.position.x) {
                posStep = new Vector2(closestEndStep.transform.position.x + 0.25f, closestEndStep.transform.position.y + 1);
            } else if (closestEndStep.transform.position.x == stairController.rightEndStep.transform.position.x) {
                posStep = new Vector2(closestEndStep.transform.position.x - 0.25f, closestEndStep.transform.position.y + 1);
            }
        } else if (stairDirection == StairController.STAIR_DIRECTION.Down) {
            if (closestEndStep.transform.position.x == stairController.leftEndStep.transform.position.x) {
                posStep = new Vector2(closestEndStep.transform.position.x + 0.25f, closestEndStep.transform.position.y + 1);
            } else if (closestEndStep.transform.position.x == stairController.rightEndStep.transform.position.x) {
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
        playerController.setPlayerStairState(PlayerController.STAIR_STATE.on_stair);
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
            float newX = player.transform.position.x - 0.5f;
            float newY = player.transform.position.y + 0.5f;
            moveTo = new Vector2(newX, newY);
            animator.Play("PlayerUpStairs");
        } 
        if (!CheckLeftBound(moveTo)) {
            Debug.Log("Moving Left on Stairs");
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
            moveTo = new Vector2(player.transform.position.x - 1, player.transform.position.y);
        } else if (directionLeft == "right") {
            moveTo = new Vector2(player.transform.position.x + 1, player.transform.position.y);
        }

        this.enabled = false;
        animator.Play("PlayerWalk");
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

    void FallOffStairs() {
        animator.Play("PlayerJumpDown");
        playerController.setPlayerStairState(PlayerController.STAIR_STATE.off_stair);
        platformerController.enabled = true;
        canMove = true;
    }

    void FlipSprite(float velocity) {
        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
        bool flipSprite = (spriteRenderer.flipX ? (velocity > 0.01f) : (velocity < 0.01f));

                if (flipSprite) {
                    spriteRenderer.flipX = !spriteRenderer.flipX;
                }
    }

    public void ClimbEnabled(bool disableClimb) {
        climbEnabled = disableClimb;
    }

    public bool GetClimbEnabled() {
        return climbEnabled;
    }
}
