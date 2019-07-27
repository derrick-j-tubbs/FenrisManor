﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairController : MonoBehaviour
{
    public enum STAIR_DIRECTION {
        Up,
        Down,
        none
    };

    private int numSteps = 0;

    protected STAIR_DIRECTION stairDirection;
    private EdgeCollider2D edgeCollider;
    private List<Vector2> verticies = new List<Vector2>();

    public GameObject leftEndStep;
    public GameObject rightEndStep;
    
    public float timer;

    void Start() {
        //Debug.Log(this.name + " numSteps: " + numSteps);
       // Debug.Log(this.name + " StairDirection: " + stairDirection);
    }

    void Awake() {
        
        edgeCollider = GetComponent<EdgeCollider2D>();
        numSteps = CalculateNumSteps();
        stairDirection = DetermineStairDirection();
        if (stairDirection == STAIR_DIRECTION.Up) {
            // create the bottom of the stair step for the line y = x
            for (int x = 0; x <= ((numSteps - 1) / 2); x++) {
                verticies.Add( new Vector2(x, x));
                verticies.Add( new Vector2(x, x + 0.5f));
                verticies.Add( new Vector2(x + 0.5f, x + 0.5f));
                verticies.Add( new Vector2(x + 0.5f, x + 1f));
                if (x == ((numSteps - 1) / 2)) {
                    verticies.Add( new Vector2(x + 1f, x + 1f));
                    verticies.Add( new Vector2(x + 1f, x + 1.5f));
                }
            }
            // create the top half of the stair step for the line y = x
            for (int x = ((numSteps - 1) / 2); x >= 0; x--) {
                verticies.Add( new Vector2(x + 0.5f, x + 1.5f));
                verticies.Add( new Vector2(x + 0.5f, x + 1f));
                verticies.Add( new Vector2(x, x + 1f));
                verticies.Add( new Vector2(x, x + 0.5f));
                if (x == 0) {
                    verticies.Add(new Vector2(0,0));
                }
            }
        } if (stairDirection == STAIR_DIRECTION.Down) {
            // same as above for y = -x
            for (int x = 0; x <= ((numSteps - 1) / 2); x++) {
                verticies.Add( new Vector2(-x, x));
                verticies.Add( new Vector2(-x, x + 0.5f));
                verticies.Add( new Vector2(-x - 0.5f, x + 0.5f));
                verticies.Add( new Vector2(-x - 0.5f, x + 1f));
                if (x == ((numSteps - 1) / 2)) {
                    verticies.Add( new Vector2(-x - 1f, x + 1f));
                    verticies.Add( new Vector2(-x - 1f, x + 1.5f));
                }
            }
            for (int x = ((numSteps - 1) / 2); x >= 0; x--) {
                verticies.Add( new Vector2(-x - 0.5f, x + 1.5f));
                verticies.Add( new Vector2(-x - 0.5f, x + 1f));
                verticies.Add( new Vector2(-x, x + 1f));
                verticies.Add( new Vector2(-x, x + 0.5f));
                if (x == 0) {
                    verticies.Add( new Vector2(0, 0));
                }
            }
        }
        SetPoints();
    }

    void SetPoints() {
        edgeCollider.points = verticies.ToArray();
    }

    public STAIR_DIRECTION getStairDirection() {
        return stairDirection;
    }

    public int getNumSteps() {
        return numSteps;
    }

    int CalculateNumSteps() {
        return (int)(rightEndStep.transform.position.x - leftEndStep.transform.position.x) * 2;
    }

    STAIR_DIRECTION DetermineStairDirection() {
        if (leftEndStep.transform.position.y < rightEndStep.transform.position.y)
        {
            return STAIR_DIRECTION.Up; 
        } else if (leftEndStep.transform.position.y > rightEndStep.transform.position.y)
        {
            return STAIR_DIRECTION.Down;
        }
        Debug.Log("Stairs in impossible state, y postion of both ends is equal. GameObject: " + this.name);
        return STAIR_DIRECTION.none;
    }
}