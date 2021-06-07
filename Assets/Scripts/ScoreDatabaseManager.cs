using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// This manager is responsible for syncing score information with the remote database
public class ScoreDatabaseManager : MonoBehaviour
{
	// Manager setup code
	public static ScoreDatabaseManager inst;
	void Awake(){ inst = this; }

	// The URL of the database file
	private const string databaseURL = "https://www.cse.unr.edu/~jdahl/NetDefense-Backend/database.php";

	// Array of scores that were downloaded from the server (The downloadedLeaderboard function must be called to update this list);
	public UserScore[] downloadedLeaderboard;

	// This function uploads the score of the current player to the server
    public IEnumerator postScores(){
		// Put together the request URL
		SharedPlayer devicePlayer = Shared.inst.getDevicePlayer();
		string request = "?action=save&hat=" + (devicePlayer.role == SharedPlayer.WHITEHAT ? "white" : "black")
			+ "&name=" + UnityWebRequest.EscapeURL(devicePlayer.username) + "&score=" + (devicePlayer.role == SharedPlayer.WHITEHAT ? Shared.inst.gameMetrics.whitehat_score : Shared.inst.gameMetrics.blackhat_score );
		// TODO: How should we deal with acquiring the player's name? (Random based on IP or mac address?)

		// Send the store request
		UnityWebRequest www = UnityWebRequest.Get( databaseURL + request );
        yield return www.Send();

		// Check to see if there are any errors
        if(www.error != null) Debug.Log(www.error);
		else Debug.Log("Data successfully saved to server.");
	}

	// This function information
	public IEnumerator downloadScores(){
		// Put together the request URL
		string request = "?action=load&hat=" + (Shared.inst.getDevicePlayer().role == SharedPlayer.WHITEHAT ? "white" : "black");

		// Send the store request
		UnityWebRequest www = UnityWebRequest.Get( databaseURL + request );
        yield return www.Send();

		// Check to see if there are any errors
        if(www.error != null) Debug.Log(www.error);
		else {
			Debug.Log("Data successfully loaded from server.");

			// Deserialize the JSON into an array of player scores
			downloadedLeaderboard = JsonUtility.FromJson<UserScore.ArrayWrapper>("{\"items\":" + www.downloadHandler.text + "}").items; // (the json is modified to include an array called items, which is deseralized into the wrapper).
			Debug.Log(downloadedLeaderboard[0].name);
 		}
	}




	// This class represents a score packet that has been received from the server
	[Serializable]
	public class UserScore {
		public string uuid;
		public string name;
		public int score;

		// Wrapper which is used for deserializing an array of UserScores
		[Serializable]
	    public class ArrayWrapper {
	        public UserScore[] items;
	    }
	}
}
