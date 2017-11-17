using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIMouseHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[System.Serializable]
	public class PointerEnterEvent : UnityEvent
	{ }
	[System.Serializable]
	public class PointerExitEvent : UnityEvent
	{ }

	public PointerEnterEvent onPointerEnter = new PointerEnterEvent();
	public PointerExitEvent onPointerExit = new PointerExitEvent();

	public void OnPointerEnter(PointerEventData aEvent)
	{
		onPointerEnter.Invoke();
	}

	public void OnPointerExit(PointerEventData aEvent)
	{
		onPointerExit.Invoke();
	}
}
