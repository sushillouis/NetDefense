using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Class which provides theme management for buttons
public class ThemedButton : Button {
	// The name of the button style currently being used
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
		var style = ThemeManager.instance.getButtonStyle(themeStyleName);
		colors = style.colors;

		// Update background sprite
		if(style.backgroundSprite != null)
			if(targetGraphic != null)
				image.sprite = style.backgroundSprite;
	}

#if UNITY_EDITOR
	// Menu item which propagates changes to the theme through the rest of the UI
	[MenuItem("CONTEXT/ThemedButton/Propagate Theme Changes")]
	public static void PropagateTheme(MenuCommand command){ ThemeManager.PropagateTheme(command); }

	// Menu item which converts Buttons to ThemedButtons
	[MenuItem("CONTEXT/Button/Make Themed")]
	public static void MakeThemed(MenuCommand command){
		Button old = command.context as Button;
		GameObject go = old.gameObject;

		ButtonClickedEvent onClick = old.onClick;

		// Selectable
		Navigation navigation = old.navigation;
		Transition transition = old.transition;
		ColorBlock colors = old.colors;
		SpriteState spriteState = old.spriteState;
		AnimationTriggers animationTriggers = old.animationTriggers;
		Graphic targetGraphic = old.targetGraphic;
		bool interactable = old.interactable;

		UnityEngine.GameObject.DestroyImmediate(old);
		ThemedButton _new = go.AddComponent<ThemedButton>();

		_new.onClick = onClick;

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
	[MenuItem("CONTEXT/Button/Make Child Text Themed")]
	public static void MakeChildTextThemed(MenuCommand command){
		TMPro.TextMeshProUGUI child = (command.context as ThemedButton).GetComponentInChildren<TMPro.TextMeshProUGUI>();
		if(child == null){
			Debug.LogWarning("No TextMeshPro text child found to convert!");
			return;
		}

		command.context = child;
		ThemedText.MakeThemed(command);
	}
#endif
}

#if UNITY_EDITOR
// Class which provides the custom Button inspector UI
// From https://answers.unity.com/questions/1304097/subclassing-button-public-variable-wont-show-up-in.html
[CustomEditor(typeof(ThemedButton))]
public class ThemedButtonEditor : UnityEditor.UI.ButtonEditor {
	public override void OnInspectorGUI() {
		ThemedButton button = (ThemedButton)target;

		// Present a dropdown menu listing the possible styles
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Button Style");
		if(EditorGUILayout.DropdownButton(new GUIContent(button.themeStyleName), FocusType.Keyboard)){
			GenericMenu menu = new GenericMenu();

			foreach(ThemeManager.ButtonStyle style in ThemeManager.instance.buttonStyles)
				 menu.AddItem(new GUIContent(style.name), button.themeStyleName.Equals(style.name), OnNameSelected, style.name);

			menu.ShowAsContext();
		}
		EditorGUILayout.EndHorizontal();

		// Show the button GUI
		base.OnInspectorGUI();
	}

   	// Handler for when a menu item is selected
    void OnNameSelected(object name) {
		ThemedButton button = (ThemedButton)target;
        button.themeStyleName = (string)name;
		button.OnThemeUpdate();
    }

}
#endif
