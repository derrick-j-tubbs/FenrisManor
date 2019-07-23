using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairController : MonoBehaviour
{
    public enum STAIR_FACING {
        Left,
        Right
    };

    public enum TRIGGER_TYPE {
        Up,
        Down
    };

    private int _numSteps = 0;
    private float _stairSize = 0.5f;

    protected float stairHeight = 0.5f;
    protected float stairLength = 0.5f;

    public STAIR_FACING stairDirection;
    public GameObject endOfStairs;
    public TRIGGER_TYPE prepDirection;
    
}
