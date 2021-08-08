
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Path : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    public Destination destination;
    public bool IsHoneypot
    {
        get
        {
            return destination.isHoneypot;
        }
    }

    void Awake()
    {
        waypoints.Add(transform);
        for(int i = 0; i < transform.childCount; i++)
        {
            waypoints.Add(transform.GetChild(i));
        }
    }
}

/*
public class Spawner : MonoBehaviour// : NetworkBehaviour
{

    public bool isMultiplayer = false;

    public GameObject enemy;
    public WaveController waveController;
    [HideInInspector] public List<Transform> spawnerWaypoints = new List<Transform>();
    [HideInInspector] public List<Transform> honeypotWaypoints = new List<Transform>();

    void notStart()
    {
        spawnerWaypoints.Add(transform);
        for(int i = 0; i < transform.childCount - 1; i++)
        {
            spawnerWaypoints.Add(transform.GetChild(i));
        }

        Transform honeypotWaypointHolder = transform.GetChild(transform.childCount - 1);
        honeypotWaypoints.Add(transform);
        for(int i = 0; i < honeypotWaypointHolder.childCount; i++)
        {
            honeypotWaypoints.Add(honeypotWaypointHolder.GetChild(i));
        }
    }

    [Command]
    void CmdSpawnEnemy()
    {
        NetworkServer.Spawn(configSpawningEnemy());
        
    }

    private GameObject configSpawningEnemy()
    {

        GameObject newEnemy = Instantiate(enemy, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
        newEnemy.GetComponent<SimpleEnemyController>().waveController = waveController;
        newEnemy.GetComponent<SimpleEnemyController>().spawner = this;
        if (waveController.malNeeded)
        {
            //waveController.MakeMalicious(newEnemy.GetComponent<SimpleEnemyController>());
            waveController.malSpawnCount++;
            waveController.malNeeded = false;
        }
        else
        {
            //MakeRandom(newEnemy.GetComponent<SimpleEnemyController>());
        }

        return newEnemy;
    }

    public void Spawn()
    {
        
        

        if (isMultiplayer) {
 //           if (!isLocalPlayer)
 //               return;
            CmdSpawnEnemy();
        } else {
            configSpawningEnemy();
        }
    }
/*
    private void MakeRandom(SimpleEnemyController enemy)
    {
        do
        {
            enemy.color = Random.Range(0, waveController.numberOfColors);
            enemy.size = Random.Range(0, waveController.numberOfSizes);
            enemy.shape = Random.Range(0, waveController.numberOfShapes);
        } while (waveController.CheckMalicious(enemy));
        enemy.malicious = false;
        enemy.UpdateTraits();
    }
    */
    /*
}
*/