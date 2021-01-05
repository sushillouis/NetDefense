using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteHatNPC : MonoBehaviour {

    public static WhiteHatNPC inst;

    public GameObject Router;
    public PathPiece[] routerSpawns;
    public bool[] hasSpawnedAtIndex;

    public List<GameObject> routers;
    public List<int> activeIndices;

    public float startTime;
    public float optimizeRouterFilterRate; // in seconds

    public int unupgradableRouterCount;

    private void Awake() {
        inst = this;
        startTime = Time.time;
        hasSpawnedAtIndex = new bool[routerSpawns.Length];
    }

    public void OnUsingWhiteAtNPC() {
        if (routers.Count != 0) {
            return;
        }

        routers = new List<GameObject>();
        activeIndices = new List<int>();
        for (int i = 0; i < 3; i++) {
            int number = Random.Range(0, routerSpawns.Length);
            if (!activeIndices.Contains(number)) {
                activeIndices.Add(number);
            } else {
                i--;
            }
        }

        for (int i = 0; i < routerSpawns.Length; i++) {
            PathPiece routerSpawn = routerSpawns[i];
            if (activeIndices.Contains(i)) {
                routers.Add(Instantiate(Router, routerSpawn.transform.position, routerSpawn.transform.rotation));
                hasSpawnedAtIndex[i] = true;
            }
        }
    }

    public void OnOptimzeRouters() {
        Debug.Log("Called");
        foreach(GameObject router in routers) {
            Router r = router.GetComponent<Router>();
            if (r.updatesRemaining == 0) {
                routers.Remove(router);
                unupgradableRouterCount++;
                for(int i = 0; i < hasSpawnedAtIndex.Length; i++) {
                    if(!hasSpawnedAtIndex[i]) {
                        hasSpawnedAtIndex[i] = true;
                        routers.Add(Instantiate(Router, routerSpawns[i].transform.position, routerSpawns[i].transform.rotation));
                    }
                }
            } else {
                // we can update router settings

                if(r.color != Shared.inst.maliciousPacketProperties.color) {
                    // update the color to something different
                    int color = r.color;
                    while(color != r.color)
                        color = Random.Range(0, 3);
                    r.color = color;
                }

                if (r.shape != Shared.inst.maliciousPacketProperties.shape) {
                    // update the shape to something different
                    int shape = r.shape;
                    while (shape != r.shape)
                        shape = Random.Range(0, 3);
                    r.shape = shape;
                }

                if (r.size != Shared.inst.maliciousPacketProperties.size) {
                    // update the size to something different
                    int size = r.size;
                    while (size != r.size)
                        size = Random.Range(0, 3);
                    r.size = size;
                }
            }
        }
    }

    
    void Update() {
        if (Game_Manager.inst.isBetweenWaves)
            return;

        float elapsed = Time.time - startTime;

        if(elapsed > optimizeRouterFilterRate) {
            OnOptimzeRouters();
            startTime = Time.time;
        }
    }


}
