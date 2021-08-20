using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Class which manages UI theming
[ExecuteInEditMode]
public class ThemeManager : Core.Utilities.Singleton<ThemeManager> {
	// Event called whenever a theme property changes
	public static Utilities.VoidEventCallback themeUpdateEvent;

	// Function which applies the current theme
	public void ApplyTheme(){
		instance = this;
		themeUpdateEvent?.Invoke();
	}



	[Header("Detailed Theme Settings")]
	// Text stylesheet used in the theme (calls theme update callback when changed)
	[SerializeField] TMPro.TMP_StyleSheet _textStyleSheet;
	public TMPro.TMP_StyleSheet textStyleSheet {
		get => _textStyleSheet;
		set {
			_textStyleSheet = value;
			themeUpdateEvent?.Invoke();
		}
	}

	// Font asset used globally
	[SerializeField] TMPro.TMP_FontAsset _font;
	public TMPro.TMP_FontAsset textFont {
		get => _font;
		set {
			_font = value;
			themeUpdateEvent?.Invoke();
		}
	}


	// -- Panel Style --


	// Class representing a Panel's style
	[System.Serializable]
	public struct PanelStyle {
		// Name of the the style, referenced in components which use it (calls theme update callback when changed)
		[SerializeField] string _name;
		public string name {
			get => _name;
			set {
				_name = value;
				themeUpdateEvent?.Invoke();
			}
		}

		[Tooltip("If this is true, then this style will be ignored by global color manipulations.")]
		public bool locked;

		// Sprite for the button (calls theme update callback when changed)
		[SerializeField] Sprite _backgroundSprite;
		public Sprite backgroundSprite {
			get => _backgroundSprite;
			set {
				_backgroundSprite = value;
				themeUpdateEvent?.Invoke();
			}
		}

		// Colors of the button in differing states (calls theme update callback when changed)
		[SerializeField] Color _color;
		public Color color {
			get => _color;
			set {
				_color = value;
				themeUpdateEvent?.Invoke();
			}
		}

		public PanelStyle(string name) {
			_name = name;
			locked = false;
			_backgroundSprite = null;
			_color = Color.white;
			themeUpdateEvent?.Invoke();
		}
	}

	// List of button styles
	public List<PanelStyle> panelStyles = new List<PanelStyle> { new PanelStyle("default") };

	// Functions to access a particular button style
	public PanelStyle getPanelStyle(int index) { return panelStyles[index]; }
	public PanelStyle getPanelStyle(string name) { foreach(PanelStyle style in panelStyles) if(style.name == name) return style; /*default on failure*/ return getPanelStyle(0);  }


	// -- Button Style --


	// Class representing a Button's style
	[System.Serializable]
	public struct ButtonStyle {
		// Name of the the style, referenced in components which use it (calls theme update callback when changed)
		[SerializeField] string _name;
		public string name {
			get => _name;
			set {
				_name = value;
				themeUpdateEvent?.Invoke();
			}
		}

		[Tooltip("If this is true, then this style will be ignored by global color manipulations.")]
		public bool locked;

		// Sprite for the button (calls theme update callback when changed)
		[SerializeField] Sprite _backgroundSprite;
		public Sprite backgroundSprite {
			get => _backgroundSprite;
			set {
				_backgroundSprite = value;
				themeUpdateEvent?.Invoke();
			}
		}

		// Colors of the button in differing states (calls theme update callback when changed)
		[SerializeField] ColorBlock _colors;
		public ColorBlock colors {
			get => _colors;
			set {
				_colors = value;
				themeUpdateEvent?.Invoke();
			}
		}

		public ButtonStyle(string name) {
			_name = name;
			locked = false;
			_backgroundSprite = null;
			_colors = ColorBlock.defaultColorBlock;
			themeUpdateEvent?.Invoke();
		}
	}

	// List of button styles
	public List<ButtonStyle> buttonStyles = new List<ButtonStyle> { new ButtonStyle("default") };

	// Functions to access a particular button style
	public ButtonStyle getButtonStyle(int index) { return buttonStyles[index]; }
	public ButtonStyle getButtonStyle(string name) { foreach(ButtonStyle style in buttonStyles) if(style.name == name) return style; /*default on failure*/ return getButtonStyle(0);  }


