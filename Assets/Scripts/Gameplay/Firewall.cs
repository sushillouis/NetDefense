using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Firewall : MonoBehaviourPun, SelectionManager.ISelectable {
	public static Firewall[] firewalls = null;

	// Reference to the attached mesh renderer
	new public MeshRenderer renderer;

	// The material which represents the firewall's gate
	public Material gateMaterial;
	// The colors that the firewall's gate can become
	public Color[] colors;

	// The details of packets that should be filtered
	public Packet.Details filterRules = Packet.Details.Default;

	// The number of updates gained after each wave (based on difficulty)
	public int[] updatesGrantedPerWave = new int[3] {/*easy*/4, /*medium*/4, /*hard*/4};
	// Property representing the number of updates currently available
	[SerializeField]
	public int updatesRemaining = 1; // Starts at 1 to account for initial settings


	// De/register the start function on wave ends
	void OnEnable(){
		firewalls = FindObjectsOfType<Firewall>(); // Update the list of firewalls
		GameManager.waveEndEvent += Start;
	}
	void OnDisable(){
		firewalls = FindObjectsOfType<Firewall>(); // Update the list of firewalls
		GameManager.waveEndEvent -= Start;
	}

	// When the this is created or a wave starts grant its updates per wave
	void Start(){
		SetFilterRules(Packet.Details.Default); // Make sure that the base filter rules are applied
		updatesRemaining += updatesGrantedPerWave[(int)GameManager.difficulty];
	}

	// Update the packet rules (Network Synced)
	// Returns true if we successfully updated, returns false otherwise
	public bool SetFilterRules(Packet.Color color, Packet.Size size, Packet.Shape shape){
		// Only update the settings if we have updates remaining
		if(updatesRemaining > 0){
			// Take away an update if something actually changed
			if(color != filterRules.color || size != filterRules.size || shape != filterRules.shape)
				updatesRemaining--;
			photonView.RPC("RPC_Firewall_SetFilterRules", RpcTarget.AllBuffered, color, size, shape);
			return true;
		} else return false;
	}
	public bool SetFilterRules(Packet.Details details){ return SetFilterRules(details.color, details.size, details.shape); }
	[PunRPC] void RPC_Firewall_SetFilterRules(Packet.Color color, Packet.Size size, Packet.Shape shape){
		filterRules = new Packet.Details(color, size, shape);

		SetGateColor(color);
	}

	// Function which sets the firewall's gate color (network synced)
	public void SetGateColor(Packet.Color color, bool _default = false){ photonView.RPC("RPC_Firewall_SetGateColor", RpcTarget.AllBuffered, color, _default); }
	[PunRPC] void RPC_Firewall_SetGateColor(Packet.Color color, bool _default){
		// Get the list of materials off the mesh
		Material[] mats = renderer.materials;
		// Replace the second one with a new instance of the gate material
		mats[1] = new Material(gateMaterial);
		if(!_default) mats[1].SetColor("_EmissionColor", colors[(int)color]); // Set the firewall color
		// Copy the material changes back to the model
		renderer.materials = mats;
	}


	// Function which starts the gradual move coroutine if it isn't already started
	public bool StartGradualMove(Vector3 targetPosition, Quaternion targetRotation){
		if(gradualMoveFirewallIsMoving) return false;

		StartCoroutine(gradualMoveFirewall(transform.position, transform.rotation, targetPosition, targetRotation));
		return true;
	}

	// Coroutine which gradually moves the firewall to the targeted position
	public float gradualMoveFirewallTimeTaken = 1f/3f; // The amount of time that it should take to rotate
	float gradualMoveFirewallStartTime; // Start time when the coroutine begins
	public bool gradualMoveFirewallIsMoving = false;
 	IEnumerator gradualMoveFirewall(Vector3 startingPosition, Quaternion startingRotation, Vector3 targetPosition, Quaternion targetRotation){
		// Save the start time
		gradualMoveFirewallStartTime = Time.time;
		gradualMoveFirewallIsMoving = true;
		// While the scaled time is less than 110% of the total time (some extra buffer just in case)
		while((Time.time - gradualMoveFirewallStartTime) / gradualMoveFirewallTimeTaken < 1.1){
			// Lerp from the starting direction to the target direction based on scaled time
			transform.position = Vector3.Lerp(startingPosition, targetPosition, (Time.time - gradualMoveFirewallStartTime) / gradualMoveFirewallTimeTaken);
			transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, (Time.time - gradualMoveFirewallStartTime) / gradualMoveFirewallTimeTaken);
			yield return null;
		}

		gradualMoveFirewallIsMoving = false;
	}
}
