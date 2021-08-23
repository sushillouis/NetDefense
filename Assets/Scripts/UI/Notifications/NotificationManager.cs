using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NotificationManager : EnhancedUIBehaviorSingleton<NotificationManager> {
	[Tooltip("The notification prefab which is spawned (it must have a notification component attached)")]
	public GameObject notificationPrefab;
	[Tooltip("The padding which should be added between notifications")]
	public float padding = 5;
	[Tooltip("The number of elements which should appear over the notifications (if two text elements should appear over the notifications this is 2)")]
	public int topIndex = 0;

	// The list of currently spawned notifications
	List<Notification> notifications = new List<Notification>();


	// Testing // TODO: Remove
	void Start(){ StartCoroutine(test()); }

	// Function which adds a new notification with the given text to the list of notifications
	// Optionally you can provide the amount of time a notification should remain visible, and the time it should take to fade away
	public void addNotification(string text, float timeVisible = 10, float fadeTime = 2){
		// Instantiate a notification (as a child of this object) and set its properties
		var notification = Instantiate(notificationPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Notification>();
		notification.SetText(text);
		notification.StartFade(this, timeVisible, fadeTime);

		// Calculate how much the height needs to change to accommodate the new notification
		float heightDelta = padding + notification.rectTransform.rect.height;

		// Increase the height of the manager to accommodate the new notification
		rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, rectTransform.rect.height + heightDelta);
		Vector2 pos = rectTransform.anchoredPosition;
		pos.y += heightDelta / 2;
		rectTransform.anchoredPosition = pos;

		// Add the notification to the list of notifications
		notifications.Insert(0, notification);
		// Make sure that the newly addeded notification has pushed the other notifications to the top of the list
		foreach(var note in notifications)
			note.transform.SetSiblingIndex(topIndex);
	}

	// Function which removes a notification from the list of notifications and destroies it
	public void removeNotification(Notification notification){
		// Calculate the new height after this notification is removed
		float heightDelta = padding + notification.rectTransform.rect.height;
		float newHeight = rectTransform.rect.height - heightDelta;

		// Remove the provided notification from the list
		notifications.Remove(notification);

		// Decrease the height of the object to represent thatw e no longer have the notification
		rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, newHeight);
		Vector2 pos = rectTransform.anchoredPosition;
		pos.y -= heightDelta / 2;
		rectTransform.anchoredPosition = pos;

		// Destroy the notification object
		Destroy(notification.gameObject);
	}
	// Override which removes a particular index
	public void removeNotification(int index){ removeNotification(notifications[index]); }

	// Testbench
	IEnumerator test(){
		yield return new WaitForSeconds(1);

		Debug.Log("Pushing Notifications");
		addNotification("Hello!");

		yield return new WaitForSeconds(2);

		addNotification("World!");
		Debug.Log("Pushed Notifications");
	}
}
