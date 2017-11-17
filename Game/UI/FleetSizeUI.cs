using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FleetSizeUI : MonoBehaviour
{
	[Header("Red")]
	[SerializeField]
	private Text myRedFleetText;
	[SerializeField]
	private Image myRedFiller;

	[Header("Blue")]
	[SerializeField]
	private Text myBlueFleetText;
	[SerializeField]
	private Image myBlueFiller;

	private int myRedPower;
	private int myRedPowerMax;
	private int myBluePower;
	private int myBluePowerMax;

	public void AddShip(Ship aShip)
	{
		if (aShip.TeamIndex == 0)
		{
			myBluePower += aShip.Data.cost;
			myBluePowerMax += aShip.Data.cost;
		}
		else
		{
			myRedPower += aShip.Data.cost;
			myRedPowerMax += aShip.Data.cost;
		}
		UpdateLabels();
	}

	public void RemoveShip(Ship aShip)
	{
		if (aShip.TeamIndex == 0)
		{
			myBluePower -= aShip.Data.cost;
			myBluePowerMax -= aShip.Data.cost;
		}
		else
		{
			myRedPower -= aShip.Data.cost;
			myRedPowerMax -= aShip.Data.cost;
		}
		UpdateLabels();
	}

	public void ShipDestroyed(Ship aShip)
	{
		if (aShip.TeamIndex == 0)
		{
			myBluePower -= aShip.Data.cost;
		}
		else
		{
			myRedPower -= aShip.Data.cost;
		}
		UpdateLabels();
	}

	private void UpdateLabels()
	{
		myBlueFleetText.text = myBluePower.ToString() + "/" + myBluePowerMax.ToString();
		myBlueFiller.fillAmount = (float)myBluePower / myBluePowerMax;
		myRedFleetText.text = myRedPower.ToString() + "/" + myRedPowerMax.ToString();
		myRedFiller.fillAmount = (float)myRedPower / myRedPowerMax;
	}
}
