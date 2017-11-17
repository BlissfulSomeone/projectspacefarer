using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum eEventType
{
	HighlightUI
}

public class EventManager : MonoBehaviour
{
	public class ActionEvent : UnityEvent<object[]> { }

	private Dictionary<eEventType, ActionEvent> myEvents;

	private static EventManager myInstance;
	public static EventManager Instance { get { return myInstance; } }

	void Awake()
	{ 
		if (myInstance != null)
		{
			Destroy(gameObject);
			return;
		}
		myInstance = this;
		DontDestroyOnLoad(this);
		Init();
	}

	private void Init()
	{
		myEvents = new Dictionary<eEventType, ActionEvent>();
	}

	public void AddListener(eEventType aEventType, UnityAction<object[]> aListener)
	{
		ActionEvent thisEvent = null;
		if (myEvents.TryGetValue(aEventType, out thisEvent) == true)
		{
			thisEvent.AddListener(aListener);
		}
		else
		{
			thisEvent = new ActionEvent();
			thisEvent.AddListener(aListener);
			myEvents.Add(aEventType, thisEvent);
		}
	}

	public void RemoveListener(eEventType aEventType, UnityAction<object[]> aListener)
	{
		ActionEvent thisEvent = null;
		if (myEvents.TryGetValue(aEventType, out thisEvent) == true)
		{
			thisEvent.RemoveListener(aListener);
		}
	}

	public void TriggerEvent(eEventType aEventType, params object[] args)
	{
		ActionEvent thisEvent = null;
		if (myEvents.TryGetValue(aEventType, out thisEvent) == true)
		{
			thisEvent.Invoke(args);
		}
	}
}
