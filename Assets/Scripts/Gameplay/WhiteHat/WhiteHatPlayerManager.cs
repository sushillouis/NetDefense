using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class WhiteHatPlayerManager : WhiteHatBaseManager {

	// Reference to the click action
	public InputActionReference clickAction;
	// Reference to the GUI element where we drop error messages
	public TMPro.TextMeshProUGUI errorText;



	// Enum what a click currently means
	enum ClickState {
		Selecting,
		SpawningFirewall,
		SelectingFirewallToMove,
		MovingFirewall,
	}
	// Variable defining what should happen when we click
	ClickState clickState = ClickState.Selecting;


	// De/register the click listener as well as Slection Manager event listeners
	void OnEnable(){
		clickAction.action.Enable();
		clickAction.action.performed += OnClickPressed;
		SelectionManager.packetSelectEvent += OnPacketSelected;
	}
	void OnDisable(){
		clickAction.action.performed -= OnClickPressed;
		SelectionManager.packetSelectEvent -= OnPacketSelected;
	}


	// -- Callbacks --


	// Function which responds to UI buttons that change the current click state
	public void OnSetClickState(int clickState){
		this.clickState = (ClickState)clickState;
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

	// Callback which handles when the selected packet changes
	void OnPacketSelected(Packet p){
		// Simply print out the details of the packet
		Debug.Log(p.details);
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
		}
	}

	// Function which handles clicks when we should be selecting a firewall to move
	void OnClick_SelectingFirewallToMove(){
		// If we need to select a firewall...
		if(SelectionManager.instance.selected == null || SelectionManager.instance.selected.GetComponent<Firewall>() == null){
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
		if(MoveFirewall(SelectionManager.instance.selected.GetComponent<Firewall>(), HoverManager.instance.hovered))
			clickState = ClickState.Selecting;
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
