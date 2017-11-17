using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustField : MonoBehaviour
{
	[SerializeField]
	private Mesh myStoneMesh;
	[SerializeField]
	private Material myStoneMaterial;

	private Matrix4x4[] myOrientations;
	private Vector3[] myMovementSpeed;
	private Vector3[] myRotationSpeed;

	[SerializeField]
	private int myNumberOfRocks;
	[SerializeField]
	private float myMaxDistance;
	[SerializeField]
	private float myMaxMovementSpeed;
	[SerializeField]
	private float myMaxRotationSpeed;
	[SerializeField]
	private float myStoneScales;

	private int myActiveNumberOfRocks;
	
	void Awake()
	{
		if (PlayerPreferences.EnvironmentQuality == PlayerPreferences.eEnvironmentQuality.MEDIUM)
		{
			myNumberOfRocks /= 2;
		}
		else if (PlayerPreferences.EnvironmentQuality == PlayerPreferences.eEnvironmentQuality.LOW)
		{
			myNumberOfRocks /= 3;
		}

		myOrientations = new Matrix4x4[myNumberOfRocks];
		myMovementSpeed = new Vector3[myNumberOfRocks];
		myRotationSpeed = new Vector3[myNumberOfRocks];
		for (int i = 0; i < myNumberOfRocks; ++i)
		{
			Vector3 position = new Vector3(Random.Range(-myMaxDistance, myMaxDistance), Random.Range(-myMaxDistance, myMaxDistance), Random.Range(-myMaxDistance, myMaxDistance));
			Quaternion rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
			Vector3 scale = new Vector3(Random.Range(0.05f, 0.2f), Random.Range(0.05f, 0.2f), Random.Range(0.05f, 0.2f));
			Matrix4x4 orientation = Matrix4x4.TRS(position, rotation, scale * myStoneScales);
			myOrientations[i] = orientation;
			
			myMovementSpeed[i] = new Vector3(Random.Range(-myMaxMovementSpeed, myMaxMovementSpeed), Random.Range(-myMaxMovementSpeed, myMaxMovementSpeed), Random.Range(-myMaxMovementSpeed, myMaxMovementSpeed));
			
			myRotationSpeed[i] = new Vector3(Random.Range(-myMaxRotationSpeed, myMaxRotationSpeed), Random.Range(-myMaxRotationSpeed, myMaxRotationSpeed), Random.Range(-myMaxRotationSpeed, myMaxRotationSpeed));
		}

		UpdateNumberOfRocks(PlayerPreferences.EnvironmentQuality);
	}

	void OnEnable()
	{
		PlayerPreferences.OnEnvironmentQualityChanged += UpdateNumberOfRocks;
	}

	void OnDisable()
	{
		PlayerPreferences.OnEnvironmentQualityChanged -= UpdateNumberOfRocks;
	}

	private void UpdateNumberOfRocks(PlayerPreferences.eEnvironmentQuality aQuality)
	{
		myActiveNumberOfRocks = myNumberOfRocks;
		if (aQuality == PlayerPreferences.eEnvironmentQuality.MEDIUM)
		{
			myActiveNumberOfRocks /= 2;
		}
		else if (aQuality == PlayerPreferences.eEnvironmentQuality.LOW)
		{
			myActiveNumberOfRocks /= 3;
		}
	}

	void Update()
	{
		Vector4 camera = Camera.main.transform.position;
		for (int i = 0; i < myActiveNumberOfRocks; ++i)
		{
			Vector4 position = myOrientations[i].GetColumn(3);
			position.x += myMovementSpeed[i].x * Time.deltaTime;
			position.y += myMovementSpeed[i].y * Time.deltaTime;
			position.z += myMovementSpeed[i].z * Time.deltaTime;
			Vector4 delta = position - camera;
			if (delta.x < -myMaxDistance) position.x += myMaxDistance * 2;
			if (delta.x > myMaxDistance) position.x -= myMaxDistance * 2;
			if (delta.y < -myMaxDistance) position.y += myMaxDistance * 2;
			if (delta.y > myMaxDistance) position.y -= myMaxDistance * 2;
			if (delta.z < -myMaxDistance) position.z += myMaxDistance * 2;
			if (delta.z > myMaxDistance) position.z -= myMaxDistance * 2;
			myOrientations[i].SetColumn(3, position);
			myOrientations[i] = myOrientations[i] * Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(myRotationSpeed[i] * Time.deltaTime), Vector3.one);
		}
		Graphics.DrawMeshInstanced(myStoneMesh, 0, myStoneMaterial, myOrientations, myActiveNumberOfRocks);
	}
}
