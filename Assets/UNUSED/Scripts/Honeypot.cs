using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Honeypot : MonoBehaviour
{
    /*
    public Material inactiveMaterial;
    public Material activeMaterial;

    public GameObject canvas;
    public Slider healthSlider;

    public int cost = 1500;

    public int startingHealth;
    private int health;

    public int damageValue;

    private static int current_id = 0;

    public int id;
    private bool isActivated;

    public Text costText;

    public HoneypotPath path;

    public WaveController waveController;

    public ScoreManager scoreManager;

    public void Awake()
    {
        id = current_id++;
        isActivated = false;
    }

    public void Activate()
    {

        if(EntityManager.inst.isMultiplayer && !isActivated)
        {
            Shared.inst.sync_events.Add(new SyncEvent(MessageTypes.UPDATE_HONEY_POT_ACTIVATION, id.ToString()));
            isActivated = true;
        }


        if(scoreManager.Money >= cost)
        {
            health = startingHealth;
            canvas.SetActive(true);
            healthSlider.value = healthSlider.maxValue;
            Material[] tempMaterials = GetComponent<Renderer>().materials;
            tempMaterials[0] = activeMaterial;
            GetComponent<Renderer>().materials = tempMaterials;
            path.StartCoroutine("RaisePath");
            waveController.honeypotAvailable = true;
        }
    }

    private void Deactivate()
    {
        isActivated = false;
        canvas.SetActive(false);
        Material[] tempMaterials = GetComponent<Renderer>().materials;
        tempMaterials[0] = inactiveMaterial;
        GetComponent<Renderer>().materials = tempMaterials;
        path.StartCoroutine("LowerPath");
        waveController.honeypotAvailable = false;
    }

    private void Start()
    {
        costText.text = "$" + cost.ToString();
        canvas.SetActive(false);
        healthSlider.maxValue = startingHealth;
    }

    public void Damage()
    {
        if(health > damageValue)
        {
            health -= damageValue;
            healthSlider.value = health;
        }
        else
        {
            Deactivate();
        }
    }


    public static Honeypot getHoneyPotById(int id)
    {

        GameObject[] honey = GameObject.FindGameObjectsWithTag("HoneyPot");
        foreach(GameObject go in honey)
        {
            Honeypot hp = go.GetComponent<Honeypot>();
            if (hp.id == id)
                return hp;
        }

        return null;
    }
    */
}
