using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum LobbyStates {
    JOIN_OR_HOST,
    JOIN_BY_IP_STATE,
    READY_UP_STATE,
    WAITING_TO_BEGIN_STATE,
    NETWORK_DISCOVERY_STATE
}

public class LobbyMenuManager : MonoBehaviour {

    private LobbyStates _currentState;

    public LobbyStates HostOrJoinState;

    public LobbyStates CurrentState {
        get { return _currentState; }

        set {
            _currentState = value;

            joinOrHost.SetActive(_currentState == LobbyStates.JOIN_OR_HOST);
            joinSelection.SetActive(_currentState == LobbyStates.JOIN_BY_IP_STATE);
            playerSettings.SetActive(_currentState == LobbyStates.READY_UP_STATE);
            lobbyPlayerList.SetActive(_currentState == LobbyStates.WAITING_TO_BEGIN_STATE);
            networkDiscovery.SetActive(_currentState == LobbyStates.NETWORK_DISCOVERY_STATE);
        }

    }

    public GameObject joinOrHost;
    public GameObject joinSelection;
    public GameObject playerSettings;
    public GameObject lobbyPlayerList;
    public GameObject networkDiscovery;

    public Button hostOrjoinDoneButton;
    [SerializeField]
    private Color originalColorhostOrjoinDoneButton;

    public Button joinButton;
    public Button hostButton;

    public Text nameText;


    [SerializeField]
    private Color originalJoinButtonColor;
    [SerializeField]
    private Color originalHostButtonColor;

    public Button joinByIpDoneButton;
    [SerializeField]
    private Color originalJoinByIpDoneButton;

    public Button lobbyListDoneButton;
    [SerializeField]
    private Color orginiallobbyListDoneButtonColor;

    public Button readyUpDoneButton;
    [SerializeField]
    private Color originalReadyButtonColor;

    public Button blackhatButton;
    public Button whitehatButton;
    [SerializeField]
    private Color originaBlkButtonColor;
    [SerializeField]
    private Color originalWhtButtonColor;

    [SerializeField]
    private int roleValue;

    [SerializeField]
    private bool isReady;

    public Text usernameText;
    public Text readyButtonText;
    public Text clientText;
    public Text hostText;
    public Text titlePlayerList;

    public NetworkManagerHLAPI network;
    public InputField IpInputField;

    public bool hasPressedBegin;

    public static LobbyMenuManager inst;

    // public Dictionary<int, ConnectedPlayerPanel> connected_players_ui;

    private void Awake() {
        inst = this;
        //  connected_players_ui = new Dictionary<int, ConnectedPlayerPanel>();
    }

    private void Start() {
        CurrentState = LobbyStates.JOIN_OR_HOST;
        originalReadyButtonColor = readyUpDoneButton.image.color;
        orginiallobbyListDoneButtonColor = lobbyListDoneButton.image.color;
        originalJoinByIpDoneButton = joinByIpDoneButton.image.color;
        originalJoinButtonColor = joinButton.image.color;
        originalHostButtonColor = hostButton.image.color;
        originalColorhostOrjoinDoneButton = hostOrjoinDoneButton.image.color;

        originaBlkButtonColor = blackhatButton.image.color;
        originalWhtButtonColor = whitehatButton.image.color;

        HostOrJoinState = LobbyStates.JOIN_OR_HOST;
    }

    void Update() {


        if (CurrentState == LobbyStates.JOIN_OR_HOST) {
            hostOrjoinDoneButton.enabled = HostOrJoinState != LobbyStates.JOIN_OR_HOST;
            hostOrjoinDoneButton.image.color = hostOrjoinDoneButton.enabled ? originalColorhostOrjoinDoneButton : new Color(originalColorhostOrjoinDoneButton.r, originalColorhostOrjoinDoneButton.g, originalColorhostOrjoinDoneButton.b, 0.25f);
        }

        if (CurrentState == LobbyStates.READY_UP_STATE) {
            readyUpDoneButton.enabled = (usernameText.text.Length != 0 && roleValue != 0);
            if (readyUpDoneButton.enabled) {
                readyUpDoneButton.image.color = originalReadyButtonColor;
            } else {
                readyUpDoneButton.image.color = new Color(originalReadyButtonColor.r, originalReadyButtonColor.g, originalReadyButtonColor.b, .25f);

            }
        }

        if (CurrentState == LobbyStates.JOIN_BY_IP_STATE) {
            joinByIpDoneButton.enabled = (IpInputField.text.Length != 0);
            readyUpDoneButton.image.color = (joinByIpDoneButton.enabled ? originalJoinByIpDoneButton : new Color(originalJoinByIpDoneButton.r, originalJoinByIpDoneButton.g, originalJoinByIpDoneButton.b, .25f));
        }

        if (CurrentState == LobbyStates.WAITING_TO_BEGIN_STATE) {

            lobbyListDoneButton.enabled = isReadyToLaunchGame() && Shared.inst.getDevicePlayer().isHost;
            lobbyListDoneButton.image.color = (lobbyListDoneButton.enabled ? orginiallobbyListDoneButtonColor : new Color(orginiallobbyListDoneButtonColor.r, orginiallobbyListDoneButtonColor.g, orginiallobbyListDoneButtonColor.b, 0.25f));

            foreach (SharedPlayer p in Shared.inst.players) {
                if (p.isHost) {
                    hostText.text = p.username + " is " + (p.role == 1 ? "Blackhat" : "Whitehat") + " (Host)";
                } else {
                    clientText.text = p.username.Length == 0 ? "Connected" : (p.username + " is " + (p.role == 1 ? "Blackhat" : "Whitehat") + " (Joined)");
                }
            }

            if (Shared.inst.players.Count == 2) {
                titlePlayerList.text = Shared.inst.players[0].username + " vs. " + Shared.inst.players[1].username;
            }
        }

    }


