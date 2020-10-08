using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RouterManager : MonoBehaviour {
    private Router selected;
    public Router Selected {
        get {
            return selected;
        }
        set {
            selected = value;
            if (!EventSystem.current.IsPointerOverGameObject()) {
                //ActivateUI(value != null);
            }
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

    public void ActivateUI(bool active) {
#if UNITY_ANDROID
        
        if(active) {
            settings.SetActive(active);
            sell.SetActive(active);
        }

#else
        settings.SetActive(active);
        sell.SetActive(active);
#endif
    }
}