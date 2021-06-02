﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Scipt exists on all instances of a router
public class Router : MonoBehaviour {

    public int cost = 0;                               //The cost to the player to place a router
    public int updatesRemaining = 2;
    public bool hasUpdated;
    private int initUpdates;
    public Material highlightedColor;                   //The material for the router beams when highlighted
    public Material nonHighlightedColor;                //The material for the router beams when not highlighted
    public Material[] gateMaterials;                    //Gate materials corresponding to all possible packet colors

    public int lastColor = -1;
    public int color {
        set {
            //Debug.Log("-------------------------");

            //Debug.Log(" updatesRemaining before: " + updatesRemaining);

            if (value != lastColor) {
                // updatesRemaining--;
            }
            //Debug.Log(" lastColor: " + lastColor + ", " + value);
            lastColor = value;
            //Debug.Log(" updatesRemaining after: " + updatesRemaining);
            //Debug.Log("-------------------------");


        }

        get { return lastColor == -1 ? 0 : lastColor; }
    }                 //The crurent color setting of the router
    public bool colorSet = false;     //True if the color setting has been assigned

    public int lastShape = -1;
    public int shape {
        set {
            if (value != lastShape) {
                //  updatesRemaining--;
            }
            lastShape = value;
        }

        get { return lastShape == -1 ? 0 : lastShape; }
    }                 //The current shape setting of the router
    public bool shapeSet = false;     //True if the shape setting has been assigned

    public int lastSize = -1;
    public int size {
        set {//The current size setting of the router
            if (value != lastSize) {
                //   updatesRemaining--;
            }
            lastSize = value;
        }

        get { return lastSize == -1 ? 0 : lastSize; }
    }


    public bool sizeSet = false;      //True if the size setting has been assigned

    private RouterManager manager;                      //Holds a reference to the router manager

    // public DynamicHud hud;

    public GameObject selectedRing;

    //Called each time the gate material needs to be changed
    private Material BeamMaterial {
        set {
            Material[] materials = GetComponent<Renderer>().materials;
            materials[0] = value;
            GetComponent<Renderer>().materials = materials;
        }
    }

    //True if router is currently selected
    private bool selected;
    //Updates highlighting and manager state when selection status is changed
    private bool Selected {
        get { return selected; }
        set {
            selected = value;
            if (selected) {
                manager.Selected = this;
            } else {
                BeamMaterial = nonHighlightedColor;
                if (manager.Selected == this)
                    manager.Selected = null;
            }
        }
    }

    //True if the mouse is currently over the router
    private bool mouseOver;
    //Sets highlighting when mouse enters or exits router
    private bool MouseOver {
        get {
            return mouseOver;
        }
        set {
            mouseOver = value;
            if (value) {
                BeamMaterial = highlightedColor;
            } else if (!Selected) {
                BeamMaterial = nonHighlightedColor;
            }
        }
    }

    void OnMouseEnter() {
        MouseOver = true;
    }

    void OnMouseExit() {
        MouseOver = false;
    }

    //Selects router on click if the player is not building
    void OnMouseDown() {
        if (!GameObject.Find("CursorController").GetComponent<CursorController>().building) {
            Selected = true;

        }
    }

    private static int count = 1;
    private void Start() {
        initUpdates = updatesRemaining;
        // hud = GetComponent<DynamicHud>();
        name = "Router " + count++;
        // hud.hide();
        selectedRing.SetActive(false);
        GameManager.inst.routersPlaceable--;
		// Play the router place sound (if the player is a whitehat)
		if(Shared.inst.getDevicePlayer().role == SharedPlayer.WHITEHAT) Camera.main.transform.GetChild(4).GetComponent<AudioSource>().Play();
        WhiteHatMenu.inst.OnRouterPlaced();

    }
    //Checks for click anywhere else to control deselection
    void Update() {
        if (Input.GetMouseButtonDown(0) && !MouseOver && !EventSystem.current.IsPointerOverGameObject()) {
            //Debug.Log("!MouseOver: " + !MouseOver + " !IsPointerOverGameObject(): " + !EventSystem.current.IsPointerOverGameObject());
            Selected = false;

        } else if (Input.GetMouseButtonDown(0)) {
            //Debug.Log("!MouseOver: " + !MouseOver + " !IsPointerOverGameObject(): " + !EventSystem.current.IsPointerOverGameObject());
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                //   Debug.Log("Raycasted Router Value: " + hit.transform.gameObject.name);
                if (hit.transform.gameObject.name.Equals(name)) {
                    Selected = true;
                }
            }
        }
    }

    //Assigns color setting and update gate material
    public void SetColor(int newColor) {
        if (newColor >= 0 && newColor < 3) {
            Material[] materials = GetComponent<Renderer>().materials;
            materials[1] = gateMaterials[newColor];
            GetComponent<Renderer>().materials = materials;
            color = newColor;
            colorSet = true;
            hasUpdated = true;
        }
    }

    //Assigns shape setting
    public void SetShape(int newShape) {
        if (newShape >= 0 && newShape < 3) {
            shape = newShape;
            shapeSet = true;
            hasUpdated = true;
        }
    }

    //Assigns size setting
    public void SetSize(int newSize) {
        if (newSize >= 0 && newSize < 3) {
            size = newSize;
            sizeSet = true;
            hasUpdated = true;
        }
    }

    //Called when router is placed
    void Awake() {
        manager = GameObject.Find("RouterManager").GetComponent<RouterManager>();
    }


    public bool HasUpdated() {
        return hasUpdated;
    }
}
