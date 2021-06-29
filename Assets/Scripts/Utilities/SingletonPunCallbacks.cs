using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Core.Utilities {
	public class SingletonPunCallbacks<T>: Singleton<T>, IConnectionCallbacks, IMatchmakingCallbacks, IInRoomCallbacks, ILobbyCallbacks, IWebRpcCallback, IErrorInfoCallback where T : Singleton<T> {
		public virtual void OnEnable() { PhotonNetwork.AddCallbackTarget(this); }
		public virtual void OnDisable() { PhotonNetwork.RemoveCallbackTarget(this); }

		// IConnectionCallbacks
		public virtual void OnConnected () {}
		public virtual void OnConnectedToMaster () {}
		public virtual void OnDisconnected (DisconnectCause cause) {}
		public virtual void OnRegionListReceived (RegionHandler regionHandler) {}
		public virtual void OnCustomAuthenticationResponse (Dictionary<string, object> data) {}
		public virtual void OnCustomAuthenticationFailed (string debugMessage) {}

		// IMatchmakingCallbacks
		public virtual void OnFriendListUpdate (List<FriendInfo> friendList) {}
		public virtual void OnCreatedRoom () {}
		public virtual void OnCreateRoomFailed (short returnCode, string message) {}
		public virtual void OnJoinedRoom () {}
		public virtual void OnJoinRoomFailed (short returnCode, string message) {}
		public virtual void OnJoinRandomFailed (short returnCode, string message) {}
		public virtual void OnLeftRoom () {}

		// IInRoomCallbacks
		public virtual void OnPlayerEnteredRoom (Player newPlayer) {}
		public virtual void OnPlayerLeftRoom (Player otherPlayer) {}
		public virtual void OnRoomPropertiesUpdate (ExitGames.Client.Photon.Hashtable propertiesThatChanged) {}
		public virtual void OnPlayerPropertiesUpdate (Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) {}
		public virtual void OnMasterClientSwitched (Player newMasterClient) {}

		// ILobbyCallbacks
		public virtual void OnJoinedLobby () {}
		public virtual void OnLeftLobby () {}
		public virtual void OnRoomListUpdate (List<RoomInfo> roomList) {}
		public virtual void OnLobbyStatisticsUpdate (List<TypedLobbyInfo> lobbyStatistics) {}

		// IWebRpcCallback
		public virtual void OnWebRpcResponse (ExitGames.Client.Photon.OperationResponse response) {}

		// IErrorInfoCallback
		public virtual void OnErrorInfo (ErrorInfo errorInfo) {}
	}

	public class PersistentSingletonPunCallbacks<T> : SingletonPunCallbacks<T> where T : SingletonPunCallbacks<T> {
		protected override void Awake() {
			base.Awake();
			DontDestroyOnLoad(gameObject);
		}
	}
}
