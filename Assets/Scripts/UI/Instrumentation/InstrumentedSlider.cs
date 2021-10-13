using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Class which provides instrumentation of a slider
public class InstrumentedSlider : ThemedSlider {
	[Tooltip("The name in the event log of this source")]
	public string sourceName;

	// On awake register a callback to log changes
	protected virtual void Awake() { onValueChanged.AddListener(logValueChangedCallback); }

	// When the slider's value is changed start a coroutine which will log the value in a short period of time
	// (if a new value arives before the coroutine has logged its value, the coroutine is canceled and new one starts, thus only the final value when you drag the slider a bunch is logged)
	Coroutine logValueChangedCoroutine = null;
	void logValueChangedCallback(float value){
		if(logValueChangedCoroutine is object) StopCoroutine(logValueChangedCoroutine);
		logValueChangedCoroutine = StartCoroutine(logValueChanged(value));
	}

	// Coroutine which logs a change in slider value after a short period of time
	[Tooltip("The number of seconds after the value stops changing before its value is considered final.")]
	public float settleTime = 1;
	IEnumerator logValueChanged(float value){
		// Wait for the settle time to pass by
		float startTime = Time.time;
		while(Time.time - startTime < settleTime) yield return null;

		// Create an instrumentation event
		var e = InstrumentationManager.instance.generateNewEvent();
		e.timestamp = startTime;
		e.source = sourceName;
		e.eventType = "ValueChanged";
		e.data = "" + value;

		// Log the event
		InstrumentationManager.instance.LogInstrumentationEvent(e);
		// Make sure the coroutine is marked as stopped
		logValueChangedCoroutine = null;
	}

	#if UNITY_EDITOR
		// Menu item which converts ThemedSliders to InstrumentedSliders
		[MenuItem("CONTEXT/ThemedSlider/Make Instrumented")]
		public static void MakeInstrumented(MenuCommand command){
			ThemedSlider old = command.context as ThemedSlider;
			GameObject go = old.gameObject;

			RectTransform fillRect = old.fillRect;
			RectTransform handleRect = old.handleRect;
			Direction direction = old.direction;
			float minValue = old.minValue;
			float maxValue = old.maxValue;
			bool wholeNumbers = old.wholeNumbers;
			float value = old.value;
			SliderEvent onValueChanged = old.onValueChanged;

			// Selectable
			Navigation navigation = old.navigation;
			Transition transition = old.transition;
			ColorBlock colors = old.colors;
			SpriteState spriteState = old.spriteState;
			AnimationTriggers animationTriggers = old.animationTriggers;
			Graphic targetGraphic = old.targetGraphic;
			bool interactable = old.interactable;

			UnityEngine.GameObject.DestroyImmediate(old);
			InstrumentedSlider _new = go.AddComponent<InstrumentedSlider>();

			// By default set the instrumented's name to the name of the game object it is attached to
			_new.sourceName = _new.gameObject.name;

			_new.fillRect = fillRect;
			_new.handleRect = handleRect;
			_new.direction = direction;
			_new.minValue = minValue;
			_new.maxValue = maxValue;
			_new.wholeNumbers = wholeNumbers;
			_new.value = value;
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
		[MenuItem("CONTEXT/InstrumentedSlider/Remove Instrumentation")]
		public static void RemoveInstrumented(MenuCommand command){
			InstrumentedSlider old = command.context as InstrumentedSlider;
			GameObject go = old.gameObject;

			RectTransform fillRect = old.fillRect;
			RectTransform handleRect = old.handleRect;
			Direction direction = old.direction;
			float minValue = old.minValue;
			float maxValue = old.maxValue;
			bool wholeNumbers = old.wholeNumbers;
			float value = old.value;
			SliderEvent onValueChanged = old.onValueChanged;

			// Selectable
			Navigation navigation = old.navigation;
			Transition transition = old.transition;
			ColorBlock colors = old.colors;
			SpriteState spriteState = old.spriteState;
			AnimationTriggers animationTriggers = old.animationTriggers;
			Graphic targetGraphic = old.targetGraphic;
			bool interactable = old.interactable;

			UnityEngine.GameObject.DestroyImmediate(old);
			ThemedSlider _new = go.AddComponent<ThemedSlider>();

			_new.fillRect = fillRect;
			_new.handleRect = handleRect;
			_new.direction = direction;
			_new.minValue = minValue;
			_new.maxValue = maxValue;
			_new.wholeNumbers = wholeNumbers;
			_new.value = value;
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
// Class which provides the custom inspector view for a instrumented slider
[CustomEditor(typeof(InstrumentedSlider))]
public class InstrumentedSliderEditor : ThemedSliderEditor {
	public override void OnInspectorGUI() {
		InstrumentedSlider slider = (InstrumentedSlider)target;

		// Give an option for the name of the source
		slider.sourceName = EditorGUILayout.TextField(new GUIContent("Source Name:", "The name in the event log of this source"), slider.sourceName);
		// Show the themed slider GUI
		base.OnInspectorGUI();
		// Give the option to set the settling time
		slider.settleTime = EditorGUILayout.FloatField(new GUIContent("Settling Time:", "The number of seconds after the value stops changing before its value is considered final."), slider.settleTime);
	}
}
#endif
