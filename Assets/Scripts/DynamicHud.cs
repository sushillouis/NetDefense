using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicHud : MonoBehaviour {

    public GameObject dynamicHud;
    public GameObject hud;


    private void UpdateDynamicHud() {
        if (dynamicHud == null) {
            dynamicHud = GameObject.FindGameObjectWithTag("DYNAMIC_INFO_HUD");
            hud = Instantiate(hud);
            hud.transform.SetParent(dynamicHud.transform);
        }

        RectTransform rt = dynamicHud.GetComponent<RectTransform>();

        Vector2 screen_space = Camera.main.WorldToScreenPoint(transform.position);

        float scaleFactor = MainCanvasManager.inst.scaleFactor();

        Vector2 scaled = new Vector2(screen_space.x / scaleFactor, screen_space.y / scaleFactor);

        Vector2 finalTransform = new Vector2(scaled.x - rt.rect.width / 2, scaled.y - rt.rect.height / 2);

        hud.GetComponent<RectTransform>().localPosition = finalTransform;


    }

    public void show() {
        hud.SetActive(true);
    }

    public void hide() {
        hud.SetActive(false);
    }


    // Update is called once per frame
    void Update() {
        UpdateDynamicHud();
    }
}
