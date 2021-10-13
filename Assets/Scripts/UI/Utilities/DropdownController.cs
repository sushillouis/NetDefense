using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Class which controls a TextMeshPro dropdown
[DisallowMultipleComponent]
public class DropdownController : MonoBehaviour, IPointerClickHandler {
    [Tooltip("Indexes that should be ignored. Indexes are 0 based.")]
    public List<int> indicesToDisable = new List<int>();

	[Tooltip("Events should be registered here instead of on the dropdown itself, any event registered on the dropdown will not benefit from its events being disabled")]
	public TMPro.TMP_Dropdown.DropdownEvent onValueChanged;
	// Flag which can be used to prevent events from propigating
	bool fireingEvents = true;

	// The dropdown that is being managed
    TMPro.TMP_Dropdown dropdown;

	// OnAwake get a reference to the managed dropdown
    private void Awake() { dropdown = GetComponent<TMPro.TMP_Dropdown>(); }

	// When we click on the dropdown, if the dropdown is expanded apply the list of disabled indices
    public void OnPointerClick(PointerEventData eventData) {
		// Dropdown lists spawn a child with a canvas when they are selected... make sure that child exists
        var dropDownList = GetComponentInChildren<Canvas>();
        if (!dropDownList) return;

		// Get the list of toggles
        var toggles = dropDownList.GetComponentsInChildren<Toggle>(true);
		// Disable all of the items in the to be disabled list
        for (var i = 1; i < toggles.Length; i++)
            toggles[i].interactable = !indicesToDisable.Contains(i - 1);
    }

	// Sets the value without triggering events
	public void SetValueWithoutTriggeringEvents(int value){
		// Disable events
		fireingEvents = false;
		// Set the value
		dropdown.value = value;
		// Re-enable events
		fireingEvents = true;
	}

	// Event handler which is given to the dropdown's event list, passes value changed events onto the listeners (if events aren't disabled)
	public void OnValueChanged(int value){
		if(fireingEvents)
			onValueChanged?.Invoke(value);
	}
}
