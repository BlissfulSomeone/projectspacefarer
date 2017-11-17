using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : PooledObject
{
	private AudioSource myAudio;

	private float myTimer = 0.0f;

	void Awake()
	{
		myAudio = GetComponent<AudioSource>();
	}

	public void Init(float aScale)
	{
		transform.localScale = Vector3.one * aScale;
		myAudio.maxDistance *= aScale;
		myAudio.Play();
		myTimer = 0.0f;
	}
	
	void Update()
	{
		myTimer += Time.deltaTime;
		if (myTimer > 5.0f)
		{
			ReturnToPool();
		}
	}
}
