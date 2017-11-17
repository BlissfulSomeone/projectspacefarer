using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuFullScreen : MonoBehaviour
{
	[SerializeField]
	private Toggle myToggle;

	void Awake()
	{
		myToggle.isOn = Screen.fullScreen;

		myToggle.onValueChanged.AddListener((value) => { SetFullscreen(value); });
	}

	private void SetFullscreen(bool aValue)
	{
		Screen.SetResolution(PlayerPreferences.CurrentResolution.width, PlayerPreferences.CurrentResolution.height, aValue, PlayerPreferences.CurrentResolution.refreshRate);
		PlayerPreferences.SetFullscreen(aValue);
	}
}
