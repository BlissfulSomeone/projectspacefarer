using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PooledObject
{
	protected Rigidbody myRidigBody;
	protected TrailRenderer myTrail;

	[SerializeField]
	protected Impact myImpactPrefab;
	[SerializeField]
	protected float mySpeed;
	public float Speed { get { return mySpeed; } }

	protected float timer;

	void Awake()
	{
		myRidigBody = GetComponent<Rigidbody>();
		myTrail = GetComponent<TrailRenderer>();
	}

	public virtual void Init(Vector3 aDirection, float aLifeTime)
	{
		myRidigBody.velocity = aDirection.normalized * mySpeed;
		timer = aLifeTime;
		myTrail.Clear();
	}

	void Update()
	{
		UpdateProjectile();
	}

	protected virtual void UpdateProjectile()
	{
		timer -= Time.deltaTime;
		if (timer < 0.0f)
		{
			Impact impact = myImpactPrefab.GetPooledInstance<Impact>();
			impact.Init();
			impact.transform.position = transform.position - myRidigBody.velocity * Time.deltaTime;
			ReturnToPool();
		}
	}
}
