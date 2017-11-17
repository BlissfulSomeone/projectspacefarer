using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Project Spacefare/Weapon Data", order = 1)]
public class WeaponData : ScriptableObject
{
	public enum eWeaponClass
	{ 
		Ballistic,
		Energy,
		Explosive,
		PointDefence,
	}

	[System.Serializable]
	public struct WeaponDamage
	{
		public float small;
		public float medium;
		public float large;
	}

	[System.Serializable]
	public struct WeaponCost
	{
		public int small;
		public int medium;
		public int large;
	}

	[System.Serializable]
	public struct PowerUsage
	{
		public float small;
		public float medium;
		public float large;
	}

	public string weaponName = string.Empty;
	public eWeaponClass weaponClass;
	public WeaponDamage damage;
	public float range = 0.0f;
	public float fireRate = 0.0f;
	public int shotsPerBurst = 1;
	public float burstSpeed = 0.0f;
	public float projectileSpeed = 0.0f;
	public float trackingRate = 0.0f;
	public float hitChance = 100.0f;
	public PowerUsage powerUsage;
	public float hullEfficiency = 0.0f;
	public float armourEfficiency = 0.0f;
	public float shieldEfficiency = 0.0f;
	public bool ignoreShield = false;
	public WeaponCost cost;
	public Turret turretPrefab;
	public Mesh mesh;
	//public Projectile projectilePrefab;
	public ParticleSystem projectileEmitter;
	public AudioClip firingSound;
	public bool loopSound = false;
	public string description = string.Empty;
	public Sprite icon;
}
