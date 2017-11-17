using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipDesignInfoPanel : MonoBehaviour
{
	[SerializeField]
	private Text myShipClassText;
	[SerializeField]
	private Text myLabelsText;
	[SerializeField]
	private Text myShipStatsText;
	[SerializeField]
	private Text myShipHardpointsText;

	void Awake()
	{
		Hide();
	}

	public void Hide()
	{
		myShipClassText.gameObject.SetActive(false);
		myLabelsText.gameObject.SetActive(false);
		myShipStatsText.gameObject.SetActive(false);
		myShipHardpointsText.gameObject.SetActive(false);
	}

	public void Show(ShipLoader.ShipLoaderData aData)
	{
		myShipClassText.gameObject.SetActive(true);
		myLabelsText.gameObject.SetActive(true);
		myShipStatsText.gameObject.SetActive(true);
		myShipHardpointsText.gameObject.SetActive(true);

		myShipClassText.text = aData.baseData.className + "\n" + aData.baseData.modelName;
		float hull = aData.baseData.hull;
		float armour = aData.baseData.armour;
		float shield = aData.baseData.shield;
		float speed = aData.baseData.speed;
		float powerOutput = aData.baseData.powerOutput;
		float powerUsage = shield + speed;
		float cost = aData.baseData.cost;
		for (int i = 0; i < aData.softpoints.Count; ++i)
		{
			armour += aData.softpoints[i].armourBonus;
			shield += aData.softpoints[i].shieldBonus;
			speed += aData.softpoints[i].speedBonus;
			powerOutput += aData.softpoints[i].powerBonus;
			powerUsage += aData.softpoints[i].powerUsage;
			cost += aData.softpoints[i].cost;
		}

		myShipHardpointsText.text = "<b>Small Hardpoints</b>\n";
		for (int i = 0; i < aData.smallHardpoints.Count; ++i)
		{
			myShipHardpointsText.text += "\t" + aData.smallHardpoints[i].weapon.weaponName + "\n";
			powerUsage += aData.smallHardpoints[i].weapon.powerUsage.small;
			cost += aData.smallHardpoints[i].weapon.cost.small;
		}
		myShipHardpointsText.text += "<b>Medium Hardpoints</b>\n";
		for (int i = 0; i < aData.mediumHardpoints.Count; ++i)
		{
			myShipHardpointsText.text += "\t" + aData.mediumHardpoints[i].weapon.weaponName + "\n";
			powerUsage += aData.mediumHardpoints[i].weapon.powerUsage.medium;
			cost += aData.mediumHardpoints[i].weapon.cost.medium;
		}
		myShipHardpointsText.text += "<b>Large Hardpoints</b>\n";
		for (int i = 0; i < aData.largeHardpoints.Count; ++i)
		{
			myShipHardpointsText.text += "\t" + aData.largeHardpoints[i].weapon.weaponName + "\n";
			powerUsage += aData.largeHardpoints[i].weapon.powerUsage.large;
			cost += aData.largeHardpoints[i].weapon.cost.large;
		}
		myShipStatsText.text = (int)hull + "\n" + (int)armour + "\n" + (int)shield + "\n" + (int)speed + "\n" + (int)powerUsage + " / " + (int)powerOutput + "\n$" + (int)cost;
	}
}
