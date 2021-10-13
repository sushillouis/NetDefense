using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Class which provides theme management to toggles
public class ThemedToggle : Toggle {
	// The name of the toggle style currently being used
	public string themeStyleName = "default";

	// Un/register to theme updates on dis/enable
	protected override void OnEnable(){ ThemeManager.themeUpdateEvent += OnThemeUpdate; }
	protected override void OnDisable(){ ThemeManager.themeUpdateEvent -= OnThemeUpdate; }
	// On start update the theme and do any tweaks necessary to make the changes apply for the first time
	protected override void Start(){
		base.Start();
		OnThemeUpdate();
		DoStateTransition(currentSelectionState, true); // Transition into the current state to make sure that the colors from the theme get applied
	}

	// Function which applies theme updates
	public void OnThemeUpdate(){
		var style = ThemeManager.instance.getToggleStyle(themeStyleName);
		colors = style.colors;

		// Update the background sprite
		if(style.backgroundSprite != null)
			if(targetGraphic != null)
				image.sprite = style.backgroundSprite;

		// Update the checkmark sprite
		if(style.checkmarkSprite != null)
			if(graphic != null)
				(graphic as Image).sprite = style.checkmarkSprite;
	}

#if UNITY_EDITOR
	// Menu item which propagates changes to the theme through the rest of the UI
	[MenuItem("CONTEXT/ThemedToggle/Propagate Theme Changes")]
	public static void PropagateTheme(MenuCommand command){ ThemeManager.PropagateTheme(command); }

	// Menu item which converts Toggles to ThemedToggles
	[MenuItem("CONTEXT/Toggle/Make Themed")]
	public static void MakeThemed(MenuCommand command){
		Toggle old = command.context as Toggle;
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

	// Menu item which converts the child TextMeshPro text into ThemedText
	[MenuItem("CONTEXT/Toggle/Make Child Text Themed")]
	public static void MakeChildTextThemed(MenuCommand command){
		Text child = (command.context as ThemedToggle).GetComponentInChildren<Text>();
		if(child == null){
			Debug.LogWarning("No Text child found to convert!");
			return;
		}

		command.context = child;
		ThemedText.MakeUGUITextThemed(command);
	}
#endif
}

#if UNITY_EDITOR
// Class which provides the custom inspector view for a themed slider
// From https://answers.unity.com/questions/1304097/subclassing-button-public-variable-wont-show-up-in.html
[CustomEditor(typeof(ThemedToggle))]
public class ThemedToggleEditor : UnityEditor.UI.ToggleEditor {
	public override void OnInspectorGUI() {
		ThemedToggle toggle = (ThemedToggle)target;

		// Present a dropdown menu listing the possible styles
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Toggle Style");
		if(EditorGUILayout.DropdownButton(new GUIContent(toggle.themeStyleName), FocusType.Keyboard)){
			GenericMenu menu = new GenericMenu();

			foreach(var style in ThemeManager.instance.toggleStyles)
				 menu.AddItem(new GUIContent(style.name), toggle.themeStyleName.Equals(style.name), OnNameSelected, style.name);

			menu.ShowAsContext();
		}
		EditorGUILayout.EndHorizontal();

		// Show the toggle GUI
		base.OnInspectorGUI();
	}

   	// Handler for when a menu item is selected
    void OnNameSelected(object name) {
		ThemedToggle toggle = (ThemedToggle)target;
        toggle.themeStyleName = (string)name;
		toggle.OnThemeUpdate();
    }

}
#endif
