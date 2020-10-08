using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkDiscoveryManager : NetworkDiscovery
{

    public static NetworkDiscoveryManager inst;


    private void Awake() {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public override void OnReceivedBroadcast(string fromAddress, string data) {
        base.OnReceivedBroadcast(fromAddress, data);
        Debug.Log(fromAddress);
        Debug.Log(data);
        NetworkDiscoveryUIManager.inst.OnNewServerFound(fromAddress, data);
    }
}
