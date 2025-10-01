using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// EventManager class from
// https://learn.unity.com/tutorial/create-a-simple-messaging-system-with-events


// Typed event class/functions from https://www.youtube.com/watch?v=EvqdcyTgZNg&t=1334s
[System.Serializable]
public class TypedEvent : UnityEvent<object> { }

[ExecuteAlways]
public class EventManager : MonoBehaviour
{
	public static bool DEBUG = false;

	// hold references to events
	private Dictionary<string, UnityEvent> eventDictionary;
	private Dictionary<string, TypedEvent> typedEventDictionary;

	// show event count
	public int eventCount = 0;

	// show event names (as list because you can serialize)
	public List<string> eventNames = new List<string>();

	// singleton
	private static EventManager eventManager;
	public static EventManager instance
	{
		get
		{
			// if we don't have it
			if (!eventManager)
			{
				// then find it
				eventManager = FindFirstObjectByType(typeof(EventManager)) as EventManager;
				// if null
				if (!eventManager)
				{
					Debug.LogError(
						"There needs to be one active EventManger script on a GameObject in your scene."
					);
				}
				else
				{
					// initialize
					eventManager.Init();
				}
			}
			return eventManager;
		}
	}

	void Init()
	{
		if (eventDictionary == null)
		{
			eventDictionary = new Dictionary<string, UnityEvent>();
			typedEventDictionary = new Dictionary<string, TypedEvent>();
		}
	}

	public static void StartListening(string eventName, UnityAction listener)
	{
		// Debug.Log("StartListening() eventName = " + eventName);
		UnityEvent thisEvent = null;
		// is there already a key/value pair?
		if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.AddListener(listener);
		}
		else
		{
			// add new event
			thisEvent = new UnityEvent();
			thisEvent.AddListener(listener);
			instance.eventDictionary.Add(eventName, thisEvent);
		}
		UpdateEventDetails();
	}

	public static void StartListening(string eventName, UnityAction<object> listener)
	{
		TypedEvent thisEvent = null;
		// is there already a key/value pair?
		if (instance.typedEventDictionary.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.AddListener(listener);
		}
		else
		{
			// add new event
			thisEvent = new TypedEvent();
			thisEvent.AddListener(listener);
			instance.typedEventDictionary.Add(eventName, thisEvent);
		}
		UpdateEventDetails();
	}

	public static void StopListening(string eventName, UnityAction listener)
	{
		// make sure not null
		if (eventManager == null)
			return;
		// otherwise remove event
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.RemoveListener(listener);
		}
		UpdateEventDetails();
	}

	public static void StopListening(string eventName, UnityAction<object> listener)
	{
		// make sure not null
		if (eventManager == null)
			return;
		// otherwise remove event
		TypedEvent thisEvent = null;
		if (instance.typedEventDictionary.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.RemoveListener(listener);
		}
		UpdateEventDetails();
	}

	public static void TriggerEvent(string eventName)
	{
		LogEventName(eventName);
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
			thisEvent.Invoke();
	}

	public static void TriggerEvent(string eventName, object data)
	{
		LogEventName(eventName);
		TypedEvent thisEvent = null;
		if (instance.typedEventDictionary.TryGetValue(eventName, out thisEvent))
			thisEvent.Invoke(data);
	}

	static void LogEventName(string str)
	{
		// return;
		// if application is playing
		if (Application.IsPlaying(instance))
		{
			str = str.ToString().Trim();
			// Best practices for comparing strings in .NET
			// https://learn.microsoft.com/en-us/dotnet/standard/base-types/best-practices-strings
			if (
				!str.Contains("ResetFeedbackData", System.StringComparison.OrdinalIgnoreCase)
				&& !str.Contains("UpdateFeedback", System.StringComparison.OrdinalIgnoreCase)
				&& !str.Contains("SetProgressBarFull", System.StringComparison.OrdinalIgnoreCase)
				&& !str.Contains("SetProgressBarBumps", System.StringComparison.OrdinalIgnoreCase)
				&& !str.Contains("UpdateProgressBarFill", System.StringComparison.OrdinalIgnoreCase)
				&& !str.Contains("UpdateHintButtons", System.StringComparison.OrdinalIgnoreCase)
				&& !str.Contains(
					"PlayPatternProgressionSound",
					System.StringComparison.OrdinalIgnoreCase
				)
			)
				if (DEBUG)
					Debug.Log(
						$"##################  EventManager.TriggerEvent() {str}  ##################"
					);
		}
	}

	public static void UpdateEventDetails()
	{
		// get count
		instance.eventCount = instance.eventDictionary.Count + instance.typedEventDictionary.Count;
		// clear list
		instance.eventNames.Clear();
		// add names to list to display in inspector
		foreach (var e in instance.eventDictionary)
		{
			instance.eventNames.Add(e.Key);
		}

		foreach (var e in instance.typedEventDictionary)
		{
			instance.eventNames.Add(e.Key);
		}
		instance.eventNames.Sort();
	}
}
