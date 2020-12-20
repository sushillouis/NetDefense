﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarchartManager : MonoBehaviour {

    public RectTransform container;
    public Color barColor;
    public Vector2 barSize;
    public float spacing;
    public Vector2 padding;

    public int barCount;

    private List<GameObject> bars;

    void Start() {
        bars = new List<GameObject>();

        for (int i = 0; i < 10; i++) {
            appendBar(i + 1);
        }

        deleteBar(5);
        deleteBar(6);
    }

    // Update is called once per frame
    void Update() {

    }

    public void appendBar(float height) {
        NewBar(barCount, 0, height);
    }

    public void deleteBar(int index) {
        Destroy(bars[index]);
    }


    private GameObject NewBar(int x, int y, float value) {
        x++;
        // Vector2 position = new Vector2(x * barSize.x + x * spacing, y * barSize.y);
        GameObject bar = new GameObject("bar");
     //   bar.AddComponent<GeneralDelegateStore>().delegates.Add((vec, t, X, Y, Padding, Spacing, BarSize) => {
     //       Vector2 position = new Vector2((X * BarSize.x) + X * Spacing, Y * BarSize.y);
     //       vec = new Vector2(position.x + Padding.x, position.y + Padding.y + t.sizeDelta.y / 2f);
     //   });

          
        //updatePosition action = 

        bars.Add(bar);
        bar.AddComponent<Image>();
        bar.GetComponent<Image>().color = barColor;
        bar.transform.SetParent(container, false);
        RectTransform transform = bar.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(barSize.x, barSize.y * value);
        //transform.anchoredPosition = new Vector2(position.x + padding.x, position.y + padding.y + transform.sizeDelta.y/2f);
       // action.Invoke(transform.anchoredPosition, transform, x, y, padding, spacing, barSize);
        transform.anchorMin = Vector2.zero;
        transform.anchorMax = Vector2.zero;
        barCount++;

        return bar;
    }
}
