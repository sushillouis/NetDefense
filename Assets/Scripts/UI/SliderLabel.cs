using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Class which provides an automatic label for a slider
public class SliderLabel : MonoBehaviour {
	// Reference to the label text
	ThemedText text;
	// Reference to the slider we are labeling
	public Slider trackedSlider;
	// Text that should be pre/suffixed before the label
	public string prefix = "", suffix = "";
	// The format string which determines how the number is converted into a string
	public string numberFormat = "F2";

	// When we are created get a reference to the object and appropriately present its initial value
	void Awake() {
		text = GetComponent<ThemedText>();
		SliderValueChanged(trackedSlider.value);
	}
	// Un/register ourselves to value change events from the slider
	void OnEnable(){ trackedSlider.onValueChanged.AddListener(SliderValueChanged); }
	void OnDisable(){ trackedSlider.onValueChanged.RemoveListener(SliderValueChanged); }

	// Function which responds to change events from the slider
	void SliderValueChanged(float value){
		text.text = prefix + value.ToString(numberFormat).Replace(" ", "") + suffix;
	}
}
