using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMissile : Turret
{
	[SerializeField]
	private HomingProjectile myProjectilePrefab;

	protected override void Fire()
	{
		if (myReload <= 0.0f && myTarget != null)
		{
			Vector3 deltaToTargetShip = myTarget.transform.position - transform.position;
			transform.rotation = Quaternion.LookRotation(deltaToTargetShip.normalized, Vector3.up);

			HomingProjectile homingProjectile = myProjectilePrefab.GetPooledInstance<HomingProjectile>();
			homingProjectile.transform.position = transform.position;
			homingProjectile.Init(deltaToTargetShip, 1.0f);
			homingProjectile.SetData(myWeapon, mySize, myTarget, Owner.TeamIndex);

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
}
