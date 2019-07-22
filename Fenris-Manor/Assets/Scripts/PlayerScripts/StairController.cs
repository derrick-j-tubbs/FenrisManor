using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairController : MonoBehaviour
{
    public enum STAIR_FACING {
        Left,
        Right
    }

    public STAIR_FACING stairDirection;
    public GameObject upTrigger;
    public GameObject downTrigger;
    public int numSteps;

    protected void playerClimbStairs() {
        
    }


}
