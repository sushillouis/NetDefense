using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class WhiteHatPlayerManager : WhiteHatBaseManager {

	// Reference to the click action
	public InputActionReference leftClickAction;
	// Reference to the right click action
	public InputActionReference rightClickAction;
	// Reference to the cancel action
	public InputActionReference cancelAction;
	// Reference to the GUI element where we drop error messages
	public TMPro.TextMeshProUGUI errorText;

	public GameObject routerCursor;

	// Enum what a click currently means
	enum ClickState {
		Selecting,
		SpawningFirewall,
		SelectingFirewallToMove,
		MovingFirewall,
	}
	// Variable defining what should happen when we click
	ClickState clickState = ClickState.Selecting;


	// De/register the click listener as well as Selection Manager event listeners
	void OnEnable(){
		leftClickAction.action.Enable();
		leftClickAction.action.performed += OnClickPressed;
		rightClickAction.action.Enable();
		rightClickAction.action.performed += OnCancel;
		cancelAction.action.Enable();
		cancelAction.action.performed += OnCancel;
		HoverManager.hoverChanged += OnHoverChanged;
		SelectionManager.packetSelectEvent += OnPacketSelected;
	}
	void OnDisable(){
		leftClickAction.action.performed -= OnClickPressed;
		rightClickAction.action.performed -= OnCancel;
		cancelAction.action.performed -= OnCancel;
		HoverManager.hoverChanged -= OnHoverChanged;
		SelectionManager.packetSelectEvent -= OnPacketSelected;
	}


	// -- Callbacks --


	// Function which responds to UI buttons that change the current click state
	public void OnSetClickState(int clickState){
		this.clickState = (ClickState)clickState;
	}

	// Function which responds to the remove selected firewall button
	public void OnRemoveSelectedFirewall(){
		Firewall selected = SelectionManager.instance.selected?.GetComponent<Firewall>();
		SelectionManager.instance.selected = null; // Make sure that the selection manager is not pointed at the item when we delete it
		DestroyFirewall(selected);
	}

	// Callback which responds to click events (ignoring cick release events and events already handled by the UI)
	void OnClickPressed(InputAction.CallbackContext ctx){
		// Ignore click releases
		if(!ctx.ReadValueAsButton()) return;
		// Ignore UI clicks
		if(EventSystem.current.IsPointerOverGameObject()) return;

		switch(clickState){
			case ClickState.Selecting: SelectionManager.instance.SelectUnderCursor(); break; // If we are selecting, simply tell the selection manager to select whatever is under the mouse
			case ClickState.SpawningFirewall: OnClick_SpawningFirewall(); break;
			case ClickState.SelectingFirewallToMove: OnClick_SelectingFirewallToMove(); break;
			case ClickState.MovingFirewall: OnClick_MovingFirewall(); break;
		}
	}

	void OnCancel(InputAction.CallbackContext ctx){
		// Ignore click releases
		if(!ctx.ReadValueAsButton()) return;
		// Ignore UI clicks
		if(EventSystem.current.IsPointerOverGameObject()) return;

		clickState = ClickState.Selecting;
		OnHoverChanged(HoverManager.instance.hovered);
	}

	// Callback which handles when the selected packet changes
	void OnPacketSelected(Packet p){
		// Simply print out the details of the packet
		Debug.Log(p.details);
	}

	// Callback which handles when the currently hovered grid piece changes
	void OnHoverChanged(GameObject newHover){
		if( newHover == null 					// If there isn't a new hover...
		  || newHover.tag != "FirewallTarget"	// Or the hover target can't host a firewall...
		  // Or we aren't in a state where we need the firewall placement indicator...
		  || !(clickState == ClickState.SpawningFirewall || clickState == ClickState.MovingFirewall || (clickState == ClickState.SelectingFirewallToMove && SelectionManager.instance.selected?.GetComponent<Firewall>() != null))
		){
			// Disable the firewall cursor
			routerCursor.SetActive(false);
			return;
		}

		// Otherwise enable the firewall cursor and snap it to the hovered point
		routerCursor.SetActive(true);
		routerCursor.transform.position = newHover.transform.position;
		routerCursor.transform.rotation = newHover.transform.rotation;
	}


	// -- Click Events --


	// Function which handles clicks when we should be placing firewalls
	void OnClick_SpawningFirewall(){
		// Place a firewall at the currently hovered path piece (the base class takes care of most error handling)
		Firewall spawned = SpawnFirewall(HoverManager.instance.hovered);
		// If we succeded, mark the new fire wall as selected and reset the click state
		if(spawned != null){
			SelectionManager.instance.selected = spawned.gameObject;
			clickState = ClickState.Selecting;

			// Make sure the placement cursor is hidden
			OnHoverChanged(HoverManager.instance.hovered);
		}
	}

	// Function which handles clicks when we should be selecting a firewall to move
	void OnClick_SelectingFirewallToMove(){
		// If we need to select a firewall...
		if(SelectionManager.instance.selected?.GetComponent<Firewall>() == null){
			// Tell the selection manager to select whatever is under it
			SelectionManager.instance.SelectUnderCursor(/*No events*/ false);

			// If its selection isn't a firewall, give the user an error message
			if(SelectionManager.instance.selected == null || SelectionManager.instance.selected.GetComponent<Firewall>() == null){
				SelectionManager.instance.selected = null;
				ErrorHandler(ErrorCodes.FirewallNotSelected, "A Firewall to move must be selected!");
				return;
			}

			// If the selection is a firewall, switch the state so the next click moves the selected firewall
			clickState = ClickState.MovingFirewall;
		// If we already have a firewall selected, simply move the selected firewall
		} else
			OnClick_MovingFirewall();
	}

	// Function which handles clicks when we are supposed to be moving firewalls
	void OnClick_MovingFirewall(){
		if(MoveFirewall(SelectionManager.instance.selected.GetComponent<Firewall>(), HoverManager.instance.hovered)){
			clickState = ClickState.Selecting;

			// Make sure the placement cursor is hidden
			OnHoverChanged(HoverManager.instance.hovered);
		}
	}


	// -- Error Handling --


	// Override the error handler.
	protected override void ErrorHandler(ErrorCodes errorCode, string error){
		// Continue all of the logic in the base handler
		base.ErrorHandler(errorCode, error);

		// But also present the new error to the screen
		errorText.text = error;
	}
}
