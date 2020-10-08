using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Destination : MonoBehaviour
{
    private static List<Destination> destinations = new List<Destination>();

    public string inst_id;


    public Material defaultMaterial;
    public Material honeypotMaterial;

    public bool isHoneypot;

    public List<Path> paths;

    public void Start() {
        destinations.Add(this);
    }

    private void OnMouseDown()
    {
        isHoneypot = !isHoneypot;

        if (EntityManager.inst.isMultiplayer && Shared.inst.gameMetrics.isWhiteHat)
            Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_HONEY_POT_ACTIVATION, inst_id + "," + isHoneypot));

        setIsHoneyPot(isHoneypot);
    }

    public void setIsHoneyPot(bool value) {
        this.isHoneypot = value;
        Material newMaterial = (isHoneypot) ? honeypotMaterial : defaultMaterial;
        Material[] tempMaterials = GetComponent<Renderer>().materials;
        tempMaterials[0] = newMaterial;
        GetComponent<Renderer>().materials = tempMaterials;
    }

    public static Destination getDestinationByID(string id) {
        foreach(Destination d in destinations) {
            if (d.inst_id.Equals(id))
                return d;
        }
        Debug.LogError("Invalid Path ID");
        return null;
    }
}
