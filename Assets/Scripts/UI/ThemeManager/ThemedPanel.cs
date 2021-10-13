using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Class which provides theme management for panels
public class ThemedPanel : Image {
	// The name of the button style currently being used
	public string themeStyleName = "default";

	// Un/register to theme updates on dis/enable
	protected override void OnEnable(){
		base.OnEnable();
		OnThemeUpdate();
		ThemeManager.themeUpdateEvent += OnThemeUpdate;
	}
	protected override void OnDisable(){ base.OnDisable(); ThemeManager.themeUpdateEvent -= OnThemeUpdate; }
	// On start update the theme and do any tweaks necessary to make the changes apply for the first time
	protected override void Start(){
		base.Start();
		OnThemeUpdate();
	}

	// Function which applies theme updates
	public void OnThemeUpdate(){
		var style = ThemeManager.instance.getPanelStyle(themeStyleName);
		color = style.color;

		// Update background sprite
		if(style.backgroundSprite != null)
			sprite = style.backgroundSprite;
	}

#if UNITY_EDITOR
	// Menu item which propagates changes to the theme through the rest of the UI
	[MenuItem("CONTEXT/ThemedPanel/Propagate Theme Changes")]
	public static void PropagateTheme(MenuCommand command){ ThemeManager.PropagateTheme(command); }

	// Menu item which converts Images to ThemedPanels
	[MenuItem("CONTEXT/Image/Make Themed")]
	public static void MakeThemed(MenuCommand command){
		Image old = command.context as Image;
		GameObject go = old.gameObject;

		UnityEngine.GameObject.DestroyImmediate(old);
		ThemedPanel _new = go.AddComponent<ThemedPanel>();

		command.context = _new;
	}

#endif
}

#if UNITY_EDITOR
// Class which provides the custom inspector view for a themed panel
// From https://answers.unity.com/questions/1304097/subclassing-button-public-variable-wont-show-up-in.html
[CustomEditor(typeof(ThemedPanel))]
public class ThemedPanelEditor : UnityEditor.UI.ImageEditor {
	public override void OnInspectorGUI() {
		ThemedPanel panel = (ThemedPanel)target;

		// Present a dropdown menu listing the possible styles
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Panel Style");
		if(EditorGUILayout.DropdownButton(new GUIContent(panel.themeStyleName), FocusType.Keyboard)){
			GenericMenu menu = new GenericMenu();

			foreach(var style in ThemeManager.instance.panelStyles)
				 menu.AddItem(new GUIContent(style.name), panel.themeStyleName.Equals(style.name), OnNameSelected, style.name);

			menu.ShowAsContext();
		}
		EditorGUILayout.EndHorizontal();

		// Show the rest of the inspector
		// DrawDefaultInspector();
		base.OnInspectorGUI();
	}

   	// Handler for when a menu item is selected
    void OnNameSelected(object name) {
		ThemedPanel panel = (ThemedPanel)target;
        panel.themeStyleName = (string)name;
		panel.OnThemeUpdate();
    }
}
#endif
