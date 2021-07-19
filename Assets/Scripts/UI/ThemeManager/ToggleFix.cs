using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Script which adds a toggle to a toggle group, there were issues where the group was claiming the toggle was not a part of the group. So this script ensures that the toggle is registered with the group
public class ToggleFix : MonoBehaviour {
	// Property representing the toggle group attached to (will automatically update the toggle group whenever it changes)
	[SerializeField] ToggleGroup _group;
    public ToggleGroup group {
		get => _group;
		set {
			_group = value;
			OnEnable();
		}
	}

	// Function called when the toggle is enabled (or its group changes) which registers the group with the toggle and the toggle with the group
    void OnEnable( ) {
        Toggle toggle = GetComponent<Toggle>( );
        toggle.group = group;
		group.RegisterToggle(toggle);
    }
}
