using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITooltip : MonoBehaviour
{
	private RectTransform myRect;
	private CanvasGroup myCanvasGroup;
	private bool myIsOpen = false;
	public bool IsOpen { get { return myIsOpen; } }
	[SerializeField]
	private Text myTitle;
	[SerializeField]
	private Text myDescription;

	private static UITooltip myInstance = null;
	public static UITooltip Instance { get { return myInstance; } }

	void Awake()
	{
		if (myInstance != null)
		{
			Destroy(gameObject);
			return;
		}
		myInstance = this;

		myRect = GetComponent<RectTransform>();
		myCanvasGroup = GetComponent<CanvasGroup>();
		Hide();
	}

	void Update()
	{
		if (myIsOpen == true)
		{
			Vector3 anchoredPosition = myRect.anchoredPosition;
			anchoredPosition = Input.mousePosition;
			myRect.anchoredPosition = anchoredPosition;
		}
	}

	public void Open(string aTitle, string aDescription)
	{
		myIsOpen = true;
		myCanvasGroup.alpha = 1;
		myTitle.text = aTitle;
		myDescription.text = aDescription;
		StartCoroutine(Coroutine_FixSize());
	}

	public void Hide()
	{
		myIsOpen = false;
		myCanvasGroup.alpha = 0;
	}

	private IEnumerator Coroutine_FixSize()
	{
		yield return new WaitForEndOfFrame();
		Vector3 size = myRect.sizeDelta;
		size.y = myTitle.rectTransform.sizeDelta.y + myDescription.rectTransform.sizeDelta.y + 2.0f;
		myRect.sizeDelta = size;
	}
}
