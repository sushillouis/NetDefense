using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPacketInfo : MonoBehaviour
{
    public static DisplayPacketInfo inst;

    public GameObject SimpleEnemyController; 

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
        //Color Labels 

        if (SimpleEnemyController.GetComponent<SimpleEnemyController>().color == 0)
        {
            color.text = SimpleEnemyController.GetComponent<SimpleEnemyController>().color.ToString("Pink");
        }
        if (SimpleEnemyController.GetComponent<SimpleEnemyController>().color == 1)
        {
            color.text = SimpleEnemyController.GetComponent<SimpleEnemyController>().color.ToString("Green");
        }
        if (SimpleEnemyController.GetComponent<SimpleEnemyController>().color == 2)
        {
            color.text = SimpleEnemyController.GetComponent<SimpleEnemyController>().color.ToString("blue");
        }

        //Size Labels
        if (SimpleEnemyController.GetComponent<SimpleEnemyController>().size == 0)
        {
            size.text = SimpleEnemyController.GetComponent<SimpleEnemyController>().size.ToString("S");
        }
        if (SimpleEnemyController.GetComponent<SimpleEnemyController>().size == 1)
        {
            size.text = SimpleEnemyController.GetComponent<SimpleEnemyController>().size.ToString("M");
        }
        if (SimpleEnemyController.GetComponent<SimpleEnemyController>().size == 2)
        {
            size.text = SimpleEnemyController.GetComponent<SimpleEnemyController>().size.ToString("L");
        }

        //Shape Labels 
        if (SimpleEnemyController.GetComponent<SimpleEnemyController>().shape == 0)
        {
            shape.text = SimpleEnemyController.GetComponent<SimpleEnemyController>().shape.ToString("Cube");
        }
        if (SimpleEnemyController.GetComponent<SimpleEnemyController>().shape == 1)
        {
            shape.text = SimpleEnemyController.GetComponent<SimpleEnemyController>().shape.ToString("Cone");
        }
        if (SimpleEnemyController.GetComponent<SimpleEnemyController>().shape == 2)
        {
            shape.text = SimpleEnemyController.GetComponent<SimpleEnemyController>().shape.ToString("Sphere");
        }
        
    }
}
 