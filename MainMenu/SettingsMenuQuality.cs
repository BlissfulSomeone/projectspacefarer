using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuQuality : MonoBehaviour
{
	[SerializeField]
	private Dropdown myDropdown;

	void Awake()
	{
		myDropdown.AddOptions(new List<Dropdown.OptionData> { new Dropdown.OptionData("High"), new Dropdown.OptionData("Medium"), new Dropdown.OptionData("Low") });
		myDropdown.value = 2;

		myDropdown.onValueChanged.AddListener((value) => { SetQuality(value); });

		myDropdown.value = (int)PlayerPreferences.EnvironmentQuality;
	}

	private void SetQuality(int aValue)
	{
		PlayerPreferences.SetEnvironmentQuality((PlayerPreferences.eEnvironmentQuality)aValue);
	}
}
