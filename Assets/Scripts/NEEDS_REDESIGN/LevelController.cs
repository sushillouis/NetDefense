using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public GameObject gameOverCanvas;
    public GameObject mainCanvas;

    private void Start()
    {
        gameOverCanvas.SetActive(false);
        mainCanvas.SetActive(false);
    }

    public void GameOver()
    {
        mainCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);
    }

    public void MainMenu()
    {
        Destroy(GameObject.Find("Music"));
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        SceneManager.LoadScene("Gameplay");
    }
}