    public void OnBlackhatButtonSelected() {
        roleValue = 1;
        blackhatButton.image.color = new Color(originaBlkButtonColor.r * 1.25f, originaBlkButtonColor.g * 1.25f, originaBlkButtonColor.b * 1.25f, 1);
        whitehatButton.image.color = originalWhtButtonColor;
    }

    public void OnWhitehatButtonSelected() {
        roleValue = 2;
        whitehatButton.image.color = new Color(originalWhtButtonColor.r * 1.25f, originalWhtButtonColor.g * 1.25f, originalWhtButtonColor.b * 1.25f, 1);
        blackhatButton.image.color = originaBlkButtonColor;
    }


    public void OnHostButtonSelected() {
        HostOrJoinState = LobbyStates.READY_UP_STATE;
        hostButton.image.color = new Color(originalHostButtonColor.r * 1.25f, originalHostButtonColor.g * 1.25f, originalHostButtonColor.b * 1.25f, 1);
        joinButton.image.color = originalJoinButtonColor;
    }

    public void OnJoinButtonSelected() {
        HostOrJoinState = LobbyStates.JOIN_BY_IP_STATE;
        hostButton.image.color = originalHostButtonColor;
        joinButton.image.color = new Color(originalJoinButtonColor.r * 1.25f, originalJoinButtonColor.g * 1.25f, originalJoinButtonColor.b * 1.25f, 1);
    }

    public void OnPressedBackWaitingtoBegin() {
        CurrentState = LobbyStates.READY_UP_STATE;
        roleValue = 0;
        blackhatButton.image.color = originaBlkButtonColor;
        whitehatButton.image.color = originalWhtButtonColor;
        UpdateLobbyPlayerChanges();
    }


    public void OnNewPlayerConnected(SharedPlayer p) {
        UpdateLobbyPlayerChanges();
    }

    public void OnSettingsChanged(int net_id) {
        /* connected_players_ui[net_id].username = Shared.inst.getOrAddPlayerById(net_id).username;
         connected_players_ui[net_id].isReady = Shared.inst.getOrAddPlayerById(net_id).isReady;
         connected_players_ui[net_id].role = Shared.inst.getOrAddPlayerById(net_id).role == 0 ? "Blackhat" : "Whitehat";
         connected_players_ui[net_id].OnSettingsChanged();*/
    }


    public bool isReadyToLaunchGame() {

        foreach (SharedPlayer np in Shared.inst.players) {
            if (!np.isReady) {
                return false;
            }
        }

        if (Shared.inst.players.Count != 2) {
            return false;
        }

        SharedPlayer p1 = Shared.inst.players[0];
        SharedPlayer p2 = Shared.inst.players[1];

        if (p1.role == p2.role || p1.role == 0 || p2.role == 0)
            return false;

        return true;
    }


    public void OnJoinOrHostDoneButtonSelected() {
        CurrentState = HostOrJoinState;
        if (HostOrJoinState == LobbyStates.READY_UP_STATE) {
            network.LaunchHost(true);
        }
    }

    public void UpdateLobbyPlayerChanges() {
        isReady = CurrentState == LobbyStates.WAITING_TO_BEGIN_STATE;
        Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.SET_LOBBY_PLAYER_SETTINGS, SharedPlayer.playerIdForThisDevice + "," + usernameText.text + "," + isReady + "," + roleValue));
    }

    public void OnReadyUpDoneButtonSelected() {
        CurrentState = LobbyStates.WAITING_TO_BEGIN_STATE;
        UpdateLobbyPlayerChanges();
    }

    public void OnJoinByIpDoneSelected() {
        network.networkValues.ip = IpInputField.text;
        network.LaunchClient();
        CurrentState = LobbyStates.READY_UP_STATE;
    }

    public void OnLobbyPlayersListBeginButtonSelected() {
        hasPressedBegin = true;
    }

    public void OnLaunchNetworkDiscovery() {
        CurrentState = LobbyStates.NETWORK_DISCOVERY_STATE;
        NetworkDiscoveryManager.inst.StartAsClient();
    }

    public void OnQuitButtonSelected() {
        Shared.inst.OnQuitButtonPressed();
    }
}
