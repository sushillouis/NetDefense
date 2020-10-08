using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvasManager : MonoBehaviour
{
    public static MainCanvasManager inst;

    private void Awake() {
        inst = this;
    }

    public float scaleFactor() {
        return GetComponent<Canvas>().scaleFactor;
    }
}
