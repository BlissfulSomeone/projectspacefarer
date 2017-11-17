using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraBobbing : MonoBehaviour
{
	[SerializeField]
	private float myHorizontalIntensity;
	[SerializeField]
	private float myHorizontalSpeed;
	[SerializeField]
	private float myVerticalIntensity;
	[SerializeField]
	private float myVerticalSpeed;

	private Vector3 myOriginalRotation;
	private float myRandomTimeOffset;
	
	void Awake()
	{
		myOriginalRotation = transform.eulerAngles;
		myRandomTimeOffset = Random.Range(0.0f, 1000.0f);
	}

	void Update()
	{
		Vector3 rotation = myOriginalRotation;
		rotation.x += Mathf.Cos(myHorizontalSpeed * Time.timeSinceLevelLoad + myRandomTimeOffset) * myHorizontalIntensity;
		rotation.y += Mathf.Sin(myVerticalSpeed * Time.timeSinceLevelLoad + myRandomTimeOffset) * myVerticalIntensity;
		transform.eulerAngles = rotation;
	}
}
