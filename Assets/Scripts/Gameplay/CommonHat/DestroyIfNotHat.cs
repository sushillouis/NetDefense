using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component which destroys the attached object if the player is of the wrong hat. Used to simply spawning of networked assets which should only be seen by one hat.
public class DestroyIfNotHat : MonoBehaviour {
	public Networking.Player.Side allowedSide;

    void OnEnable(){
		// Destroy for players that aren't of the allowed side, or are common spectators
		if( !(NetworkingManager.localPlayer.side == allowedSide
		  || (NetworkingManager.localPlayer.side == Networking.Player.Side.Common && NetworkingManager.localPlayer.role == Networking.Player.Role.Spectator)) )
			Destroy(gameObject);
	}
}
