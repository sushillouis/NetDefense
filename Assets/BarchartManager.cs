using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BarchartManager : MonoBehaviour {

    public string name;

    public bool ShouldUpdate;
    public bool shouldUpdate {
        get {
            return ShouldUpdate;
        }
        set {
            ShouldUpdate = value;
            OnShouldUpdateBarChart();
        }
    }

    public RectTransform container;
    public Color barColor;
    public Vector2 barSize;
    public float spacing;
    public Vector2 padding;
    public float maxValue;

    public int fontSize;
    public Font labelFont;


    public List<GameObject> bars;
    public OnBarClickedDialogueManager OnBarClickedDialogueManager;

    public static List<BarchartManager> insts = new List<BarchartManager>();

    public static BarchartManager getBarManagerByName(string name) {
        foreach (BarchartManager bm in insts) {
            if (name.ToLower().Equals(bm.name.ToLower())) {
                return bm;
            }
        }

        return null;
    }

    private void Awake() {
        insts.Add(this);
    }

    void Start() {
        bars = new List<GameObject>();
    }

    public void OnShouldUpdateBarChart() {
        clearBars();

        if (this == getBarManagerByName("Chart1") && shouldUpdate)
            UpdateBarChart1();

        if (this == getBarManagerByName("Chart2") && shouldUpdate)
            UpdateBarChart2();

        normalizedValues();

    }

    private void UpdateBarChart2() {

        // plot the most recent n datapoints
        int n = 10;
        for (int i = Math.Max(ScoreManager.inst.badPacketsFilteredHistory.Count - n, 0); i < ScoreManager.inst.badPacketsFilteredHistory.Count; i++) {

            GameObject key = appendBar(ScoreManager.inst.badPacketsFilteredHistory[i].Key);
            float totalSpawned = key.GetComponent<FloatValueList>().values[0];
            key.GetComponent<StringValueList>().values.Add("Total Infected Packets Spawned");
            key.GetComponent<StringValueList>().values.Add("Spawned: " + totalSpawned);
            key.GetComponent<StringValueList>().values.Add("Time: " + i + "s");

            GameObject value = appendBar(ScoreManager.inst.badPacketsFilteredHistory[i].Value, Color.red);
            value.GetComponent<StringValueList>().values.Add("Total Infected Packets Successes");
            value.GetComponent<StringValueList>().values.Add("Successes: " + (int)(100f * (value.GetComponent<FloatValueList>().values[0] / totalSpawned)) + "%");
            value.GetComponent<StringValueList>().values.Add("Time: " + i + "s");

            appendBar(0, Color.clear).SetActive(false);
        }
    }

    private void UpdateBarChart1() {
        ScoreManager.inst.updateHistogram();

        for (int i = 0; i < ScoreManager.inst.histogram.Count; i++) {
            KeyValuePair<PacketProfile, int> dataPoint = ScoreManager.inst.histogram[i];

            appendBar(dataPoint.Value, dataPoint.Key.color + "" + dataPoint.Key.size + "" + dataPoint.Key.shape);
        }


    }

    public void clearBars() {
        while (bars.Count != 0) {
            Destroy(bars[0]);
            bars.RemoveAt(0);
        }
    }

    public GameObject appendBar(float height, string label = "") {
        return appendBar(height, barColor, label);
    }

    public GameObject appendBar(float height, Color barColor, string label = "") {
        return NewBar(bars.Count, 0, height, label, barColor);
    }

    public void deleteBar(int index) {
        Destroy(bars[index]);
        bars.RemoveAt(index);
    }

    public void closeGaps() {
        for (int i = 0; i < bars.Count; i++) {
            GameObject bar = bars[i];
            setBarTransform(i + 1, 0, bar.GetComponent<FloatValueList>().values[0], bar);
        }
    }

    public void normalizedValues() {
        float max = 0;
        foreach (GameObject bar in bars) {
            max = Mathf.Max(bar.GetComponent<FloatValueList>().values[0], max);
        }

        if (max == 0)
            max = 1;

        for (int i = 0; i < bars.Count; i++) {
            GameObject bar = bars[i];
            float normalizedValue = (bar.GetComponent<FloatValueList>().values[0] / max) * maxValue;
            bar.GetComponent<FloatValueList>().values[0] = normalizedValue;
            setBarTransform(i + 1, 0, normalizedValue, bar);
        }
    }

    public void attachLabel(GameObject bar, string label) {
        bar.GetComponent<StringValueList>().values[0] = label;
        GameObject textLabel = new GameObject("label");
        Text text = textLabel.AddComponent<Text>();
        text.text = label;
        text.font = labelFont;
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        textLabel.transform.SetParent(bar.transform, false);
        RectTransform transform = textLabel.GetComponent<RectTransform>();
        transform.anchorMin = Vector2.zero;
        transform.anchorMax = Vector2.zero;
        transform.Translate(barSize.x / 2, 0, 0);
        transform.sizeDelta = new Vector2(35, 35);
        textLabel.transform.rotation = Quaternion.Euler(0, 0, 45);
    }

    private GameObject NewBar(int x, int y, float value, string label, Color barColor) {
        x++;
        GameObject bar = new GameObject("bar");
        bar.transform.SetParent(container, false);
        bar.transform.SetAsFirstSibling();
        bar.AddComponent<FloatValueList>().values.Add(value);
        bar.GetComponent<FloatValueList>().values.Add(value);
        bar.AddComponent<StringValueList>().values.Add(label);
        bar.AddComponent<Image>();
        bar.GetComponent<Image>().color = barColor;

        Button button = bar.AddComponent<Button>();

        button.onClick.AddListener(delegate () { OnBarPressed(label, value, bar.GetComponent<FloatValueList>().values, bar.GetComponent<StringValueList>().values); });

        setBarTransform(x, y, value, bar);
        bars.Add(bar);

        attachLabel(bar, label);
        return bar;
    }

    public void OnBarPressed(string label, float value, List<float> floats, List<string> strings) {
        if (this == getBarManagerByName("Chart1"))
            OnBarChart1Pressed(label, value);

        if (this == getBarManagerByName("Chart2"))
            OnBarChart2Pressed(strings);
    }

    private void OnBarChart2Pressed(List<string> strings) {
        if (strings.Count == 4) {
            OnBarClickedDialogueManager.OnShowDialogue(strings[1], strings[2], strings[3]);

        }
    }

    private void OnBarChart1Pressed(string label, float value) {
        char color = label[0];
        char size = label[1];
        char shape = label[2];

        string name = (color == '0' ? "Pink" : color == '1' ? "Green" : "Blue") + " " + (size == '0' ? "Small" : size == '1' ? "Medium" : "Large") + " " + (shape == '0' ? "Cube" : shape == '1' ? "Cone" : "Sphere");

        OnBarClickedDialogueManager.OnShowDialogue(label, name, "Frequency " + (int)value);
    }

    private void setBarTransform(int x, int y, float value, GameObject bar) {
        Vector2 position = createBarPosition(x, y);
        anchorBar(value, position, bar);
    }

    private void anchorBar(float value, Vector2 position, GameObject bar) {
        value++;
        RectTransform transform = bar.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(barSize.x, barSize.y * value);
        transform.anchoredPosition = new Vector2(position.x + padding.x, position.y + padding.y + transform.sizeDelta.y / 2f);
        transform.anchorMin = Vector2.zero;
        transform.anchorMax = Vector2.zero;
    }

    private Vector2 createBarPosition(int x, int y) {
        return new Vector2(x * barSize.x + x * spacing, y * barSize.y);
    }
}
