using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Destination : MonoBehaviour {
    public static List<Destination> destinations = new List<Destination>();

    public string inst_id;


    public Material defaultMaterial;
    public Material honeypotMaterial;

    public bool isHoneypot;

    public List<Path> paths;

    public bool isReadyTobeHoneypot;
    public GameObject honeypotSelection;

    public void Start() {
        destinations.Add(this);
        honeypotSelection.SetActive(false);
    }

    private void OnMouseDown() {
        if (Shared.inst.getDevicePlayer().role == SharedPlayer.WHITEHAT && honeypotSelection.activeSelf) {
            isHoneypot = !isHoneypot;
            Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_HONEY_POT_ACTIVATION, inst_id + "," + isHoneypot));
            setIsHoneyPot(isHoneypot);

            foreach (Destination d in destinations) {
                d.isReadyTobeHoneypot = false;
                d.updateSelection();
            }
        }
    }

    public void setIsHoneyPot(bool value) {
		// If we are setting a honey pot, make sure that all of the other destinations are not honey pots
		if(value) foreach (Destination d in Destination.destinations)
			d.setIsHoneyPot(false);

        this.isHoneypot = value;

        // only update the material if the player is the whitehat
        if (Shared.inst.getDevicePlayer().role == SharedPlayer.WHITEHAT) {
            Material newMaterial = (isHoneypot) ? honeypotMaterial : defaultMaterial;
            Material[] tempMaterials = GetComponent<Renderer>().materials;
            tempMaterials[0] = newMaterial;
            GetComponent<Renderer>().materials = tempMaterials;
        }
        Debug.Log(honeypotMaterial);
        Debug.Log("Honeypot is " + isHoneypot );

		// Play the router place sound (if we are creating a honeypot and the player is a whitehat)
		if(value && Shared.inst.getDevicePlayer().role == SharedPlayer.WHITEHAT) Camera.main.transform.GetChild(4).GetComponent<AudioSource>().Play();
    }
    public static Destination getDestinationByID(string id) {
        foreach (Destination d in destinations) {
            if (d.inst_id.Equals(id))
                return d;
        }
        Debug.LogError("Invalid Path ID");
        return null;
    }

    public static void CleanUpStatics() {
        destinations.Clear();
    }

    public bool canBecomeHoneyPot() {
        return !isHoneypot && isReadyTobeHoneypot;
    }

    public void updateSelection() {
        honeypotSelection.SetActive(canBecomeHoneyPot());
    }
}
