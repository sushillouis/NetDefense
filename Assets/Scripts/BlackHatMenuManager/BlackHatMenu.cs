using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackHatMenu : MonoBehaviour
{
    public static class types
    {
        public static int NO_MENU = -1;
        public static int TARGET_MENU = 1;
        public static int MALIC_PACKET_MENU = 2;
    }

    private int currentMenu;

    public GameObject target_panel;

    public Button targettingCoolDownButton;
    public Text targettingCoolDownButton_text;
    public float targettingMaxCoolDown;

    public Button packetCoolDownButton;
    public Text packetCoolDownButton_text;
    public float packetMaxCoolDown;

    //   public Text text;

    public Slider s1;
    public Slider s2;
    public Slider s3;

    private float t1, t2, t3;

    [Space(25)]
    public GameObject malic_packet_panel;

    [Space(25)]
    public RectTransform nav_menu;
    public Vector3 init_pos_of_nav_menu;

    public float targettingStartTime;
    public float targetting_elapsed;


    public float packetStartTime;
    public float packet_elapsed;

    public Toggle pink, green, blue, square, cone, sphere, small, med, large;


    void Start() {
        Construct();
    }

    private void Construct() {
        targettingStartTime = Time.time;
        packetStartTime = Time.time;

        currentMenu = -1;
        init_pos_of_nav_menu = new Vector3(nav_menu.localPosition.x, nav_menu.localPosition.y, nav_menu.localPosition.z); ;

        targettingCoolDownButton_text.text = "Confirm Strategy";
    }

    private void handleGUIpositionsOnStateChanged()
    {
        if (currentMenu == types.NO_MENU)
        {
            target_panel.SetActive(false);
            malic_packet_panel.SetActive(false);
        }
        else
        {
            nav_menu.localPosition = init_pos_of_nav_menu;
        }

        if (currentMenu == types.MALIC_PACKET_MENU)
        {
            target_panel.SetActive(false);
            malic_packet_panel.SetActive(true);
        }

        if (currentMenu == types.TARGET_MENU)
        {
            target_panel.SetActive(true);
            malic_packet_panel.SetActive(false);
        }
    }

    public void updateMalicPacketValues() {
        packetStartTime = Time.time;

        int color = 0;
        if(pink.isOn) {
            color = 0;
        }
        if (green.isOn) {
            color = 1;
        }
        if (blue.isOn) {
            color = 2;
        }
        Shared.inst.maliciousPacketProperties.color = color;

        int size = 0;
        if(small.isOn) {
            size = 0;
        }
        if (med.isOn) {
            size = 1;
        }
        if (large.isOn) {
            size = 2;
        }
        Shared.inst.maliciousPacketProperties.size = size;

        int shape = 0;
        if (square.isOn) {
            shape = 0;
        }
        if (cone.isOn) {
            shape = 1;
        }
        if (sphere.isOn) {
            shape = 2;
        }


 //       Debug.Log("SIZE: " + size + "\n" + "SHAPE: " + shape + "\n" + "color: " + color + "\n");

        Shared.inst.maliciousPacketProperties.shape = shape;

        Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.UPDATE_MALIC_PACKETS, Shared.inst.maliciousPacketProperties.shape + "," + Shared.inst.maliciousPacketProperties.size + "," + Shared.inst.maliciousPacketProperties.color));

        packetCoolDownButton.enabled = false;

    }


    public void updateTargetPercentages()
    {
        targettingStartTime = Time.time;

        float percentage = s1.value + s2.value + s3.value;

        if (percentage == 0)
            percentage = 1;

        t1 = s1.value / percentage;
        t2 = s2.value / percentage;
        t3 = s3.value / percentage;

        Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_SERVER_TARGETTING_PROBABILITY, "LEFT" + "," + t1));
        Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_SERVER_TARGETTING_PROBABILITY, "RIGHT" + "," + t2));
        Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_SERVER_TARGETTING_PROBABILITY, "CENTRE" + "," + t3));

        targettingCoolDownButton.enabled = false;

 //       text.text = trgt(1, t1) + "\n" + trgt(2, t2) + "\n" + trgt(3, t3);
    }

    void Update()
    {
        handleGUIpositionsOnStateChanged();

        targetting_elapsed = Time.time - targettingStartTime;

        if (targetting_elapsed > targettingMaxCoolDown) {
            targettingCoolDownButton.enabled = true;
            targettingCoolDownButton_text.text = "Confirm Strategy";
        } else {
            targettingCoolDownButton_text.text = "Cooling Down... ";
        }

       
        packet_elapsed = Time.time - packetStartTime;

        if (packet_elapsed > packetMaxCoolDown) {
            packetCoolDownButton.enabled = true;
            packetCoolDownButton_text.text = "Confirm Rules Update";
        } else {
            packetCoolDownButton_text.text = "Cooling Down... ";
        }

        ///updateTargetPercentages();

    }

    // nav menu callbacks

    public void OnTargetButtonPressed()
    {
        currentMenu = types.TARGET_MENU;
    }

    public void OnTypeButtonPressed()
    {
        currentMenu = types.MALIC_PACKET_MENU;
    }

    public void OnClosedButtonPressed()
    {
        currentMenu = types.NO_MENU;
    }
}
