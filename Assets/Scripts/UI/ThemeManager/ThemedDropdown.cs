using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Class which provides theme management for buttons
public class ThemedDropdown : TMP_Dropdown {
	// The name of the button style currently being used
	public string themeStyleName = "default";
	// Font override for captions
	[SerializeField] bool captionFontOverride_ = false;
	public bool captionFontOverride {
		get => captionFontOverride_;
		set {
			captionFontOverride_ = value;
			(captionText as ThemedText).fontOverride = value;
		}
	}
	// Font override for items
	[SerializeField] bool itemFontOverride_ = false;
	public bool itemFontOverride {
		get => itemFontOverride_;
		set {
			itemFontOverride_ = value;
			(itemText as ThemedText).fontOverride = value;
		}
	}

	// Un/register to theme updates on dis/enable
	protected override void OnEnable(){
		OnThemeUpdate();
		ThemeManager.themeUpdateEvent += OnThemeUpdate;
	}
	protected override void OnDisable(){ ThemeManager.themeUpdateEvent -= OnThemeUpdate; }
	// On start update the theme and do any tweaks necessary to make the changes apply for the first time
	protected override void Start(){
		base.Start();
		OnThemeUpdate();
		DoStateTransition(currentSelectionState, true); // Transition into the current state to make sure that the colors from the theme get applied
	}

	// Function which applies theme updates
	public void OnThemeUpdate(){
		var style = ThemeManager.instance.getDropdownStyle(themeStyleName);
		colors = style.colors;
	}

#if UNITY_EDITOR
	// Menu item which propagates changes to the theme through the rest of the UI
	[MenuItem("CONTEXT/ThemedDropdown/Propagate Theme Changes")]
	public static void PropagateTheme(MenuCommand command){ ThemeManager.PropagateTheme(command); }

	// Menu item which converts Dropdowns to ThemedDropdowns
	[MenuItem("CONTEXT/TMP_Dropdown/Make Themed")]
	public static void MakeThemed(MenuCommand command){
		TMP_Dropdown old = command.context as TMP_Dropdown;
		GameObject go = old.gameObject;

		RectTransform template = old.template;
		TMP_Text captionText = old.captionText;
		Image captionImage = old.captionImage;
		TMP_Text itemText = old.itemText;
		Image itemImage = old.itemImage;
		List<OptionData> options = old.options;
		DropdownEvent onValueChanged = old.onValueChanged;

		TMP_Text copyCaptionText = new GameObject().AddComponent<TextMeshProUGUI>();
		CopyValues(copyCaptionText, captionText);
		TMP_Text copyItemText = new GameObject().AddComponent<TextMeshProUGUI>();
		CopyValues(copyItemText, itemText);

		UnityEngine.GameObject.DestroyImmediate(old);
		ThemedDropdown _new = go.AddComponent<ThemedDropdown>();

		_new.template = template;
		_new.captionText = captionText;
		_new.captionImage = captionImage;
		_new.itemText = itemText;
		_new.itemImage = itemImage;
		_new.options = options;
		_new.onValueChanged = onValueChanged;

		MenuCommand captionTextCMD = new MenuCommand(_new.captionText);
		ThemedText.MakeThemed(captionTextCMD);
		MenuCommand itemTextCMD = new MenuCommand(_new.itemText);
		ThemedText.MakeThemed(itemTextCMD);

		_new.captionText = captionTextCMD.context as ThemedText;
		_new.itemText = itemTextCMD.context as ThemedText;

		CopyValues(_new.captionText, copyCaptionText);
		CopyValues(_new.itemText, copyItemText);

		UnityEngine.GameObject.DestroyImmediate(copyCaptionText.gameObject);
		UnityEngine.GameObject.DestroyImmediate(copyItemText.gameObject);

		var templateOld = _new.transform.GetChild(2);
		ThemedPanel.MakeThemed(new MenuCommand(templateOld.GetComponent<Image>()));
		var _template = _new.transform.GetChild(2).GetComponent<ThemedPanel>();
		_template.type = Image.Type.Sliced;
		_template.themeStyleName = "Dropdown Background";
		_template.OnThemeUpdate();

		var viewport = _template.transform.GetChild(0);
		var content = viewport.transform.GetChild(0);
		var itemOld = content.GetChild(0);
		ThemedToggle.MakeThemed(new MenuCommand(itemOld.GetComponent<Toggle>()));
		var item = content.GetChild(0).GetComponent<ThemedToggle>();
		item.themeStyleName = "Dropdown";
		item.OnThemeUpdate();

		var background = item.transform.GetChild(0).GetComponent<Image>();//.GetComponent<ThemedPanel>();
		background.type = Image.Type.Sliced;

		command.context = _new;
	}

