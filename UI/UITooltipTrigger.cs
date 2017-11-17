using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UITooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]
	private string myTooltipTitle;
	[SerializeField]
	private string myTooltipDescription;

	public void OnPointerEnter(PointerEventData eventData)
	{
		UITooltip.Instance.Open(myTooltipTitle, myTooltipDescription);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		UITooltip.Instance.Hide();
	}

	public void SetTitle(string aTitle)
	{
		myTooltipTitle = aTitle;
		if (UITooltip.Instance.IsOpen == true)
		{
			UITooltip.Instance.Open(myTooltipTitle, myTooltipDescription);
		}
	}

	public void SetDescription(string aDescription)
	{
		myTooltipDescription = aDescription;
		if (UITooltip.Instance.IsOpen == true)
		{
			UITooltip.Instance.Open(myTooltipTitle, myTooltipDescription);
		}
	}
}
