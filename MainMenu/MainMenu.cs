using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[Header("Panels")]
	[SerializeField]
	private Transform myMainPanel;
	[SerializeField]
	private Transform mySettingsPanel;

	[Header("Main Menu")]
	[SerializeField]
	private Button myStartGameButton;
	[SerializeField]
	private Button myEditorButton;
	[SerializeField]
	private Button mySettingsButton;
	[SerializeField]
	private Button myQuitButton;

	[Header("Settings Menu")]
	//[SerializeField]
	//private Dropdown myResolutionDropdown;
	//[SerializeField]
	//private Dropdown myEnvironmentQualityDropdown;
	[SerializeField]
	private Button myBackButton;

	void Awake()
	{
		myStartGameButton.onClick.AddListener(() => { GotoRoom("SceneMission"); });
		myEditorButton.onClick.AddListener(() => { GotoRoom("SceneShipEditor"); });
		mySettingsButton.onClick.AddListener(() => { SetPanel(1); });
		myQuitButton.onClick.AddListener(() => { QuitGame(); });

		//myResolutionDropdown.AddOptions(new List<Dropdown.OptionData> { new Dropdown.OptionData("1280x720"), new Dropdown.OptionData("1360x768x"), new Dropdown.OptionData("1920x1080") });
		//myResolutionDropdown.value = 2;
		//myEnvironmentQualityDropdown.AddOptions(new List<Dropdown.OptionData> { new Dropdown.OptionData("High"), new Dropdown.OptionData("Medium"), new Dropdown.OptionData("Low") });
		//myEnvironmentQualityDropdown.value = 2;
		myBackButton.onClick.AddListener(() => { SetPanel(0); });

		SetPanel(0);
	}
	
	private void GotoRoom(string aSceneName)
	{
		SceneManager.LoadScene(aSceneName, LoadSceneMode.Single);
	}

	private void QuitGame()
	{
		Application.Quit();
	}

	private void SetPanel(int aPanel)
	{
		myMainPanel.gameObject.SetActive(aPanel == 0);
		mySettingsPanel.gameObject.SetActive(aPanel == 1);
	}
}
