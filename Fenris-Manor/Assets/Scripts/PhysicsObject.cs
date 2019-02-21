using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    // Publicly modifiable values used to determine:
    public float fGravityModifier = 1f;     // the strength of gravity relative to in-built gravity
    public float minGroundNormalY = 0.65f;  // what angles are considered 'grounded'
    
    // Tools to determine whether an object is on the ground
    protected bool isGrounded;
    protected Vector2 groundNormal;

    // Values for player control and horizontal movement
    protected Vector2 targetVelocity;   // Stores incoming input from outside of the class

    protected Rigidbody2D rbObject;     // Variable to hold the Rigidbody2D Component of the object being affected by phsyics
    protected Vector2 velocity;         // Variable to hold the Vector2 (x,y) velocity of the object
    protected ContactFilter2D contactFilter;    // A filter for which layers should be included for collision [see note in declaration during Start()]
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];  // An array used to get the results of from the Rigidbody2D.cast
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D> (16); // A list to store the array for later uses

    // Constants that help prevent items passing through each other or getting stuck inside of each other
    protected const float minMoveDistance = 0.0001f;
    protected const float shellRadius = 0.01f;

    void OnEnable() {
        // Get the Rigidbody2d component of the object this script is attached to
        rbObject=GetComponent<Rigidbody2D>();    
    }
    // Start is called before the first frame update
    void Start() {
        // Set the contactfilter2d item to use the layer mask [Edit > project settings > Physics2D]
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    // Update is called once per frame
    void Update() {
        //Zero out the target velocity so that you don't use the velocity from the previous frame.
        targetVelocity = Vector2.zero;
        ComputeVelocity();
        AnimateActor();
    }

    // This will be overridden in PlayerPlatformerController.cs and used to calculate the new target velocity of our player object.
    protected virtual void ComputeVelocity() {

    }

    protected virtual void AnimateActor( ) {

    }

    // Use fixed update for physic
    void FixedUpdate() {
        // Use the gravity modifier, default gravity value from Unity, and the time since the last frame to calculate the velocity vector for the current object
        velocity += (fGravityModifier * Physics2D.gravity * Time.deltaTime);

        // Use target velocity from the external class and add it to our current x velocity to determine the new velocity
        velocity.x = targetVelocity.x;

        isGrounded = false;

        // Use veloicty and time since last frame to calculate the change in position
        Vector2 deltaPostion = velocity * Time.deltaTime;
        
        /* If using sloped tiles this is necessary to make sure your character walks up/down a slope instead of into a slope.
         Since this game does not use slopes, I'm going to take this part of the calculation out, but leave it here just as a note */
        //Vector2 followGround = new Vector2 (groundNormal.y, -groundNormal.x);

        Vector2 move = Vector2.right * deltaPostion.x;

        Movement(move, false);

        // Determine how much the object needs to move by using deltaPosition's y value
        move = Vector2.up * deltaPostion.y;

        Movement(move, true);
    }

    // Function to make the given object move, this function is only called if the character is moving along the y axist
    void Movement(Vector2 move, bool yMovement) {
        //Distance is used to determine whether or not collision should be checked. This prevents stationary objects from constantly checking their collision
        float distance = move.magnitude;

        //use cast to get the number of other colliders that collide with this one using the contact filter [Edit > project settings > Physics2d] layer collision matrix
        if (distance > minMoveDistance) {
            int count = rbObject.Cast(move, contactFilter, hitBuffer, (distance + shellRadius));

            // Populate a list with the items from the hit buffer array after clearing the old list
            hitBufferList.Clear();
            for(int i = 0; i < count; i++) {
                hitBufferList.Add(hitBuffer[i]);
            }

            // Iterate through the hit list to determine if the character hit anything that would cause them to be grounded
            // Also check if they collided with walls or ceilings to cancel out any momentum in directions that they player should not move to avoid clipping
            for (int i = 0; i < hitBufferList.Count; i++)
            {
                // This is used to insure that the player is truely grounded, not just collided with a wall
                Vector2 currentNormal = hitBufferList[i].normal;
                if (currentNormal.y > minGroundNormalY) {
                    isGrounded = true;
                    if (yMovement) {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                // Project is used to ensure that if a player collides with a ceiling that their horizontal movement is preserved.
                float projection = Vector2.Dot(velocity, currentNormal);
                if (projection < 0) {
                    velocity -= projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                // If the modified distance is less than the distance use it, otherwise use distance
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }

        rbObject.position += move.normalized * distance;
    }
}
