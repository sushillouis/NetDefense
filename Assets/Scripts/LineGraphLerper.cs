using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGraphLerper : MonoBehaviour
{
    public LineRenderer lineRenderer;

    private int index;
    public float currentData;

    private float startTime;
    private float currentMax;

    void Start()
    {
        startTime = Time.time;
        index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currentData = Mathf.Sin(3.1415f * Time.time);
        float elapsed = Time.time - startTime;
        lineRenderer.SetPosition(index++, new Vector3(0, currentData + transform.position.y, 0));
    }
}
