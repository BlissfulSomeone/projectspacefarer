using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : Projectile
{
	private Vector3 myTargetPosition;
	private bool myShouldMove;

	[SerializeField]
	private LayerMask myShipLayerMask;

	public override void Init(Vector3 aDirection, float aLifeTime)
	{
		myTargetPosition = transform.position + aDirection;
		myShouldMove = true;
		timer = 0.1f;
		myTrail.Clear();
	}

	protected override void UpdateProjectile()
	{
		if (myShouldMove == true)
		{
			Ray ray = new Ray(transform.position, myTargetPosition - transform.position);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 512.0f, myShipLayerMask) == true)
			{
				transform.position = hitInfo.point;
				Impact impact = myImpactPrefab.GetPooledInstance<Impact>();
				impact.Init();
				impact.transform.position = transform.position;
			}
			myShouldMove = false;
		}
		timer -= Time.deltaTime;
		if (timer < 0.0f)
		{
			ReturnToPool();
		}
	}
}
