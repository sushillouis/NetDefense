using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

	// Reference to the cursor displayed when placing a firewall
	public GameObject firewallCursor;

	// Reference to the panel which displays information about firewalls and packets
	public GameObject firewallPacketPanel;
	// Reference to the firewall and packet header labels
	public TMPro.TextMeshProUGUI firewallPacketPanelFirewallText, firewallPacketPanelPacketText;
	// References to all of the toggles in the firewall panel
	public Toggle[] firewallPacketPanelToggles;

	// Enum what a click currently means
	enum ClickState {
		Selecting,
		SpawningFirewall,
		SelectingFirewallToMove,
		MovingFirewall,
		SelectingDestinationToMakeHoneypot,
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
		SelectionManager.hoverChanged += OnHoverChanged;
		SelectionManager.packetSelectEvent += OnPacketSelected;
		SelectionManager.firewallSelectEvent += OnFirewallSelected;
	}
	void OnDisable(){
		leftClickAction.action.performed -= OnClickPressed;
		rightClickAction.action.performed -= OnCancel;
		cancelAction.action.performed -= OnCancel;
		SelectionManager.hoverChanged -= OnHoverChanged;
		SelectionManager.packetSelectEvent -= OnPacketSelected;
		SelectionManager.firewallSelectEvent -= OnFirewallSelected;
	}


	// -- Callbacks --


	// Function which responds to UI buttons that change the current click state
	public void OnSetClickState(int clickState){
		this.clickState = (ClickState)clickState;
	}

	// Function which responds to the remove selected firewall button
	public void OnRemoveSelectedFirewall(){
		Firewall selected = getSelected<Firewall>();
		SelectionManager.instance.selected = null; // Make sure that the selection manager is not pointed at the item when we delete it
		DestroyFirewall(selected);

		// Make sure the firewall panel closes
		OnClosePacketFirewallPanel();
	}

	// Function called when the close button of the firewall panel is pressed
	public void OnClosePacketFirewallPanel(){
		firewallPacketPanel.SetActive(false);
		firewallPacketPanelFirewallText.gameObject.SetActive(false);
		firewallPacketPanelPacketText.gameObject.SetActive(false);
	}

	// Callback which responds to cancel (escape and right click) events
	void OnCancel(InputAction.CallbackContext ctx){
		// Ignore click releases
		if(!ctx.ReadValueAsButton()) return;
		// Ignore UI clicks
		if(EventSystem.current.IsPointerOverGameObject()) return;

		clickState = ClickState.Selecting;
		OnHoverChanged(SelectionManager.instance.hovered);
	}

	// Callback which handles when the selected packet changes
	void OnPacketSelected(Packet p){
		showPacketPanel(p);
	}

	// Callback which handles when the selected firewall changes
	void OnFirewallSelected(Firewall f){
		// Error if we don't own the firewall
		if(f.photonView.Controller != NetworkingManager.localPlayer){
			ErrorHandler(ErrorCodes.WrongPlayer, "You can't modify the settings of a Firewall you don't own.");
			return;
		}

		showFirewallPanel(f);
	}

	// Callback which handles when the currently hovered grid piece changes
	void OnHoverChanged(GameObject newHover){
		if( newHover == null 					// If there isn't a new hover...
		  || newHover.tag != "FirewallTarget"	// Or the hover target can't host a firewall...
		  // Or we aren't in a state where we need the firewall placement indicator...
		  || !(clickState == ClickState.SpawningFirewall || clickState == ClickState.MovingFirewall || (clickState == ClickState.SelectingFirewallToMove && SelectionManager.instance.selected?.GetComponent<Firewall>() != null))
		){
			// Disable the firewall cursor
			firewallCursor.SetActive(false);
			return;
		}

		// Otherwise enable the firewall cursor and snap it to the hovered point
		firewallCursor.SetActive(true);
		firewallCursor.transform.position = newHover.transform.position;
		firewallCursor.transform.rotation = newHover.transform.rotation;
	}

	// Callback which handles when one of the toggles in the firewall panel is adjusted
	public void OnFirewallToggleSelected(int deltaNumber){
		// Disable filter settings when we are just opening the panel for the first time
		if(firewallJustSelected) return;
		// Don't bother with this function if we don't have a firewall selected
		Firewall selected = getSelected<Firewall>();
		if(selected == null) return;

		Packet.Details rules = selected.filterRules;

		// Set the correct filter rules based on the given input
		switch(deltaNumber){
			case 0: rules.size = Packet.Size.Small; break;
			case 1: rules.size = Packet.Size.Medium; break;
			case 2: rules.size = Packet.Size.Large; break;
			case 3: rules.shape = Packet.Shape.Cube; break;
			case 4: rules.shape = Packet.Shape.Sphere; break;
			case 5: rules.shape = Packet.Shape.Cone; break;
			case 6: rules.color = Packet.Color.Blue; break;
			case 7: rules.color = Packet.Color.Green; break;
			case 8: rules.color = Packet.Color.Pink; break;
		}

		if(!SetFirewallFilterRules(selected, rules))
			showFirewallPanel(selected); // Reload the firewall panel if we failed to update the settings
	}

	// Callback which makes the selected destination a honeypot (or sets the relevant click state so that the next click will make a destination a honeypot)
	public void MakeHoneypot(){
		// If we need to select a destination...
		if(getSelected<Destination>() == null){
			clickState = ClickState.SelectingDestinationToMakeHoneypot;
			return;
		}

		// If the selection is a destination, make it a honeypot
		if(MakeDestinationHoneypot(getSelected<Destination>()))
			// Play a sound to indicate that settings were updated
			AudioManager.instance.uiSoundFXPlayer.PlayTrackImmediate("SettingsUpdated", .5f);

		// Reset the click state
		clickState = ClickState.Selecting;
	}

	// Callback which responds to click events (ignoring click release events and events already handled by the UI)
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
			case ClickState.SelectingDestinationToMakeHoneypot: OnClick_SelectingDestinationToMakeHoneypot(); break;
		}
	}


	// -- Click Events --


	// Function which handles clicks when we should be placing firewalls
	void OnClick_SpawningFirewall(){
		// Place a firewall at the currently hovered path piece (the base class takes care of most error handling)
		Firewall spawned = SpawnFirewall(SelectionManager.instance.hovered);
		// If we succeeded, mark the new fire wall as selected and reset the click state
		if(spawned != null){
			SelectionManager.instance.selected = spawned.gameObject;
			clickState = ClickState.Selecting;

			// Make sure the placement cursor is hidden
			OnHoverChanged(SelectionManager.instance.hovered);

			// Play a sound to indicate that it has spawned
			AudioManager.instance.soundFXPlayer.PlayTrackImmediate("FirewallSpawn", .5f);
		}
	}

	// Function which handles clicks when we should be selecting a firewall to move
	void OnClick_SelectingFirewallToMove(){
		// If we need to select a firewall...
		if(getSelected<Firewall>() == null){
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
		if(MoveFirewall(getSelected<Firewall>(), SelectionManager.instance.hovered)){
			clickState = ClickState.Selecting;

			// Make sure the placement cursor is hidden
			OnHoverChanged(SelectionManager.instance.hovered);

			// Play a sound to indicate that it has moved
			AudioManager.instance.soundFXPlayer.PlayTrackImmediate("FirewallSpawn", .5f);
		}
	}

	// function which handles clicks when we are supposed to be making destinations into honeypots
	public void OnClick_SelectingDestinationToMakeHoneypot(){
		// If we need to select a destination...
		if(getSelected<Destination>() == null){
			// Tell the selection manager to select whatever is under it
			SelectionManager.instance.SelectUnderCursor(/*No events*/ false);

			// If its selection isn't a destination, give the user an error message
			if(SelectionManager.instance.selected == null || SelectionManager.instance.selected.GetComponent<Destination>() == null){
				SelectionManager.instance.selected = null;
				ErrorHandler(ErrorCodes.FirewallNotSelected, "A Destination to make into a Honeypot must be selected!");
				return;
			}
		}

		MakeHoneypot();
	}


	// -- Show Panels --


	// Function which shows the firewall panel
	bool firewallJustSelected = false; // Boolean which tracks if we just selected the firewall, and if we did it prevents toggle updates from registering
	public void showFirewallPanel(Firewall f){
		firewallJustSelected = true; // Disable toggle callbacks

		// Set all of the toggles as interactable
		foreach(Toggle t in firewallPacketPanelToggles)
			t.interactable = true;

		// Set the correct toggle states
		firewallPacketPanelToggles[0].isOn = f.filterRules.size == Packet.Size.Small;
		firewallPacketPanelToggles[1].isOn = f.filterRules.size == Packet.Size.Medium;
		firewallPacketPanelToggles[2].isOn = f.filterRules.size == Packet.Size.Large;
		firewallPacketPanelToggles[3].isOn = f.filterRules.shape == Packet.Shape.Cube;
		firewallPacketPanelToggles[4].isOn = f.filterRules.shape == Packet.Shape.Sphere;
		firewallPacketPanelToggles[5].isOn = f.filterRules.shape == Packet.Shape.Cone;
		firewallPacketPanelToggles[6].isOn = f.filterRules.color == Packet.Color.Blue;
		firewallPacketPanelToggles[7].isOn = f.filterRules.color == Packet.Color.Green;
		firewallPacketPanelToggles[8].isOn = f.filterRules.color == Packet.Color.Pink;

		// Update the text to represent the number of updates remaining
		FirewallSettingsUpdated(f);

		// Display the correct header
		firewallPacketPanelFirewallText.gameObject.SetActive(true);
		firewallPacketPanelPacketText.gameObject.SetActive(false);
		// Display the panel
		firewallPacketPanel.SetActive(true);

		firewallJustSelected = false; // Re-enable toggle callbacks
	}


	// Function which shows the packet panel
	public void showPacketPanel(Packet p){
		// Set all of the toggles as uninteractable
		foreach(Toggle t in firewallPacketPanelToggles)
			t.interactable = false;

		// Set the correct toggle states
		firewallPacketPanelToggles[0].isOn = p.details.size == Packet.Size.Small;
		firewallPacketPanelToggles[1].isOn = p.details.size == Packet.Size.Medium;
		firewallPacketPanelToggles[2].isOn = p.details.size == Packet.Size.Large;
		firewallPacketPanelToggles[3].isOn = p.details.shape == Packet.Shape.Cube;
		firewallPacketPanelToggles[4].isOn = p.details.shape == Packet.Shape.Sphere;
		firewallPacketPanelToggles[5].isOn = p.details.shape == Packet.Shape.Cone;
		firewallPacketPanelToggles[6].isOn = p.details.color == Packet.Color.Blue;
		firewallPacketPanelToggles[7].isOn = p.details.color == Packet.Color.Green;
		firewallPacketPanelToggles[8].isOn = p.details.color == Packet.Color.Pink;

		// Display the correct header
		firewallPacketPanelFirewallText.gameObject.SetActive(false);
		firewallPacketPanelPacketText.gameObject.SetActive(true);
		// Display the panel
		firewallPacketPanel.SetActive(true);
	}


	// -- Callbacks --


	// Function called whenever a firewall's settings are meaninfully updated (updated and actually changed)
	protected override void FirewallSettingsUpdated(Firewall updated){
		firewallPacketPanelFirewallText.text = "Firewall - " + updated.updatesRemaining;

		// Play the success sound
		if(updated.updatesRemaining > 0) AudioManager.instance.uiSoundFXPlayer.PlayTrackImmediate("SettingsUpdated");
	}


	// -- Error Handling --


	// Override the error handler.
	protected override void ErrorHandler(BaseSharedBetweenHats.ErrorCodes errorCode, string error){
		// Continue all of the logic in the base handler
		base.ErrorHandler(errorCode, error);

		// Play a sound to indicate that an error occurred
		AudioManager.instance.uiSoundFXPlayer.PlayTrackImmediate("SettingsUpdateFailed");

		// If there are too many firewalls switch back to selecting mode
		if(errorCode == ErrorCodes.TooManyFirewalls){
			clickState = ClickState.Selecting;
			// Make sure the placement cursor is hidden
			OnHoverChanged(SelectionManager.instance.hovered);
		}

		// But also present the new error to the screen
		errorText.text = error;
	}
}
