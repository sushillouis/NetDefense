using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class HoverManager : MonoBehaviour {
	// Interace which defines that an object is hoverable
	public interface IHoverable {
		void OnHoverChanged(GameObject newHover);
	}

	// Callback for when the currently hovered item changes
	public delegate void HoverEventCallback(GameObject newHover);
	public static HoverEventCallback hoverChanged;

	// Reference to the currently hovered item
	public static GameObject currentHover = null;
	// Reference to the mouse position action
	public InputActionReference mousePositionAction;

	//  De/Register as a mouse position listener when it is Dis/enabled.
	void OnEnable(){
		mousePositionAction.action.Enable(); // Make sure the mouse position action is enabled
		mousePositionAction.action.performed += OnMouseMoved;
	}
	void OnDisable(){ mousePositionAction.action.performed -= OnMouseMoved; }

	// Callback which handles mouse movements
	RaycastHit hit; // Raycast hit used in the callback
	List<IHoverable> hoverables; // List of components that implement the hoverable interface
	void OnMouseMoved(InputAction.CallbackContext ctx){
		// Ensure that there is a current camera (not 100% sure why this is necessary, but it throws null exceptions if not present)
		Camera currentCamera = Camera.current;
		if(currentCamera != null)
			// When the mouse moves raycast into the scene
			if(Physics.Raycast( currentCamera.ScreenPointToRay(ctx.ReadValue<Vector2>()), out hit )){
				// If we hit something different than we are currently hovering over
				if(hit.transform && hit.transform.gameObject != currentHover){
					// If the object has hover logic, then fire the hover changed event
					Utilities.GetInterfaces(out hoverables, hit.transform.gameObject);
					if(hoverables.Count > 0){
						hoverChanged(hit.transform.gameObject);
						currentHover = hit.transform.gameObject;
					}
				}
			// If the raycast failed to hit anything, then the current hover is null
			} else {
				hoverChanged(null);
				currentHover = null;
			}
	}
}
