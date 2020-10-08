using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagerTest {

    [Test]
    public void UIManager_OnGameStateChanged_ShowBlackHatUiOnPlay() {
        Object ui = new Object();
        UIManagerModule module = new UIManagerModule((ui, UIConditions.isBlackHat));
        module.OnGameStateChanged(SharedGameStates.PLAY, SharedPlayer.BLACKHAT);
        Assert.IsTrue(module[ui]);
    }

    [Test]
    public void UIManager_OnGameStateChanged_HideBlackHatUiOnPlayifWhitehat() {
        Object ui = new Object();
        UIManagerModule module = new UIManagerModule((ui, UIConditions.isWhitehat));
        module.OnGameStateChanged(SharedGameStates.PLAY, SharedPlayer.BLACKHAT);
        Assert.IsFalse(module[ui]);
    }

    [Test]
    public void UIManager_OnGameStateChanged_ShowWhiteHatUiOnPlay() {
        Object ui = new Object();
        UIManagerModule module = new UIManagerModule((ui, UIConditions.isWhitehat));
        module.OnGameStateChanged(SharedGameStates.PLAY, SharedPlayer.WHITEHAT);
        Assert.IsTrue(module[ui]);
    }

    [Test]
    public void UIManager_OnGameStateChanged_HideWhiteHatUiOnPlayifBlackHat() {
        Object ui = new Object();
        UIManagerModule module = new UIManagerModule((ui, UIConditions.isBlackHat));
        module.OnGameStateChanged(SharedGameStates.PLAY, SharedPlayer.WHITEHAT);
        Assert.IsFalse(module[ui]);
    }


    [Test]
    public void UIManager_OnGameStateChanged_ShowLobby() {
        Object ui = new Object();
        UIManagerModule module = new UIManagerModule((ui, UIConditions.isLobbyUI));
        module.OnGameStateChanged(SharedGameStates.LOBBY, SharedPlayer.WHITEHAT);
        Assert.IsTrue(module[ui]);
    }

    [Test]
    public void UIManager_OnGameStateChanged_ShowGraph() {
        Object ui = new Object();
        UIManagerModule module = new UIManagerModule((ui, UIConditions.isGraph));
        module.OnGameStateChanged(SharedGameStates.PLAY, SharedPlayer.WHITEHAT);
        Assert.IsTrue(module[ui]);
        module.OnGameStateChanged(SharedGameStates.PLAY, SharedPlayer.BLACKHAT);
        Assert.IsTrue(module[ui]);
    }
}
