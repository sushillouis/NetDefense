using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BlackHatBaseManager : BaseSharedBetweenHats {
	// Error codes used by the error handling system
	public new class ErrorCodes : BaseSharedBetweenHats.ErrorCodes {
		public static readonly int StartingPointNotSelected = 3;
		public static readonly int InvalidProbability = 4;

		// Required function to get the class up to par
		public ErrorCodes() {}
		public ErrorCodes(int _value) : base(_value) {}
		public static implicit operator int(ErrorCodes e) => e.value;
		public static implicit operator ErrorCodes(int value) => new ErrorCodes(value);
	}

	// Override instance to represent the BlackkHat type
	new static public BlackHatBaseManager instance {
		get => BaseSharedBetweenHats.instance as BlackHatBaseManager;
	}


	// -- GameState Manipulators


	// Function which updates all of the starting points to have the specified malicious packet details
	public bool ChangeAllStartPointsMalciousPacketDetails(Packet.Details details){
		bool success = true;
		foreach(StartingPoint p in StartingPoint.startingPoints)
			success &= ChangeStartPointMalciousPacketDetails(p, details);

		return success;
	}

	// Function which updates the specified starting point to have the specified malicious packet details
	public bool ChangeStartPointMalciousPacketDetails(StartingPoint toModify, Packet.Details details){
		// Error if the starting point to destroy is null
		if(toModify == null){
			ErrorHandler(ErrorCodes.StartingPointNotSelected, "A Starting Point to modify must be selected!");
			return false;
		}
		// Error if we don't own the starting point
		if(toModify.photonView.Controller != NetworkingManager.localPlayer){
			ErrorHandler(ErrorCodes.WrongPlayer, "You can't modify Starting Points you don't own!");
			return false;
		}

		toModify.SetMaliciousPacketDetails(details);
		return true;
	}

	// Function which changes the probability of a spawned packet being malicious for of all the starting points
	public bool ChangeAllStartPointsMaliciousPacketProbabilities(float probability){
		bool success = true;
		foreach(StartingPoint p in StartingPoint.startingPoints)
			success &= ChangeStartPointMaliciousPacketProbability(p, probability);

		return success;
	}

	// Function which changes the probability of a spawned packet being malicious for the specified starting point
	public bool ChangeStartPointMaliciousPacketProbability(StartingPoint toModify, float probability){
		// Error if the starting point to modify is null
		if(toModify == null){
			ErrorHandler(ErrorCodes.StartingPointNotSelected, "A Starting Point to modify must be selected!");
			return false;
		}
		// Error if we don't own the starting point
		if(toModify.photonView.Controller != NetworkingManager.localPlayer){
			ErrorHandler(ErrorCodes.WrongPlayer, "You can't modify Starting Points you don't own!");
			return false;
		}
		// Error if the provided probability is invalid
		if(probability < 0 || probability > 1){
			ErrorHandler(ErrorCodes.InvalidProbability, "The probability " + probability + " is invalid!");
			return false;
		}

		toModify.maliciousPacketProbability = probability;
		return true;
	}

	// Function which changes the likelihood that a malicious packet will target the specified destination
	public bool ChangeDestinationMaliciousPacketTargetLikelihood(Destination toModify, int likelihood){
		// Error if the destination to modify is null
		if(toModify == null){
			ErrorHandler(ErrorCodes.StartingPointNotSelected, "A Destination to modify must be selected!");
			return false;
		}
		// Error if we don't own the destination
		if(toModify.photonView.Controller != NetworkingManager.localPlayer){
			ErrorHandler(ErrorCodes.WrongPlayer, "You can't modify Destinations you don't own!");
			return false;
		}

		toModify.maliciousPacketDestinationLikelihood = likelihood;
		return true;
	}

}
