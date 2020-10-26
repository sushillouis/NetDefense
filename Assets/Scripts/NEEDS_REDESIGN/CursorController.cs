using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script on each piece of grid to control the location of the cursor
public class CursorController : MonoBehaviour {
    //Tile object marking mouse location
    public GameObject cursor;
    public GameObject pathCursor;
    public GameObject gridCursor;
    public GameObject routerCursor;
    public GameObject router;
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