	// -- Toggle Style --


	// Class representing a toggle's style
	[System.Serializable]
	public struct ToggleStyle {
		// Name of the style, used to reference it in attached components (calls theme update callback when changed)
		[SerializeField] string _name;
		public string name {
			get => _name;
			set {
				_name = value;
				themeUpdateEvent?.Invoke();
			}
		}

		[Tooltip("If this is true, then this style will be ignored by global color manipulations.")]
		public bool locked;

		// Sprite used for the background of the toggle (calls theme update callback when changed)
		[SerializeField] Sprite _backgroundSprite;
		public Sprite backgroundSprite {
			get => _backgroundSprite;
			set {
				_backgroundSprite = value;
				themeUpdateEvent?.Invoke();
			}
		}

		// Sprite used as the toggle's checkmark (calls theme update callback when changed)
		[SerializeField] Sprite _checkmarkSprite;
		public Sprite checkmarkSprite {
			get => _checkmarkSprite;
			set {
				_checkmarkSprite = value;
				themeUpdateEvent?.Invoke();
			}
		}

		// Colors used by the style (calls theme update callback when changed)
		[SerializeField] ColorBlock _colors;
		public ColorBlock colors {
			get => _colors;
			set {
				_colors = value;
				themeUpdateEvent?.Invoke();
			}
		}

		public ToggleStyle(string name) {
			_name = name;
			locked = false;
			_backgroundSprite = null;
			_checkmarkSprite = null;
			_colors = ColorBlock.defaultColorBlock;
			themeUpdateEvent?.Invoke();
		}
	}

	// List of toggle styles
	public List<ToggleStyle> toggleStyles = new List<ToggleStyle> { new ToggleStyle("default") };

	// Functions to get a particular style
	public ToggleStyle getToggleStyle(int index) { return toggleStyles[index]; }
	public ToggleStyle getToggleStyle(string name) { foreach(ToggleStyle style in toggleStyles) if(style.name == name) return style; /*default on failure*/ return getToggleStyle(0);  }


	// -- Slider Style --


	// Class representing a slider's style
	[System.Serializable]
	public struct SliderStyle {
		// The name of the style, used to reference it in components (calls theme update callback when changed)
		[SerializeField] string _name;
		public string name {
			get => _name;
			set {
				_name = value;
				themeUpdateEvent?.Invoke();
			}
		}

		[Tooltip("If this is true, then this style will be ignored by global color manipulations.")]
		public bool locked;

		// Background sprite of the unfilled section of the slider (calls theme update callback when changed)
		[SerializeField] Sprite _backgroundSprite;
		public Sprite backgroundSprite {
			get => _backgroundSprite;
			set {
				_backgroundSprite = value;
				themeUpdateEvent?.Invoke();
			}
		}

		// Background sprite of the filled section of the slider (calls theme update callback when changed)
		[SerializeField] Sprite _fillSprite;
		public Sprite fillSprite {
			get => _fillSprite;
			set {
				_fillSprite = value;
				themeUpdateEvent?.Invoke();
			}
		}

		// Sprite for the handle of the slider (calls theme update callback when changed)
		[SerializeField] Sprite _handleSprite;
		public Sprite handleSprite {
			get => _handleSprite;
			set {
				_handleSprite = value;
				themeUpdateEvent?.Invoke();
			}
		}

		// Color tint of the filled section of the slider (calls theme update callback when changed)
		[SerializeField] Color _fillColor;
		public Color fillColor {
			get => _fillColor;
			set {
				_fillColor = value;
				themeUpdateEvent?.Invoke();
			}
		}

		// Color tint of the unfilled section of the slider (calls theme update callback when changed)
		[SerializeField] Color _backgroundColor;
		public Color backgroundColor {
			get => _backgroundColor;
			set {
				_backgroundColor = value;
				themeUpdateEvent?.Invoke();
			}
		}

		// Slider colors (calls theme update callback when changed)
		[SerializeField] ColorBlock _colors;
		public ColorBlock colors {
			get => _colors;
			set {
				_colors = value;
				themeUpdateEvent?.Invoke();
			}
		}

