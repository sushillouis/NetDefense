using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPacketInfo : MonoBehaviour
{
    public static DisplayPacketInfo inst;

    public SimpleEnemyController packet; 

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        packet = transform.parent.gameObject.GetComponent<SimpleEnemyController>();
    }

    public Text color;               
    public Text shape;
    public Text size;

    // Update is called once per frame
    void Update()
    {
        //Color Labels 

        if (packet.color == 0)
        {
            color.text = packet.color.ToString("Pink");
            color.color = Color.magenta;
        }
        if (packet.color == 1)
        {
            color.text = packet.color.ToString("Green");
            color.color = Color.green; 
        }
        if (packet.color == 2)
        {
            color.text = packet.color.ToString("Blue");
            color.color = Color.blue;
        }

        //Size Labels
        if (packet.size == 0)
        {
            size.text = packet.size.ToString("Small");
        }
        if (packet.size == 1)
        {
            size.text = packet.size.ToString("Medium");
        }
        if (packet.size == 2)
        {
            size.text = packet.size.ToString("Large");
        }

        //Shape Labels 
        if (packet.shape == 0)
        {
            shape.text = packet.shape.ToString("Cube");
        }
        if (packet.shape == 1)
        {
            shape.text = packet.shape.ToString("Cone");
        }
        if (packet.shape == 2)
        {
            shape.text = packet.shape.ToString("Sphere");
        }
        
    }
}
 