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

    public Button updateBtn;
    public GameObject DisablePanel;
    public int color;
    public int shape;
    public int size;

    public static RouterManager inst;

    private void Awake() {
        inst = this;
    }

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

        if(!selected.HasUpdated()) {
            for(int i = 0; i < 3; i++) {
                ColorSelections[i].isOn = ShapeSelections[i].isOn = SizeSelections[i].isOn = false;
            }
        }

        DisablePanel.SetActive(selected.updatesRemaining == 0);
        updateBtn.interactable = selected.updatesRemaining != 0;
    }

    void Start() {
        settings.SetActive(false);
    }

    public void SetColor(int color) {
        this.color = color;
    }

    public void SetShape(int shape) {
        this.shape = shape;
    }

    public void SetSize(int size) {
        this.size = size;
    }

    public void OnUpdateRouterSelected() {
        if (selected.color == color && selected.size == size && selected.shape == shape)
            return;

        if (selected.updatesRemaining > 0) {
            selected.SetSize(size);
            selected.SetShape(shape);
            selected.SetColor(color);
            selected.updatesRemaining--;
            updatesRemainingText.text = "Updates " + selected.updatesRemaining;
        }

        DisablePanel.SetActive(selected.updatesRemaining == 0);
        updateBtn.interactable = selected.updatesRemaining != 0;
    }

    public void Sell() {
        Destroy(selected.gameObject);
        Selected = null;
        routerHUD.SetActive(false);
        Shared.inst.gameMetrics.whitehat_cash += 0;
        WhiteHatMenu.inst.OnCashChanged();
    }
}
