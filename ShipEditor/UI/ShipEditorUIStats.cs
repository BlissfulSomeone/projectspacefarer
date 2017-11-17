using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipEditorUIStats : MonoBehaviour
{
	[SerializeField]
	private Text myShipTitleText;
	[SerializeField]
	private Text myShipStatsText;
	
	void OnEnable()
	{
		//EventManager.Instance.AddListener(eEventType.EditorShipChanged, UpdateStats);
	}

	void OnDisable()
	{
		//EventManager.Instance.RemoveListener(eEventType.EditorShipChanged, UpdateStats);
	}

	public void UpdateStats(ShipData aShipData)
	{
		myShipTitleText.text = aShipData.className + "\n" + aShipData.modelName;
		myShipStatsText.text = "\n" + aShipData.smallHardpoints.Count + " / " + aShipData.maxSmallHardpoints + "\n";
		myShipStatsText.text += aShipData.mediumHardpoints.Count + " / " + aShipData.maxMediumHardpoints + "\n";
		myShipStatsText.text += aShipData.largeHardpoints.Count + " / " + aShipData.maxLargeHardpoints + "\n";
		myShipStatsText.text += aShipData.softpoints.Length + "\n";
		myShipStatsText.text += aShipData.hull + "\n";
		myShipStatsText.text += aShipData.armour + "\n";
		myShipStatsText.text += (int)aShipData.shield + "\n";
		myShipStatsText.text += (int)aShipData.speed + "\n";
		if (aShipData.powerUsage > aShipData.powerOutput)
		{
			myShipStatsText.text += "<color=#ff0000ff>" + Mathf.CeilToInt(aShipData.powerUsage) + " / " + Mathf.FloorToInt(aShipData.powerOutput) + "</color>\n";
		}
		else
		{
			myShipStatsText.text += Mathf.CeilToInt(aShipData.powerUsage) + " / " + Mathf.FloorToInt(aShipData.powerOutput) + "\n";
		}
		myShipStatsText.text += aShipData.cost;
	}

	///// <summary>
	///// UpdateStats(ShipData aShipData)
	///// </summary>
	///// <param name="args"></param>
	//private void UpdateStats(params object[] args)
	//{
	//	ShipData aShipData = (ShipData)args[0];
	//
	//	myShipTitleText.text = aShipData.className + "\n" + aShipData.modelName;
	//	myShipStatsText.text = "\n" + aShipData.smallHardpoints.Count + " / " + aShipData.maxSmallHardpoints + "\n";
	//	myShipStatsText.text += aShipData.mediumHardpoints.Count + " / " + aShipData.maxMediumHardpoints + "\n";
	//	myShipStatsText.text += aShipData.largeHardpoints.Count + " / " + aShipData.maxLargeHardpoints + "\n";
	//	myShipStatsText.text += aShipData.softpoints.Length + "\n";
	//	myShipStatsText.text += aShipData.hull + "\n";
	//	myShipStatsText.text += aShipData.armour + "\n";
	//	myShipStatsText.text += (int)aShipData.shield + "\n";
	//	myShipStatsText.text += (int)aShipData.speed + "\n";
	//	myShipStatsText.text += (int)aShipData.powerUsage + " / " + (int)aShipData.powerOutput + "\n";
	//	myShipStatsText.text += aShipData.cost;
	//}
}
