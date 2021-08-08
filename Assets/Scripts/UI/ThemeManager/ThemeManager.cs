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


	// -- Panel Style


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


	// -- Button Style


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



#if UNITY_EDITOR
	// Polling which propagates theme updates in the editor, only happens when something in the scene updates
	void Update (){
		if (EditorApplication.isPlaying) return; // Skip this function in play mode

		// Make sure that the instance is correctly marked before we dispatch (having two of these components in the editor will cause problems)
		instance = this;

		// NOTE: if this ever becomes to heavy to run, then disable this line and rely on the context menu options to propagate changes to the theme
		themeUpdateEvent?.Invoke();
	}

	// Menu item which propagates changes to the theme through the rest of the UI
	[MenuItem("CONTEXT/ThemeManager/Propagate Theme Changes")]
	public static void PropagateTheme(MenuCommand command){
		themeUpdateEvent?.Invoke();
	}
#endif
}
