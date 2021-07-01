using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WhiteHatBaseManager : Core.Utilities.SingletonPun<WhiteHatBaseManager> {
	// Error codes used by the error handling system
	public enum ErrorCodes {
		Generic,
		FirewallIsMoving,		// Error code stating that the firewall is still moving
		FirewallNotSelected,	// Error code stating that no firewall has been selected
		TargetNotSelected,		// Error code stating that no target has been selected
		InvalidTarget,			// Error code stating that the selected target is invalid
	}

	// A string referencing the firewall prefab path
	public string firewallPrefabPath;

	// When we awake preform all of the code for a singleton and also ensure that the prefab path is good to be used (removes extra stuff unity's copy path feature gives us)
	override protected void Awake(){
		base.Awake();
		Utilities.PreparePrefabPath(ref firewallPrefabPath);
	}


	// -- GameState Manipulators


	// Function which spawns and returns a firewall on the given path piece (network synced)
	protected Firewall SpawnFirewall(GameObject targetPathPiece){
		// Error on invalid path piece
		if(targetPathPiece == null){
			ErrorHandler(ErrorCodes.TargetNotSelected, "A location to place the firewall at must be selcted!");
			return null;
		}
		// Error if the path piece can't have firewalls on it
		if(targetPathPiece.tag != "FirewallTarget"){
			ErrorHandler(ErrorCodes.InvalidTarget, "Firewalls can't be placed on the selected location!");
			return null;
		}

		// Spawn the new firewall in the network
		Firewall spawned = PhotonNetwork.Instantiate(firewallPrefabPath, new Vector3(0, 100, 0), Quaternion.identity).GetComponent<Firewall>();
		// Move it to its proper position
		MoveFirewall(spawned, targetPathPiece, /*not animated*/ false);

		return spawned;
	}

	// Function which moves a firewall to the targetedPathPiece
	// This function returns true if the move was successfull, and false if any errors occured
	// The function by default causes the firewall to be smoothly moved to its new location over the course of half a second (this behavior can be disabled by passing false to animated)
	protected virtual bool MoveFirewall(Firewall toMove, GameObject targetPathPiece, bool animated = true){
		// Error if the firewall to move is null
		if(toMove == null){
			ErrorHandler(ErrorCodes.FirewallNotSelected, "A Firewall to move must be selected!");
			return false;
		}
		// Error if the path piece to move too is null
		if(targetPathPiece == null){
			ErrorHandler(ErrorCodes.TargetNotSelected, "A location to move to must be selcted!");
			return false;
		}
		// Error if the path piece can't have firewalls on it
		if(targetPathPiece.tag != "FirewallTarget"){
			ErrorHandler(ErrorCodes.InvalidTarget, "Firewalls can't be moved to the selected location!");
			return false;
		}

		// If we should be animating the movement...
		if(animated){
			// Try to start the movement and return an error if it is already moving
			if(!toMove.StartGradualMove(targetPathPiece.transform.position, targetPathPiece.transform.rotation)){
				ErrorHandler(ErrorCodes.FirewallIsMoving, "Wait until it is done moving!");
				return false;
			}
		// If we shouldn't be animating, simply snap the path piece to its destination
		} else {
			toMove.transform.position = targetPathPiece.transform.position;
			toMove.transform.rotation = targetPathPiece.transform.rotation;
		}

		// We have successfully moved the path piece, so return true
		return true;
	}

	// Function which destroys the given firewall
	protected virtual void DestroyFirewall(Firewall toDestroy){
		// TODO: Possibly add a particle system!

		// Network destroy the firewall
		PhotonNetwork.Destroy(toDestroy.gameObject);
	}


	// -- GameState Accessors --


	public static GameObject[] getFirewallTargets(){ return GameObject.FindGameObjectsWithTag("FirewallTarget"); }
	public static GameObject[] getSwitchTargets(){ return GameObject.FindGameObjectsWithTag("SwitchTarget"); }


	// -- Error Handling --


	protected virtual void ErrorHandler(ErrorCodes errorCode, string error){
		Debug.LogError(error);
	}
	protected virtual void ErrorHandler(string error){ ErrorHandler(ErrorCodes.Generic, error); }
}
