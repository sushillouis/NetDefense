using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarchartManager : MonoBehaviour
{

    public RectTransform container;
    public Color barColor;
    public Vector2 barSize;
    public float spacing;
    public Vector2 padding;

    void Start()
    {


        for(int i=0; i < 10; i++) {
            NewBar(i, 0, i+1);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private GameObject NewBar(int x, int y, float value) {
        x++;
        Vector2 position = new Vector2(x * barSize.x + x * spacing, y * barSize.y);
        GameObject bar = new GameObject("bar");
        bar.AddComponent<Image>();
        bar.GetComponent<Image>().color = barColor;
        bar.transform.SetParent(container, false);
        RectTransform transform = bar.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(barSize.x, barSize.y * value);
        transform.anchoredPosition = new Vector2(position.x + padding.x, position.y + padding.y + transform.sizeDelta.y/2f);
        transform.anchorMin = Vector2.zero;
        transform.anchorMax = Vector2.zero;

        return bar;
    }
}
