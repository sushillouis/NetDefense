using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSelectionEffect : MonoBehaviour {

    public bool isOn;
    public float maxScale = 1.5f;
    public float period = 2;
    public Vector2 sizeDelta;

    public bool isTextOn;
    public Text text;
    public int fontSize;

    private void Start() {
        sizeDelta = GetComponent<RectTransform>().sizeDelta;
        fontSize = text.font.fontSize;
    }
    void Update() {
        if (isOn) {
            float scale = maxScale + (maxScale - 1) * Mathf.Sin(Mathf.PI * Time.time / period);

            GetComponent<RectTransform>().sizeDelta = sizeDelta * scale;


            if (isTextOn) {
                text.fontSize = (int)(fontSize * scale);
            }
        }

    }

    public void OnTurnOff() {
        isOn = false;
        GetComponent<RectTransform>().sizeDelta = sizeDelta;
        if (fontSize != 0)
            text.fontSize = fontSize;
    }
}
