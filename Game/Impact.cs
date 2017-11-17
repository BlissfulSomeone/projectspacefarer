using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : PooledObject
{
	private float timer = 0.0f;

	public void Init()
	{
		timer = 0.0f;
	}

	void Update()
	{
		timer += Time.deltaTime;
		if (timer > 2.0f)
		{
			ReturnToPool();
		}
	}
}
