using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInputFieldTextHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]
	private Color myNormalTextColor;
	[SerializeField]
	private Color myActiveTextColor;

	private InputField myInput;

	private Text[] myTextComponents;
	public Text[] TextComponents { get { if (myTextComponents == null) { myTextComponents = GetComponentsInChildren<Text>(); } return myTextComponents; } }

	private bool myIsMouseOver = false;
	private bool myLastIsMouseOver = false;
	private bool myIsInputFocused = false;
	private bool myLastIsInputFocused = false;

	void Awake()
	{
		myInput = GetComponent<InputField>();
		myTextComponents = GetComponentsInChildren<Text>();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		myIsMouseOver = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		myIsMouseOver = false;
	}

	void Update() 
	{
		myIsInputFocused = myInput.isFocused;
		if (myIsMouseOver != myLastIsMouseOver || myIsInputFocused != myLastIsInputFocused)
		{
			for (int i = 0; i < myTextComponents.Length; ++i)
			{
				myTextComponents[i].color = ((myInput.isFocused == true || myIsMouseOver == true) ? myActiveTextColor : myNormalTextColor);
			}
		}
		myLastIsMouseOver = myIsMouseOver;
		myLastIsInputFocused = myIsInputFocused;
	}
}
