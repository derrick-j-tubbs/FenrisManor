using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStairController : MonoBehaviour
{
    private enum STAIR_STATE {
        on_stair,
        near_stair,
        off_stair
    }
    
    // Transform location for actor to start climbing from
    // public Transform startClimb;
    // public STAIR_STATE playerStairState;

   public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Stairs")
            return;

    }
}
