using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WindowDrag : EnhancedUIBehavior, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler {

	// Reference to the window which we are dragging around
	public Window dragTaget;
	// Reference to the window's content
	public ThemedPanel windowContent;

	// When a window is clicked, if it is outside of the window (can't be dragged) snap it in bounds
	public void OnPointerDown(PointerEventData e){
		// While the window title bar is not full visible, shift it slightly towards the center of the screen
		while(!IsFullyVisibleFrom(Camera.main))
			dragTaget.transform.position = Vector3.Lerp(dragTaget.transform.position, Camera.main.pixelRect.center, .01f);

		// Make sure the windows's pointer down function is also called
		dragTaget.OnPointerDown(e);
	}

	// When the window's title bar is dragged then move the window appropriately
	public void OnDrag(PointerEventData e){
		dragTaget.rectTransform.anchoredPosition += e.delta / dragTaget.parentCanvas.scaleFactor;

		// If the drag movement would move the titlebar off the screen, then undo the movement
		if(!IsFullyVisibleFrom(Camera.main))
			dragTaget.rectTransform.anchoredPosition -= e.delta / dragTaget.parentCanvas.scaleFactor;
	}

	// Function which causes the window to become slightly transparent when dragged
	float[] savedAlpha = new float[2]; // Variable tracking the saved alpha value to restore it once done dragging
	public void OnBeginDrag(PointerEventData e){
		Image img = dragTaget.GetComponent<Image>();
		// Save the alpha of the content and its background
		savedAlpha[0] = img.color.a;
		savedAlpha[1] = windowContent.color.a;
		// Set the alpha of the content and its background to 40%
		Color newColor = img.color; newColor.a = .4f; img.color = newColor;
		newColor = windowContent.color; newColor.a = .4f; windowContent.color = newColor;
	}

	// Function which restores a window's transparency when it is done being dragged
	public void OnEndDrag(PointerEventData e){
		Image img = dragTaget.GetComponent<Image>();
		// Restore the saved alpha of the content and its background
		Color newColor = img.color; newColor.a = savedAlpha[0]; img.color = newColor;
		newColor = windowContent.color; newColor.a = savedAlpha[1]; windowContent.color = newColor;
	}

}
