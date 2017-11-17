using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBeam : Turret
{
	[SerializeField]
	private LineRenderer myBeamRenderer;

	protected override void StartFire()
	{
		if (myIsFiring == false)
		{
			base.StartFire();
			myBeamRenderer.gameObject.SetActive(true);
			myAudio.Play();
		}
	}

	protected override void StopFire()
	{
		if (myIsFiring == true)
		{
			base.StopFire();
			myBeamRenderer.gameObject.SetActive(false);
			myAudio.Stop();
		}
	}

	protected override void Fire()
	{
		Keyframe[] widthKeys = myBeamRenderer.widthCurve.keys;
		widthKeys[0].time = 0.0f;
		widthKeys[0].value = 0.2f;
		widthKeys[4].time = 1.0f;
		widthKeys[4].value = 0.2f;
		float time = Time.timeSinceLevelLoad - Mathf.Floor(Time.timeSinceLevelLoad);
		widthKeys[1].time = Mathf.Max(time - 0.05f, 0.0f);
		widthKeys[1].value = 0.2f;
		widthKeys[2].time = time;
		widthKeys[2].value = 1.0f;
		widthKeys[3].time = Mathf.Min(time + 0.05f, 1.0f);
		widthKeys[3].value = 0.2f;
		AnimationCurve widthCurve = new AnimationCurve(widthKeys);
		myBeamRenderer.widthCurve = widthCurve;
		
		Vector3 deltaToTargetShip = myTarget.transform.position - transform.position;
		transform.rotation = Quaternion.LookRotation(deltaToTargetShip.normalized, Vector3.up);
		myTarget.TakeDamage(myWeapon, mySize, Vector3.zero);

		Vector3 dir = deltaToTargetShip.normalized;
		float step = deltaToTargetShip.magnitude / 99.0f;
		for (int i = 0; i < 100; ++i)
		{
			myBeamRenderer.SetPosition(i, transform.position + dir * step * i);
		}
	}
}
