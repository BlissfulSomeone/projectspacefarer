using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPointDefense : Turret
{
	void Update()
	{
		Debug.DrawRay(transform.position, transform.rotation * myBaseRotation, Color.white, 0.01f);
		if (myHomingTarget != null)
		{
			Vector3 deltaToTargetProjectile = myHomingTarget.transform.position - transform.position;
			float distanceToTargetProjectile2 = deltaToTargetProjectile.sqrMagnitude;
			float range = Owner.Data.baseData.firingRange * 2.0f;
			if (distanceToTargetProjectile2 < range * range)
			{
				if (Vector3.Angle(deltaToTargetProjectile.normalized, transform.rotation * myBaseRotation) < 45.0f)
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

		if (myIsFiring == true)
		{
			Fire();
		}

		if (myReload > 0.0f)
		{
			myReload -= Time.deltaTime;
		}
	}

	protected override void Fire()
	{
		if (myReload <= 0.0f && myTarget != null)
		{
			Vector3 deltaToTargetShip = myHomingTarget.transform.position - transform.position;
			Vector3 targetPosition = myHomingTarget.transform.position + myHomingTarget.RigidBody.velocity * (deltaToTargetShip.magnitude / myWeapon.projectileSpeed);
			Vector3 deltaToTarget = targetPosition - transform.position;

			transform.rotation = Quaternion.LookRotation(deltaToTarget.normalized, Vector3.up);

			if (Random.Range(0, 5) == 0)
			{
				ParticleSystem.MainModule mainModule = myProjectileEmitter.main;
				mainModule.startLifetime = deltaToTarget.magnitude / myWeapon.projectileSpeed;
				myProjectileEmitter.Play();
				myAudio.Play();
			}

			myHomingTarget.TakeDamage(myWeapon, mySize);
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
