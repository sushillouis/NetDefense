using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PacketInfoDisplay : MonoBehaviour
{
    public static PacketInfoDisplay inst;

    public SimpleEnemyController packet;

    string[] colors = new string[3] { "PINK", "GREEN", "BLUE" };
    string[] sizes = new string[3]{"SMALL","MEDIUM","LARGE"};
    string[] shapes = new string[3]{"CUBE","CONE","SPHERE"};

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public Text color;
    public Text shape;
    public Text size;

    // Update is called once per frame
    void Update()
    {

        if (SelectionManager.inst.selected is object)
        {
            packet = SelectionManager.inst.selected;
            Debug.Log(packet.color);
            Debug.Log(packet.shape);
            Debug.Log(packet.size);

            //Color Labels 
            color.text = colors[packet.color].ToString();

            //Size Labels
            size.text = sizes[packet.size];

            //Shape Labels 
            shape.text = shapes[packet.shape];
        }
    }
}