using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WindowDrag : EnhancedUIBehavior, IDragHandler, IBeginDragHandler, IEndDragHandler {

	// Reference to the window which we are dragging around
	public Window dragTaget;

	// When the window's title bar is dragged then move the window appropriately
	public void OnDrag(PointerEventData e){
		dragTaget.rectTransform.anchoredPosition += e.delta / dragTaget.parentCanvas.scaleFactor;

		// If the drag movement would move the titlebar off the screen, then undo the movement
		if(!IsFullyVisibleFrom(Camera.main))
			dragTaget.rectTransform.anchoredPosition -= e.delta / dragTaget.parentCanvas.scaleFactor;
	}

	// Function which causes the window to become slightly transparent when dragged
	float savedAlpha; // Variable tracking the saved alpha value to restore it once done dragging
	public void OnBeginDrag(PointerEventData e){
		Image img = dragTaget.GetComponent<Image>();
		savedAlpha = img.color.a;
		Color newColor = img.color; newColor.a = .4f; img.color = newColor;
	}

	// Function which restores a window's transparency when it is done being dragged
	public void OnEndDrag(PointerEventData e){
		Image img = dragTaget.GetComponent<Image>();
		Color newColor = img.color; newColor.a = savedAlpha; img.color = newColor;
	}

}
