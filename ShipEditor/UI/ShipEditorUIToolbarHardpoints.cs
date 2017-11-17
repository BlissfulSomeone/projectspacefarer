using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipEditorUIToolbarHardpoints : MonoBehaviour
{
	[Header("Size")]
	[SerializeField]
	private Button[] mySizeIndexButtons;

	[Header("Weapons")]
	[SerializeField]
	private Button myWeaponButtonPrefab;
	[SerializeField]
	private Transform myListContent;

	public delegate void SelectedWeaponSizeChanged(int aSizeIndex);
	public SelectedWeaponSizeChanged OnWeaponSizeChanged;

	public delegate void SelectedWeaponChanged(WeaponData aWeaponData);
	public SelectedWeaponChanged OnWeaponChanged;

	void Awake()
	{
		for (int i = 0; i < mySizeIndexButtons.Length; ++i)
		{
			int tempIndex = i;
			mySizeIndexButtons[i].onClick.AddListener(() => { SelectWeaponSize(tempIndex); });
		}

		foreach (WeaponData data in ContentLoader.Instance.WeaponData)
		{
			Button button = Instantiate(myWeaponButtonPrefab);
			button.transform.SetParent(myListContent);
			button.image.rectTransform.anchoredPosition3D = Vector3.zero;
			button.image.rectTransform.localScale = Vector3.one;

			Text text = button.GetComponentInChildren<Text>();
			text.text = data.weaponName;

			Image icon = button.transform.FindChild("Icon").GetComponent<Image>();
			icon.sprite = data.icon;

			//myButtons.Add(button);
			//myHardpoints.Add(data);

			WeaponData tempData = data;
			button.onClick.AddListener(() => { SelectWeapon(tempData); });

			UITooltipTrigger tooltipTrigger = button.gameObject.AddComponent<UITooltipTrigger>();
			tooltipTrigger.SetTitle(data.weaponName);
			string description = string.Empty;
			description += "DPS:\t\t\t\t\t\t" + CalculateDPS(data, data.damage.small) + "/" + CalculateDPS(data, data.damage.medium) + "/" + CalculateDPS(data, data.damage.large) + "\n";
			description += "Hull:\t\t\t\t\t\t" + Mathf.RoundToInt(data.hullEfficiency * 100) + "%\n";
			description += "Armour:\t\t\t\t" + Mathf.RoundToInt(data.armourEfficiency * 100) + "%\n";
			description += "Shield:\t\t\t\t\t" + (data.ignoreShield == true ? "Ignore\n" : Mathf.RoundToInt(data.shieldEfficiency * 100) + "%\n");
			description += "Speed:\t\t\t\t\t" + Mathf.RoundToInt(data.projectileSpeed) + "\n";
			description += "Aim:\t\t\t\t\t\t" + Mathf.RoundToInt(data.hitChance) + "%\n";
			description += "Power Usage:\t" + Mathf.RoundToInt(data.powerUsage.small) + "/" + Mathf.RoundToInt(data.powerUsage.medium) + "/" + Mathf.RoundToInt(data.powerUsage.large) + "\n";
			description += "Cost:\t\t\t\t\t\t" + data.cost.small + "/" + data.cost.medium + "/" + data.cost.large;
			tooltipTrigger.SetDescription(description);
		}

		myListContent.GetComponentInChildren<UIButtonGroup>().ManualTrigger();

		SelectWeapon(ContentLoader.Instance.WeaponData[0]);
	}

	private int CalculateDPS(WeaponData aData, float aDamage)
	{
		if (aData.fireRate == 0.0f)
		{
			return Mathf.RoundToInt(aDamage);
		}

		if (aData.shotsPerBurst <= 1)
		{
			return Mathf.RoundToInt(aDamage / (1.0f / aData.fireRate));
		}
		else
		{
			float timePerBurst = (1.0f / aData.fireRate) + ((aData.shotsPerBurst - 1) * (1.0f / aData.burstSpeed));
			return (Mathf.RoundToInt((aDamage * aData.shotsPerBurst) / timePerBurst));
		}
	}

	private void SelectWeaponSize(int aSizeIndex)
	{
		if (OnWeaponSizeChanged != null)
		{
			OnWeaponSizeChanged(aSizeIndex);
		}
	}

	private void SelectWeapon(WeaponData aWeaponData)
	{
		if (OnWeaponChanged != null)
		{
			OnWeaponChanged(aWeaponData);
		}
	}

	//[System.Serializable]
	//public class SizeButton
	//{
	//	public int sizeIndex;
	//	public Button button;
	//}
	//
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
	//private List<WeaponData> myHardpoints;
	//
	//[SerializeField]
	//private SizeButton[] mySizeButtons;

	//void Awake()
	//{
	//	myButtons = new List<Button>();
	//	myHardpoints = new List<WeaponData>();
	//	
	//	foreach (WeaponData data in ContentLoader.Instance.WeaponData)
	//	{
	//		Button button = Instantiate(myButtonPrefab);
	//		button.transform.SetParent(myListContent);
	//		button.image.rectTransform.anchoredPosition3D = Vector3.zero;
	//		button.image.rectTransform.localScale = Vector3.one;
	//
	//		Text text = button.GetComponentInChildren<Text>();
	//		text.text = data.weaponName;
	//
	//		myButtons.Add(button);
	//		myHardpoints.Add(data);
	//
	//		WeaponData tempData = data;
	//		button.onClick.AddListener(() => { SelectHardpoint(tempData); });
	//	}
	//
	//	for (int i = 0; i < mySizeButtons.Length; ++i)
	//	{
	//		int tempSize = mySizeButtons[i].sizeIndex;
	//		mySizeButtons[i].button.onClick.AddListener(() => { SelectSizeIndex(tempSize); });
	//	}
	//
	//	SelectHardpoint(myHardpoints[0]);
	//	SelectSizeIndex(0);
	//}
	//
	//private void SelectHardpoint(WeaponData aData)
	//{
	//	for (int i = 0; i < myButtons.Count; ++i)
	//	{
	//		ColorBlock colors = myButtons[i].colors;
	//		colors.normalColor = (myHardpoints[i] == aData ? myLabelActiveColor : myLabelNormalColor);
	//		myButtons[i].colors = colors;
	//
	//		UIButtonTextHandler textHandler = myButtons[i].GetComponent<UIButtonTextHandler>();
	//		textHandler.NormalTextColor = (myHardpoints[i] == aData ? myLabelTextActiveColor : myLabelTextNormalColor);
	//		textHandler.HoverTextColor = (myHardpoints[i] == aData ? myLabelTextActiveColor : myLabelTextActiveColor);
	//		textHandler.Refresh();
	//	}
	//	EventManager.Instance.TriggerEvent(eEventType.EditorHardpointSelected, aData);
	//}
	//
	//private void SelectSizeIndex(int aSizeIndex)
	//{
	//	for (int i = 0; i < mySizeButtons.Length; ++i)
	//	{
	//		ColorBlock colors = mySizeButtons[i].button.colors;
	//		colors.normalColor = (mySizeButtons[i].sizeIndex == aSizeIndex ? myLabelActiveColor : myLabelNormalColor);
	//		mySizeButtons[i].button.colors = colors;
	//
	//		UIButtonTextHandler textHandler = mySizeButtons[i].button.GetComponent<UIButtonTextHandler>();
	//		textHandler.NormalTextColor = (mySizeButtons[i].sizeIndex == aSizeIndex ? myLabelTextActiveColor : myLabelTextNormalColor);
	//		textHandler.HoverTextColor = (mySizeButtons[i].sizeIndex == aSizeIndex ? myLabelTextActiveColor : myLabelTextActiveColor);
	//		textHandler.Refresh();
	//	}
	//	EventManager.Instance.TriggerEvent(eEventType.EditorSizeIndexSelected, aSizeIndex);
	//}
}
