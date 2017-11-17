using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShipEditorUILoadDesign : MonoBehaviour
{
	[SerializeField]
	private Button myButtonPrefab;
	[SerializeField]
	private RectTransform myListContent;
	[SerializeField]
	private Button myLoadButton;
	[SerializeField]
	private InputField myInputField;

	private List<string> myAvailableShips;
	private List<Button> myButtons;

	void Awake()
	{
		myAvailableShips = new List<string>();
		myButtons = new List<Button>();
		myLoadButton.onClick.AddListener(() => { LoadShipFromInput(); });
	}

	void OnEnable()
	{
		ShipEditor.Instance.myUI.OnUIModeChanged += UIModeChanged;
	}

	void OnDisable()
	{
		ShipEditor.Instance.myUI.OnUIModeChanged -= UIModeChanged;
	}

	private void UIModeChanged(ShipEditorUI.eShipEditorUIMode aMode)
	{
		if (aMode != ShipEditorUI.eShipEditorUIMode.Load) return;

		for (int i = 0; i < myButtons.Count; ++i)
		{
			Destroy(myButtons[i].gameObject);
		}
		myButtons.Clear();
		myAvailableShips.Clear();

		string currentDirectory = Environment.CurrentDirectory;
		DirectoryInfo directory = new DirectoryInfo(currentDirectory + "\\Data\\Ships");
		FileInfo[] files = directory.GetFiles();
		Debug.Log(files.Length);
		for (int i = 0; i < files.Length; ++i)
		{
			FileInfo file = files[i];
			if (file.Extension == ".xml")
			{
				string[] fileName = file.Name.Split('.');
				AddButton(fileName[0]);
			}
		}
	}

	private void AddButton(string aName)
	{
		Button button = Instantiate(myButtonPrefab);
		button.transform.SetParent(myListContent);
		//button.image.rectTransform.sizeDelta = new Vector2(button.image.rectTransform.sizeDelta.x, 48.0f);
		button.image.rectTransform.anchoredPosition3D = Vector3.zero;
		button.transform.localScale = Vector3.one;

		Text text = button.GetComponentInChildren<Text>();
		text.text = aName;

		UIButtonDoubleClickHandler doubleClickHandler = button.gameObject.AddComponent<UIButtonDoubleClickHandler>();
		doubleClickHandler.onPointerDoubleClicked.AddListener(() => { LoadShip(aName); });
		button.onClick.AddListener(() => { SelectShip(aName); });

		myAvailableShips.Add(aName);
		myButtons.Add(button);
	}

	private void LoadShipFromInput()
	{
		string inputText = myInputField.text;
		if (myAvailableShips.Contains(inputText) == true)
		{
			LoadShip(inputText);
		}
	}

	private void LoadShip(string aName)
	{
		ShipEditor.Instance.LoadShip(aName);
		//EventManager.Instance.TriggerEvent(eEventType.EditorLoadDesign, aName);
	}

	private void SelectShip(string aName)
	{
		myInputField.text = aName;
	}
}
