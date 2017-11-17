using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuResolution : MonoBehaviour
{
	[SerializeField]
	private Dropdown myDropdown;

	private Resolution[] myResolutions;

	void Awake()
	{
		UpdateAvailableResolutions();

		myDropdown.onValueChanged.AddListener((value) => { SetResolution(myResolutions[value]); });
	}

	void OnEnable()
	{
		PlayerPreferences.OnFullscreenChanged += FullscreenToggled;
	}

	void OnDisable()
	{
		PlayerPreferences.OnFullscreenChanged -= FullscreenToggled;
	}

	private void FullscreenToggled(bool aFullscreen)
	{
		UpdateAvailableResolutions();
	}

	private void UpdateAvailableResolutions()
	{
		myDropdown.ClearOptions();
		Resolution[] resolutions = Screen.resolutions;
		List<Resolution> tmp = new List<Resolution>();
		for (int i = 0; i < resolutions.Length; ++i)
		{
			if (tmp.Contains(resolutions[i]) == false)
			{
				tmp.Add(resolutions[i]);
			}
		}
		myResolutions = new Resolution[tmp.Count];
		List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
		for (int i = 0; i < tmp.Count; ++i)
		{
			myResolutions[i] = tmp[i];
			string line = tmp[i].width.ToString() + "x" + tmp[i].height.ToString() + " " + tmp[i].refreshRate.ToString() + "hz";
			options.Add(new Dropdown.OptionData(line));
		}
		myDropdown.AddOptions(options);
		myDropdown.RefreshShownValue();

		SetCurrentValue();
	}

	private void SetCurrentValue()
	{
		for (int i = 0; i < myResolutions.Length; ++i)
		{
			if (myResolutions[i].width == Screen.currentResolution.width &&
				myResolutions[i].height == Screen.currentResolution.height &&
				myResolutions[i].refreshRate == Screen.currentResolution.refreshRate)
			{
				myDropdown.value = i;
			}
		}
	}

	private void SetResolution(Resolution aResolution)
	{
		Screen.SetResolution(aResolution.width, aResolution.height, PlayerPreferences.IsFullscreen, aResolution.refreshRate);
		PlayerPreferences.SetResolution(aResolution);
	}
}
