using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ShipDesignButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
	[System.Serializable]
	public class PointerEnterEvent : UnityEvent
	{ }
	[System.Serializable]
	public class PointerExitEvent : UnityEvent
	{ }
	[System.Serializable]
	public class PointerDownEvent : UnityEvent
	{ }

	public PointerEnterEvent onPointerEnter = new PointerEnterEvent();
	public PointerExitEvent onPointerExit = new PointerExitEvent();
	public PointerDownEvent onPointerDown = new PointerDownEvent();

	public void OnPointerEnter(PointerEventData aEvent)
	{
		onPointerEnter.Invoke();
	}

	public void OnPointerExit(PointerEventData aEvent)
	{
		onPointerExit.Invoke();
	}

	public void OnPointerDown(PointerEventData aEvent)
	{
		onPointerDown.Invoke();
	}
}
