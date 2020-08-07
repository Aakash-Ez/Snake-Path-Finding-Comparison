using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBody : MonoBehaviour
{
    public GameObject Lead;

    Vector2 nextPos;
    Quaternion nextRot; 

    void FixedUpdate() {
        if (Snake.move) {
            Snake.occupied[(int)(-(transform.position.y - 3.5f) / .5f), (int)((transform.position.x + 8.5f) / .5f)] = false;
            this.transform.position = nextPos;
            this.transform.rotation = nextRot;
            Snake.occupied[(int)(-(transform.position.y - 3.5f) / .5f), (int)((transform.position.x + 8.5f) / .5f)] = true;

            nextPos = Lead.transform.position;
            nextRot = Lead.transform.rotation;
        }      
    }

    public void SetNextPos(Vector2 pos, Quaternion rot) {
        nextPos = pos;
        nextRot = rot;
    }
}
