using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Class which provides instrumentation for a dropdown
public class InstrumentedDropdown : ThemedDropdown {
	[Tooltip("The name in the event log of this source")]
	public string sourceName;

	// On awake register a callback to log changes
	protected virtual void Awake() { onValueChanged.AddListener(valueChanged); }

	// When the button is pressed... log that it was pressed
	void valueChanged(int opt){
		// Create an instrumentation event
		var e = InstrumentationManager.instance.generateNewEvent();
		e.source = sourceName;
		e.eventType = "ValueChanged";
		e.data = options[opt].text;

		// Log the event
		InstrumentationManager.instance.LogInstrumentationEvent(e);
	}

	#if UNITY_EDITOR
		// Menu item which converts ThemedButton to InstrumentedButton
		[MenuItem("CONTEXT/ThemedDropdown/Make Instrumented")]
		public static void MakeInstrumented(MenuCommand command){
			ThemedDropdown old = command.context as ThemedDropdown;
			GameObject go = old.gameObject;

			RectTransform template = old.template;
			TMPro.TMP_Text captionText = old.captionText;
			Image captionImage = old.captionImage;
			TMPro.TMP_Text itemText = old.itemText;
			Image itemImage = old.itemImage;
			List<OptionData> options = old.options;
			DropdownEvent onValueChanged = old.onValueChanged;

			UnityEngine.GameObject.DestroyImmediate(old);
			InstrumentedDropdown _new = go.AddComponent<InstrumentedDropdown>();

			// By default set the instrumented's name to the name of the game object it is attached to
			_new.sourceName = _new.gameObject.name;

			_new.template = template;
			_new.captionText = captionText;
			_new.captionImage = captionImage;
			_new.itemText = itemText;
			_new.itemImage = itemImage;
			_new.options = options;
			_new.onValueChanged = onValueChanged;

			command.context = _new;
		}

		// Menu item which converts InstrumentedSlider back to ThemedSliders
		[MenuItem("CONTEXT/InstrumentedDropdown/Remove Instrumentation")]
		public static void RemoveInstrumented(MenuCommand command){
			InstrumentedDropdown old = command.context as InstrumentedDropdown;
			GameObject go = old.gameObject;

			RectTransform template = old.template;
			TMPro.TMP_Text captionText = old.captionText;
			Image captionImage = old.captionImage;
			TMPro.TMP_Text itemText = old.itemText;
			Image itemImage = old.itemImage;
			List<OptionData> options = old.options;
			DropdownEvent onValueChanged = old.onValueChanged;

			UnityEngine.GameObject.DestroyImmediate(old);
			ThemedDropdown _new = go.AddComponent<ThemedDropdown>();

			_new.template = template;
			_new.captionText = captionText;
			_new.captionImage = captionImage;
			_new.itemText = itemText;
			_new.itemImage = itemImage;
			_new.options = options;
			_new.onValueChanged = onValueChanged;

			command.context = _new;
		}
	#endif
}

#if UNITY_EDITOR
// Class which provides the custom inspector view for a instrumented dropdown
[CustomEditor(typeof(InstrumentedDropdown))]
public class InstrumentedDropdownEditor : ThemedDropdownEditor {
	public override void OnInspectorGUI() {
		InstrumentedDropdown dropdown = (InstrumentedDropdown)target;

		// Give an option for the name of the source
		dropdown.sourceName = EditorGUILayout.TextField(new GUIContent("Source Name:", "The name in the event log of this source"), dropdown.sourceName);
		// Show the themed dropdown GUI
		base.OnInspectorGUI();
	}
}
#endif
