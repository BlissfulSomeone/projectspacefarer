using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipEditorUIToolbarMirror : MonoBehaviour
{
	private bool myIsMirrorOn = false;
	//public bool IsMirrorOn { get { return myToggle.isOn; } }

	private static bool gIsMirrorOn = false;
	public static bool IsMirrorOn { get { return gIsMirrorOn; } }

	[SerializeField]
	private Toggle myToggle;

	void Awake()
	{
		myToggle = GetComponent<Toggle>();
		myToggle.onValueChanged.AddListener((value) => { StartToggle(value); });
	}

	private void StartToggle(bool aValue)
	{
		gIsMirrorOn = aValue;
	}

	void Update()
	{
		if (myToggle.isOn != gIsMirrorOn)
		{
			myToggle.isOn = !myToggle.isOn; 
		}
		if (Input.GetButtonDown("Toggle Mirror") == true)
		{
			myToggle.isOn = !myToggle.isOn;
			StartToggle(myToggle.isOn);
		}
	}

	private void ToggleMirrorMode()
	{
		myIsMirrorOn = !myIsMirrorOn;
		//myIcon.sprite = (myIsMirrorOn == true ? myMirrorOnSprite : myMirrorOffSprite);
	}
}
