using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
	[SerializeField]
	private Transform myWeaponTransform;
	[SerializeField]
	private DiscMesh myRangeDisplay;

	protected WeaponData myWeapon;
	public WeaponData Weapon { get { return myWeapon; } }

	public float WeaponAngle { get { return myWeaponTransform.eulerAngles.y; } }

	protected AudioSource myAudio;
	public AudioSource Audio { get { return myAudio; } }

	protected int mySize = 0;
	public int Size { get { return mySize; } }

	protected MeshFilter myMeshFilter;

	protected Ship myOwner;
	public Ship Owner { get { return myOwner; } set { myOwner = value; } }

	protected Ship myTarget;
	public Ship Target { get { return myTarget; } set { myTarget = value; } }

	protected HomingProjectile myHomingTarget;
	public HomingProjectile HomingTarget { get { return myHomingTarget; } set { myHomingTarget = value; } }

	protected Vector3 myBaseRotation;
	protected float myReload;
	protected int myRemainingShots;

	protected bool myIsFiring;

	protected ParticleSystem myProjectileEmitter;

	void Awake()
	{
		myAudio = GetComponent<AudioSource>();
		ShowRangeDisplay(false);
		myMeshFilter = GetComponentInChildren<MeshFilter>();
		myReload = 0.0f;
		myRemainingShots = 0;

		Destroy(transform.FindChild("Sphere").gameObject);
		Destroy(myRangeDisplay.gameObject);
	}

	public void PointWeapon(Vector3 aForward)
	{ 
		myWeaponTransform.rotation = Quaternion.LookRotation(aForward.normalized, Vector3.up);
	}

	public void ShowRangeDisplay(bool aFlag)
	{
		myRangeDisplay.gameObject.SetActive(aFlag);
	}

	public void Init(WeaponData aWeaponData, int aSize)
	{
		myWeapon = aWeaponData;
		mySize = aSize;
		myAudio.loop = aWeaponData.loopSound;
		myAudio.volume = (aWeaponData.loopSound == true ? 0.5f : 1.0f);
		myAudio.clip = aWeaponData.firingSound;
		myMeshFilter.mesh = aWeaponData.mesh;
		SetupProjectileEmitter();
		//myBaseRotation = new Vector3(0.0f, transform.localEulerAngles.y, 0.0f);
		myBaseRotation = Quaternion.Euler(0.0f, transform.localEulerAngles.y, 0.0f) * Vector3.forward;
	}

	private void SetupProjectileEmitter()
	{
		// Create
		myProjectileEmitter = Instantiate(myWeapon.projectileEmitter);
		myProjectileEmitter.transform.SetParent(transform);
		myProjectileEmitter.transform.localPosition = Vector3.zero;
		myProjectileEmitter.transform.localRotation = Quaternion.identity;
		myProjectileEmitter.transform.localScale = Vector3.one;

		// Fire rate
		ParticleSystem.EmissionModule emissionModule = myProjectileEmitter.emission;
		ParticleSystem.MinMaxCurve rateOverTime = emissionModule.rateOverTime;
		rateOverTime.constant = myWeapon.fireRate / 4.0f;
		emissionModule.rateOverTime = rateOverTime;

		// Speed
		ParticleSystem.MainModule mainModule = myProjectileEmitter.main;
		mainModule.startSpeed = myWeapon.projectileSpeed;
	}

	public void SetHoverType(int aLayer)
	{
		myMeshFilter.gameObject.layer = aLayer;
	}

	void Update()
	{
		//Debug.DrawRay(transform.position, Owner.transform.rotation * myBaseRotation, Color.white, 0.01f);
		if (myTarget != null)
		{
			Vector3 deltaToTargetShip = myTarget.transform.position - transform.position;
			float distanceToTargetShip2 = deltaToTargetShip.sqrMagnitude;
			float range = Owner.Data.baseData.firingRange * 2.0f;
			if (distanceToTargetShip2 < range * range)
			{
				if (Vector3.Angle(deltaToTargetShip.normalized, Owner.transform.rotation * myBaseRotation) < 45.0f)
				{
					StartFire();
				}
				else
				{
					StopFire();
				}
			}
			else
			{
				StopFire();
			}
		}
		else
		{
			StopFire();
		}

		//if (myIsFiring == false)
		//{
		//	int randomShipIndex = Random.Range(0, GameManager.Instance.Ships.Count);
		//	Ship randomShip = GameManager.Instance.Ships[randomShipIndex];
		//	if (randomShip.TeamIndex != Owner.TeamIndex)
		//	{
		//		Target = randomShip;
		//		Vector3 deltaToTargetShip = myTarget.transform.position - transform.position;
		//		float distanceToTargetShip2 = deltaToTargetShip.sqrMagnitude;
		//		float range = Owner.Data.baseData.firingRange * 2.0f;
		//		if (distanceToTargetShip2 < range * range)
		//		{
		//			if (Vector3.Angle(deltaToTargetShip.normalized, Owner.transform.rotation * myBaseRotation) < 45.0f)
		//			{
		//				StartFire();
		//			}
		//		}
		//	}
		//}

		if (myIsFiring == true || myRemainingShots > 0)
		{
			Fire();
		}

		if (myReload > 0.0f)
		{
			myReload -= Time.deltaTime;
		}
	}

	protected virtual void StartFire()
	{
		myIsFiring = true;
		if (myWeapon.shotsPerBurst > 1 && myRemainingShots <= 0)
		{
			myRemainingShots = myWeapon.shotsPerBurst;
		}
	}

	protected virtual void StopFire()
	{
		myIsFiring = false;
	}

	protected virtual void Fire()
	{
		if (myReload <= 0.0f && myTarget != null)
		{
			Vector3 deltaToTargetShip = myTarget.transform.position - transform.position;
			Vector3 targetPosition = myTarget.transform.position + myTarget.Velocity * (deltaToTargetShip.magnitude / myWeapon.projectileSpeed);
			Vector3 deltaToTarget = targetPosition - transform.position;

			float accuracy = Random.Range(0, 100.0f);
			bool hit = (accuracy < myWeapon.hitChance);
			if (hit == false)
			{
				deltaToTarget = Quaternion.Euler(0.0f, Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f)) * deltaToTarget;
			}
			transform.rotation = Quaternion.LookRotation(deltaToTarget.normalized, Vector3.up);

			float t = deltaToTarget.magnitude / myWeapon.projectileSpeed;

			if (Random.Range(0, 5) == 0)
			{
				ParticleSystem.MainModule mainModule = myProjectileEmitter.main;
				mainModule.startLifetime = t;
				myProjectileEmitter.Play();
				myAudio.Play();
			}
			
			if (hit == true)
			{
				StartCoroutine(DelayDamage(t, myTarget));
			}
			myReload += 1.0f / myWeapon.fireRate;
			if (myWeapon.shotsPerBurst > 1)
			{
				if (myRemainingShots > 1)
				{
					myReload = 1.0f / myWeapon.burstSpeed;
				}
			}
			myRemainingShots = Mathf.Max(myRemainingShots - 1, 0);
		}
	}

	protected IEnumerator DelayDamage(float aTime, Ship aTarget)
	{
		yield return new WaitForSeconds(aTime);
		if (aTarget != null)
		{
			aTarget.LastAttacker = Owner;
			aTarget.TakeDamage(myWeapon, mySize, transform.position);
		}
	}
}
