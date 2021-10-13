using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Class which provides instrumentation for a Toggle
public class InstrumentedToggle : ThemedToggle {
	[Tooltip("The name in the event log of this source")]
	public string sourceName;

	// On awake register a callback to log changes
	protected virtual void Awake() { onValueChanged.AddListener(valueChanged); }

	// When the toggle is toggled... log that it was toggled
	void valueChanged(bool on){
		// Create an instrumentation event
		var e = InstrumentationManager.instance.generateNewEvent();
		e.source = sourceName;
		e.eventType = "Toggled";
		e.data = on ? "on" : "off";

		// Log the event
		InstrumentationManager.instance.LogInstrumentationEvent(e);
	}

	#if UNITY_EDITOR
		// Menu item which converts ThemedButton to InstrumentedButton
		[MenuItem("CONTEXT/ThemedToggle/Make Instrumented")]
		public static void MakeInstrumented(MenuCommand command){
			ThemedToggle old = command.context as ThemedToggle;
			GameObject go = old.gameObject;

			ToggleTransition toggleTransition = old.toggleTransition;
			Graphic graphic = old.graphic;
			ToggleGroup group = old.group;
			ToggleEvent onValueChanged = old.onValueChanged;

			// Selectable
			Navigation navigation = old.navigation;
			Transition transition = old.transition;
			ColorBlock colors = old.colors;
			SpriteState spriteState = old.spriteState;
			AnimationTriggers animationTriggers = old.animationTriggers;
			Graphic targetGraphic = old.targetGraphic;
			bool interactable = old.interactable;

			UnityEngine.GameObject.DestroyImmediate(old);
			InstrumentedToggle _new = go.AddComponent<InstrumentedToggle>();

			// By default set the instrumented's name to the name of the game object it is attached to
			_new.sourceName = _new.gameObject.name;

			_new.toggleTransition = toggleTransition;
			_new.graphic = graphic;
			_new.group = group;
			_new.onValueChanged = onValueChanged;

			// Selectable
			_new.navigation = navigation;
			_new.transition = transition;
			_new.colors = colors;
			_new.spriteState = spriteState;
			_new.animationTriggers = animationTriggers;
			_new.targetGraphic = targetGraphic;
			_new.interactable = interactable;

			command.context = _new;
		}

		// Menu item which converts InstrumentedSlider back to ThemedSliders
		[MenuItem("CONTEXT/InstrumentedToggle/Remove Instrumentation")]
		public static void RemoveInstrumented(MenuCommand command){
			InstrumentedToggle old = command.context as InstrumentedToggle;
			GameObject go = old.gameObject;

			ToggleTransition toggleTransition = old.toggleTransition;
			Graphic graphic = old.graphic;
			ToggleGroup group = old.group;
			ToggleEvent onValueChanged = old.onValueChanged;

			// Selectable
			Navigation navigation = old.navigation;
			Transition transition = old.transition;
			ColorBlock colors = old.colors;
			SpriteState spriteState = old.spriteState;
			AnimationTriggers animationTriggers = old.animationTriggers;
			Graphic targetGraphic = old.targetGraphic;
			bool interactable = old.interactable;

			UnityEngine.GameObject.DestroyImmediate(old);
			ThemedToggle _new = go.AddComponent<ThemedToggle>();

			_new.toggleTransition = toggleTransition;
			_new.graphic = graphic;
			_new.group = group;
			_new.onValueChanged = onValueChanged;

			// Selectable
			_new.navigation = navigation;
			_new.transition = transition;
			_new.colors = colors;
			_new.spriteState = spriteState;
			_new.animationTriggers = animationTriggers;
			_new.targetGraphic = targetGraphic;
			_new.interactable = interactable;

			command.context = _new;
		}
	#endif
}

#if UNITY_EDITOR
// Class which provides the custom inspector view for a instrumented toggle
[CustomEditor(typeof(InstrumentedToggle))]
public class InstrumentedToggleEditor : ThemedToggleEditor {
	public override void OnInspectorGUI() {
		InstrumentedToggle toggle = (InstrumentedToggle)target;

		// Give an option for the name of the source
		toggle.sourceName = EditorGUILayout.TextField(new GUIContent("Source Name:", "The name in the event log of this source"), toggle.sourceName);
		// Show the themed toggle GUI
		base.OnInspectorGUI();
	}
}
#endif