	public static void CopyValues(TMP_Text newText, TMP_Text oldText, bool create = true, bool delete = true){
		newText.autoSizeTextContainer = oldText.autoSizeTextContainer;
		newText.text = oldText.text;
		newText.textPreprocessor = oldText.textPreprocessor;
		newText.isRightToLeftText = oldText.isRightToLeftText;
		newText.font = oldText.font;
		newText.fontSharedMaterial = oldText.fontSharedMaterial;
		try{ newText.fontSharedMaterials = oldText.fontSharedMaterials; } catch (System.NullReferenceException) {}
		newText.fontMaterial = oldText.fontMaterial;
		try{ newText.fontMaterials = oldText.fontMaterials; } catch (System.NullReferenceException) {}
		newText.color = oldText.color;
		newText.alpha = oldText.alpha;
		newText.enableVertexGradient = oldText.enableVertexGradient;
		newText.colorGradient = oldText.colorGradient;
		newText.colorGradientPreset = oldText.colorGradientPreset;
		newText.spriteAsset = oldText.spriteAsset;
		newText.tintAllSprites = oldText.tintAllSprites;
		newText.styleSheet = oldText.styleSheet;
		newText.textStyle = oldText.textStyle;
		newText.overrideColorTags = oldText.overrideColorTags;
		newText.faceColor = oldText.faceColor;
		newText.outlineColor = oldText.outlineColor;
		newText.outlineWidth = oldText.outlineWidth;
		newText.fontSize = oldText.fontSize;
		newText.fontWeight = oldText.fontWeight;
		newText.enableAutoSizing = oldText.enableAutoSizing;
		newText.fontSizeMin = oldText.fontSizeMin;
		newText.fontSizeMax = oldText.fontSizeMax;
		newText.fontStyle = oldText.fontStyle;
		newText.horizontalAlignment = oldText.horizontalAlignment;
		newText.verticalAlignment = oldText.verticalAlignment;
		newText.alignment = oldText.alignment;
		newText.characterSpacing = oldText.characterSpacing;
		newText.wordSpacing = oldText.wordSpacing;
		newText.lineSpacing = oldText.lineSpacing;
		newText.lineSpacingAdjustment = oldText.lineSpacingAdjustment;
		newText.paragraphSpacing = oldText.paragraphSpacing;
		newText.characterWidthAdjustment = oldText.characterWidthAdjustment;
		newText.enableWordWrapping = oldText.enableWordWrapping;
		newText.wordWrappingRatios = oldText.wordWrappingRatios;
		newText.overflowMode = oldText.overflowMode;
	}
#endif
}

#if UNITY_EDITOR
// TODO: Make dropdown UI
// Class which provides the custom Button inspector UI
// From https://answers.unity.com/questions/1304097/subclassing-button-public-variable-wont-show-up-in.html
[CustomEditor(typeof(ThemedDropdown))]
public class ThemedDropdownEditor : TMPro.EditorUtilities.DropdownEditor {
	public override void OnInspectorGUI() {
		ThemedDropdown dropdown = (ThemedDropdown)target;

		// Present a dropdown menu listing the possible styles
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Dropdown Style");
		if(EditorGUILayout.DropdownButton(new GUIContent(dropdown.themeStyleName), FocusType.Keyboard)){
			GenericMenu menu = new GenericMenu();

			foreach(ThemeManager.DropdownStyle style in ThemeManager.instance.dropdownStyles)
				 menu.AddItem(new GUIContent(style.name), dropdown.themeStyleName.Equals(style.name), OnNameSelected, style.name);

			menu.ShowAsContext();
		}
		EditorGUILayout.EndHorizontal();

		// Present toggles for setting font overrides for the children
		dropdown.captionFontOverride = EditorGUILayout.Toggle("Caption Font Override", dropdown.captionFontOverride);
		dropdown.itemFontOverride = EditorGUILayout.Toggle("Item Font Override", dropdown.itemFontOverride);

		// Show the dropdown GUI
		base.OnInspectorGUI();
	}

   	// Handler for when a menu item is selected
    void OnNameSelected(object name) {
		ThemedDropdown dropdown = (ThemedDropdown)target;
        dropdown.themeStyleName = (string)name;
		dropdown.OnThemeUpdate();
    }

}
#endif
