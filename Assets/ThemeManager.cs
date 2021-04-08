using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{

    public GameObject[] themes;
    public Material[] skyboxes;

    public GameObject themeUI;

    public Dropdown themeDropdown;

    public Button updateButton;

    public static int index;

    // Start is called before the first frame update
    void Start()
    {
        
        for (int i = 0; i < themes.Length; i++) {
            themes[i].SetActive(false);
        }
        themes[index].SetActive(true);
        RenderSettings.skybox = skyboxes[index];

        updateButton.onClick.AddListener(() => { 
            for(int i = 0; i < themes.Length; i++) {
                themes[i].SetActive(false);
            }

            themes[index = themeDropdown.value].SetActive(true);
            themeUI.SetActive(false);
            RenderSettings.skybox = skyboxes[index];
        });   
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.F1)) {
            themeUI.SetActive(!themeUI.activeSelf);
        }
    }
}
