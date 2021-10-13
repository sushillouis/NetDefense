using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Wrapper class which provides theme management for sliders
public class ThemedSlider : Slider {
	// The name of the slider them style to use
	public string themeStyleName = "default";

	// Dis/connect to theme updates when this object is dis/enabled
	protected override void OnEnable(){
		OnThemeUpdate();
		ThemeManager.themeUpdateEvent += OnThemeUpdate;
	}
	protected override void OnDisable(){ ThemeManager.themeUpdateEvent -= OnThemeUpdate; }
	// On start update the theme and do any tweaks necessary to make the changes apply for the first time
	protected override void Start(){
		base.Start();

		// Detach then reattach the fill rects, if this step isn't done then the handle won't appear and the slider won't be interactable
		string fillName = fillRect.name; fillRect = null;
		string handleName = handleRect.name; handleRect = null;
		fillRect = Utilities.RecursiveFindChild(transform, fillName) as RectTransform;
		handleRect = Utilities.RecursiveFindChild(transform, handleName) as RectTransform;

		OnThemeUpdate();
	}

	// Function which applies theme updates
	public void OnThemeUpdate(){
		var style = ThemeManager.instance.getSliderStyle(themeStyleName);
		colors = style.colors;

		// Update the background sprite and color
		if(style.backgroundSprite != null)
			if(transform.GetChild(0)){
				Image backgroundImage = transform.GetChild(0).gameObject.GetComponent<Image>();
				if(backgroundImage){
					backgroundImage.sprite = style.backgroundSprite;
					backgroundImage.color = style.backgroundColor;
				}
			}

		// Update the fill sprite
		if(fillRect != null){
			Image fillImage = fillRect.gameObject.GetComponent<Image>();
			if(fillImage) {
				if(style.fillSprite != null) fillImage.sprite = style.fillSprite;
				fillImage.color = style.fillColor;
			}
		}

		// Update the handle sprite
		if(style.handleSprite != null)
			if(targetGraphic != null)
				image.sprite = style.handleSprite;
	}



#if UNITY_EDITOR
	// Menu item which propagates changes to the theme through the rest of the UI
	[MenuItem("CONTEXT/ThemedSlider/Propagate Theme Changes")]
	public static void PropagateTheme(MenuCommand command){ ThemeManager.PropagateTheme(command); }

	// Menu item which converts Sliders to ThemedSliders
	[MenuItem("CONTEXT/Slider/Make Themed")]
	public static void MakeThemed(MenuCommand command){
		Slider old = command.context as Slider;
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
// Class which provides the custom inspector view for a themed slider
// From https://answers.unity.com/questions/1304097/subclassing-button-public-variable-wont-show-up-in.html
[CustomEditor(typeof(ThemedSlider))]
public class ThemedSliderEditor : UnityEditor.UI.SliderEditor {
	public override void OnInspectorGUI() {
		ThemedSlider slider = (ThemedSlider)target;

		// Present a dropdown menu listing the possible styles
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Slider Style");
		if(EditorGUILayout.DropdownButton(new GUIContent(slider.themeStyleName), FocusType.Keyboard)){
			GenericMenu menu = new GenericMenu();

			foreach(var style in ThemeManager.instance.sliderStyles)
				 menu.AddItem(new GUIContent(style.name), slider.themeStyleName.Equals(style.name), OnNameSelected, style.name);

			menu.ShowAsContext();
		}
		EditorGUILayout.EndHorizontal();

		// Show the slider GUI
		base.OnInspectorGUI();
	}

   	// Handler for when a menu item is selected
    void OnNameSelected(object name) {
		ThemedSlider slider = (ThemedSlider)target;
        slider.themeStyleName = (string)name;
		slider.OnThemeUpdate();
    }
}
#endif
