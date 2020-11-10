using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum Difficulty {
    EASY,
    HARD,
    MEDIUM
}


public class MainMenu : MonoBehaviour {

    public static bool isMultiplayerSelectedFromMenu = false;
    public static Difficulty difficulty;

    public GameObject levelSelectionPanel;

    public GameObject[] environmentUnits;

    public float movementSpeed = 0;


    public Image easy;
    public Image medium;
    public Image hard;
    public Image play;

    private int level = -1;


    // Start is called before the first frame update
    void Start() {
        if(GameObject.FindGameObjectsWithTag("Music").Length == 2) {
            Destroy(GameObject.FindGameObjectsWithTag("Music")[0]);
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            PlayGameSinglePlayer();
        }

        foreach (GameObject unit in environmentUnits) {
            unit.transform.position -= new Vector3(0, 0, Time.deltaTime * movementSpeed);
            if (unit.transform.position.z < -400) {
                unit.transform.position += new Vector3(0, 0, 1600);
            }
        }
    }

    public void PlayGameSinglePlayer() {
        isMultiplayerSelectedFromMenu = false;
        levelSelectionPanel.GetComponent<Displaceable>().isValid = (true);
    }

    public void PlayGameMultiplayer() {
        isMultiplayerSelectedFromMenu = true;
        levelSelectionPanel.GetComponent<Displaceable>().isValid = (true);

    }

    // level=0 means easy
    // level=1 means hard
    public void OnLevelButtonPressed(int level) {
        this.level = level;

        play.color = new Color(play.color.r, play.color.g, play.color.b, 1);
        easy.color = level == 0 ? new Color(easy.color.r, easy.color.g, easy.color.b, 1) : new Color(easy.color.r, easy.color.g, easy.color.b, 0.5f);
        medium.color = level == 1 ? new Color(medium.color.r, medium.color.g, medium.color.b, 1) : new Color(medium.color.r, medium.color.g, medium.color.b, 0.5f);
        hard.color = level == 2 ? new Color(hard.color.r, hard.color.g, hard.color.b, 1) : new Color(hard.color.r, hard.color.g, hard.color.b, 0.5f);

        if (level == 0)
            difficulty = Difficulty.EASY;

        if (level == 1)
            difficulty = Difficulty.MEDIUM;

        if (level == 2)
            difficulty = Difficulty.HARD;

    }

    public void StartGamePlay() {
        if (level == -1)
            return;
        SceneManager.LoadScene("MultiStateGameplay");
    }

    public void OnBackSelected() {
        levelSelectionPanel.GetComponent<Displaceable>().isValid = false;
    }

    public void OnQuitSelected() {
        Application.Quit();
    }
}
