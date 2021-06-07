using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// This manager is responsible for syncing score infromation with the remote database

public class ScoreDatabaseManager : MonoBehaviour
{
	// Manager code
	public static ScoreDatabaseManager inst;
	void Awake(){ inst = this; }

	// The url of the database file
	private const string databaseURL = "https://www.cse.unr.edu/~jdahl/NetDefense-Backend/database.php";

	// This function uploads the score of the current player to the server
    public IEnumerator postScores(){
		// Put together the request url
		SharedPlayer devicePlayer = Shared.inst.getDevicePlayer();
		string request = "?action=save&hat=" + (devicePlayer.role == SharedPlayer.WHITEHAT ? "white" : "black")
			+ "&name=" + UnityWebRequest.EscapeURL(devicePlayer.username) + "&score=" + (devicePlayer.role == SharedPlayer.WHITEHAT ? Shared.inst.gameMetrics.whitehat_score : Shared.inst.gameMetrics.blackhat_score );

		// Send the store request
		UnityWebRequest www = UnityWebRequest.Get( databaseURL + request );
        yield return www.Send();

		// Check to see if there are any errors
        if(www.isError) Debug.Log(www.error);
		else Debug.Log("Data successfullly saved");
	}
}
