using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class BlackHatPlayerManager : BlackHatBaseManager {

	// Reference to the click action
	public InputActionReference leftClickAction;
	// Reference to the GUI element where we drop error messages
	public TMPro.TextMeshProUGUI errorText;

	// Reference to the packet panel
	public GameObject packetStartPanel;
	// Reference to the packet panel headers
	public GameObject packetStartPanelPacketHeader, packetStartPanelStartHeader;
	// Reference to all of the toggles in the packet panel
	public Toggle[] packetStartPanelToggles;

	// Reference to the probability/likelihood panel
	public GameObject probabilityLikelihoodPanel;
	// Reference to the probability/likelihood panel headers
	public GameObject probabilityLikelihoodPanelProbabilityHeader, probabilityLikelihoodPanelLikelihoodHeader;
	// Reference to the probability/likelihood slider
	public Slider probabilityLikelihoodPanelSlider;
	// Reference to the text field representing the value of the probability/likelihood slider
	public TMPro.TextMeshProUGUI probabilityLikelihoodPanelValueText;

	// De/register the click listener as well as Selection Manager event listeners
	void OnEnable(){
		leftClickAction.action.Enable();
		leftClickAction.action.performed += OnClickPressed;
		SelectionManager.packetSelectEvent += OnPacketSelected;
		SelectionManager.startingPointSelectEvent += OnStartingPointSelected;
		SelectionManager.destinationSelectEvent += OnDestinationSelected;
	}
	void OnDisable(){
		leftClickAction.action.performed -= OnClickPressed;
		SelectionManager.packetSelectEvent -= OnPacketSelected;
		SelectionManager.startingPointSelectEvent -= OnStartingPointSelected;
		SelectionManager.destinationSelectEvent += OnDestinationSelected;
	}


	// -- Callbacks --


	// Function called when the close button of the packet/starting point panel is pressed
	public void OnClosePacketStartPanel(){
		packetStartPanel.SetActive(false);
	}

	// Function called when when the close button of the probability/likelihood panel is pressed
	public void OnCloseProbabilityLikelihoodPanel(){
		probabilityLikelihoodPanel.SetActive(false);
	}

	// Callback which responds to click events (ignoring cick release events and events already handled by the UI)
	void OnClickPressed(InputAction.CallbackContext ctx){
		// Ignore click releases
		if(!ctx.ReadValueAsButton()) return;
		// Ignore UI clicks
		if(EventSystem.current.IsPointerOverGameObject()) return;

		SelectionManager.instance.SelectUnderCursor();
	}

	// Callback which shows the packet panel when a packet is selected
	void OnPacketSelected(Packet p){
		showPacketPanel(p);
		OnCloseProbabilityLikelihoodPanel();
	}

	// Callback which shows the starting point panels when a packet is selected
	bool startingPointJustSelected = false;
	void OnStartingPointSelected(StartingPoint p){
		startingPointJustSelected = true;
		showStartingPointPanel(p);
		showProbabilityPanel(p);
		startingPointJustSelected = false;
	}

	// Callback which shows the the destination panel when a destination is selected
	bool destinationJustSelected = false;
	void OnDestinationSelected(Destination d){
		destinationJustSelected = true;
		showLikelihoodPanel(d);
		destinationJustSelected = false;

		OnClosePacketStartPanel();
	}

	// Callback which handles when one of the toggles in the starting point panel is adjusted
	public void OnStartingPointToggleSelected(int deltaNumber){
		// Disable adjusting settings when we are just opening the panel for the first time
		if(startingPointJustSelected) return;
		// Don't bother with this function if we don't have a starting point selected
		StartingPoint selected = getSelected<StartingPoint>();
		if(selected == null) return;

		// Set the correct spawning rules based on the given input
		Packet.Details rules = selected.maliciousPacketDetails;
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

		ChangeStartPointMalciousPacketDetails(selected, rules);
	}

	public void OnProbabilityLikelihoodSliderUpdate(float value){
		// Don't bother with this function if we just selected something
		if(destinationJustSelected || startingPointJustSelected) return;

		// Get a reference to staring point and destination (one of them should be null)
		StartingPoint startPoint = getSelected<StartingPoint>();
		Destination destination = getSelected<Destination>();

		// Attempt to change the starting point's malicious probability
		if (startPoint != null) ChangeStartPointMaliciousPacketProbability(startPoint, value);
		// Attempt to change the destination's malicious target probability
		else if(destination != null) ChangeDestinationMaliciousPacketTargetLikelihood(destination, (int) value);

		updateProbabilityLikelihoodText();
	}


	// -- Show Panels --


	// Function which shows the packet panel
	public void showPacketPanel(Packet p){
		// Set all of the toggles as not interactable
		foreach(Toggle t in packetStartPanelToggles)
			t.interactable = false;

		// Set the correct toggle states
		packetStartPanelToggles[0].isOn = p.details.size == Packet.Size.Small;
		packetStartPanelToggles[1].isOn = p.details.size == Packet.Size.Medium;
		packetStartPanelToggles[2].isOn = p.details.size == Packet.Size.Large;
		packetStartPanelToggles[3].isOn = p.details.shape == Packet.Shape.Cube;
		packetStartPanelToggles[4].isOn = p.details.shape == Packet.Shape.Sphere;
		packetStartPanelToggles[5].isOn = p.details.shape == Packet.Shape.Cone;
		packetStartPanelToggles[6].isOn = p.details.color == Packet.Color.Blue;
		packetStartPanelToggles[7].isOn = p.details.color == Packet.Color.Green;
		packetStartPanelToggles[8].isOn = p.details.color == Packet.Color.Pink;

		// Display the correct header
		packetStartPanelPacketHeader.SetActive(true);
		packetStartPanelStartHeader.SetActive(false);
		// Display the panel
		packetStartPanel.SetActive(true);
	}

	// Function which shows the starting point panel
	public void showStartingPointPanel(StartingPoint p){
		// Set all of the toggles as not interactable
		foreach(Toggle t in packetStartPanelToggles)
			t.interactable = true;

		// Set the correct toggle states
		packetStartPanelToggles[0].isOn = p.maliciousPacketDetails.size == Packet.Size.Small;
		packetStartPanelToggles[1].isOn = p.maliciousPacketDetails.size == Packet.Size.Medium;
		packetStartPanelToggles[2].isOn = p.maliciousPacketDetails.size == Packet.Size.Large;
		packetStartPanelToggles[3].isOn = p.maliciousPacketDetails.shape == Packet.Shape.Cube;
		packetStartPanelToggles[4].isOn = p.maliciousPacketDetails.shape == Packet.Shape.Sphere;
		packetStartPanelToggles[5].isOn = p.maliciousPacketDetails.shape == Packet.Shape.Cone;
		packetStartPanelToggles[6].isOn = p.maliciousPacketDetails.color == Packet.Color.Blue;
		packetStartPanelToggles[7].isOn = p.maliciousPacketDetails.color == Packet.Color.Green;
		packetStartPanelToggles[8].isOn = p.maliciousPacketDetails.color == Packet.Color.Pink;

		// Display the correct header
		packetStartPanelPacketHeader.SetActive(false);
		packetStartPanelStartHeader.SetActive(true);
		// Display the panel
		packetStartPanel.SetActive(true);
	}

	// Function which shows the starting point probability panel
	public void showProbabilityPanel(StartingPoint p){
		// Update the slider's properties
		probabilityLikelihoodPanelSlider.minValue = 0;
		probabilityLikelihoodPanelSlider.maxValue = 1;
		probabilityLikelihoodPanelSlider.wholeNumbers = false;
		// Update the slider's value and text
		probabilityLikelihoodPanelSlider.value = p.maliciousPacketProbability;
		updateProbabilityLikelihoodText();

		// Display the correct header
		probabilityLikelihoodPanelProbabilityHeader.SetActive(true);
		probabilityLikelihoodPanelLikelihoodHeader.SetActive(false);
		// Display the panel
		probabilityLikelihoodPanel.SetActive(true);
	}

	// Function which shows the destination likelihood panel
	public void showLikelihoodPanel(Destination d){
		// Update the slider's properties
		probabilityLikelihoodPanelSlider.minValue = 0;
		probabilityLikelihoodPanelSlider.maxValue = 20;
		probabilityLikelihoodPanelSlider.wholeNumbers = true;
		// Update the slider's value and text
		probabilityLikelihoodPanelSlider.value = d.maliciousPacketDestinationLikelihood;
		updateProbabilityLikelihoodText();

		// Display the correct header
		probabilityLikelihoodPanelProbabilityHeader.SetActive(false);
		probabilityLikelihoodPanelLikelihoodHeader.SetActive(true);
		// Display the panel
		probabilityLikelihoodPanel.SetActive(true);
	}

	// Function which updates the slider label to reflec the state of the slider
	void updateProbabilityLikelihoodText(){
		// Format the text appropriately to the starting point (2 decimal place probability) and destination (integer likelihood)
		if(getSelected<StartingPoint>() != null)
			probabilityLikelihoodPanelValueText.text = probabilityLikelihoodPanelSlider.value.ToString("0.##");
		else if(getSelected<Destination>() != null)
			probabilityLikelihoodPanelValueText.text = "" + (int) probabilityLikelihoodPanelSlider.value;
	}


	// -- Error Handling --


	// Override the error handler.
	protected override void ErrorHandler(BaseSharedBetweenHats.ErrorCodes errorCode, string error){
		// Continue all of the logic in the base handler
		base.ErrorHandler(errorCode, error);

		// But also present the new error to the screen
		errorText.text = error;
	}
}
