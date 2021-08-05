using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionManager : Core.Utilities.Singleton<SelectionManager> {
	// Path to the selection cylinder prefab
	public const string SELECTION_CYLINDER_PREFAB_PATH = "SelectionCylinder/SelectionCylinder";

	// Interface objects must implement to be selectable
	public interface ISelectable { }

	// Callbacks
	public delegate void FirwallCallback(Firewall newSelect);
	public delegate void PacketCallback(Packet newSelect);
	public delegate void StartingPointCallback(StartingPoint newSelect);
	public delegate void DestinationCallback(Destination newSelect);
	// Events
	public static FirwallCallback firewallSelectEvent;
	public static PacketCallback packetSelectEvent;
	public static StartingPointCallback startingPointSelectEvent;
	public static DestinationCallback destinationSelectEvent;
	public static Utilities.VoidEventCallback deselectEvent;

	// Reference the mouse's position
	public InputActionReference mousePositionAction;
	// Reference to the selectionCylinder showing the currently selected item
	public GameObject selectionCylinder;

	// Property tracking the currently selected item
	GameObject _selected;
	public GameObject selected {
		get => _selected;
		set {
			_selected = value;
			// When we select something wrap the selection cylinder around it
			if(_selected != null) AttachSelectionCylinderToObject(_selected);
			// When we deselect something hide the selection cylinder
			else {
				selectionCylinder.transform.parent = transform;
				selectionCylinder.SetActive(false);
			}
		}
	}

	// Function which updates the selection to whatever is currently under the mouse
	RaycastHit hit; // Raycast target
	public void SelectUnderCursor(bool shouldTriggerEvents = true){
		// If the selection cyldinder gets deleted, then spawn a new one
		if(selectionCylinder == null)
			selectionCylinder = Instantiate(Resources.Load(SELECTION_CYLINDER_PREFAB_PATH) as GameObject, Vector3.zero, Quaternion.identity);

		// Reset the selection cylinder's parent
		selectionCylinder.transform.parent = transform;
		selectionCylinder.SetActive(false);

		// Perform the raycast
		if(Physics.Raycast( Camera.main.ScreenPointToRay(mousePositionAction.action.ReadValue<Vector2>()), out hit, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide )){
			// If we hit a firewall...
			if(hit.transform.tag == "Firewall"){
				// Select it
				selected = hit.transform.gameObject;

				// Trigger a firewall selection event (if we are triggering events)
				if(shouldTriggerEvents)
					firewallSelectEvent?.Invoke(selected.GetComponent<Firewall>());
			// If we hit a packet...
			} else if(hit.transform.tag == "Packet"){
				// Select it
				_selected = hit.transform.gameObject;
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
			// If we hit a starting point...
			} else if(hit.transform.tag == "StartingPoint"){
				// Select it
				selected = hit.transform.gameObject;

				// Trigger a StartingPoint selection event (if we are triggering events)
				if(shouldTriggerEvents)
					startingPointSelectEvent?.Invoke(selected.GetComponent<StartingPoint>());
			// If we hit a destination...
			} else if(hit.transform.tag == "Destination"){
				// Select it
				selected = hit.transform.gameObject;

				// Trigger a StartingPoint selection event (if we are triggering events)
				if(shouldTriggerEvents)
					destinationSelectEvent?.Invoke(selected.GetComponent<Destination>());
			}
		// If the raycast failed to find anything trigger a deselect event (if we are triggering events)
		} else if(selected != null){
			selected = null;
			if(shouldTriggerEvents) deselectEvent?.Invoke();
		}
	}

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
