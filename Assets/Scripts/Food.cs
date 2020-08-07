using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    void Start() {
        //RandomizePosition();
    }

    void RandomizePosition() {
        float x = Random.Range(-16, 17) * 0.5f;
        float y = Random.Range(-8, 7) * 0.5f;
        this.transform.position = new Vector2(x, y);
        if (Snake.occupied[(int) (-(transform.position.y - 3.5f) / .5f), (int) ((transform.position.x + 8.5f) / .5f)]) {
            RandomizePosition();
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        RandomizePosition();
    }
}