		public SliderStyle(string name) {
			_name = name;
			locked = false;
			_backgroundSprite = null;
			_fillSprite = null;
			_handleSprite = null;
			_colors = ColorBlock.defaultColorBlock;
			_fillColor = Color.white;
			_backgroundColor = Color.white;
			themeUpdateEvent?.Invoke();
		}
	}

	// List of slider styles
	public List<SliderStyle> sliderStyles = new List<SliderStyle> { new SliderStyle("default") };

	// Functions to get a particular slider style
	public SliderStyle getSliderStyle(int index) { return sliderStyles[index]; }
	public SliderStyle getSliderStyle(string name) { foreach(SliderStyle style in sliderStyles) if(style.name == name) return style; /*default on failure*/ return getSliderStyle(0);  }


	// -- Dropdown Style --


	// Class representing a dropdown's style
	[System.Serializable]
	public struct DropdownStyle {
		// Name of the the style, referenced in components which use it (calls theme update callback when changed)
		[SerializeField] string _name;
		public string name {
			get => _name;
			set {
				_name = value;
				themeUpdateEvent?.Invoke();
			}
		}

		[Tooltip("If this is true, then this style will be ignored by global color manipulations.")]
		public bool locked;

		// Slider colors (calls theme update callback when changed)
		[SerializeField] ColorBlock _colors;
		public ColorBlock colors {
			get => _colors;
			set {
				_colors = value;
				themeUpdateEvent?.Invoke();
			}
		}

		public DropdownStyle(string name) {
			_name = name;
			locked = false;
			_colors = ColorBlock.defaultColorBlock;
			themeUpdateEvent?.Invoke();
		}
	}

	// List of button styles
	public List<DropdownStyle> dropdownStyles = new List<DropdownStyle> { new DropdownStyle("default") };

	// Functions to access a particular button style
	public DropdownStyle getDropdownStyle(int index) { return dropdownStyles[index]; }
	public DropdownStyle getDropdownStyle(string name) { foreach(DropdownStyle style in dropdownStyles) if(style.name == name) return style; /*default on failure*/ return getDropdownStyle(0);  }


#if UNITY_EDITOR
	// Polling which propagates theme updates in the editor, only happens when something in the scene updates
	void Update (){
		if (EditorApplication.isPlaying) return; // Skip this function in play mode

		ApplyTheme();
	}

	// Menu item which propagates changes to the theme through the rest of the UI
	[MenuItem("CONTEXT/ThemeManager/Propagate Theme Changes")]
	public static void PropagateTheme(MenuCommand command){
		ThemeManager mgr = command.context as ThemeManager;
		mgr.ApplyTheme();
	}
#endif
}

#if UNITY_EDITOR
// Class which provides the custom ThemeManager inspector UI
[CustomEditor(typeof(ThemeManager))]
public class ThemeManagerEditor : Editor {
	// Color representing the theme's primary dark color
	Color globalDarkThemeColor = new Color(0.2745098f, 0, 0.4117647f);
	// Reset color for the primary dark color
	Color globalDarkResetColor = new Color(0.2745098f, 0, 0.4117647f);

	// Color representing the theme's primary bright color
	Color globalBrightThemeColor = new Color(0.9254903f, 0.5490196f, 0.9215687f);
	// Reset color for the primary bright color
	Color globalBrightResetColor = new Color(0.9254903f, 0.5490196f, 0.9215687f);

	// Boolean tracking weather or not the global settings should be shown
	bool showGlobal = true;

	public override void OnInspectorGUI() {
		// Global settings foldout (display content if we are currently flodded out)
		if((showGlobal = EditorGUILayout.BeginFoldoutHeaderGroup(showGlobal, "Global Theme Management")) == true) {
			// Color fields for the theme's primary colors
			globalBrightThemeColor = EditorGUILayout.ColorField("Global Bright Color", globalBrightThemeColor);
			globalDarkThemeColor = EditorGUILayout.ColorField("Global Dark Color", globalDarkThemeColor);

			// Aligned buttons
			EditorGUILayout.BeginHorizontal();
				// Apply Hue button... applies the color hue to the theme
				if (GUILayout.Button("Apply Global Hue!")){
	            	ApplyGlobalHue();
					// Update the reset colors to the new primary theme color
					globalDarkResetColor = globalDarkThemeColor;
					globalBrightResetColor = globalBrightThemeColor;
				}

				// Reset button... resets the primary colors to the saved reset colors
				if (GUILayout.Button("Reset")){
					globalDarkThemeColor = globalDarkResetColor;
					globalBrightThemeColor = globalBrightResetColor;
				}
			EditorGUILayout.EndHorizontal();
		} EditorGUILayout.EndFoldoutHeaderGroup();

		// Show the script GUI
		DrawDefaultInspector();
	}

