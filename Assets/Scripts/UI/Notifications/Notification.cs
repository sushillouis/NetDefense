using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification : EnhancedUIBehavior {

	[Tooltip("Reference to the provided text element")]
	public TMPro.TMP_Text text;
	[Tooltip("Reference to the background panel")]
	public ThemedPanel panel;

	// Function which sets the text stored in the text element
	public virtual void SetText(string _text){
		text.text = _text;
	}

	// Function which begins timer representing how long a notification will stay visible
	public virtual void StartFade(NotificationManager manager, float timeVisible, float fadeTime){
		StartCoroutine(FadeOut(manager, timeVisible, fadeTime));
	}

	// Coroutine which fades out the notifications over time
	IEnumerator FadeOut(NotificationManager manager, float timeVisible, float fadeTime){
		// Start the timer
		float startTime = Time.time;

		// Wait until we are <fadeTime> away from being destroyed
		while(timeVisible - fadeTime > Time.time - startTime) yield return null;

		// Reset the timer
		startTime = Time.time;
		// Save the initial alpha values
		float initialPanelAlpha = panel.color.a;
		float initialTextAlpha = text.color.a;

		// While we are fading away the element
		while(fadeTime > Time.time - startTime){
			// Normalize how far into the fade we are
			float normalizedFade = (Time.time - startTime) / fadeTime;

			// Fade out the panel's color
			Color tmp = panel.color;
			tmp.a = Mathf.Lerp(initialPanelAlpha, 0, normalizedFade);
			panel.color = tmp;

			// Fade out the text's color
			tmp = text.color;
			tmp.a = Mathf.Lerp(initialTextAlpha, 0, normalizedFade);
			text.color = tmp;

			yield return null;
		}

		// Remove the notification from the list of notifications
		manager.removeNotification(this);
	}
}
