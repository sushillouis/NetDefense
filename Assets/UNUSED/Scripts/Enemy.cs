using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    /*
    public int startingResources;
    private int resources;
    
    public int earnValue;
    public int spendValue;
    public float breakTime;

    public int startingEnemiesPerWave;
    public float enemyAcceleration;

    public int spawnRateMin, spawnRateMax;

    public Text resourcesText;

    public Text remainingEnemyText;
    public GameObject currentWaveText;
    public Text waveCountText;
    public Text timeToNextWaveText;
    public GameObject nextWaveText;
    public WaveController waveController;
    public LevelController levelController;

    private int waveCount = 0;

    private void Start()
    {
        resources = startingResources;
        resourcesText.text = "$" + resources.ToString() + ".00";
    }

    private IEnumerator wave()
    {
        int enemiesPerWave = (int)(startingEnemiesPerWave + (enemyAcceleration * waveCount));
        //waveController.GenerateMalValues();
        for (int i = 0; i < enemiesPerWave; i++)
        {
            if(resources >= spendValue)
            {
                //waveController.malNeeded = true;
                Spend();
                remainingEnemyText.text = (enemiesPerWave - i).ToString() + "/" + enemiesPerWave.ToString();
                yield return new WaitForSeconds(Random.Range(spawnRateMin, spawnRateMax));
            }
            else
            {
                Debug.Log("resources:" + resources + " spendValue: " + spendValue + " malSpawnCount: " + waveController.malSpawnCount);
                if(waveController.malSpawnCount == 0)
                {
                    levelController.GameOver();
                }
                i--;
                yield return null;
            }
        }
        waveCount++;
        StartCoroutine("waveBreak");
    }

    private IEnumerator waveBreak()
    {
        currentWaveText.SetActive(false);
        nextWaveText.SetActive(true);
        float startTime = Time.time;
        waveCountText.text = "Wave " + (waveCount + 1).ToString() + " in:";
        while (Time.time - startTime < breakTime)
        {
            timeToNextWaveText.text = ((int)(breakTime - (Time.time - startTime))).ToString();
            yield return null;
        }
        nextWaveText.SetActive(false);
        currentWaveText.SetActive(true);
        StartCoroutine("wave");
    }
    
    public void Earn()
    {
        resources += earnValue;
        resourcesText.text = "$" + resources.ToString() + ".00";
    }

    private void Spend()
    {
        resources -= spendValue;
        resourcesText.text = "$" + resources.ToString() + ".00";
    }
    */
}
