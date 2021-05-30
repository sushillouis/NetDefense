using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public GameObject graph;
    public Displaceable blackhatUI;
    public Displaceable whitehatUI;
    public Displaceable lobbyUI;
    public Displaceable countdownUI;
    public Displaceable gameOver;
    public Displaceable pauseMenu;

    [SerializeField]
    private UIManagerModule module;

    public static UIManager inst;

    private void Awake() {
        inst = this;
        module = new UIManagerModule(
            (blackhatUI, UIConditions.isBlackHat),
            (graph, UIConditions.isGraph),
            (whitehatUI, UIConditions.isWhitehat),
            (lobbyUI, UIConditions.isLobbyUI),
            (countdownUI, UIConditions.isCountDownUI),
            (gameOver, UIConditions.isGameOver),
            (pauseMenu, UIConditions.isPause));
    }

    private void Update() {
       /* module.OnGameStateChanged(Shared.inst.gameState.currentState, Shared.inst.getDevicePlayer().role);
        graph.SetActive(module[graph]);
        blackhatUI.gameObject.SetActive(module[blackhatUI]);
        whitehatUI.gameObject.SetActive(module[whitehatUI]);
        lobbyUI.isValid = module[lobbyUI];
        countdownUI.isValid = module[countdownUI];
        gameOver.isValid = module[gameOver];
        pauseMenu.isValid = module[pauseMenu];*/
    }


    public void OnGameStateChanged(SharedGameStates state) {
        module.OnGameStateChanged(state, Shared.inst.getDevicePlayer().role);

        graph.SetActive(module[graph]);
        blackhatUI.isValid = module[blackhatUI];
        whitehatUI.isValid = module[whitehatUI];
        lobbyUI.isValid = module[lobbyUI];
        countdownUI.isValid = module[countdownUI];
        gameOver.isValid = module[gameOver];
        pauseMenu.isValid = module[pauseMenu];

        if(countdownUI.isValid) {
            blackhatUI.isValid = false;
            whitehatUI.isValid = false;
        }

    }
}
