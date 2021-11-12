using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager inst;
    public void Awake() { inst = this; }

    public Text color;
    public Text shape;
    public Text size;

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

                        packet.selectedChild.SetActive(true);

                        Debug.Log(packet.color);
                        Debug.Log(packet.size);
                        Debug.Log(packet.shape);

                        //Color Labels 
                        if (packet.color == 0)
                        {
                            color.text = packet.color.ToString("PINK");
                        }
                        else if (packet.color == 1)
                        {
                            color.text = packet.color.ToString("GREEN");
                        }
                        else if (packet.color == 2)
                        {
                            color.text = packet.color.ToString("BLUE");
                        }

                        //Size Labels
                        if (packet.size == 0)
                        {
                            size.text = packet.size.ToString("SMALL");
                        }
                        else if (packet.size == 1)
                        {
                            size.text = packet.size.ToString("MEDIUM");
                        }
                        else if (packet.size == 2)
                        {
                            size.text = packet.size.ToString("LARGE");
                        }

                        //Shape Labels 
                        if (packet.shape == 0)
                        {
                            shape.text = packet.shape.ToString("CUBE");
                        }
                        else if (packet.shape == 1)
                        {
                            shape.text = packet.shape.ToString("CONE");
                        }
                        else if (packet.shape == 2)
                        {
                            shape.text = packet.shape.ToString("SPHERE");
                        }

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
                    Debug.Log(selected = null);
                }
            }
        }
    }
}
