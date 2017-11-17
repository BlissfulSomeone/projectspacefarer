using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipData
{
	public enum eShipClass
	{ 
		Screen,
		Destroyer,
		Cruiser,
	}

	public enum eShipModel
	{
		PatrolBoat,
		GunBoat,
		ForwardOffenceShip,
		MainlineShip,
		ArmouredCruiser,
		BattleCruiser,
	}
	
	[HideInInspector]
	public int shipClassIndex;
	[HideInInspector]
	public int shipModelIndex;

	private ShipBaseData baseData;

	public string className;
	public string modelName;
	public int maxSoftpoints = 0;
	public int maxSmallHardpoints = 0;
	public int maxMediumHardpoints = 0;
	public int maxLargeHardpoints = 0;
	public int hull = 0;
	public int armour = 0;
	public float shield = 0.0f;
	public float powerOutput = 0.0f;
	public float speed = 0.0f;
	public int cost = 0;
	
	public float powerUsage = 0;

	public List<Transform> blocks;
	public SoftpointData[] softpoints;
	public List<Turret> smallHardpoints;
	public List<Turret> mediumHardpoints;
	public List<Turret> largeHardpoints;
	public List<Transform> props;

	public ShipData()
	{
		blocks = new List<Transform>();
		smallHardpoints = new List<Turret>();
		mediumHardpoints = new List<Turret>();
		largeHardpoints = new List<Turret>();
		props = new List<Transform>();
	}

	//public void Init(eShipModel aModel)
	public void Init(ShipBaseData aBaseData)
	{
		//eShipModel shipModel = aModel;
		//shipModelIndex = (int)shipModel;
		//if (shipModel == eShipModel.PatrolBoat || shipModel == eShipModel.GunBoat) shipClassIndex = 0;
		//else if (shipModel == eShipModel.ForwardOffenceShip || shipModel == eShipModel.MainlineShip) shipClassIndex = 1;
		//else if (shipModel == eShipModel.ArmouredCruiser || shipModel == eShipModel.BattleCruiser) shipClassIndex = 2;
		if (aBaseData.modelName == "Patrol Boat") { shipClassIndex = 0; shipModelIndex = 0; }
		else if (aBaseData.modelName == "Gun Boat") { shipClassIndex = 0; shipModelIndex = 1; }
		else if (aBaseData.modelName == "Forward Offence Ship") { shipClassIndex = 1; shipModelIndex = 2; }
		else if (aBaseData.modelName == "Mainline Ship") { shipClassIndex = 1; shipModelIndex = 3; }
		else if (aBaseData.modelName == "Armoured Cruiser") { shipClassIndex = 2; shipModelIndex = 4; }
		else if (aBaseData.modelName == "Battle Cruiser") { shipClassIndex = 2; shipModelIndex = 5; }

		baseData = aBaseData;
		softpoints = new SoftpointData[baseData.maxSoftpoints];
		UpdateStats();
	}

	public void UpdateStats()
	{
		className = baseData.className;
		modelName = baseData.modelName;
		maxSoftpoints = baseData.maxSoftpoints;
		maxSmallHardpoints = baseData.maxSmallHardpoints;
		maxMediumHardpoints = baseData.maxMediumHardpoints;
		maxLargeHardpoints = baseData.maxLargeHardpoints;
		hull = baseData.hull;
		armour = baseData.armour;
		shield = baseData.shield;
		powerOutput = baseData.powerOutput;
		speed = baseData.speed;
		cost = baseData.cost;
		
		powerUsage = Mathf.Floor((shield + speed) / 30.0f);
		
		for (int i = 0; i < softpoints.Length; ++i)
		{
			if (softpoints[i]!= null)
			{
				powerOutput += softpoints[i].powerBonus;
				speed += softpoints[i].speedBonus;
				armour += softpoints[i].armourBonus;
				shield += softpoints[i].shieldBonus;
				powerUsage += softpoints[i].powerUsage;
				cost += softpoints[i].cost;
			}
		}

		for (int i = 0; i < smallHardpoints.Count; ++i)
		{
			powerUsage += smallHardpoints[i].Weapon.powerUsage.small;
			cost += smallHardpoints[i].Weapon.cost.small;
		}
		for (int i = 0; i < mediumHardpoints.Count; ++i)
		{
			powerUsage += mediumHardpoints[i].Weapon.powerUsage.medium;
			cost += mediumHardpoints[i].Weapon.cost.medium;
		}
		for (int i = 0; i < largeHardpoints.Count; ++i)
		{
			powerUsage += largeHardpoints[i].Weapon.powerUsage.large;
			cost += largeHardpoints[i].Weapon.cost.large;
		}
	}

	public void SetSoftpoint(int aSlotIndex, SoftpointData aSoftpoint)
	{
		softpoints[aSlotIndex] = aSoftpoint;
		UpdateStats();
	}

	public bool CanAffordHardpoints(int aCost, int aSizeIndex)
	{
		if (aSizeIndex == 0)
		{
			return (smallHardpoints.Count + aCost <= maxSmallHardpoints);
		}
		else if (aSizeIndex == 1)
		{
			return (mediumHardpoints.Count + aCost <= maxMediumHardpoints);
		}
		return (largeHardpoints.Count + aCost <= maxLargeHardpoints);
	}

	public void AddHardpoint(Turret aTurret, int aSizeIndex)
	{
		if (aSizeIndex == 0)
		{
			smallHardpoints.Add(aTurret);
		}
		else if (aSizeIndex == 1)
		{
			mediumHardpoints.Add(aTurret);
		}
		else
		{
			largeHardpoints.Add(aTurret);
		}
		UpdateStats();
	}

	public void RemoveHardpoint(Turret aTurret)
	{
		if (smallHardpoints.Contains(aTurret) == true)
		{
			smallHardpoints.Remove(aTurret);
		}
		if (mediumHardpoints.Contains(aTurret) == true)
		{
			mediumHardpoints.Remove(aTurret);
		}
		if (largeHardpoints.Contains(aTurret) == true)
		{
			largeHardpoints.Remove(aTurret);
		}
		UpdateStats();
	}
}
