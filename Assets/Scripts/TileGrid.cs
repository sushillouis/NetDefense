using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    private GameObject cursor;

    // Start is called before the first frame update
    void Start()
    {
        cursor = GameObject.Find("CursorController");
    }

    void OnMouseEnter()
    {
        //Activates if mouse is on grid
        cursor.GetComponent<CursorController>().cursor.SetActive(true);
        cursor.GetComponent<CursorController>().SetDefaultCursor(false);
    }

    void OnMouseExit()
    {
        //Deactivates if mouse is not on grid
        cursor.GetComponent<CursorController>().cursor.SetActive(false);
        cursor.GetComponent<CursorController>().SetDefaultCursor(true);
    }

    void OnMouseOver()
    {
        //Casting ray to get exact mouse position on grid
        RaycastHit hit;
        Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            //hit.point is exact position value of ray collision with grid
            Vector3 pos = hit.point;
            //Rounds hit.point to nearest odd number and moves cursor to that point
            cursor.GetComponent<CursorController>().cursor.transform.position = new Vector3(1 + 6 * (Mathf.Floor((pos.x + 2) / 6)), 0.01f, 1 + 6 * (Mathf.Floor((pos.z + 2) / 6)));
            cursor.GetComponent<CursorController>().cursor.transform.eulerAngles = new Vector3(-90, 0, 0);
        }
    }
}
