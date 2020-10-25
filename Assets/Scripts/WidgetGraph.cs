using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WidgetGraph : MonoBehaviour {

    public Sprite dataPointSprite;
    public RectTransform graph_container;
    public float max_y;
    public Color plotColor;

    public List<int> dataset;

    public List<GameObject> datapoints;
    public List<GameObject> lines;

    public int maxCount;

    void Start() {
        dataset = new List<int>();
        datapoints = new List<GameObject>();
        lines = new List<GameObject>();

        for (int i = 0; i < maxCount; i++) {
            dataset.Add(0);
            GameObject dp = new GameObject("datapoint-" + i, typeof(Image));
            GameObject line = new GameObject("line-" + i, typeof(Image));
            dp.transform.SetParent(transform);
            line.transform.SetParent(transform);
            datapoints.Add(dp);
            lines.Add(line);
        }
        SetDataset(dataset);
    }


    public void UpdateDataSet(int datum) {

        dataset.Add(datum);
        dataset.RemoveAt(0);
        SetDataset(dataset);

    }

    public void Update() {
        try {
            // clean up stray children
            foreach (Transform child in transform) {
                if (child.gameObject.name.StartsWith("line")) {
                    child.gameObject.SetActive(false);
                    return;
                }
            }
        } catch {

        }
    }

    private GameObject AddDataPoint(Vector2 anchoredPos, int index) {
        GameObject dataPoint = datapoints[index];

        RectTransform rt = dataPoint.GetComponent<RectTransform>();

        dataPoint.transform.SetParent(graph_container, false);
        dataPoint.GetComponent<Image>().sprite = dataPointSprite;

        rt.anchoredPosition = anchoredPos;
        rt.anchorMax = new Vector2(0, 0);
        rt.anchorMin = new Vector2(0, 0);
        rt.sizeDelta = new Vector2(10, 10);

        return dataPoint;
    }

    private GameObject AddLine(Vector2 a, Vector2 b, int index) {


        GameObject line = lines[index];

        RectTransform rt = line.GetComponent<RectTransform>();

        line.transform.SetParent(graph_container, false);
        line.GetComponent<Image>().color = plotColor;

        Vector2 delta = (b - a).normalized;
        float dist = Vector2.Distance(a, b);

        rt.anchoredPosition = a + delta * dist * .5f;
        rt.localEulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(delta.y, delta.x));

        rt.anchorMax = new Vector2(0, 0);
        rt.anchorMin = new Vector2(0, 0);
        rt.sizeDelta = new Vector2(dist, 2);

        return line;
    }

    public void SetDataset(List<int> dataset) {
        float graph_width = graph_container.sizeDelta.x;
        float graph_height = graph_container.sizeDelta.y;
        float xSize = graph_width / (dataset.Count - 1);

        GameObject last_datapoint = null;

        for (int i = 0; i < dataset.Count; i++) {
            float x = i * xSize;
            float y = (dataset[i] / max_y) * graph_height;

            GameObject datapoint = AddDataPoint(new Vector2(x, y), i);
            //toDestory.Add(datapoint);

            if (last_datapoint != null) {
                //toDestory.Add(AddLine(last_datapoint.GetComponent<RectTransform>().anchoredPosition, datapoint.GetComponent<RectTransform>().anchoredPosition, i));
                AddLine(last_datapoint.GetComponent<RectTransform>().anchoredPosition, datapoint.GetComponent<RectTransform>().anchoredPosition, i);
            }

            last_datapoint = datapoint;
        }
    }
}
