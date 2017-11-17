using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIButtonDoubleClickHandler : MonoBehaviour, IPointerClickHandler
{
	[System.Serializable]
	public class PointerDoubleClickEvent : UnityEvent
	{ }

	public PointerDoubleClickEvent onPointerDoubleClicked = new PointerDoubleClickEvent();

	public void OnPointerClick(PointerEventData aEvent)
	{
		if (aEvent.clickCount >= 2)
		{
			onPointerDoubleClicked.Invoke();
		}
	}
}
