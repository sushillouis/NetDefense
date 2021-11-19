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
    public GameObject hatSelectionPanel;
    public GameObject tutorialSelectionPanel;

    public GameObject[] environmentUnits;

    public float movementSpeed = 0;


    public Image easy;
    public Image medium;
    public Image hard;
    public Image play;
    public Image white;
    public Image black;
    public Image done;
    public Image tut1, tut2, tut3, startTut;

    public static int level = -1;
    public static int tutorial = -1;
    public static int hat = SharedPlayer.WHITEHAT;


    // Start is called before the first frame update
    void Start() {
        OnBackSelected();
        Debug.Log("HAT " + hat);
        Debug.Log("level " + level);
        if (GameObject.FindGameObjectsWithTag("Music").Length == 2) {
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
        MainMenu.level = level;

        LevelButtonSelectionColor(level);

        if (level == 0)
            difficulty = Difficulty.EASY;

        if (level == 1)
            difficulty = Difficulty.MEDIUM;

        if (level == 2)
            difficulty = Difficulty.HARD;

    }

    private void LevelButtonSelectionColor(int level) {
        play.color = new Color(play.color.r, play.color.g, play.color.b, 1);
        easy.color = level == 0 ? new Color(easy.color.r, easy.color.g, easy.color.b, 1) : new Color(easy.color.r, easy.color.g, easy.color.b, 0.5f);
        medium.color = level == 1 ? new Color(medium.color.r, medium.color.g, medium.color.b, 1) : new Color(medium.color.r, medium.color.g, medium.color.b, 0.5f);
        hard.color = level == 2 ? new Color(hard.color.r, hard.color.g, hard.color.b, 1) : new Color(hard.color.r, hard.color.g, hard.color.b, 0.5f);
    }

    public void OnHatSelected(int hat) {
        MainMenu.hat = hat;
        buttonColorForHatSelection(hat);

    }

    private void buttonColorForHatSelection(int hat) {
        done.color = new Color(done.color.r, done.color.g, done.color.b, 1);
        white.color = hat == 2 ? new Color(white.color.r, white.color.g, white.color.b, 1) : new Color(white.color.r, white.color.g, white.color.b, 0.5f);
        black.color = hat == 1 ? new Color(black.color.r, black.color.g, black.color.b, 1) : new Color(black.color.r, black.color.g, black.color.b, 0.5f);
    }


    public void StartGamePlay(int mode) {
        if (mode == 1 && hat == -1) {
            hat = 2;
        }

        if (mode == 0 && level == -1) {
            level = 0;
            hat = -1;
        }

        if (!isMultiplayerSelectedFromMenu && mode == 0) {
            levelSelectionPanel.GetComponent<Displaceable>().isValid = false;
            hatSelectionPanel.GetComponent<Displaceable>().isValid = true;
            return;
        }

        if (mode == 1)
            SceneManager.LoadScene("MultiStateGameplay");  // Loads Tron (easy) skin

        if (level == 1)
            SceneManager.LoadScene("MediumLevelBank"); // Loads Bank (medium) skin

        if (level == 2)
            SceneManager.LoadScene("MultiStateGameplay1"); // Loads PowerPlant (hard) skin
    }

    public void OnBackSelected() {
        level = -1;
        hat = -1;
        tutorial = -1;
        buttonColorForHatSelection(2);
        LevelButtonSelectionColor(0);
        OnTutorialSelected(0);
        levelSelectionPanel.GetComponent<Displaceable>().isValid = false;
        hatSelectionPanel.GetComponent<Displaceable>().isValid = false;
        tutorialSelectionPanel.GetComponent<Displaceable>().isValid = false;

    }

    public void OnQuitSelected() {
		Destroy(MusicController.inst.gameObject); // Destroy the music audio source so that in WebGL the music will stop
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnBriefingSelected() {
        SceneManager.LoadScene("BriefingMenu");
    }

    public void OnTutorialButtonSelected() {
        tutorialSelectionPanel.GetComponent<Displaceable>().isValid = true;
    }

    public void OnTutorialSelected(int option) {
        tutorial = option;
        startTut.color = new Color(startTut.color.r, startTut.color.g, startTut.color.b, 1);
        tut1.color = option == 0 ? new Color(tut1.color.r, tut1.color.g, tut1.color.b, 1) : new Color(tut1.color.r, tut1.color.g, tut1.color.b, 0.5f);
        tut2.color = option == 1 ? new Color(tut2.color.r, tut2.color.g, tut2.color.b, 1) : new Color(tut2.color.r, tut2.color.g, tut2.color.b, 0.5f);
        tut3.color = option == 2 ? new Color(tut3.color.r, tut3.color.g, tut3.color.b, 1) : new Color(tut3.color.r, tut3.color.g, tut3.color.b, 0.5f);

    }

    public void OnPlayTutorial() {
        if (tutorial == -1)
            return;


        if (tutorial == 0) {
            SceneManager.LoadScene("OverviewTutorial");

        }

        if (tutorial == 1) {
            hat = 1;
            SceneManager.LoadScene("BlackhatTutorial");

        }
        if (tutorial == 2) {
            SceneManager.LoadScene("WhitehatTutorial");

        }
    }
}
