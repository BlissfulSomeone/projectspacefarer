using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridToggler : MonoBehaviour
{
	[SerializeField]
	private Transform myGridTransform;
	private bool myIsToggled;

	void Awake()
	{
		myIsToggled = myGridTransform.gameObject.activeSelf;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.G) == true &&
			(Input.GetKey(KeyCode.LeftAlt) == true ||
			Input.GetKey(KeyCode.RightAlt) == true ||
			Input.GetKey(KeyCode.AltGr) == true))
		{
			Toggle();
		}
	}

	private void Toggle()
	{
		myIsToggled = !myIsToggled;
		myGridTransform.gameObject.SetActive(myIsToggled);
	}
}
