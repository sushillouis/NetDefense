using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPacketInfo : MonoBehaviour
{
    public static DisplayPacketInfo inst;
    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject HoverInfo = new GameObject("text");  
    }

    public Text color;               
    public Text shape;
    public Text size;


    // Update is called once per frame
    void Update()
    {
        //color.text = 
        //shape.text = SelectionManager.Instantiate.selected.name;
        //size.text = SelectionManager.Instantiate.selected.name;
    }
}
 