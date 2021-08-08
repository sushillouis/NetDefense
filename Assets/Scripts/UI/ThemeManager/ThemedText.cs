using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Class which provides theme management for text labels
public class ThemedText : TextMeshProUGUI {

	// Override on themes which disables theme control of the font
	public bool fontOverride = false;

	// De/register ourselves to theme updates on dis/enable
	protected override void OnEnable(){
		base.OnEnable();
		ThemeManager.themeUpdateEvent += OnThemeUpdate;
	}
	protected override void OnDisable(){
		base.OnDisable();
		ThemeManager.themeUpdateEvent -= OnThemeUpdate;
	}
	// On startup perform a theme update
	protected override void Start(){ base.Start(); OnThemeUpdate(); }

	// Function which updates the theme
	void OnThemeUpdate(){
		styleSheet = ThemeManager.instance.textStyleSheet;
		if(!fontOverride) font = ThemeManager.instance.textFont;
	}



#if UNITY_EDITOR
	// Menu item which propagates changes to the theme through the rest of the UI
	[MenuItem("CONTEXT/ThemedText/Propagate Theme Changes")]
	public static void PropagateTheme(MenuCommand command){ ThemeManager.PropagateTheme(command); }

	// Menu item which converts TextMeshPro to ThemedText
	[MenuItem("CONTEXT/TextMeshProUGUI/Make Themed")]
	public static void MakeThemed(MenuCommand command){
		TextMeshProUGUI oldText = command.context as TextMeshProUGUI;
		GameObject go = oldText.gameObject;

		bool autoSizeTextContainer = oldText.autoSizeTextContainer;
		string savedText = oldText.text;
		Vector4 maskOffset = oldText.maskOffset;
		ITextPreprocessor textPreprocessor = oldText.textPreprocessor;
		bool isRightToLeftText = oldText.isRightToLeftText;
		TMP_FontAsset font = oldText.font;
		Material fontSharedMaterial = oldText.fontSharedMaterial;
		Material[] fontSharedMaterials = oldText.fontSharedMaterials;
		Material fontMaterial = oldText.fontMaterial;
		Material[] fontMaterials = oldText.fontMaterials;
		Color color = oldText.color;
		float alpha = oldText.alpha;
		bool enableVertexGradient = oldText.enableVertexGradient;
		VertexGradient colorGradient = oldText.colorGradient;
		TMP_ColorGradient colorGradientPreset = oldText.colorGradientPreset;
		TMP_SpriteAsset spriteAsset = oldText.spriteAsset;
		bool tintAllSprites = oldText.tintAllSprites;
		TMP_StyleSheet styleSheet = oldText.styleSheet;
		TMP_Style textStyle = oldText.textStyle;
		bool overrideColorTags = oldText.overrideColorTags;
		Color32 faceColor = oldText.faceColor;
		Color32 outlineColor = oldText.outlineColor;
		float outlineWidth = oldText.outlineWidth;
		float fontSize = oldText.fontSize;
		FontWeight fontWeight = oldText.fontWeight;
		bool enableAutoSizing = oldText.enableAutoSizing;
		float fontSizeMin = oldText.fontSizeMin;
		float fontSizeMax = oldText.fontSizeMax;
		FontStyles fontStyle = oldText.fontStyle;
		HorizontalAlignmentOptions horizontalAlignment = oldText.horizontalAlignment;
		VerticalAlignmentOptions verticalAlignment = oldText.verticalAlignment;
		TextAlignmentOptions alignment = oldText.alignment;
		float characterSpacing = oldText.characterSpacing;
		float wordSpacing = oldText.wordSpacing;
		float lineSpacing = oldText.lineSpacing;
		float lineSpacingAdjustment = oldText.lineSpacingAdjustment;
		float paragraphSpacing = oldText.paragraphSpacing;
		float characterWidthAdjustment = oldText.characterWidthAdjustment;
		bool enableWordWrapping = oldText.enableWordWrapping;
		float wordWrappingRatios = oldText.wordWrappingRatios;
		TextOverflowModes overflowMode = oldText.overflowMode;

		UnityEngine.GameObject.DestroyImmediate(oldText);
		ThemedText newText = go.AddComponent<ThemedText>();

		newText.autoSizeTextContainer = autoSizeTextContainer;
		newText.text = savedText;
		newText.maskOffset = maskOffset;
		newText.textPreprocessor = textPreprocessor;
		newText.isRightToLeftText = isRightToLeftText;
		newText.font = font;
		newText.fontSharedMaterial = fontSharedMaterial;
		newText.fontSharedMaterials = fontSharedMaterials;
		newText.fontMaterial = fontMaterial;
		newText.fontMaterials = fontMaterials;
		newText.color = color;
		newText.alpha = alpha;
		newText.enableVertexGradient = enableVertexGradient;
		newText.colorGradient = colorGradient;
		newText.colorGradientPreset = colorGradientPreset;
		newText.spriteAsset = spriteAsset;
		newText.tintAllSprites = tintAllSprites;
		newText.styleSheet = styleSheet;
		newText.textStyle = textStyle;
		newText.overrideColorTags = overrideColorTags;
		newText.faceColor =  faceColor;
		newText.outlineColor = outlineColor;
		newText.outlineWidth = outlineWidth;
		newText.fontSize = fontSize;
		newText.fontWeight = fontWeight;
		newText.enableAutoSizing = enableAutoSizing;
		newText.fontSizeMin = fontSizeMin;
		newText.fontSizeMax = fontSizeMax;
		newText.fontStyle = fontStyle;
		newText.horizontalAlignment = horizontalAlignment;
		newText.verticalAlignment = verticalAlignment;
		newText.alignment = alignment;
		newText.characterSpacing = characterSpacing;
		newText.wordSpacing = wordSpacing;
		newText.lineSpacing = lineSpacing;
		newText.lineSpacingAdjustment = lineSpacingAdjustment;
		newText.paragraphSpacing = paragraphSpacing;
		newText.characterWidthAdjustment = characterWidthAdjustment;
		newText.wordWrappingRatios = wordWrappingRatios;
		newText.overflowMode = overflowMode;
	}

