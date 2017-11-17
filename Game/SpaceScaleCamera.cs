using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceScaleCamera : MonoBehaviour
{
	[SerializeField]
	private Camera myTargetCamera;
	[SerializeField]
	private float myScaleFactor;

	void Update()
	{
		transform.rotation = myTargetCamera.transform.rotation;
		transform.position = myTargetCamera.transform.position * (1.0f / myScaleFactor);
	}
}
