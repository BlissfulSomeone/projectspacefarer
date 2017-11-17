using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipDesignList : MonoBehaviour
{
	[SerializeField]
	private RectTransform myContentList;
	[SerializeField]
	private Button myButtonPrefab;

	//[SerializeField]
	//private Button myTeamOneButton;
	//[SerializeField]
	//private Button myTeamTwoButton;

	//[SerializeField]
	//private Color myLabelNormalColor;
	//[SerializeField]
	//private Color myLabelActiveColor;
	//[SerializeField]
	//private Color myLabelTextNormalColor;
	//[SerializeField]
	//private Color myLabelTextActiveColor;

	//[SerializeField]
	//private ShipDesignInfoPanel myInfoPanel;

	private Button[] myTeamButtons;
	private int myTeamIndex;

	void Awake()
	{
		//myTeamOneButton.onClick.AddListener(() => { SetTeamMode(0); });
		//myTeamTwoButton.onClick.AddListener(() => { SetTeamMode(1); });
		//myTeamButtons = new Button[] { myTeamOneButton, myTeamTwoButton };
		//SetTeamMode(0);

		foreach (KeyValuePair<string, ShipLoader.ShipLoaderData> kvp in ShipLoader.Instance.LoadedData)
		{
			Button button = Instantiate(myButtonPrefab);
			button.transform.SetParent(myContentList.transform);

			Text text = button.GetComponentInChildren<Text>();
			text.text = kvp.Key;

			ShipDesignButtonHandler buttonHandler = button.gameObject.AddComponent<ShipDesignButtonHandler>();
			ShipLoader.ShipLoaderData tempData = kvp.Value;
			//buttonHandler.onPointerEnter.AddListener(() => { myInfoPanel.Show(tempData); });
			//buttonHandler.onPointerExit.AddListener(() => { myInfoPanel.Hide(); });

			float powerOutput = kvp.Value.baseData.powerOutput;
			float powerUsage = Mathf.Floor((kvp.Value.baseData.shield + kvp.Value.baseData.speed) / 30.0f);
			for (int i = 0; i < kvp.Value.softpoints.Count; ++i)
			{
				powerOutput += kvp.Value.softpoints[i].powerBonus;
				powerUsage += kvp.Value.softpoints[i].powerUsage;
			}
			for (int i = 0; i < kvp.Value.smallHardpoints.Count; ++i)
			{
				powerUsage += kvp.Value.smallHardpoints[i].weapon.powerUsage.small;
			}
			for (int i = 0; i < kvp.Value.mediumHardpoints.Count; ++i)
			{
				powerUsage += kvp.Value.mediumHardpoints[i].weapon.powerUsage.medium;
			}
			for (int i = 0; i < kvp.Value.largeHardpoints.Count; ++i)
			{
				powerUsage += kvp.Value.largeHardpoints[i].weapon.powerUsage.large;
			}
			if (powerUsage <= powerOutput)
			{
				//buttonHandler.onPointerDown.AddListener(() => { CreateSquad(kvp.Key); });
				buttonHandler.onPointerDown.AddListener(() => { SelectShipType(kvp.Key); });
			}
			else
			{
				button.GetComponent<UIButtonTextHandler>().enabled = false;
				text.text = "(WIP) " + text.text;
				text.color = new Color(32.0f / 255.0f, 32.0f / 255.0f, 32.0f / 255.0f, 1.0f);
			}
		}
	}

	//private void SetTeamMode(int aTeamIndex)
	//{
	//	if (GameManager.Instance != null)
	//	{
	//		GameManager.Instance.SetTeamIndex(aTeamIndex);
	//	}
	//	myTeamIndex = aTeamIndex;
	//	for (int i = 0; i < 2; ++i)
	//	{
	//		ColorBlock colors = myTeamButtons[i].colors;
	//		colors.normalColor = (i == aTeamIndex ? myLabelActiveColor : myLabelNormalColor);
	//		myTeamButtons[i].colors = colors;
	//
	//		UIButtonTextHandler textHandler = myTeamButtons[i].GetComponent<UIButtonTextHandler>();
	//		textHandler.NormalTextColor = (i == aTeamIndex ? myLabelTextActiveColor : myLabelTextNormalColor);
	//		textHandler.HoverTextColor = (i == aTeamIndex ? myLabelTextActiveColor : myLabelTextActiveColor);
	//		textHandler.Refresh();
	//	}
	//}

	private void SelectShipType(string aFileName)
	{
		GameManager.Instance.SetShipType(aFileName);
	}

	//private void CreateSquad(string aFileName)
	//{
	//	Vector3 spawnPoint = (myTeamIndex == 0 ? new Vector3(0.0f, 0.0f, -125.0f) : new Vector3(0.0f, 0.0f, 125.0f));
	//	spawnPoint += new Vector3(Random.Range(-125.0f, 125.0f), Random.Range(-125.0f, 125.0f), Random.Range(-25.0f, 25.0f));
	//}
}
