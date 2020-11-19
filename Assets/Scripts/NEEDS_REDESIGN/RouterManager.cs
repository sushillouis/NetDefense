using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RouterManager : MonoBehaviour {


    public Router selected;

    public Text routerNameText;
    public Text updatesRemainingText;
    public GameObject routerHUD;

    public Toggle[] ColorSelections;
    public Toggle[] ShapeSelections;
    public Toggle[] SizeSelections;

    public Router Selected {
        get {
            return selected;
        }
        set {
            if (selected != null) {
                // selected.hud.hide();
                selected.selectedRing.SetActive(false);
                routerNameText.text = value.name;
                updatesRemainingText.text = "Updates " + value.updatesRemaining;
                OnRouterSelected();
            }
            //value.hud.show();
            value.selectedRing.SetActive(true);


            selected = value;

            settings.SetActive(value != null);
        }
    }

    public GameObject settings;

    public GameObject sell;

    public float[] indicatorColumns;

    public void OnRouterSelected() {
        updatesRemainingText.text = "Updates " + selected.updatesRemaining;
        routerNameText.text = selected.name;

        int color = selected.color;
        int shape = selected.shape;
        int size = selected.size;

        ColorSelections[color].isOn = true;
        ShapeSelections[shape].isOn = true;
        SizeSelections[size].isOn = true;
    }

    void Start() {
        settings.SetActive(false);
    }

    public void SetColor(int color) {
        if (selected.updatesRemaining > 0) {
            selected.SetColor(color);
            updatesRemainingText.text = "Updates " + selected.updatesRemaining;
        }
    }

    public void SetShape(int shape) {
        if (selected.updatesRemaining > 0) {
            selected.SetShape(shape);
            updatesRemainingText.text = "Updates " + selected.updatesRemaining;
        }
    }

    public void SetSize(int size) {
        if (selected.updatesRemaining > 0) {
            selected.SetSize(size);
            updatesRemainingText.text = "Updates " + selected.updatesRemaining;
        }
    }

    public void Sell() {
        Destroy(selected.gameObject);
        Selected = null;
        routerHUD.SetActive(false);
        Shared.inst.gameMetrics.whitehat_cash += 0;
        WhiteHatMenu.inst.OnCashChanged();
    }
}