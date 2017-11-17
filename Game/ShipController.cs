using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
	private struct ShipMover
	{
		public bool isAlive;
		public int id;
		public int team;
		public float speed;
		public float maxSpeed;
		public float turnSpeed;
		public float range;
		public Vector3 position;
		public Vector3 velocity;
		public Vector3 destination;
		public Quaternion rotation;
		
		public int targetId;
		public List<ShipOrder> orders;
		public eManouverType manouverType;
		public eTargetType targetType;
	}

	private int myNumberOfShips;
	private Ship[] myShips;
	private ShipMover[] myMovers;

	[SerializeField]
	private Texture2D myShipIndicatorScreen;
	[SerializeField]
	private Texture2D myShipIndicatorDestroyer;
	[SerializeField]
	private Texture2D myShipIndicatorBattle;

	[SerializeField]
	private float myEngagementRange;

	private float myTicker = 0.0f;
	private const float INTERNAL_DELTA_TIME = 0.02f;

	public void Init()
	{
		//Debug.Log(System.Runtime.InteropServices.Marshal.SizeOf(typeof(ShipMover)));
		myShips = FindObjectsOfType<Ship>();
		myNumberOfShips = myShips.Length;
		myMovers = new ShipMover[myNumberOfShips];
		InitPositions();
	}

	private void InitPositions()
	{
		for (int i = 0; i < myNumberOfShips; ++i)
		{
			myMovers[i] = new ShipMover();
			myMovers[i].id = i;
			myMovers[i].isAlive = true;
			myMovers[i].team = myShips[i].TeamIndex;
			myMovers[i].position = myShips[i].transform.position;
			myMovers[i].velocity = Vector3.zero;
			myMovers[i].destination = myMovers[i].position;
			myMovers[i].rotation = myShips[i].transform.rotation;
			myMovers[i].speed = 0.0f;
			myMovers[i].maxSpeed = myShips[i].Speed;
			myMovers[i].turnSpeed = myShips[i].Data.baseData.turnRate;
			myMovers[i].range = myShips[i].Data.baseData.firingRange;
			myMovers[i].targetId = -1;

			myMovers[i].orders = new List<ShipOrder>();
			for (int j = 0; j < myShips[i].GetNumberOfOrders(); ++j)
			{
				ShipOrder temp = myShips[i].GetOrder(j);
				myMovers[i].orders.Add(new ShipOrder(temp.manouverType, temp.targetType, temp.classType, temp.targetShip));
			}
		}
	}

	void Update()
	{
		myTicker += Time.deltaTime;
		while (myTicker >= INTERNAL_DELTA_TIME)
		{
			Tick();
			myTicker -= INTERNAL_DELTA_TIME;
		}
	}

	private void Tick()
	{
		for (int i = 0; i < myNumberOfShips; ++i)
		{
			if (myMovers[i].isAlive == false) continue;
			UpdateMover(ref myMovers[i]);
		}

		for (int i = 0; i < myNumberOfShips; ++i)
		{
			if (myShips[i] != null && myMovers[i].isAlive == true)
			{
				if (myShips[i].IsDestroyed == false)
				{
					ShipMover mover = myMovers[i];
					myShips[i].transform.position = mover.position;
					myShips[i].transform.rotation = mover.rotation;
					myShips[i].Velocity = mover.velocity;
					myShips[i].SetTarget((mover.targetId != -1 && myMovers[mover.targetId].isAlive == true) ? myShips[mover.targetId] : null);
				}
				else
				{
					Explosion explosion = GameManager.Instance.GetExplosionPrefabByTeamIndex(myMovers[i].team).GetPooledInstance<Explosion>();
					explosion.transform.position = myMovers[i].position;
					explosion.Init(myShips[i].Data.size / 6.5f);

					FindObjectOfType<GameManager>().FleetSizeUI.ShipDestroyed(myShips[i]);

					myMovers[i].isAlive = false;
					Destroy(myShips[i].gameObject);
				}
			}
		}

		for (int i = 0; i < myNumberOfShips; ++i)
		{
			if (myShips[i] != null && myShips[i].IsDestroyed == false && myShips[i].HasPointDefense == true)
			{
				HomingProjectile closest = null;
				float closestDistance = float.MaxValue;
				for (int j = 0; j < HomingProjectile.HomingProjectiles.Count; ++j)
				{
					float distance = (myShips[i].transform.position - HomingProjectile.HomingProjectiles[j].transform.position).magnitude;
					if (distance < closestDistance)
					{
						closestDistance = distance;
						closest = HomingProjectile.HomingProjectiles[j];
					}
				}
				if (closest != null)
				{
					myShips[i].SetTarget(closest);
				}
			}
		}
	}

	private void UpdateMover(ref ShipMover aMover)
	{
		if (aMover.orders.Count == 0)
		{
			aMover.manouverType = eManouverType.HeadOn;
			aMover.targetType = eTargetType.Closest;
			// UpdateAttack(ref aMover);
			UpdateTarget(ref aMover);
			UpdateMovement(ref aMover);
		}
		else
		{
			aMover.manouverType = aMover.orders[0].manouverType;
			aMover.targetType = aMover.orders[0].targetType;
			UpdateTarget(ref aMover);
			UpdateMovement(ref aMover);
		}
	}

	private void UpdateTarget(ref ShipMover aMover)
	{
		if ((HasValidTarget(ref aMover) == true || Random.Range(0, 100) != 0) && aMover.targetType != eTargetType.Protect) return;

		aMover.targetId = -1;
		switch (aMover.targetType)
		{
			case eTargetType.Closest:
				GetTargetClosest(ref aMover);
				break;
		
			case eTargetType.Furthest:
				GetTargetFurthest(ref aMover);
				break;
		
			case eTargetType.Weakest:
				GetTargetWeakest(ref aMover);
				break;
			
			case eTargetType.Strongest:
				GetTargetStrongest(ref aMover);
				break;
			
			//case eTargetType.Assist:
			//	break;
			//
			case eTargetType.Protect:
				GetTargetProtect(ref aMover);
				break;
			
			//case eTargetType.Specific:
			//	break;
		
			case eTargetType.None:
				Debug.LogError("Ship has target type [NONE]. This should not happen!");
				break;
		
			default:
				//GetTargetClosest(ref aMover);
				Debug.LogError("Ship has no target type at all. This should definitely not happen!");
				break;
		}
	}

	#region GetTarget
	private void GetTargetClosest(ref ShipMover aMover)
	{
		int closestId = -1;
		float closestDistance = float.MaxValue;
		for (int i = 0; i < myNumberOfShips; ++i)
		{
			if (i == aMover.id) continue;
			if (myMovers[i].team == aMover.team) continue;
			if (myMovers[i].isAlive == false) continue;

			float distance = (myMovers[i].position - aMover.position).magnitude;
			if (distance < closestDistance && distance < myEngagementRange)
			{
				closestDistance = distance;
				closestId = i;
			}
		}
		aMover.targetId = closestId;
	}

	private void GetTargetFurthest(ref ShipMover aMover)
	{
		int furthestId = -1;
		float furthestDistance = 0.0f;
		for (int i = 0; i < myNumberOfShips; ++i)
		{
			if (i == aMover.id) continue;
			if (myMovers[i].team == aMover.team) continue;
			if (myMovers[i].isAlive == false) continue;

			float distance = (myMovers[i].position - aMover.position).magnitude;
			if (distance > furthestDistance && distance < myEngagementRange)
			{
				furthestDistance = distance;
				furthestId = i;
			}
		}
		aMover.targetId = furthestId;
	}

	private void GetTargetWeakest(ref ShipMover aMover)
	{
		int weakestId = -1;
		float lowestHealth = float.MaxValue;
		for (int i = 0; i < myNumberOfShips; ++i)
		{
			if (i == aMover.id) continue;
			if (myMovers[i].team == aMover.team) continue;
			if (myMovers[i].isAlive == false) continue;

			float distance = (myMovers[i].position - aMover.position).magnitude;
			float health = myShips[i].Hull + myShips[i].Armour + myShips[i].Shield;
			if (health < lowestHealth && distance < myEngagementRange)
			{
				lowestHealth = health;
				weakestId = i;
			}
		}
		aMover.targetId = weakestId;
	}

	private void GetTargetStrongest(ref ShipMover aMover)
	{
		int strongestId = -1;
		float highestHealth = 0.0f;
		for (int i = 0; i < myNumberOfShips; ++i)
		{
			if (i == aMover.id) continue;
			if (myMovers[i].team == aMover.team) continue;
			if (myMovers[i].isAlive == false) continue;

			float distance = (myMovers[i].position - aMover.position).magnitude;
			float health = myShips[i].Hull + myShips[i].Armour + myShips[i].Shield;
			if (health > highestHealth && distance < myEngagementRange)
			{
				highestHealth = health;
				strongestId = i;
			}
		}
		aMover.targetId = strongestId;
	}

	private void GetTargetProtect(ref ShipMover aMover)
	{
		int escortId = System.Array.IndexOf(myShips, aMover.orders[0].targetShip);
		bool validEscort = (escortId != -1 && escortId != aMover.id && myMovers[escortId].team == aMover.team && myMovers[escortId].isAlive == true);
		if (validEscort == true)
		{
			if (aMover.orders[0].targetShip.LastAttacker != null)
			{
				aMover.targetId = System.Array.IndexOf(myShips, aMover.orders[0].targetShip.LastAttacker);
			}
		}
	}
	#endregion

	private void UpdateMovement(ref ShipMover aMover)
	{
		aMover.speed = aMover.maxSpeed;
		UpdateDestination(ref aMover);
		RotateTowardsDestination(ref aMover);
		Move(ref aMover);
	}

	private void UpdateDestination(ref ShipMover aMover)
	{
		if (aMover.targetType == eTargetType.Protect)
		{
			GetDestinationProtect(ref aMover);
			return;
		}

		switch (aMover.manouverType)
		{
			case eManouverType.HeadOn:
				GetDestinationHeadOn(ref aMover);
				break;

			case eManouverType.Broadside:
				GetDestinationBroadside(ref aMover);
				break;

			case eManouverType.Artillery:
				GetDestinationHeadOn(ref aMover);
				break;

			case eManouverType.None:
				Debug.LogError("Ship has manouver type [NONE]. This should not happen!");
				break;

			default:
				Debug.LogError("Ship has no manouver type at all. This should definitely not happen!");
				break;
		}
		if (aMover.targetType == eTargetType.Protect)
		{
			
		}
	}

	private void GetDestinationHeadOn(ref ShipMover aMover)
	{
		if (HasValidTarget(ref aMover) == true)
		{
			aMover.destination = myMovers[aMover.targetId].position;
		}
		else
		{
			aMover.destination = aMover.position + (aMover.rotation * Vector3.forward) * 10.0f;
		}
	}

	private void GetDestinationBroadside(ref ShipMover aMover)
	{
		if (HasValidTarget(ref aMover) == true)
		{
			Vector3 delta = myMovers[aMover.targetId].position - aMover.position;
			Vector3 dir = -delta.normalized;
			dir = Quaternion.Euler(0.0f, 90.0f, 0.0f) * dir;
			aMover.destination = myMovers[aMover.targetId].position + dir * 100.0f;
		}
		else
		{
			aMover.destination = aMover.position + (aMover.rotation * Vector3.forward) * 10.0f;
		}
	}

	private void GetDestinationArtillery(ref ShipMover aMover)
	{
		
	}

	private void GetDestinationProtect(ref ShipMover aMover)
	{
		int escortId = System.Array.IndexOf(myShips, aMover.orders[0].targetShip);
		bool validEscort = (escortId != -1 && escortId != aMover.id && myMovers[escortId].team == aMover.team && myMovers[escortId].isAlive == true);
		if (validEscort == true)
		{
			if (HasValidTarget(ref aMover) == true)
			{
				aMover.destination = myMovers[aMover.targetId].position;
			}
			else
			{
				//aMover.destination = myMovers[escortId].position;
				Vector3 delta = myMovers[escortId].position - aMover.position;
				Vector3 dir = -delta.normalized;
				dir = Quaternion.Euler(0.0f, 90.0f, 0.0f) * dir;
				aMover.destination = myMovers[escortId].position + dir * 100.0f;
				aMover.speed = aMover.maxSpeed / 2.0f;
			}
		}
		else
		{
			GetTargetClosest(ref aMover);
			GetDestinationHeadOn(ref aMover);
		}
	}

	private void RotateTowardsDestination(ref ShipMover aMover)
	{
		Vector3 destination = GetAdjustedDestination(ref aMover);
		Vector3 delta = destination - aMover.position;
		Vector3 deltaNorm = delta.normalized;
		Vector3 forward = aMover.rotation * Vector3.forward;
		//float anglesToTarget = Vector3.Angle(forward, deltaNorm);
		//float rotatePower = Mathf.Pow(anglesToTarget / 180.0f, 0.5f);
		float rotatePower = 1;
		Vector3 rotation = Vector3.RotateTowards(forward, deltaNorm, aMover.turnSpeed * Mathf.Deg2Rad * INTERNAL_DELTA_TIME * rotatePower, 0.0f);
		//float angles = Vector3.Angle(transform.forward, rotation);
		//float d = Vector3.Dot(transform.right, rotation);

		aMover.rotation = Quaternion.LookRotation(rotation, Vector3.up);

		//if (d > 0.0f)
		//{
		//	angles *= -1.0f;
		//}
		//leaning = Mathf.MoveTowards(leaning, angles, 10.0f * Time.fixedDeltaTime);
		//transform.Rotate(new Vector3(0.0f, 0.0f, leaning * 25.0f), Space.Self);
	}

	private Vector3 GetAdjustedDestination(ref ShipMover aMover)
	{
		Vector3 destination = aMover.destination;
		return destination;
		float distanceFromCenter = destination.magnitude;
		float weightToCenter = Mathf.Clamp01(distanceFromCenter / 2048.0f);
		destination *= (1 - weightToCenter);
		return destination;
	}

	private void Move(ref ShipMover aMover)
	{
		aMover.velocity = (aMover.rotation * Vector3.forward) * aMover.speed;
		aMover.position += aMover.velocity * INTERNAL_DELTA_TIME;
	}

	private bool HasValidTarget(ref ShipMover aMover)
	{
		return (aMover.targetId != -1 &&
				aMover.targetId != aMover.id &&
				myMovers[aMover.targetId].team != aMover.team &&
				myMovers[aMover.targetId].isAlive == true);
	}

	void OnGUI()
	{
		if (Input.GetKey(KeyCode.Space) == false) return;
		float iconSize = 16.0f;
		Camera camera = Camera.main;
		Rect rect = Rect.zero;
		rect.width = iconSize;
		rect.height = iconSize;
		for (int i = 0; i < myNumberOfShips; ++i)
		{
			if (myMovers[i].isAlive == false) continue;

			Vector3 delta = camera.transform.position - myMovers[i].position;
			if (Vector3.Dot(camera.transform.forward, delta) > 0.0f) continue;

			Vector3 screenPosition = camera.WorldToScreenPoint(myMovers[i].position);
			rect.x = screenPosition.x - (iconSize / 2);
			rect.y = Screen.height - screenPosition.y - (iconSize / 2);
			GUI.color = (myMovers[i].team == 0 ? Color.blue : Color.red);
			Texture2D icon = myShipIndicatorScreen;
			if (myShips[i].Data.baseData.className == "Destroyer") icon = myShipIndicatorDestroyer;
			else if (myShips[i].Data.baseData.className == "Cruiser") icon = myShipIndicatorBattle;
			GUI.DrawTexture(rect, icon);
		}
	}
}