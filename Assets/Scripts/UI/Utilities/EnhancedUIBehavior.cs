using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Helper class which provides additional information about the the UI element
public class EnhancedUIBehavior : MonoBehaviour {
	// The rectTransform of this element
	[HideInInspector] public RectTransform rectTransform;

	// The canvas this element is attached to
	[HideInInspector] public Canvas canvas;
	// The transform of the canvas this element is attached to
	[HideInInspector] public RectTransform canvasTransform;

	// When the object is created determine its connections
	protected virtual void Awake(){
		rectTransform = GetComponent<RectTransform>();

		canvas = Findcanvas(transform);
		canvasTransform = canvas.GetComponent<RectTransform>();
	}

	// Function which centers the UI element on the screen
	public void CenterOnScreen(){
		transform.position = new Vector3 (Screen.width * 0.5f, Screen.height * 0.5f, 0);
	}


	// -- Visibility Functions --


	// Function which determines if any amount of the element can be viewed from the provided rect
	protected bool IsVisibleInRect(Rect pixelRect){
		return GetVisibleCornersInRect(pixelRect) > 0;
	}

	// Function which determiens if any amount of the element can be viewed from the provided camera
	public bool IsVisibleFrom(Camera viewCamera){
		return IsVisibleInRect(viewCamera.pixelRect);
	}

	// Function which determines if any amount of the element can be viewed from the provided canvas (if no canvas is provided the parent canvas is used)
	public bool IsVisibleFrom(Canvas canvas = null){
		if(canvas == null) return IsVisibleInRect(canvas.pixelRect);
		return IsVisibleInRect(canvas.pixelRect);
	}

	// function which determines if the element can be fully viewed fromt he provided rect
	protected bool IsFullyVisibleInRect(Rect pixelRect){
		return GetVisibleCornersInRect(pixelRect) == 4;
	}

	// Function which determiens if the element can be viewed from the provided camera
	public bool IsFullyVisibleFrom(Camera viewCamera){
		return IsFullyVisibleInRect(viewCamera.pixelRect);
	}

	// Function which determines if the element can be viewed from the provided canvas (if no canvas is provided the parent canvas is used)
	public bool IsFullyVisibleFrom(Canvas canvas = null){
		if(canvas == null) return IsFullyVisibleInRect(canvas.pixelRect);
		return IsFullyVisibleInRect(canvas.pixelRect);
	}


	// -- Helpers --


	// Function which determines how many corners of the element can be viewed from the provided camera
	protected int GetVisibleCornersInRect(Rect pixelRect) {
		Vector3[] objectCorners = new Vector3[4];
        rectTransform.GetWorldCorners(objectCorners);

        int visibleCorners = 0;
        for (var i = 0; i < objectCorners.Length; i++) // For each corner in rectTransform
            if (pixelRect.Contains(objectCorners[i])) // If the corner is inside the screen
                visibleCorners++;

		return visibleCorners;
	}

	// Helper function which finds the parent canvas of UI element
	public static Canvas Findcanvas(Transform us){
		while(us.parent != null){
			Canvas canvas = us.parent.gameObject.GetComponent<Canvas>();
			if(canvas != null)
				return canvas;

			us = us.parent;
		}
		return null;
	}
}

// Version of EnhancedUIBehaviour which manages a singleton instance
public class EnhancedUIBehaviorSingleton<T> : EnhancedUIBehavior where T : EnhancedUIBehaviorSingleton<T> {
	public static T instance { get; protected set; }

	public static bool instanceExists {
		get => instance != null;
	}

	protected override void Awake() {
		base.Awake();

		if (instanceExists)
			Destroy(gameObject);
		else
			instance = (T) this;
	}

	protected virtual void OnDestroy() {
		if (instance == this)
			instance = null;
	}

}