	// Function which applies the theme colors to all of the specific theme components
   	void ApplyGlobalHue(){
		// Get a reference to the manged theme manager
		ThemeManager mgr = target as ThemeManager;
		// Save the current state so we can undo this action
		Undo.RecordObject (mgr, "Applying Global Color");

		// Get the hues for the bright and dark colors
		float darkHue, brightHue, s, v;
		Color.RGBToHSV(globalDarkThemeColor, out darkHue, out s, out v);
		Color.RGBToHSV(globalBrightThemeColor, out brightHue, out s, out v);

		// Apply the hue to the (unlocked) panel styles
		for(int i = 0; i < mgr.panelStyles.Count; i++)
			if(!mgr.panelStyles[i].locked){
				var tmp = mgr.panelStyles[i];
				tmp.color = ChangeColorHue(tmp.color, darkHue);
				mgr.panelStyles[i] = tmp;
			}

		// Apply the hue to the (unlocked) button styles
		for(int i = 0; i < mgr.buttonStyles.Count; i++)
			if(!mgr.buttonStyles[i].locked){
				var tmp = mgr.buttonStyles[i];
				tmp.colors = ChangeColorBlockHue(tmp.colors, darkHue, brightHue);
				mgr.buttonStyles[i] = tmp;
			}

		// Apply the hue to the (unlocked) toggle styles
		for(int i = 0; i < mgr.toggleStyles.Count; i++)
			if(!mgr.toggleStyles[i].locked){
				var tmp = mgr.toggleStyles[i];
				tmp.colors = ChangeColorBlockHue(tmp.colors, darkHue, brightHue);
				mgr.toggleStyles[i] = tmp;
			}

		// Apply the hue to the (unlocked) slider styles
		for(int i = 0; i < mgr.sliderStyles.Count; i++)
			if(!mgr.sliderStyles[i].locked){
				var tmp = mgr.sliderStyles[i];
				tmp.colors = ChangeColorBlockHue(tmp.colors, darkHue, brightHue);
				tmp.fillColor = ChangeColorHue(tmp.fillColor, brightHue);
				tmp.backgroundColor = ChangeColorHue(tmp.backgroundColor, darkHue);
				mgr.sliderStyles[i] = tmp;
			}

		// Apply the hue to the (unlocked) dropdown styles
		for(int i = 0; i < mgr.dropdownStyles.Count; i++)
			if(!mgr.dropdownStyles[i].locked){
				var tmp = mgr.dropdownStyles[i];
				tmp.colors = ChangeColorBlockHue(tmp.colors, darkHue, brightHue);
				mgr.dropdownStyles[i] = tmp;
			}
	}

	// Function which changes the hue of a color
	Color ChangeColorHue(Color c, float hue){
		// Extract the color's saturation and value
		float h, s, v;
		Color.RGBToHSV(c, out h, out s, out v);
		// Apply the new hue to the old saturation and value
		Color _out = Color.HSVToRGB(hue, s, v);
		// Make sure alpha is saved
		_out.a = c.a;
		return _out;
	}

	// Function which changes the hue of a color block
	ColorBlock ChangeColorBlockHue(ColorBlock b, float darkHue, float brightHue){
		// Bright Colors
		b.normalColor = ChangeColorHue(b.normalColor, brightHue);
		b.highlightedColor = ChangeColorHue(b.highlightedColor, brightHue);
		// Dark colors
		b.pressedColor = ChangeColorHue(b.pressedColor, brightHue);
		b.selectedColor = ChangeColorHue(b.selectedColor, brightHue);
		b.disabledColor = ChangeColorHue(b.disabledColor, brightHue);

		return b;
	}
}
#endif
