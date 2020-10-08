using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphProperties : MonoBehaviour {
    [SerializeField]
    private bool isDebug;

    public GameObject dataPoint;
    public int maxPoints;

    public LineRenderer line;

    private List<Vector3> dataPoints;
    private float startTime;
    private float currentMax;

    public float currentData;
    private int index;
    public float pointPadding;

    public Vector3 offset;
    public float distnaceFromCamea = 10;
    public float cameraOffsetX;

    public float data_scale;

    void Start() {
        dataPoints = new List<Vector3>();
        startTime = Time.time;
        index = 0;
    }

    void Update() {
        if (isDebug)
            currentData = (Mathf.Sin(index / 5f) * 50);
        else
            currentData = ScoreManager.inst.black_hat_score_total_for_end_game;

        float elapsed = Time.time - startTime;
        if (elapsed > 0.09f) {
            addPoint();
            moveCamera();
            updateLine();
            updateMax();
            index++;
            startTime = Time.time;
        }
    }

    private void updateLine() {

        line.positionCount = dataPoints.Count;
        for (int i = 0; i < dataPoints.Count; i++) {
            line.SetPosition(i, dataPoints[i] + transform.position + offset);
        }
    }

    private void moveCamera() {
        if (dataPoints.Count == 0)
            return;

        Camera camera = transform.GetChild(0).gameObject.GetComponent<Camera>();
        Vector3 head = dataPoints[dataPoints.Count - 1] + offset;

        camera.transform.position = new Vector3(head.x + cameraOffsetX, offset.y + (ScoreManager.inst.scoreFactor * data_scale) / 2f, head.z - distnaceFromCamea);
    }

    private void addPoint() {
        if (dataPoints.Count < maxPoints + 1) {
            dataPoints.Add(new Vector3(transform.position.x + (index * pointPadding), currentData * data_scale, transform.position.z));
        } else {
            dataPoints.Clear();
        }

    }

    private void updateMax() {
        foreach (Vector3 datum in dataPoints) {
            currentMax = Mathf.Max(currentMax, datum.y);
        }

        if (currentMax == 0)
            currentMax = 1;
    }
}
