using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class ContentLoader : MonoBehaviour
{
	private static ContentLoader myInstance = null;
	public static ContentLoader Instance { get { return myInstance; } }

	[SerializeField]
	private CampaignData[] myCampaignData;
	[SerializeField]
	private MissionData[] myMissionData;
	[SerializeField]
	private ShipBaseData[] myShipData;
	[SerializeField]
	private WeaponData[] myWeaponData;
	[SerializeField]
	private SoftpointData[] mySoftpointData;
	[SerializeField]
	private ModuleData[] myModuleData;
	[SerializeField]
	private PropData[] myPropData;

	public CampaignData[] CampaignData { get { return myCampaignData; } }
	public MissionData[] MissionData { get { return myMissionData; } }
	public ShipBaseData[] ShipData { get { return myShipData; } }
	public WeaponData[] WeaponData { get { return myWeaponData; } }
	public SoftpointData[] SoftpointData { get { return mySoftpointData; } }
	public ModuleData[] ModuleData { get { return myModuleData; } }
	public PropData[] PropData { get { return myPropData; } }

	void Awake()
	{
		if (myInstance != null)
		{
			Destroy(gameObject);
			return;
		}
		myInstance = this;
		DontDestroyOnLoad(this);

		LoadShipDataFromXml();
		LoadSoftpointDataFromXml();
		LoadWeaponDataFromXml();
	}

	public CampaignData GetCampaignData(string aCampaignName)
	{
		for (int i = 0; i < myCampaignData.Length; ++i)
		{
			if (myCampaignData[i].campaignName == aCampaignName)
			{
				return myCampaignData[i];
			}
		}
		return null;
	}

	public MissionData GetMissionData(string aMissionName)
	{
		for (int i = 0; i < myMissionData.Length; ++i)
		{
			if (myMissionData[i].missionName == aMissionName)
			{
				return myMissionData[i];
			}
		}
		return null;
	}

	public ShipBaseData GetShipData(string aShipName)
	{
		for (int i = 0; i < myShipData.Length; ++i)
		{
			if (myShipData[i].modelName == aShipName)
			{
				return myShipData[i];
			}
		}
		return null;
	}

	public WeaponData GetWeaponData(string aWeaponName)
	{
		for (int i = 0; i < myWeaponData.Length; ++i)
		{
			if (myWeaponData[i].weaponName == aWeaponName)
			{
				return myWeaponData[i];
			}
		}
		return null;
	}

	public SoftpointData GetSoftpointData(string aSoftpointName)
	{
		for (int i = 0; i < mySoftpointData.Length; ++i)
		{
			if (mySoftpointData[i].softpointName == aSoftpointName)
			{
				return mySoftpointData[i];
			}
		}
		return null;
	}

	public ModuleData GetModuleData(string aModuleName)
	{
		for (int i = 0; i < myModuleData.Length; ++i)
		{
			if (myModuleData[i].moduleName == aModuleName)
			{
				return myModuleData[i];
			}
		}
		return null;
	}

	public PropData GetPropData(string aPropName)
	{
		for (int i = 0; i < myPropData.Length; ++i)
		{
			if (myPropData[i].propName == aPropName)
			{
				return myPropData[i];
			}
		}
		return null;
	}

	private void LoadShipDataFromXml()
	{
		XmlDocument document = new XmlDocument();
		document.Load(Environment.CurrentDirectory + "\\Data\\Core\\ships.xml");
		XmlNodeList shipNodes = document.DocumentElement.SelectNodes("Ship");
		foreach (XmlNode shipNode in shipNodes)
		{
			string shipName = shipNode.Attributes["name"].Value;
			ShipBaseData data = GetShipData(shipName);
			data.cost = (int)float.Parse(shipNode.SelectSingleNode("Base").Attributes["cost"].Value);
			data.maxSoftpoints = (int)float.Parse(shipNode.SelectSingleNode("Base").Attributes["maxSoftpoints"].Value);
			data.maxSmallHardpoints = (int)float.Parse(shipNode.SelectSingleNode("Base").Attributes["maxSmall"].Value);
			data.maxMediumHardpoints = (int)float.Parse(shipNode.SelectSingleNode("Base").Attributes["maxMedium"].Value);
			data.maxLargeHardpoints = (int)float.Parse(shipNode.SelectSingleNode("Base").Attributes["maxLarge"].Value);
			data.hull = (int)float.Parse(shipNode.SelectSingleNode("Health").Attributes["hull"].Value);
			data.armour = (int)float.Parse(shipNode.SelectSingleNode("Health").Attributes["armour"].Value);
			data.shield = float.Parse(shipNode.SelectSingleNode("Health").Attributes["shield"].Value);
			data.powerOutput = float.Parse(shipNode.SelectSingleNode("Power").Attributes["baseOutput"].Value);
			data.speed = float.Parse(shipNode.SelectSingleNode("Engine").Attributes["speed"].Value);
			data.turnRate = float.Parse(shipNode.SelectSingleNode("Engine").Attributes["turnRate"].Value);
			data.firingRange = float.Parse(shipNode.SelectSingleNode("Radar").Attributes["firingRange"].Value);
			data.description = shipNode.SelectSingleNode("Description").Attributes["text"].Value;
		}
	}

	private void LoadSoftpointDataFromXml()
	{
		XmlDocument document = new XmlDocument();
		document.Load(Environment.CurrentDirectory + "\\Data\\Core\\softpoints.xml");
		XmlNodeList softpointNodes = document.DocumentElement.SelectNodes("Softpoint");
		foreach (XmlNode softpointNode in softpointNodes)
		{
			string softpointName = softpointNode.Attributes["name"].Value;
			SoftpointData data = GetSoftpointData(softpointName);
			data.cost = (int)float.Parse(softpointNode.SelectSingleNode("Base").Attributes["cost"].Value);
			data.powerUsage = (int)float.Parse(softpointNode.SelectSingleNode("Base").Attributes["powerUsage"].Value);
			data.powerBonus = (int)float.Parse(softpointNode.SelectSingleNode("Bonus").Attributes["powerOutput"].Value);
			data.speedBonus = (int)float.Parse(softpointNode.SelectSingleNode("Bonus").Attributes["speed"].Value);
			data.armourBonus = (int)float.Parse(softpointNode.SelectSingleNode("Bonus").Attributes["armour"].Value);
			data.shieldBonus = (int)float.Parse(softpointNode.SelectSingleNode("Bonus").Attributes["shield"].Value);
			data.description = softpointNode.SelectSingleNode("Description").Attributes["text"].Value;
		}
	}

	private void LoadWeaponDataFromXml()
	{
		XmlDocument document = new XmlDocument();
		document.Load(Environment.CurrentDirectory + "\\Data\\Core\\weapons.xml");
		XmlNodeList weaponNodes = document.DocumentElement.SelectNodes("Weapon");
		foreach (XmlNode weaponNode in weaponNodes)
		{
			string weaponName = weaponNode.Attributes["name"].Value;
			WeaponData data = GetWeaponData(weaponName);
			data.damage.small = float.Parse(weaponNode.SelectSingleNode("Damage").Attributes["small"].Value);
			data.damage.medium = float.Parse(weaponNode.SelectSingleNode("Damage").Attributes["medium"].Value);
			data.damage.large = float.Parse(weaponNode.SelectSingleNode("Damage").Attributes["large"].Value);
			data.fireRate = float.Parse(weaponNode.SelectSingleNode("FireRate").Attributes["main"].Value);
			data.shotsPerBurst = int.Parse(weaponNode.SelectSingleNode("FireRate").Attributes["shotsPerBurst"].Value);
			data.burstSpeed = float.Parse(weaponNode.SelectSingleNode("FireRate").Attributes["burstFireRate"].Value);
			data.projectileSpeed = float.Parse(weaponNode.SelectSingleNode("ProjectileSpeed").Attributes["speed"].Value);
			data.hitChance = float.Parse(weaponNode.SelectSingleNode("HitChance").Attributes["hitChance"].Value);
			data.powerUsage.small = float.Parse(weaponNode.SelectSingleNode("PowerUsage").Attributes["small"].Value);
			data.powerUsage.medium = float.Parse(weaponNode.SelectSingleNode("PowerUsage").Attributes["medium"].Value);
			data.powerUsage.large = float.Parse(weaponNode.SelectSingleNode("PowerUsage").Attributes["large"].Value);
			data.hullEfficiency = float.Parse(weaponNode.SelectSingleNode("Efficiency").Attributes["hull"].Value);
			data.armourEfficiency = float.Parse(weaponNode.SelectSingleNode("Efficiency").Attributes["armour"].Value);
			data.shieldEfficiency = float.Parse(weaponNode.SelectSingleNode("Efficiency").Attributes["shield"].Value);
			data.ignoreShield = bool.Parse(weaponNode.SelectSingleNode("Efficiency").Attributes["ignoreShield"].Value);
			data.cost.small = (int)float.Parse(weaponNode.SelectSingleNode("Cost").Attributes["small"].Value);
			data.cost.medium = (int)float.Parse(weaponNode.SelectSingleNode("Cost").Attributes["medium"].Value);
			data.cost.large = (int)float.Parse(weaponNode.SelectSingleNode("Cost").Attributes["large"].Value);
		}
	}
}
