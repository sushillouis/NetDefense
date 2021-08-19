using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SuggestedFirewall : MonoBehaviour {
	void Start(){ StartCoroutine(DeleteSuggestedFirewallAfterSeconds(5)); }

	public void ResetDeleteTimer(){
		deleteSuggestedFirewallAfterSeconds_StartTime = Time.time;
	}

	// Function which starts the gradual move coroutine if it isn't already started
	public bool StartGradualMove(Vector3 targetPosition, Quaternion targetRotation){
		if(gradualMoveSuggestedFirewallIsMoving) return false;

		StartCoroutine(gradualMoveSuggestedFirewall(transform.position, transform.rotation, targetPosition, targetRotation));
		return true;
	}


	// Coroutine which deletes the suggested firewall after the specified number of seconds
	float deleteSuggestedFirewallAfterSeconds_StartTime;
	IEnumerator DeleteSuggestedFirewallAfterSeconds(float seconds){
		deleteSuggestedFirewallAfterSeconds_StartTime = Time.time;

		while(seconds > Time.time - deleteSuggestedFirewallAfterSeconds_StartTime) yield return null;
		PhotonNetwork.Destroy(gameObject);
		WhiteHatBaseManager.instance.suggestedFirewall = null;
	}

	// Coroutine which gradually moves the suggested firewall to the targeted position
	public float gradualMoveSuggestedFirewallTimeTaken = 1f/3f; // The amount of time that it should take to rotate
	float gradualMoveSuggestedFirewallStartTime; // Start time when the coroutine begins
	public bool gradualMoveSuggestedFirewallIsMoving = false;
 	IEnumerator gradualMoveSuggestedFirewall(Vector3 startingPosition, Quaternion startingRotation, Vector3 targetPosition, Quaternion targetRotation){
		// Save the start time
		gradualMoveSuggestedFirewallStartTime = Time.time;
		gradualMoveSuggestedFirewallIsMoving = true;
		// While the scaled time is less than 110% of the total time (some extra buffer just in case)
		while((Time.time - gradualMoveSuggestedFirewallStartTime) / gradualMoveSuggestedFirewallTimeTaken < 1.1){
			// Lerp from the starting direction to the target direction based on scaled time
			transform.position = Vector3.Lerp(startingPosition, targetPosition, (Time.time - gradualMoveSuggestedFirewallStartTime) / gradualMoveSuggestedFirewallTimeTaken);
			transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, (Time.time - gradualMoveSuggestedFirewallStartTime) / gradualMoveSuggestedFirewallTimeTaken);
			yield return null;
		}

		gradualMoveSuggestedFirewallIsMoving = false;
	}
}
