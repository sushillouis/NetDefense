using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteHatNPC : MonoBehaviour
{

    public static WhiteHatNPC inst;

    public GameObject Router;
    public PathPiece[] routerSpawns;

    public List<GameObject> routers;


    private void Awake() {
        inst = this;

    }

    public void OnUsingWhiteAtNPC() {
        if(routers.Count != 0) {
            return;
        }

        routers = new List<GameObject>();

        foreach(PathPiece routerSpawn in routerSpawns) {
            routers.Add(Instantiate(Router, routerSpawn.transform.position, routerSpawn.transform.rotation));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
