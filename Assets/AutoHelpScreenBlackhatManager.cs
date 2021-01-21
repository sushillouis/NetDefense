using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoHelpScreenBlackhatManager : MonoBehaviour
{
    public GameObject dialogue;

    public Text confirmTarget;
    public bool hasConfirmedTarget;

    public Text confirmType;
    public bool hasConfirmedType;

    public bool isReady;

    public static AutoHelpScreenBlackhatManager inst;

    private void Awake() {
        inst = this;
        hasConfirmedTarget = false;
        isReady = false;
        hasConfirmedType = false;
        dialogue.SetActive(false);
    }

    public void OnReady() {
        isReady = true;
        OnUpdated();
    }


    public void OnConfirmedTarget() {
        Destroy(confirmTarget.gameObject);
        hasConfirmedTarget = true;
        OnUpdated();
    }

    public void OnConfirmedType() {
        Destroy(confirmType.gameObject);
        hasConfirmedType = true;
        OnUpdated();
    }

    private void OnUpdated() {
       dialogue.SetActive(isReady && (!hasConfirmedTarget || !hasConfirmedType));
    }
}
