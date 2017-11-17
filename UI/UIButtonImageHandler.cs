using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonImageHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]
	private Color myNormalImageColor = Color.white;
	[SerializeField]
	private Color myHoverImageColor = Color.white;

	public Color NormalImageColor { get { return myNormalImageColor; } set { myNormalImageColor = value; } }
	public Color HoverImageColor { get { return myHoverImageColor; } set { myHoverImageColor = value; } }

	private Selectable myParentButton;
	private Image[] myImageComponents;
	public Image[] ImageComponents
	{
		get
		{
			if (myImageComponents == null)
			{
				List<Image> images = new List<Image>();
				Image[] imagesInChildren = GetComponentsInChildren<Image>();
				for (int i = 0; i < imagesInChildren.Length; ++i)
				{
					if (imagesInChildren[i].transform != transform)
					{
						images.Add(imagesInChildren[i]);
					}
				}
				myImageComponents = images.ToArray();
			}
			return myImageComponents;
		}
	}
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
		for (int i = 0; i < ImageComponents.Length; ++i)
		{
			ImageComponents[i].color = myNormalImageColor;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		myIsMouseOver = true;
		if (myParentButton.interactable == true)
		{
			for (int i = 0; i < ImageComponents.Length; ++i)
			{
				ImageComponents[i].color = myHoverImageColor;
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		myIsMouseOver = false;
		for (int i = 0; i < ImageComponents.Length; ++i)
		{
			ImageComponents[i].color = myNormalImageColor;
		}
	}

	public void Refresh()
	{
		for (int i = 0; i < ImageComponents.Length; ++i)
		{
			ImageComponents[i].color = (myIsMouseOver == true && myParentButton.interactable == true ? myHoverImageColor : myNormalImageColor);
		}
	}
}
