using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkDiscoveryUIManager : MonoBehaviour {

    public static NetworkDiscoveryUIManager inst;

    public GameObject ServerList;
    public GameObject ServerElement;

    public List<string> addresses;

    private void Awake() {
        inst = this;
        addresses = new List<string>();
    }

    public void OnNewServerFound(string address, string serverName) {
        
        foreach(string s in addresses) {
            if(s.Equals(address)) {
                return;
            }
        }

        addresses.Add(address);

        GameObject element = Instantiate(ServerElement);
        element.GetComponent<ServerElementManager>().gameName = serverName;
        element.GetComponent<ServerElementManager>().ipAddress = address;
        element.GetComponent<ServerElementManager>().OnDataChanged();
        element.transform.parent = ServerList.transform;
     
    }
}
