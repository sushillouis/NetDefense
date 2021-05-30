using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Script on each piece of grid to control the location of the cursor
public class CursorController : MonoBehaviour {
    //Tile object marking mouse location
    public GameObject cursor;
    public GameObject pathCursor;
    public GameObject gridCursor;
    public GameObject routerCursor;
    public GameObject router;
	public Text buildButtonText;
    public Material highlightedRouter;
    public Material nonHighlightedRouter;
    public bool building = false;
    private RouterManager routerManager;

    public void SetDefaultCursor(bool onPath) {
        if (!building) {
            if (onPath) {
                Destroy(cursor);
                cursor = Instantiate(pathCursor);
            } else {
                Destroy(cursor);
                cursor = Instantiate(gridCursor);
            }
        }
    }

    void Start() {
        //Finds cursor object in hierarchy
        cursor = Instantiate(gridCursor, new Vector3(0, -1, 0), Quaternion.Euler(-90, 0, 0));
        routerManager = GameObject.Find("RouterManager").GetComponent<RouterManager>();
    }

    public void ToggleBuilding() {

        Destroy(cursor);
        cursor = Instantiate(routerCursor);
        building = !building;

        routerManager.routerHUD.SetActive(building);
		buildButtonText.text = (building ? "close" : "get");
    }

	public void CloseRouterPannel() {
		ToggleBuilding(); // This is such a silly solution, but it also works 100% of the time and has 0 code reuse ;(
		if(building) ToggleBuilding();
	}

    public void ToggleHighlighted(bool highlighted) {
        if (highlighted) {
            Material[] materials = new Material[2] { highlightedRouter, highlightedRouter };
            cursor.GetComponent<Renderer>().materials = materials;
            cursor.GetComponent<Renderer>().materials = materials;
        } else {
            Material[] materials = new Material[2] { nonHighlightedRouter, nonHighlightedRouter };
            cursor.GetComponent<Renderer>().materials = materials;
            cursor.GetComponent<Renderer>().materials = materials;
        }
    }
}
