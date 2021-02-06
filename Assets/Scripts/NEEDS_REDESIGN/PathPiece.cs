using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathPiece : MonoBehaviour
{
    private CursorController controller;
    private ScoreManager_OLD scoreManager;
    private bool occupied = false;
    private bool targeted = false;

    // Start is called before the first frame update
    void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name.EndsWith("Tutorial"))
            return;

        controller = GameObject.Find("CursorController").GetComponent<CursorController>();
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager_OLD>();
    }

    void OnMouseEnter()
    {
        //Activates if mouse is on grid
        targeted = true;
        controller.cursor.SetActive(true);
        controller.cursor.transform.position = transform.position;
        if(controller.cursor.name == "RouterCursor(Clone)")
        {
            controller.cursor.transform.rotation = transform.rotation;
            controller.ToggleHighlighted(true);
        }
        else
        {
            controller.cursor.transform.position += new Vector3(0, 0.05f, 0);
        }
    }

    void OnMouseExit()
    {
        //Deactivates if mouse is not on grid
        targeted = false;
        controller.cursor.SetActive(false);
        if(controller.cursor.name == "RouterCursor(Clone)")
        {
            controller.ToggleHighlighted(false);
        }
    }

    private void Update()
    {
        if(targeted && !occupied && controller.building && Input.GetMouseButtonDown(0))
        {
            GameObject newRouter = Instantiate(controller.router, transform.position, transform.rotation);
            // cost for selling a router is zero now
            newRouter.tag = "Router";
            ScoreManager.inst.OnWhiteHatEarnMoney(0);
            controller.ToggleBuilding();
            occupied = true;
        }
    }
}
