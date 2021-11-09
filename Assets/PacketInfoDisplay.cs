using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PacketInfoDisplay : MonoBehaviour
{
    public static PacketInfoDisplay inst;

    public SimpleEnemyController packet;

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
        }
    }
}
