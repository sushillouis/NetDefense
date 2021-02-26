using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour {

    public Displaceable[] dialogues;
    public GameObject holder;


    // Start is called before the first frame update
    void Start() {
        if (MainMenu.difficulty == Difficulty.EASY && MainMenu.hat == SharedPlayer.WHITEHAT) {
            dialogues[0].isValid = true;
            Time.timeScale = 0;
        }
    }

    public void OnClose(int index) {
        dialogues[index].isValid = false;
        holder.SetActive(false);
        Time.timeScale = 1;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
