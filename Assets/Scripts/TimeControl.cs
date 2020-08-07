using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControl : MonoBehaviour
{
    public void Increase() {
        if (Time.timeScale <= 16) {
            Time.timeScale *= 2;
        }
    }

    public void Decrease() {
        if (Time.timeScale > 1) {
            Time.timeScale /= 2;
        }
    }
}