	// Menu item which converts TextMeshPro to ThemedText
	[MenuItem("CONTEXT/Text/Make Themed")]
	public static void MakeUGUITextThemed(MenuCommand command){
		Text oldText = command.context as Text;
		GameObject go = oldText.gameObject;

		bool enabled = oldText.enabled;
		FontStyles fontStyle = FontStyleToFontStyles(oldText.fontStyle);
		float fontSize = oldText.fontSize;
		float fontSizeMin = oldText.resizeTextMinSize;
		float fontSizeMax = oldText.resizeTextMaxSize;
		float lineSpacing = oldText.lineSpacing;
		bool richText = oldText.supportRichText;
		bool enableAutoSizing = oldText.resizeTextForBestFit;
		TextAlignmentOptions alignment = TextAnchorToTextAlignmentOptions(oldText.alignment);
		bool enableWordWrapping = oldText.horizontalOverflow == HorizontalWrapMode.Wrap;
		TextOverflowModes overflowMode = oldText.verticalOverflow == VerticalWrapMode.Truncate ? TextOverflowModes.Truncate : TextOverflowModes.Overflow;
		string text = oldText.text;
		Color color = oldText.color;
		bool raycastTarget = oldText.raycastTarget;

		UnityEngine.GameObject.DestroyImmediate(oldText);
		ThemedText newText = go.AddComponent<ThemedText>();

		newText.enabled = enabled;
        newText.fontStyle = fontStyle;
        newText.fontSize = fontSize;
        newText.fontSizeMin = fontSizeMin;
        newText.fontSizeMax = fontSizeMax;
        newText.lineSpacing = lineSpacing;
        newText.richText = richText;
        newText.enableAutoSizing = enableAutoSizing;
        newText.alignment = alignment;
        newText.enableWordWrapping = enableWordWrapping;
        newText.overflowMode = overflowMode;
        newText.text = text;
        newText.color = color;
        newText.raycastTarget = raycastTarget;
	}

	// Function which converts UGUI font style information to TMPro font style information
    static FontStyles FontStyleToFontStyles(FontStyle fontStyle) {
        switch (fontStyle) {
            case FontStyle.Normal: return FontStyles.Normal;
            case FontStyle.Bold: return FontStyles.Bold;
            case FontStyle.Italic: return  FontStyles.Italic;
            case FontStyle.BoldAndItalic: return FontStyles.Bold | FontStyles.Italic;
        }

        Debug.LogWarning("Unhandled font style " + fontStyle);
        return FontStyles.Normal;
    }

	// Function which converts UGUI text alignment information to TMPro text alignment information
    static TextAlignmentOptions TextAnchorToTextAlignmentOptions(TextAnchor textAnchor) {
        switch (textAnchor) {
            case TextAnchor.UpperLeft: return TextAlignmentOptions.TopLeft;
            case TextAnchor.UpperCenter: return TextAlignmentOptions.Top;
            case TextAnchor.UpperRight: return TextAlignmentOptions.TopRight;
            case TextAnchor.MiddleLeft: return TextAlignmentOptions.Left;
            case TextAnchor.MiddleCenter: return TextAlignmentOptions.Center;
            case TextAnchor.MiddleRight: return TextAlignmentOptions.Right;
            case TextAnchor.LowerLeft: return TextAlignmentOptions.BottomLeft;
            case TextAnchor.LowerCenter: return TextAlignmentOptions.Bottom;
            case TextAnchor.LowerRight: return TextAlignmentOptions.BottomRight;
        }

        Debug.LogWarning("Unhandled text anchor " + textAnchor);
        return TextAlignmentOptions.TopLeft;
    }
#endif
}

#if UNITY_EDITOR
// Class which provides the custom inspector view for a themed text
// From https://answers.unity.com/questions/1304097/subclassing-button-public-variable-wont-show-up-in.html
[CustomEditor(typeof(ThemedText))]
public class ThemedTextEditor : TMPro.EditorUtilities.TMP_EditorPanelUI {
	public override void OnInspectorGUI() {
		ThemedText text = (ThemedText)target;

		// Present a dropdown menu listing the possible styles
		text.fontOverride = EditorGUILayout.Toggle("Font Override", text.fontOverride);

		// Show the slider GUI
		base.OnInspectorGUI();
	}
}
#endif
