using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonTextHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]
	private Color myNormalTextColor = Color.white;
	[SerializeField]
	private Color myHoverTextColor = Color.white;

	public Color NormalTextColor { get { return myNormalTextColor; } set { myNormalTextColor = value; } }
	public Color HoverTextColor { get { return myHoverTextColor; } set { myHoverTextColor = value; } }

	private Selectable myParentButton;
	private Text[] myTextComponents;
	public Text[] TextComponents { get { if (myTextComponents == null) { myTextComponents = GetComponentsInChildren<Text>(); } return myTextComponents; } }
	private bool myIsMouseOver = false;

	void Awake()
	{
		myParentButton = GetComponent<Button>();
		if (myParentButton == null)
		{
			myParentButton = GetComponent<Dropdown>();
			if (myParentButton == null)
			{
				myParentButton = GetComponent<Toggle>(); 
			}
		}
		for (int i = 0; i < TextComponents.Length; ++i)
		{
			TextComponents[i].color = myNormalTextColor;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		myIsMouseOver = true;
		if (myParentButton.interactable == true)
		{
			for (int i = 0; i < TextComponents.Length; ++i)
			{
				TextComponents[i].color = myHoverTextColor;
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		myIsMouseOver = false;
		for (int i = 0; i < TextComponents.Length; ++i)
		{
			TextComponents[i].color = myNormalTextColor;
		}
	}

	public void Refresh()
	{
		for (int i = 0; i < TextComponents.Length; ++i)
		{
			TextComponents[i].color = (myIsMouseOver == true && myParentButton.interactable == true ? myHoverTextColor : myNormalTextColor);
		}
	}
}