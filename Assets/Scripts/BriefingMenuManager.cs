using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BriefingMenuManager : MonoBehaviour
{
    // Start is called before the first frame update


    public AudioSource narrator;

    public float timer;

    void Start()
    {
        if (GameObject.FindGameObjectsWithTag("Music").Length == 2) {
            Destroy(GameObject.FindGameObjectsWithTag("Music")[0]);
        }

        timer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        

        if((Time.time - timer > 37) || Input.GetMouseButtonDown(0)) {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
