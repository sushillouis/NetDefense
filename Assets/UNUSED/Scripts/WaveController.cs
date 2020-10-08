using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    //public static WaveController inst;
    /*
    public int enemiesPerWave;
    public int malTypes;
    public float spawnInterval;
    public float malChance;
    public float waitTime;

    public int numberOfColors;
    public int numberOfShapes;
    public int numberOfSizes;
    */
    //public Spawner leftSpawner;
    //public Spawner rightSpawner;
    //public Honeypot honeypot;

    //public LevelController levelController;

    //public MalValues[] malValues;
    //public float waveStartTime;
    //public bool honeypotAvailable;
    //public int malSpawnCount;

    //public bool malNeeded = false;

    /*
    void Awake()
    {
        inst = this;
    }

    public bool MalNeededRand
    {
        get
        {
            return ((.01f * Random.Range(0, 100)) < ((Time.time - waveStartTime) / (spawnInterval * enemiesPerWave) + .2f));
        }
    }

    public bool GetMalNeeded()
    {
        float val = Random.Range(0, 100);
        return val * .01 < malChance;
    }

    public struct MalValues
    {
        public int shape;
        public int color;
        public int size;
    };

    public void GenerateMalValues()
    {
        malValues = new MalValues[malTypes];
        for (int i = 0; i < malTypes; i++)
        {
            malValues[i] = new MalValues
            {
                shape = Random.Range(0, numberOfShapes),
                color = Random.Range(0, numberOfColors),
                size = Random.Range(0, numberOfSizes)
            };
        }
    }

    void Start()
    {
        GenerateMalValues();
        honeypotAvailable = false;
        //StartCoroutine("Wave");
    }

    private IEnumerator Wave()
    {
        yield return new WaitForSeconds(waitTime);
        malSpawnCount = 0;
        waveStartTime = Time.time;
        while(!Input.GetKey(KeyCode.LeftShift) || !Input.GetKey(KeyCode.Q))
        {
            int side = Random.Range(0, 2);
            if (side == 0)
            {
                //leftSpawner.Spawn();
                //rightSpawner.Spawn();
            }
            else
            {
                //rightSpawner.Spawn();
                //leftSpawner.Spawn();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
        levelController.GameOver();
    }
    */
}