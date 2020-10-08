using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    //THIS SCRIPT SHOULD BE A SOUND_MANAGER INSTEAD AND HANDLE SOUND EFFECTS
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
