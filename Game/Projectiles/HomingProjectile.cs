using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : Projectile
{
	[SerializeField]
	private Impact myShotDownImpactPrefab;

	private WeaponData myWeaponData;
	private int mySizeIndex;
	private Ship myTarget;
	private int myTeamIndex;
	public int TeamIndex { get { return myTeamIndex; } }

	public Rigidbody RigidBody { get { return myRidigBody; } }

	public static List<HomingProjectile> HomingProjectiles = new List<HomingProjectile>();

	private float myHull = 500.0f;

	void OnEnable()
	{
		HomingProjectiles.Add(this);
	}

	void OnDisable()
	{
		HomingProjectiles.Remove(this);
	}
	
	public void SetData(WeaponData aWeaponData, int aSizeIndex, Ship aTarget, int aTeamIndex)
	{
		myWeaponData = aWeaponData;
		mySizeIndex = aSizeIndex;
		myTarget = aTarget;
		myTeamIndex = aTeamIndex;
		myHull = 500.0f;
	}

	protected override void UpdateProjectile()
	{
		if (myTarget != null)
		{
			Vector3 deltaToTarget = myTarget.transform.position - transform.position;
			Vector3 acceleration = Vector3.RotateTowards(myRidigBody.velocity, deltaToTarget, 40.0f * Time.deltaTime, 0.0f);
			myRidigBody.velocity = acceleration.normalized * mySpeed;

			if (deltaToTarget.magnitude < (mySpeed * 2.0f) * Time.deltaTime)
			{
				myTarget.TakeDamage(myWeaponData, mySizeIndex, transform.position);
				Kill(true);
			}
		}
		else
		{
			Kill(true);
		}
	}

	private void Kill(bool aNormalImpact)
	{
		Impact impact = null;
		if (aNormalImpact == true)
		{
			impact = myImpactPrefab.GetPooledInstance<Impact>();
		}
		else
		{
			impact = myShotDownImpactPrefab.GetPooledInstance<Impact>();
		}
		impact.Init();
		impact.transform.position = transform.position;
		ReturnToPool();
	}

	public void TakeDamage(WeaponData aWeaponData, int aSizeIndex)
	{
		float damage = 0.0f;
		if (aSizeIndex == 0) { damage = aWeaponData.damage.small; }
		else if (aSizeIndex == 1) { damage = aWeaponData.damage.medium; }
		else if (aSizeIndex == 2) { damage = aWeaponData.damage.large; }
		myHull -= damage;
		if (myHull <= 0.0f)
		{
			Kill(false);
		}
	}
}
