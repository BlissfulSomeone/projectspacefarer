using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipEditorUIPause : MonoBehaviour
{
	[SerializeField]
	private Button myContinueButton;
	[SerializeField]
	private Button myQuitButton;

	void Awake()
	{
		myContinueButton.interactable = false;
		myQuitButton.onClick.AddListener(() => { UnityEngine.SceneManagement.SceneManager.LoadScene("SceneMainMenu", UnityEngine.SceneManagement.LoadSceneMode.Single); });
	}

	void OnEnable()
	{
		//EventManager.Instance.AddListener(eEventType.EditorUIEnableContinueButton, EnableButton);
	}

	void OnDisable()
	{
		//EventManager.Instance.RemoveListener(eEventType.EditorUIEnableContinueButton, EnableButton);
	}

	/// <summary>
	/// EnableButton()
	/// </summary>
	private void EnableButton(params object[] args)
	{
		myContinueButton.interactable = true;
	}
}
