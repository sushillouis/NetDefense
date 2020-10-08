using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RouterManager : MonoBehaviour {
    public Router selected;
    public Router Selected {
        get {
            return selected;
        }
        set {
            if (selected != null)
                selected.hud.hide();
            value.hud.show();


            selected = value;
        }
    }
    public GameObject settings;
    public GameObject sell;

    public float[] indicatorColumns;

    void Start() {
        // settings.SetActive(false);
        // sell.SetActive(false);
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