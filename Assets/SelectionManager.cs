using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager inst;
    public void Awake() { inst = this; }

    // Variable storing the results of raycasts
    RaycastHit hit;

    public SimpleEnemyController selected;
    // Update is called once per frame
    void Update()
    {
        // If the left mouse button is pressed
        if (Input.GetMouseButton(0))
        {
            Camera currentCamera = Camera.main;
            if (currentCamera is object)
            {
                Debug.Log("Mouse Down");
                if (Physics.Raycast(currentCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, /*Everything*/ -1, QueryTriggerInteraction.Collide))
                {
                    Debug.Log(hit.transform);
                    // Make sure that the clicked object is a packet
                    SimpleEnemyController packet = hit.transform.gameObject.GetComponent<SimpleEnemyController>();
                    if (packet is object)
                    {
                        // Save the selected packet
                        selected = packet;

                        // Make sure no other packet has its UI displayed
                        //var packets = FindObjectsOfType<SimpleEnemyController>();
                        //foreach (var p in packets)
                            //p.dynamicHud.SetActive(false);

                        // Display the UI of the selected packet
                        //packet.dynamicHud.SetActive(true);
                        // Ensure that the UI is facing towards the camera (prevents snapping)
                        //packet.dynamicHud.transform.LookAt(currentCamera.transform);
                    }
                    else
                        selected = null; 
                }
            }
        }
    }
}
