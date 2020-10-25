using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RouterManager : MonoBehaviour {


    public Router selected;

    public Text routerNameText;

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
        selected.SetColor(color);
    }

    public void SetShape(int shape) {
        selected.SetShape(shape);
    }

    public void SetSize(int size) {
        selected.SetSize(size);
    }

    public void Sell() {
        Destroy(selected.gameObject);
        Selected = null;
        //ActivateUI(false);
        Shared.inst.gameMetrics.whitehat_cash += 25;
        Debug.Log("Sold for hardcoded value");
        WhiteHatMenu.inst.OnCashChanged();
    }
}