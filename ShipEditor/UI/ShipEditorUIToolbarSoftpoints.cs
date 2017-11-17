using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipEditorUIToolbarSoftpoints : MonoBehaviour
{
	[Header("Softpoints")]
	[SerializeField]
	private Button mySoftpointButtonPrefab;
	[SerializeField]
	private Transform myListContent;
	
	public delegate void SelectedSoftpointChangedHandler(SoftpointData aSoftpointData);
	public SelectedSoftpointChangedHandler OnSoftpointChanged;

	void Awake()
	{
		foreach (SoftpointData data in ContentLoader.Instance.SoftpointData)
		{
			Button button = Instantiate(mySoftpointButtonPrefab);
			button.transform.SetParent(myListContent);
			button.image.rectTransform.anchoredPosition3D = Vector3.zero;
			button.image.rectTransform.localScale = Vector3.one;

			Text text = button.GetComponentInChildren<Text>();
			text.text = data.softpointName;

			Image icon = button.transform.FindChild("Icon").GetComponent<Image>();
			icon.sprite = data.icon;

			//myButtons.Add(button);
			//myHardpoints.Add(data);

			SoftpointData tempData = data;
			button.onClick.AddListener(() => { SelectSoftpoint(tempData); });

			UITooltipTrigger tooltipTrigger = button.gameObject.AddComponent<UITooltipTrigger>();
			tooltipTrigger.SetTitle(data.softpointName);
			string description = " ";
			//description += "DPS:\t\t\t\t\t\t" + CalculateDPS(data, data.damage.small) + "/" + CalculateDPS(data, data.damage.medium) + "/" + CalculateDPS(data, data.damage.large) + "\n";
			//description += "Hull:\t\t\t\t\t\t" + Mathf.RoundToInt(data.hullEfficiency * 100) + "%\n";
			//description += "Armour:\t\t\t\t" + Mathf.RoundToInt(data.armourEfficiency * 100) + "%\n";
			//description += "Shield:\t\t\t\t\t" + (data.ignoreShield == true ? "Ignore\n" : Mathf.RoundToInt(data.shieldEfficiency * 100) + "%\n");
			//description += "Speed:\t\t\t\t\t" + Mathf.RoundToInt(data.projectileSpeed) + "\n";
			//description += "Aim:\t\t\t\t\t\t" + Mathf.RoundToInt(data.hitChance) + "%\n";
			//description += "Power Usage:\t" + Mathf.RoundToInt(data.powerUsage.small) + "/" + Mathf.RoundToInt(data.powerUsage.medium) + "/" + Mathf.RoundToInt(data.powerUsage.large) + "\n";
			//description += "Cost:\t\t\t\t\t\t" + data.cost.small + "/" + data.cost.medium + "/" + data.cost.large;
			if (data.softpointName == "Armour")
			{
				description = "Adds " + data.armourBonus + " Armour Points to the ship";
			}
			else if (data.softpointName == "Engines")
			{
				description = "Boosts the engine's Top Speed by " + Mathf.RoundToInt(data.speedBonus) + " km/h";
			}
			else if (data.softpointName == "Reactors")
			{
				description = "Increases the ship's Power Output by " + Mathf.RoundToInt(data.powerBonus) + " GW";
			}
			else if (data.softpointName == "Shields")
			{
				description = "Adds " + Mathf.RoundToInt(data.shieldBonus) + " Shield Points to the ship";
			}
			tooltipTrigger.SetDescription(description);
		}

		myListContent.GetComponentInChildren<UIButtonGroup>().ManualTrigger();

		SelectSoftpoint(ContentLoader.Instance.SoftpointData[0]);
	}
	
	private void SelectSoftpoint(SoftpointData aSoftpointData)
	{
		if (OnSoftpointChanged != null)
		{
			OnSoftpointChanged(aSoftpointData);
		}
	}

	//[SerializeField]
	//private Button myButtonPrefab;
	//[SerializeField]
	//private RectTransform myListContent;
	//
	//[SerializeField]
	//private Color myLabelNormalColor;
	//[SerializeField]
	//private Color myLabelActiveColor;
	//[SerializeField]
	//private Color myLabelTextNormalColor;
	//[SerializeField]
	//private Color myLabelTextActiveColor;
	//
	//private List<Button> myButtons;
	//private List<SoftpointData> mySoftpoints;
	//
	//void Awake()
	//{
	//	myButtons = new List<Button>();
	//	mySoftpoints = new List<SoftpointData>();
	//
	//	foreach (SoftpointData data in ContentLoader.Instance.SoftpointData)
	//	{
	//		Button button = Instantiate(myButtonPrefab);
	//		button.transform.SetParent(myListContent);
	//		button.image.rectTransform.anchoredPosition3D = Vector3.zero;
	//		button.image.rectTransform.localScale = Vector3.one;
	//
	//		Text text = button.GetComponentInChildren<Text>();
	//		text.text = data.softpointName;
	//
	//		myButtons.Add(button);
	//		mySoftpoints.Add(data);
	//
	//		SoftpointData tempData = data;
	//		button.onClick.AddListener(() => { SelectSoftpoint(tempData); });
	//	}
	//
	//	SelectSoftpoint(mySoftpoints[0]);
	//}
	//
	//private void SelectSoftpoint(SoftpointData aData)
	//{
	//	for (int i = 0; i < myButtons.Count; ++i)
	//	{
	//		ColorBlock colors = myButtons[i].colors;
	//		colors.normalColor = (mySoftpoints[i] == aData ? myLabelActiveColor : myLabelNormalColor);
	//		myButtons[i].colors = colors;
	//
	//		UIButtonTextHandler textHandler = myButtons[i].GetComponent<UIButtonTextHandler>();
	//		textHandler.NormalTextColor = (mySoftpoints[i] == aData ? myLabelTextActiveColor : myLabelTextNormalColor);
	//		textHandler.HoverTextColor = (mySoftpoints[i] == aData ? myLabelTextActiveColor : myLabelTextActiveColor);
	//		textHandler.Refresh();
	//	}
	//	//EventManager.Instance.TriggerEvent(eEventType.EditorSoftpointSelected, aData);
	//}
}
