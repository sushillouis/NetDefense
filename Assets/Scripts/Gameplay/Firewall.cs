using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Firewall : MonoBehaviourPun {
	// Reference to the attached mesh renderer
	new public readonly MeshRenderer renderer;
	// Reference to the attached selectionCylinder
	public readonly GameObject selectionCylinder;

	// The material which represents the firewall's gate
	public Material gateMaterial;
	// The colors that the firewall's gate can become
	public Color[] colors;


	// TODO: public Packet.Details filterRules;


	// Function which sets the firewall's gate color (network synced)
	public void SetGateColor(Packet.Color color, bool _default = false){ photonView.RPC("RPC_Router_SetGateColor", RpcTarget.AllBuffered, color, _default); }
	[PunRPC] void RPC_Router_SetGateColor(Packet.Color color, bool _default){
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
