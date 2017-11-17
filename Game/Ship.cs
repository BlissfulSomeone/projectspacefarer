using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ship : MonoBehaviour
{
	private ShipLoader.ShipLoaderData myData;
	public ShipLoader.ShipLoaderData Data { get { return myData; } }

	private int myTeamIndex;
	public int TeamIndex { get { return myTeamIndex; } }

	[SerializeField]
	private Turret myTurretPrefab;
	private List<Turret> myTurrets;

	[SerializeField]
	private Shield myShieldPrefab;
	private Shield myShieldInstance;

	private Ship myTarget;
	private Ship myLastAttacker;
	public Ship LastAttacker { get { return myLastAttacker; } set { myLastAttacker = value; } }

	private GameManager.ShipGroup myGroup = null;
	public GameManager.ShipGroup Group { get { return myGroup; } set { myGroup = value; } }

	private float myMaxSpeed;
	public float Speed { get { return myMaxSpeed; } }

	private float myHull;
	private float myArmour;
	private float myShield;
	private float myShieldTimer;
	public float Hull { get { return myHull; } }
	public float Armour { get { return myArmour; } }
	public float Shield { get { return myShield; } }

	private bool myIsDestroyed;
	public bool IsDestroyed { get { return myIsDestroyed; } }

	private Vector3 myVelocity;
	public Vector3 Velocity { get { return myVelocity; } set { myVelocity = value; } }

	private bool myHasPointDefense = false;
	public bool HasPointDefense { get { return myHasPointDefense; } }

	//private eOrderType myOrder = eOrderType.None;
	//public eOrderType Order { get { return myOrder; } }

	//private List<Order> myOrders;
	private List<ShipOrder> myOrders;

	private MeshRenderer myRenderer;
	public MeshRenderer Renderer { get { return myRenderer; } }

	public delegate void ShipDestroyedHandler(Ship aShip);
	public ShipDestroyedHandler OnShipDestroyed;

	void Awake()
	{
		myIsDestroyed = false;
		//myOrders = new List<global::Order>();
		myOrders = new List<ShipOrder>();
		myRenderer = GetComponent<MeshRenderer>();
	}

	public void Init(ShipLoader.ShipLoaderData aShipData, int aTeamIndex)
	{
		myData = aShipData;
		GetComponent<MeshFilter>().mesh = aShipData.mesh;
		myTeamIndex = aTeamIndex;
		myTurrets = new List<Turret>();

		myHull = myData.baseData.hull;
		myArmour = myData.baseData.armour;
		myShield = myData.baseData.shield;

		myMaxSpeed = myData.baseData.speed;

		for (int i = 0; i < myData.softpoints.Count; ++i)
		{
			myArmour += myData.softpoints[i].armourBonus;
			myShield += myData.softpoints[i].shieldBonus;
			//myMaxSpeed += myData.softpoints[i].speedBonus;
		}

		AddTurrets(ref myData.smallHardpoints, 0, 1.0f);
		AddTurrets(ref myData.mediumHardpoints, 1, 1.5f);
		AddTurrets(ref myData.largeHardpoints, 2, 2.5f);

		AddAccessories(ref myData.accessories);

		myShieldInstance = Instantiate(myShieldPrefab);
		myShieldInstance.transform.SetParent(transform);
		myShieldInstance.Init(myData.size * 2.5f);
	}

	private void AddTurrets(ref List<ShipLoader.ShipLoaderSerializerData.Hardpoint> aHardpointList, int aSize, float aScale)
	{
		for (int i = 0; i < aHardpointList.Count; ++i)
		{
			ShipLoader.ShipLoaderSerializerData.Hardpoint hardpoint = aHardpointList[i];
			Turret turret = Instantiate(hardpoint.weapon.turretPrefab);
			turret.Owner = this;
			turret.transform.SetParent(transform);
			turret.transform.localPosition = hardpoint.position;
			turret.transform.localEulerAngles = new Vector3(0.0f, hardpoint.angle, 0.0f);
			turret.transform.localScale = Vector3.one * aScale * 0.2f;
			turret.Init(hardpoint.weapon, aSize);
			myTurrets.Add(turret);

			//Transform sphereT = turret.transform.FindChild("Sphere");
			//Transform rangeT = turret.transform.FindChild("RangeDisplay");
			//Destroy(sphereT.gameObject);
			//Destroy(rangeT.gameObject);

			if (hardpoint.weapon.weaponClass == WeaponData.eWeaponClass.PointDefence)
			{
				myHasPointDefense = true;
			}
		}
	}

	private void AddAccessories(ref List<ShipLoader.ShipLoaderSerializerData.Accessory> aAccessoryList)
	{
		for (int i = 0; i < aAccessoryList.Count; ++i)
		{
			ShipLoader.ShipLoaderSerializerData.Accessory accessory = aAccessoryList[i];
			Transform prop = Instantiate(ContentLoader.Instance.GetPropData(accessory.propName).propPrefab);
			prop.SetParent(transform);
			prop.localPosition = accessory.position;
			prop.localRotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
			prop.localScale = Vector3.one * accessory.scale;
			prop.GetComponent<Thruster>().Init(TeamIndex);
		}
	}

	void Update()
	{
		myShieldInstance.transform.rotation = Quaternion.identity;

		//if (Random.Range(0, 20) == 0)
		//{
		//	myShieldInstance.Trigger(Random.onUnitSphere);
		//}
	}

	public void SetHoverType(string aLayerName)
	{
		int layer = LayerMask.NameToLayer(aLayerName);
		gameObject.layer = layer;
		for (int i = 0; i < myTurrets.Count; ++i)
		{
			myTurrets[i].SetHoverType(layer);
		}
	}

	public void SetTarget(Ship aShip)
	{
		myTarget = aShip;
		for (int i = 0; i < myTurrets.Count; ++i)
		{
			if (myTurrets[i].Weapon.weaponClass != WeaponData.eWeaponClass.PointDefence)
			{
				myTurrets[i].Target = aShip;
			}
		} 
	}

	public void SetTarget(HomingProjectile aHomingProjectile)
	{
		for (int i = 0; i < myTurrets.Count; ++i)
		{
			if (myTurrets[i].Weapon.weaponClass == WeaponData.eWeaponClass.PointDefence)
			{
				myTurrets[i].HomingTarget = aHomingProjectile;
			}
		}
	}

	public void TakeDamage(WeaponData aWeaponData, int aSize, Vector3 aPoint)
	{
		float damage = 0.0f;
		if (aSize == 0) { damage = aWeaponData.damage.small; }
		else if (aSize == 1) { damage = aWeaponData.damage.medium; }
		else if (aSize == 2) { damage = aWeaponData.damage.large; }

		//myShieldTimer = 3.0f;

		if (aWeaponData.fireRate == 0.0f)
		{
			damage *= Time.deltaTime;
		}

		if (myShield > 0.0f && aWeaponData.ignoreShield == false)
		{
			myShield -= damage * aWeaponData.shieldEfficiency;
			if (aPoint != Vector3.zero)
			{
				Vector3 delta = aPoint - transform.position;
				myShieldInstance.Trigger(delta);
			}
		}
		else if (myArmour > 0.0f)
		{
			myArmour -= damage * aWeaponData.armourEfficiency;
		}
		else
		{
			myHull -= damage * aWeaponData.hullEfficiency;
		}
		if (myHull <= 0.0f && myIsDestroyed == false)
		{
			myIsDestroyed = true;
			if (OnShipDestroyed != null)
			{
				OnShipDestroyed(this);
			}
		}
	}

	//public void SetOrder(int aOrder)
	//{
	//	myOrder = (eOrderType)aOrder;
	//}

	public void AddOrder(ShipOrder aOrder)
	{
		myOrders.Add(aOrder);
	}
	
	public void RemoveOrder(int aOrderIndex)
	{
		myOrders.RemoveAt(aOrderIndex);
	}
	
	//public void SetOrderType(int aOrderIndex, eOrderType aOrderType)
	//{
	//	ShipOrder temp = myOrders[aOrderIndex];
	//	myOrders[aOrderIndex] = new ShipOrder(aOrderType, temp.targetType, temp.targetShip);
	//}

	//public void SetOrderTarget(int aOrderIndex, ShipData.eShipModel aTargetType)
	//{
	//	ShipOrder temp = myOrders[aOrderIndex];
	//	myOrders[aOrderIndex] = new ShipOrder(temp.orderType, aTargetType, temp.targetShip);
	//}
	//
	//public void SetOrderTarget(int aOrderIndex, Ship aTargetShip)
	//{
	//	ShipOrder temp = myOrders[aOrderIndex];
	//	myOrders[aOrderIndex] = new ShipOrder(temp.orderType, temp.targetType, aTargetShip);
	//}

	public void SetOrderManouverType(int aOrderIndex, eManouverType aManouverType)
	{
		ShipOrder temp = myOrders[aOrderIndex];
		myOrders[aOrderIndex] = new ShipOrder(aManouverType, temp.targetType, temp.classType, temp.targetShip);
	}

	public void SetOrderTargetType(int aOrderIndex, eTargetType aTargetType)
	{
		ShipOrder temp = myOrders[aOrderIndex];
		myOrders[aOrderIndex] = new ShipOrder(temp.manouverType, aTargetType, temp.classType, temp.targetShip);
	}

	public void SetOrderTargetClass(int aOrderIndex, ShipData.eShipModel aClassType)
	{
		ShipOrder temp = myOrders[aOrderIndex];
		myOrders[aOrderIndex] = new ShipOrder(temp.manouverType, temp.targetType, aClassType, temp.targetShip);
	}

	public void SetOrderTargetShip(int aOrderIndex, Ship aTargetShip)
	{
		ShipOrder temp = myOrders[aOrderIndex];
		myOrders[aOrderIndex] = new ShipOrder(temp.manouverType, temp.targetType, temp.classType, aTargetShip);
	}

	public int GetNumberOfOrders()
	{
		return myOrders.Count;
	}

	public ShipOrder GetOrder(int aOrderIndex)
	{
		return myOrders[aOrderIndex];
	}

	public Bounds GetViewportBounds(Camera aCamera)
	{
		Vector3 camPos = aCamera.transform.position;
		Vector3 camFor = aCamera.transform.forward;

		Bounds shipBound = Renderer.bounds;
		Vector3 cen = shipBound.center;
		Vector3 delta = camPos - cen;
		if (Vector3.Dot(camFor, delta) > 0.0f) return new Bounds(Vector3.zero, Vector3.zero);
		Vector3 ext = shipBound.extents;

		Vector3[] corners = new Vector3[8]
		{
			new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z),
			new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z),
			new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z),
			new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z),
			new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z),
			new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z),
			new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z),
			new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z)
		};

		for (int i = 0; i < 8; ++i)
		{
			Vector3 d = camPos - corners[i];
			if (Vector3.Dot(camFor, d) > 0.0f) return new Bounds(Vector3.zero, Vector3.zero);
		}

		Vector2[] extentPoints = new Vector2[8]
		{
			UIUtilities.WorldToGUIPoint(corners[0], aCamera),
			UIUtilities.WorldToGUIPoint(corners[1], aCamera),
			UIUtilities.WorldToGUIPoint(corners[2], aCamera),
			UIUtilities.WorldToGUIPoint(corners[3], aCamera),
			UIUtilities.WorldToGUIPoint(corners[4], aCamera),
			UIUtilities.WorldToGUIPoint(corners[5], aCamera),
			UIUtilities.WorldToGUIPoint(corners[6], aCamera),
			UIUtilities.WorldToGUIPoint(corners[7], aCamera)
		};

		Vector2 min = extentPoints[0];
		Vector2 max = extentPoints[0];

		foreach (Vector2 v in extentPoints)
		{
			min = Vector2.Min(min, v);
			max = Vector2.Max(max, v);
		}

		Rect rect = new Rect(min.x, Screen.height - min.y, max.x - min.x, min.y - max.y);
		return UIUtilities.GetViewportBounds(aCamera, rect.min, rect.max);
	}

	public Rect GetScreenRect(Camera aCamera)
	{
		Vector3 camPos = aCamera.transform.position;
		Vector3 camFor = aCamera.transform.forward;

		Bounds shipBound = Renderer.bounds;
		Vector3 cen = shipBound.center;
		Vector3 delta = camPos - cen;
		if (Vector3.Dot(camFor, delta) > 0.0f) return new Rect(Vector2.zero, Vector2.zero);
		Vector3 ext = shipBound.extents;

		Vector3[] corners = new Vector3[8]
		{
			new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z),
			new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z),
			new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z),
			new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z),
			new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z),
			new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z),
			new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z),
			new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z)
		};

		for (int i = 0; i < 8; ++i)
		{
			Vector3 d = camPos - corners[i];
			if (Vector3.Dot(camFor, d) > 0.0f) return new Rect(Vector2.zero, Vector2.zero);
		}

		Vector2[] extentPoints = new Vector2[8]
		{
			UIUtilities.WorldToGUIPoint(corners[0], aCamera),
			UIUtilities.WorldToGUIPoint(corners[1], aCamera),
			UIUtilities.WorldToGUIPoint(corners[2], aCamera),
			UIUtilities.WorldToGUIPoint(corners[3], aCamera),
			UIUtilities.WorldToGUIPoint(corners[4], aCamera),
			UIUtilities.WorldToGUIPoint(corners[5], aCamera),
			UIUtilities.WorldToGUIPoint(corners[6], aCamera),
			UIUtilities.WorldToGUIPoint(corners[7], aCamera)
		};

		Vector2 min = extentPoints[0];
		Vector2 max = extentPoints[0];

		foreach (Vector2 v in extentPoints)
		{
			min = Vector2.Min(min, v);
			max = Vector2.Max(max, v);
		}

		Debug.Log("min: " + min);
		Debug.Log("max: " + max);

		return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
	}
}
