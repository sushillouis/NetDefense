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

    public int initialSpawnCount;
    public int routerSpawnTokens;
    public int endOfWaveAward;

    public bool canSpend;

    private void Awake() {
        inst = this;
        startTime = Time.time;
        hasSpawnedAtIndex = new bool[routerSpawns.Length];

        if((MainMenu.hat == SharedPlayer.WHITEHAT && !MainMenu.isMultiplayerSelectedFromMenu) || MainMenu.isMultiplayerSelectedFromMenu) {
            Destroy(this);
        }
    }

    public void OnUsingWhiteAtNPC() {
        if (routers.Count != 0) {
            return;
        }

        routers = new List<GameObject>();
        activeIndices = new List<int>();
        for (int i = 0; i < initialSpawnCount; i++) {
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

        foreach (GameObject router in routers) {
            Router r = router.GetComponent<Router>();

            r.SetColor(Random.Range(0, 3));
            r.SetShape(Random.Range(0, 3));
            r.SetSize(Random.Range(0, 3));
        }
    }

    public void OnOptimizeRouters() {
        UpdateRouters();
        SpawnRouters();
    }

    private void SpawnRouters() {
        // trade tokens for routers if we have tokens
        if (routerSpawnTokens > 0 && canSpend)
            for (int j = 0; j < hasSpawnedAtIndex.Length; j++) {
                if (!hasSpawnedAtIndex[j]) {
                    hasSpawnedAtIndex[j] = true;
                    Router router = Instantiate(Router, routerSpawns[j].transform.position, routerSpawns[j].transform.rotation).GetComponent<Router>();
                    routers.Add(router.gameObject);
                    router.SetColor(Random.Range(0, 3));
                    router.SetShape(Random.Range(0, 3));
                    router.SetSize(Random.Range(0, 3));
                    routerSpawnTokens--; // take token when spawned
                    canSpend = false;
                    break;
                }
            }
    }

    private void UpdateRouters() {
        for (int i = 0; i < routers.Count; i++) {
            Router r = routers[i].GetComponent<Router>();
            if (r.updatesRemaining < 1) {
                routers.RemoveAt(i);
                i--;
                routerSpawnTokens++; // give a token when we have a useless router
            } else {
                // we can update router settings


                if (r.color != Shared.inst.maliciousPacketProperties.color) {
                    // update the color to something different
                    if (r.updatesRemaining > 0)
                        r.SetColor(Random.Range(0, 3));
                }

                if (r.shape != Shared.inst.maliciousPacketProperties.shape) {
                    // update the shape to something different
                    if (r.updatesRemaining > 0)

                        r.SetShape(Random.Range(0, 3));
                }

                if (r.size != Shared.inst.maliciousPacketProperties.size) {
                    // update the size to something different
                    if (r.updatesRemaining > 0)

                        r.SetSize(Random.Range(0, 3));
                }
            }
        }
    }

    void Update() {
        if (GameManager.inst.isBetweenWaves)
            return;

        float elapsed = Time.time - startTime;

        if (elapsed > optimizeRouterFilterRate) {
            OnOptimizeRouters();
            canSpend = true;
            startTime = Time.time;
        }
    }

    public void OnBetweenWaves() {
        if (MainMenu.hat == SharedPlayer.WHITEHAT)
            return;

        if (routerSpawnTokens == 0)
            routerSpawnTokens += endOfWaveAward; // award a token at the end of a wave
    }

}
