using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    /*
    *  This will be the overarching PlayerController class, this will hold values used by all the other classes and be the only one with values expsed in the 
    *  editor. Determining how to organize these so that they are meaningful and grouping them so that they can be expanded and collapsed when in use will be 
    *  important. Initially will hold things like 'Player Max Speed' and 'Player Jump Force" but will also contains state_based things for use with both 
    *  animation and logic. This could be enumerators like Stair_State or values such as whip level / sub weapon level.
    *  
    *  There is a consideration for the possibility of renaming this class simply to the Player class, as it will contain the values for all the scripts and
    *  states of the player throughout the course of gameplay and provides no controls. This change is minor and only in name and not super relevant in the
    *  long run, mostly this is just for the purpose of maintaining good object oriented practices, which isn't super important.
    */

    public GameObject Player;
    public Animator PlayerAnimator;
    
    public enum STAIR_STATE {
        on_stair,
        off_stair
    }

    public enum PLAYER_FACING {
        left,
        right
    }

    // create values for all the previous enums
    protected STAIR_STATE playerStairState;
    protected PLAYER_FACING playerFacing;

    // create boolean values to allow other classes to change thes tate of the player
    // considerations are being made as to whether to make this an enum like the two above
    // this would give us more control, howevver players can exist in two states at once for many of the states on this list
    // likely it is better for each of them to remain as individual boolean variables
    protected bool isClimbing;

    void Awake(){
        Player = GameObject.Find("Player");
        PlayerAnimator = Player.GetComponent<Animator>();
        playerStairState = STAIR_STATE.off_stair;
        playerFacing = PLAYER_FACING.right;
        isClimbing = false;
    }


    // create public getters and setters for anything that another class will need access to
    public STAIR_STATE getPlayerStairState(){
        return playerStairState;
    }

    public void setPlayerStairState(STAIR_STATE stairState) {
        playerStairState = stairState;
    }

    public PLAYER_FACING getPlayerFacing(){
        return playerFacing;
    }

    public void setPlayerFacing(PLAYER_FACING facing) {
        playerFacing = facing;
    }

    public bool getIsClimbing(){
        return isClimbing;
    }
    public void setIsClimbing(bool climbing){
        isClimbing = climbing;
    }
}
