﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIConditions {

    public delegate bool isValid(SharedGameStates state, int role);

    public static isValid isBlackHat = (state, role) => state == SharedGameStates.PLAY && role == SharedPlayer.BLACKHAT;
    
    public static isValid isGraph = (state, role) => (state == SharedGameStates.PLAY || state == SharedGameStates.OFFLINE) && (state == SharedGameStates.PLAY && role == SharedPlayer.BLACKHAT || state == SharedGameStates.PLAY && role == SharedPlayer.WHITEHAT);

    //public static isValid isWhitehat = (state, role) => (state == SharedGameStates.PLAY && role == SharedPlayer.WHITEHAT) || state == SharedGameStates.OFFLINE;
    public static isValid isWhitehat = (state, role) => !isBlackHat(state, role);

    public static isValid isLobbyUI = (state, role) => state == SharedGameStates.LOBBY;
    
    public static isValid isCountDownUI = (state, role) => state == SharedGameStates.COUNTDOWN;

    public static isValid isGameOver = (state, role) => state == SharedGameStates.OVER;

    public static isValid isPause = (state, role) => {
        bool value = state == SharedGameStates.PAUSE;
        if (value) {
            Time.timeScale = value ? 0.01f : 1f;
        }
        return value;

    };

}

