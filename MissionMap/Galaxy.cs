using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Galaxy : MonoBehaviour
{
	[System.Serializable]
	public struct GalaxySettings
	{
		public float semiMajorAxis;
		[Range(0.0f, 0.18f)]
		public float eccentricity;
		public float bulge;
		public float tiltY;
		public float coreSpeed;
		public float rimSpeed;
		public float waveFrequency;
		public float waveAmplitude;
	}

	[SerializeField]
	private GalaxySettings mySettings;
	[SerializeField]
	private int myNumberOfStars;
	[SerializeField]
	private Star myStarPrefab;

	private List<Star> myStars;

	void Awake()
	{
		myStars = new List<Star>();
		for (int i = 0; i < myNumberOfStars; ++i)
		{
			Star star = Instantiate(myStarPrefab);
			star.transform.SetParent(transform);
			star.transform.localScale = Vector3.one * Random.Range(1.0f, 2.0f);
			star.distance = Random.Range(0.0f, 1.0f);
			star.angle = Random.Range(0.0f, Mathf.PI * 2.0f);
			myStars.Add(star);
		}
	}

	void Update()
	{
		float step = 360.0f / myNumberOfStars;
		for (int i = 0; i < myNumberOfStars; ++i)
		{
			myStars[i].angle += Mathf.Lerp(mySettings.coreSpeed, mySettings.rimSpeed, myStars[i].distance) * Time.deltaTime;

			float radiX = myStars[i].distance;
			float radiY = myStars[i].distance * (1.0f - mySettings.eccentricity);

			float angle = step * i * Mathf.Deg2Rad + myStars[i].angle;

			float x = Mathf.Cos(angle) * radiX * mySettings.semiMajorAxis;
			float y = Mathf.Sin(angle) * radiY * mySettings.semiMajorAxis;

			float dist = Mathf.Sqrt(x * x + y * y);
			float a = Mathf.Atan2(y, x);
			a += mySettings.tiltY * myStars[i].distance * Mathf.Deg2Rad;
			
			x = Mathf.Cos(a) * dist;
			y = Mathf.Sin(a) * dist;

			Vector3 position = myStars[i].transform.position;
			position.x = x;
			position.y = ((Mathf.PerlinNoise(x * mySettings.waveFrequency, y * mySettings.waveFrequency) * 2.0f) - 1.0f) * mySettings.waveAmplitude;
			position.z = y;
			myStars[i].transform.position = position;

		}
	}
}
