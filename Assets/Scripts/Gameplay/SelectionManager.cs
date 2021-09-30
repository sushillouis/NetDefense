using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionManager : Core.Utilities.Singleton<SelectionManager> {
	// Path to the selection cylinder prefab
	public const string SELECTION_CYLINDER_PREFAB_PATH = "SelectionCylinder/SelectionCylinder";

	// Interface objects must implement to be selectable
	public interface ISelectable { }

	// Interface which defines that an object is hoverable
	public interface IHoverable {
		void OnHoverChanged(GameObject newHover);
	}

	// Callbacks
	public delegate void FirwallCallback(Firewall newSelect);
	public delegate void SuggestedFirwallCallback(SuggestedFirewall newSelect);
	public delegate void PacketCallback(Packet newSelect);
	public delegate void StartingPointCallback(StartingPoint newSelect);
	public delegate void DestinationCallback(Destination newSelect);
	public delegate void HoverEventCallback(GameObject newHover);
	// Events
	public static FirwallCallback firewallSelectEvent;
	public static SuggestedFirwallCallback suggestedFirewallSelectEvent;
	public static PacketCallback packetSelectEvent;
	public static StartingPointCallback startingPointSelectEvent;
	public static DestinationCallback destinationSelectEvent;
	public static Utilities.VoidEventCallback deselectEvent;
	public static HoverEventCallback hoverChangedEvent;


	// Reference the mouse's position
	public InputActionReference mousePositionAction;
	// Reference to the selectionCylinder showing the currently selected item
	public GameObject selectionCylinder;

	// Property tracking the currently selected item
	[Header("Debugging")]
	[SerializeField] GameObject _selected;
	public GameObject selected {
		get => _selected;
		protected set {
			_selected = value;
			// When we select something wrap the selection cylinder around it
			if(_selected is object) AttachSelectionCylinderToObject(_selected);
			// When we deselect something hide the selection cylinder
			else {
				selectionCylinder.transform.parent = transform;
				selectionCylinder.SetActive(false);
			}
		}
	}

	// Reference to the currently hovered item
	public GameObject hovered = null;


	//  De/Register as a mouse position listener when it is Dis/enabled.
	void OnEnable(){
		mousePositionAction.action.Enable(); // Make sure the mouse position action is enabled
		mousePositionAction.action.performed += OnMouseMoved;
	}
	void OnDisable(){ mousePositionAction.action.performed -= OnMouseMoved; }


	// Callback which handles mouse movements (updating the currently hovered object)
	RaycastHit hit; // Raycast target
	List<IHoverable> hoverables; // List of components that implement the hoverable interface
	void OnMouseMoved(InputAction.CallbackContext ctx){
		// Ensure that there is a current camera (not 100% sure why this is necessary, but it throws null exceptions if not present)
		Camera currentCamera = Camera.main;
		if(currentCamera != null)
			// When the mouse moves raycast into the scene
			if(Physics.Raycast( currentCamera.ScreenPointToRay(ctx.ReadValue<Vector2>()), out hit )){
				// If we hit something different than we are currently hovering over
				if(hit.transform && hit.transform.gameObject != hovered){
					// If the object has hover logic, then fire the hover changed event
					Utilities.GetInterfaceInstances(out hoverables, hit.transform.gameObject);
					if(hoverables.Count > 0){
						hoverChangedEvent?.Invoke(hit.transform.gameObject);
						hovered = hit.transform.gameObject;
					}
				}
			// If the raycast failed to hit anything, then the current hover is null
			} else {
				if(hovered is object) hoverChangedEvent?.Invoke(null);
				hovered = null;
			}
	}


	// Function which updates the selection to the provided game object (fireing all nessicary events)
	public void SelectGameObject(GameObject obj, bool shouldTriggerEvents = true){
		// If the provided object is null, then nothing is selecetd
		if(obj is null){
			GameObject oldSelected = selected;
			selected = null;
			if(shouldTriggerEvents && oldSelected is object) deselectEvent?.Invoke();
			return;
		}

		// If we were passed a firewall...
		if(obj.transform.tag == "Firewall"){
			// Select it
			selected = obj.transform.gameObject;

			// Trigger a firewall selection event (if we are triggering events)
			if(shouldTriggerEvents)
				firewallSelectEvent?.Invoke(selected.GetComponent<Firewall>());
		// If we were passed a suggested firewall...
		} else if(obj.transform.tag == "SuggestedFirewall"){
			// Select it
			selected = obj.transform.gameObject;

			// Trigger a suggested firewall selection event (if we are triggering events)
			if(shouldTriggerEvents)
				suggestedFirewallSelectEvent?.Invoke(selected.GetComponent<SuggestedFirewall>());
		// If we were passed a packet...
		} else if(obj.transform.tag == "Packet"){
			// Select it
			_selected = obj.transform.gameObject;
			Packet p = selected.GetComponent<Packet>();

			// We have to jump through some extra hoops, by temporarily enabling the malicious selection cylinder so that our selection cylinder is the correct size
			bool savedActive = p.selectionCylinder.activeSelf;
			p.selectionCylinder.SetActive(true);
			AttachSelectionCylinderToObject(selected, 1); // Set the scale to 1 so that the selection cylinder is the same size as the packet's selection indicator
			// Move our selection cylinder so that it is just above the one possibly already present
			selectionCylinder.transform.position += new Vector3(0, .01f, 0);
			p.selectionCylinder.SetActive(savedActive);

			// Trigger a packet selection event (if we are triggering events)
			if(shouldTriggerEvents)
				packetSelectEvent?.Invoke(p);
		// If we were passed a starting point...
		} else if(obj.transform.tag == "StartingPoint"){
			// Select it
			selected = obj.transform.gameObject;

			// Trigger a StartingPoint selection event (if we are triggering events)
			if(shouldTriggerEvents)
				startingPointSelectEvent?.Invoke(selected.GetComponent<StartingPoint>());
		// If we were passed a destination...
		} else if(obj.transform.tag == "Destination"){
			// Select it
			selected = obj.transform.gameObject;

			// Trigger a StartingPoint selection event (if we are triggering events)
			if(shouldTriggerEvents)
				destinationSelectEvent?.Invoke(selected.GetComponent<Destination>());
		}
	}

	// Function which updates the selection to whatever is currently under the mouse
	public void SelectUnderCursor(bool shouldTriggerEvents = true){
		// If the selection cyldinder gets deleted, then spawn a new one
		if(selectionCylinder == null)
			selectionCylinder = Instantiate(Resources.Load(SELECTION_CYLINDER_PREFAB_PATH) as GameObject, Vector3.zero, Quaternion.identity);

		// Reset the selection cylinder's parent
		selectionCylinder.transform.parent = transform;
		selectionCylinder.SetActive(false);

		// Perform the raycast
		if(Physics.Raycast( Camera.main.ScreenPointToRay(mousePositionAction.action.ReadValue<Vector2>()), out hit, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide ))
			SelectGameObject(hit.transform.gameObject, shouldTriggerEvents);
		// If the raycast failed to find anything trigger a deselect event (if we are triggering events)
		else if(selected != null){
			GameObject oldSelected = selected;
			selected = null;
			if(shouldTriggerEvents && oldSelected is object) deselectEvent?.Invoke();
		}
	}


	// -- Helpers --


	// Function which attaches the selection cylinder to the provided game object, takes a scale which determines how much larger than the object the selection cylinder should be
	void AttachSelectionCylinderToObject(GameObject selection, float scaleMultiplier = 2){
		// Calculate how the cylinder needs to be scaled
		Bounds selectionBounds = Utilities.GetBoundsInChildren(selection);
		float scaleFactor = selectionBounds.extents.magnitude / GetSelectionCylinderMagnitude();
		// Adjust the cylinder's scale
		selectionCylinder.transform.localScale = new Vector3(scaleFactor, 0.0001f, scaleFactor) * scaleMultiplier;
		// Position, parent, and enable the cylinder
		selectionCylinder.transform.position = selectionBounds.center;
		selectionCylinder.transform.parent = selection.transform;
		selectionCylinder.SetActive(true);
	}

	// Function which returns the relative size of the selection cylinder, used to compare it to the object it is selecting for automatic sizing
	float GetSelectionCylinderMagnitude(){
		selectionCylinder.transform.localScale = new Vector3(1, 0.0001f, 1); // Ensure that the selection cylinder's local scale is reset first
		return selectionCylinder.GetComponent<MeshRenderer>().bounds.extents.magnitude;
	}
}
