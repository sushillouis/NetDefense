using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Window : EnhancedUIBehavior, IPointerDownHandler {

	// When a window is clicked, make sure it is drawn over all other UI elements
	public void OnPointerDown(PointerEventData e){
		transform.SetAsLastSibling();
	}

	// When the window's close button is pressed destroy it
	public virtual void OnCloseButtonPressed(){
		Destroy(gameObject);
	}

}
