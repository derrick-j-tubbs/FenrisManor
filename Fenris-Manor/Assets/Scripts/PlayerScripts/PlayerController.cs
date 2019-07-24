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
    */

    public GameObject Player;
    public Animator PlayerAnimator;
    
    public enum STAIR_STATE {
        on_stair,
        near_stair,
        off_stair
    }

    void Awake(){
        Player = GameObject.Find("Player");
        PlayerAnimator = Player.GetComponent<Animator>();
    }
}
