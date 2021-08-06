using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Networking {
	// Class which represents a player
	[System.Serializable]
	public class Player : ISerializable {
		// The player's side
		[System.Serializable]
		public enum Side {
			WhiteHat,
			BlackHat,
			Common
		}

		// The player's role
		[System.Serializable]
		public enum Role {
			Player,
			Advisor,
			Spectator
		}

		// Static reference to current player
		public static Player localPlayer;


		// -- Data --


		// Reference to the Photon Player
		public Photon.Realtime.Player photonPlayer;
		public int debugPhotonPlayer; // TODO: Remove

		// Current side
		[SerializeField] Side _side;
		public Side side {
			get => _side;
			set {
				_side = value;
				// Make sure that the common side can only be a spectator
				if(value == Side.Common) role = Role.Spectator;
			}
		}

		// Current role
		public Role role;

		// Default constructor
		public Player(){}
		// Players can be implicitly converted to Photon Players
		public static implicit operator Photon.Realtime.Player(Player p) => p.photonPlayer;


		// -- Properties --


		// Alias for the photon player's actor number
		public int actorNumber {
			get => photonPlayer.ActorNumber;
		}

		// Gets the index in the room player list of this player
		public int roomIndex {
			get => System.Array.FindIndex(NetworkingManager.roomPlayers, p => p == photonPlayer);
		}

		// Gets the nickname of this player (or Player #PhotonPlayer.ActorNumber if they didn't set a nickname)
		public string nickname {
			get {
				// If the player has a default name, replace it with their actor number
				if(photonPlayer is object && photonPlayer.NickName == "You"
				  && localPlayer is object && photonPlayer != localPlayer)
					return "Player #" + actorNumber;
				return photonPlayer.NickName;
			}
			set => photonPlayer.NickName = value;
		}

		// Returns true if the player is ready
		public bool isReady {
			get {
				if(photonPlayer.CustomProperties[NetworkingManager.IS_PLAYER_READY] == null) return false;
				return (bool)photonPlayer.CustomProperties[NetworkingManager.IS_PLAYER_READY];
			}
		}


		// -- Serialization --


		// Function which serializes the player
		public void GetObjectData(SerializationInfo info, StreamingContext context){
			info.AddValue("photonPlayer", photonPlayer.ActorNumber, typeof(int));
			info.AddValue("side", side, typeof(Side));
			info.AddValue("role", role, typeof(Role));
		}

		// Constructor which creates a player from deserialized data
		public Player(SerializationInfo info, StreamingContext context){
			// Get the player's actor number
			int playerActorNumber = (int) info.GetValue("photonPlayer", typeof(int));
			// Find the photon player matching that actor number in the network list
			foreach(var player in PhotonNetwork.PlayerList)
				if(player.ActorNumber == playerActorNumber){
					photonPlayer = player;
					debugPhotonPlayer = playerActorNumber; // TODO: remove
					break;
				}

			// Deserialize the side and role of the player
			side = (Side) info.GetValue("side", typeof(Side));
			role = (Role) info.GetValue("role", typeof(Role));
		}


		// -- Photon Serialization --


		// Function which serializes a player into a byte array
		const int byteSize = sizeof(int) * 3; // The size of the output byte array
		public static byte[] PhotonSerialize(object customobject) {
			// Convert the input object to a player
			Player p = (Player)customobject;
			// Create a stream which will create our byte array
			MemoryStream ms = new MemoryStream(byteSize);

			// Serialize the data
			ms.Write(System.BitConverter.GetBytes(p.photonPlayer.ActorNumber), 0, 4);
			ms.Write(System.BitConverter.GetBytes((int) p.side), 0, 4);
			ms.Write(System.BitConverter.GetBytes((int) p.role), 0, 4);
			// Convert the serialized data to a byte array
			return ms.ToArray();
		}

		// Function which deserializes a byte array into a player
		public static object PhotonDeserialize(byte[] bytes) {
			// Deserialize the actor number, side, and role
			int actorNumber = System.BitConverter.ToInt32(bytes, 0);
			int _side = System.BitConverter.ToInt32(bytes, sizeof(int));
			int _role = System.BitConverter.ToInt32(bytes, sizeof(int) * 2);

			Player p = new Player();

			// Find the photon player matching that actor number in the network list
			foreach(var player in PhotonNetwork.PlayerList)
				if(player.ActorNumber == actorNumber){
					p.photonPlayer = player;
					p.debugPhotonPlayer = actorNumber; // TODO: remove
					Debug.Log("Found player: " + p.nickname + " (" + actorNumber + ")");
					break;
				}

			// Convert the side and role to their enum types
			p.side = (Side) _side;
			p.role = (Role) _role;

			return p;
		}


		// -- Equality Operator --


		// If we override Equals we have to override this function... just call the base version
		public override int GetHashCode(){ return base.GetHashCode(); }

		// Function which compares if a generic object equals this player
		public override bool Equals(System.Object other){
			// If the object is a player... check if they are equal
			if(other is Player p1) return Equals(p1);
			// If the object is a photon player... check if they are equal
			else if(other is Photon.Realtime.Player p2) return Equals(p2);
			// Otherwise... they are not equal
			return false;
		}

		// Check if two players are equal by comparing their photon players
		public bool Equals(Player other) => photonPlayer == other.photonPlayer;
		// Check if a player is equal to a photon player by comparing their photon players
		public bool Equals(Photon.Realtime.Player other) => photonPlayer == other;

		// All the permutations of equality operators between players and photon players
		public static bool operator==(Player a, Player b) => a.Equals(b);
		public static bool operator==(Photon.Realtime.Player a, Player b) => b.Equals(a);
		public static bool operator==(Player a, Photon.Realtime.Player b) => a.Equals(b);

		// All the permutations of inequality opperators between players and photon players
		public static bool operator!=(Player a, Player b) => !a.Equals(b);
		public static bool operator!=(Photon.Realtime.Player a, Player b) => !b.Equals(a);
		public static bool operator!=(Player a, Photon.Realtime.Player b) => !a.Equals(b);
	}
}
