using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShipEditorUISaveDesign : MonoBehaviour
{
	[SerializeField]
	private Button myButtonPrefab;
	[SerializeField]
	private RectTransform myListContent;
	[SerializeField]
	private Button mySaveButton;
	[SerializeField]
	private InputField myInputField;

	private List<Button> myButtons;

	void Awake()
	{
		myButtons = new List<Button>();
		mySaveButton.onClick.AddListener(() => { SaveShipFromInput(); });
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
		if (aMode != ShipEditorUI.eShipEditorUIMode.Save) return;

		for (int i = 0; i < myButtons.Count; ++i)
		{
			Destroy(myButtons[i].gameObject);
		}
		myButtons.Clear();

		string currentDirectory = Environment.CurrentDirectory;
		DirectoryInfo directory = new DirectoryInfo(currentDirectory + "\\Data\\Ships");
		FileInfo[] files = directory.GetFiles();
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
		button.image.rectTransform.anchoredPosition3D = Vector3.zero;
		button.transform.localScale = Vector3.one;

		Text text = button.GetComponentInChildren<Text>();
		text.text = aName;

		UIButtonDoubleClickHandler doubleClickHandler = button.gameObject.AddComponent<UIButtonDoubleClickHandler>();
		doubleClickHandler.onPointerDoubleClicked.AddListener(() => { SaveShip(aName); });
		button.onClick.AddListener(() => { SelectShip(aName); });

		myButtons.Add(button);
	}

	private void SaveShipFromInput()
	{
		string inputText = myInputField.text;
		//if (myAvailableShips.Contains(inputText) == true)
		{
			SaveShip(inputText);
		}
	}

	private void SaveShip(string aName)
	{
		ShipEditor.Instance.SaveShip(aName);
	}

	private void SelectShip(string aName)
	{
		myInputField.text = aName;
	}
}
