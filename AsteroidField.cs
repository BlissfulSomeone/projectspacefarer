using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidField : MonoBehaviour
{
	[SerializeField]
	private Transform myOrigin;
	[SerializeField]
	private float myRadius;
	[SerializeField]
	private float myThickness;
	[SerializeField]
	private float myHeight;
	[SerializeField]
	private int myNumberOfAsteroids;
	[SerializeField]
	private Transform[] myAsteroidPrefabs;
	//[SerializeField]
	//private float myMinScale;
	//[SerializeField]
	//private float myMaxScale;
	[SerializeField]
	private AnimationCurve myScaleDistribution;
	[SerializeField]
	private float myRotationSpeed;

	private int myActiveNumberOfAsteroids;

	Transform[] myAsteroids;
	MeshRenderer[] myAsteroidRenderers;
	Vector3[] myRotationSpeeds;

	void Awake()
	{
		if (myOrigin != null)
		{
			transform.position = myOrigin.position;
		}
		
		myAsteroids = new Transform[myNumberOfAsteroids];
		myAsteroidRenderers = new MeshRenderer[myNumberOfAsteroids];
		myRotationSpeeds = new Vector3[myNumberOfAsteroids];

		for (int i = 0; i < myNumberOfAsteroids; ++i)
		{
			int randomIndex = Random.Range(0, myAsteroidPrefabs.Length);
			Transform asteroid = Instantiate(myAsteroidPrefabs[randomIndex]);
			asteroid.SetParent(transform);

			float angle = Random.Range(0.0f, 360.0f) * Mathf.Deg2Rad;
			float distance = Random.Range(-myThickness, myThickness);
			float altitude = Random.Range(-myHeight, myHeight) * (1.0f - Mathf.Abs((distance / myThickness)));
			Vector3 spawnPosition = new Vector3(Mathf.Cos(angle) * (myRadius + distance), altitude, Mathf.Sin(angle) * (myRadius + distance));

			asteroid.localPosition = spawnPosition;
			asteroid.eulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
			asteroid.localScale = new Vector3(Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f)) * myScaleDistribution.Evaluate(Random.Range(0.0f, 1.0f)) * 100.0f;

			myAsteroids[i] = asteroid;
			myAsteroidRenderers[i] = asteroid.GetComponentInChildren<MeshRenderer>();

			myRotationSpeeds[i] = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * 10.0f;
		}

		UpdateAsteroidNumber(PlayerPreferences.EnvironmentQuality);
	}

	void OnEnable()
	{
		PlayerPreferences.OnEnvironmentQualityChanged += UpdateAsteroidNumber;
	}

	void OnDisable()
	{
		PlayerPreferences.OnEnvironmentQualityChanged -= UpdateAsteroidNumber;
	}

	private void UpdateAsteroidNumber(PlayerPreferences.eEnvironmentQuality aQuality)
	{
		myActiveNumberOfAsteroids = myNumberOfAsteroids;
		if (aQuality == PlayerPreferences.eEnvironmentQuality.MEDIUM)
		{
			myActiveNumberOfAsteroids /= 2;
		}
		else if (aQuality == PlayerPreferences.eEnvironmentQuality.LOW)
		{
			myActiveNumberOfAsteroids /= 3;
		}
		for (int i = 0; i < myNumberOfAsteroids; ++i)
		{
			myAsteroids[i].gameObject.SetActive(i < myActiveNumberOfAsteroids);
		}
	}

	void Update()
	{
		transform.Rotate(new Vector3(0.0f, myRotationSpeed, 0.0f) * Time.deltaTime);
		for (int i = 0; i < myActiveNumberOfAsteroids; ++i)
		{
			if (myAsteroids[i].position.x > 0)
			{
				myAsteroidRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			}
			else
			{
				myAsteroidRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			}
			//myAsteroids[i].Rotate(myRotationSpeeds[i] * Time.deltaTime);
		}
	}
}
