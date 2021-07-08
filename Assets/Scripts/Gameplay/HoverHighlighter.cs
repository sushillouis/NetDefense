using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HoverHighlighter : MonoBehaviour, HoverManager.IHoverable /* It has to be IHoverable so that the HoverManager can detect it*/ {

	// Object which is enabled when we hover over this object
	public GameObject highlightObject;

	// De/Register this object with the hover manager when it is dis/enabled.
	void OnEnable(){ HoverManager.hoverChanged += OnHoverChanged; }
	void OnDisable(){ HoverManager.hoverChanged -= OnHoverChanged; }

	// Callback which changes the hovered state of the object
	public void OnHoverChanged(GameObject newHover){
		if(newHover == gameObject)
			requestHoverEnable(true);
		else if(HoverManager.instance.hovered == gameObject && newHover != gameObject)
			requestHoverEnable(false);
	}

	// Function which enables the hovering logic
	void requestHoverEnable(bool enable){
		highlightObject.SetActive(enable);
	}
}
