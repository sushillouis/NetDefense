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

    public Text updatesRemainingValue;
    public int max_updates_remaining;
	[SerializeField]
	private int _updates_remaining;
    public int updates_remaining {
		get => _updates_remaining;
		set {
			_updates_remaining = value;
			updatesRemainingValue.text = _updates_remaining + "";
		}
	}

    public OnReadyUpController nextWaveButton;
    public static BlackHatMenu inst;

    private void Awake() {
        inst = this;
    }

    void Start() {
		targettingStartTime = Time.time;
        packetStartTime = Time.time;

        currentMenu = -1;
        init_pos_of_nav_menu = new Vector3(nav_menu.localPosition.x, nav_menu.localPosition.y, nav_menu.localPosition.z); ;

        targettingCoolDownButton_text.text = "Confirm Strategy";

        updates_remaining = max_updates_remaining;
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
        if (updates_remaining > 0) {
			bool updateOccured = false;
			while(Camera.main.transform.GetChild(3).GetComponent<AudioSource>().isPlaying); // Wait for the confirm sound to stop playing

            int color = 0;
            if (green.isOn) color = 1;
            else if (blue.isOn) color = 2;
			updateOccured |= Shared.inst.maliciousPacketProperties.color != color;
            Shared.inst.maliciousPacketProperties.color = color;

            int size = 0;
            if (med.isOn) size = 1;
            else if (large.isOn) size = 2;
			updateOccured |= Shared.inst.maliciousPacketProperties.size != size;
            Shared.inst.maliciousPacketProperties.size = size;

            int shape = 0;
            if (cone.isOn) shape = 1;
            else if (sphere.isOn) shape = 2;
			updateOccured |= Shared.inst.maliciousPacketProperties.shape != shape;
            Shared.inst.maliciousPacketProperties.shape = shape;

			if(updateOccured){
				packetStartTime = Time.time;
				packetCoolDownButton.enabled = false;

				// Play settings update sound
				Camera.main.transform.GetChild(3).GetComponent<AudioSource>().Play();
				updates_remaining--;

				AutoHelpScreenBlackhatManager.inst.OnConfirmedType();
		        if(!MainMenu.isMultiplayerSelectedFromMenu) {
		            PacketPoolManager.inst.OnBlackhatUpdateStrategy();
		        } else {

		            Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.UPDATE_MALIC_PACKETS, Shared.inst.maliciousPacketProperties.shape + "," + Shared.inst.maliciousPacketProperties.size + "," + Shared.inst.maliciousPacketProperties.color));
		        }
			} else
				// Play settings failed to update sound
				Camera.main.transform.GetChild(5).GetComponent<AudioSource>().Play();
        } else
			// Play settings failed to update sound
			Camera.main.transform.GetChild(5).GetComponent<AudioSource>().Play();


    }


    public void updateTargetPercentages()
    {
        if (updates_remaining > 0) {
			while(Camera.main.transform.GetChild(3).GetComponent<AudioSource>().isPlaying); // Wait for the confirm sound to stop playing

            float percentage = s1.value + s2.value + s3.value;

            if (percentage == 0)
                percentage = 1;

            t1 = s1.value / percentage;
            t2 = s2.value / percentage;
            t3 = s3.value / percentage;

			bool updateOccured = Shared.inst.gameMetrics.target_probabilities["LEFT"] != t1 || Shared.inst.gameMetrics.target_probabilities["RIGHT"] != t2 || Shared.inst.gameMetrics.target_probabilities["CENTRE"] != t3;

            if (MainMenu.isMultiplayerSelectedFromMenu) {
                Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_SERVER_TARGETTING_PROBABILITY, "LEFT" + "," + t1));
                Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_SERVER_TARGETTING_PROBABILITY, "RIGHT" + "," + t2));
                Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_SERVER_TARGETTING_PROBABILITY, "CENTRE" + "," + t3));
            } else {
                Shared.inst.gameMetrics.target_probabilities["LEFT"] = t1;
                Shared.inst.gameMetrics.target_probabilities["RIGHT"] = t2;
                Shared.inst.gameMetrics.target_probabilities["CENTRE"] = t3;
                PacketPoolManager.inst.OnBlackhatUpdateStrategy();

            }

			if(updateOccured){
				targettingStartTime = Time.time;
				targettingCoolDownButton.enabled = false;
	            AutoHelpScreenBlackhatManager.inst.OnConfirmedTarget();

				// Play settings update sound
				Camera.main.transform.GetChild(3).GetComponent<AudioSource>().Play();
				updates_remaining--;
			} else
				// Play settings failed to update sound
				Camera.main.transform.GetChild(5).GetComponent<AudioSource>().Play();

			// TODO: Is check if this is updating properly
        } else
			// Play settings failed to update sound
			Camera.main.transform.GetChild(5).GetComponent<AudioSource>().Play();


    }

    public void updateTargetPercentagesTutorial() {
        if (updates_remaining > 0) {
			while(Camera.main.transform.GetChild(3).GetComponent<AudioSource>().isPlaying); // Wait for the confirm sound to stop playing

            float percentage = s1.value + s2.value;

            if (percentage == 0)
                percentage = 1;

            t1 = s1.value / percentage;
            t2 = s2.value / percentage;

			bool updateOccured = Shared.inst.gameMetrics.target_probabilities["TOP"] != t1 || Shared.inst.gameMetrics.target_probabilities["BOTTOM"] != t2;

            if (MainMenu.isMultiplayerSelectedFromMenu) {
                Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_SERVER_TARGETTING_PROBABILITY, "TOP" + "," + t1));
                Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_SERVER_TARGETTING_PROBABILITY, "BOTTOM" + "," + t2));
            } else {
                Shared.inst.gameMetrics.target_probabilities["TOP"] = t1;
                Shared.inst.gameMetrics.target_probabilities["BOTTOM"] = t2;
                PacketPoolManager.inst.OnBlackhatUpdateStrategy();

            }

			if(updateOccured){
				targettingStartTime = Time.time;
				targettingCoolDownButton.enabled = false;
	            AutoHelpScreenBlackhatManager.inst.OnConfirmedTarget();

				// Play settings update sound
				Camera.main.transform.GetChild(3).GetComponent<AudioSource>().Play();
				updates_remaining--;
			} else
				// Play settings failed to update sound
				Camera.main.transform.GetChild(5).GetComponent<AudioSource>().Play();
        } else
			// Play settings failed to update sound
			Camera.main.transform.GetChild(5).GetComponent<AudioSource>().Play();


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

	public void OnWaveEnd(){
		nextWaveButton.OnWaveEnd();

		// Between each wave give the blackhat their updates back
		updates_remaining = max_updates_remaining;
	}
}
