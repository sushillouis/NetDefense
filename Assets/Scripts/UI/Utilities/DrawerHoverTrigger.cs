using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Script which changes the IsHovered state of a Drawer's animator when it gets hovered
public class DrawerHoverTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	// The animator to trigger
	public Animator animator;

	// Function called when the hovering begins
	public void OnPointerEnter(PointerEventData e) {
		animator.SetBool("IsHovered", true);
	}

	// Function called when the hovering ends
	public void OnPointerExit(PointerEventData e) {
		animator.SetBool("IsHovered", false);
	}

}
