using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Sun : MonoBehaviour
{
	public float minTemperature;
	public float maxTemperature;
	public float temperature = 2400.0f;
	public float radius = 100.0f;
	public float coronaScale = 1.0f;

	private float oldTemperature;
	private float oldRadius;

	private MeshRenderer meshRenderer;
	private Material sunMaterial;
	private MeshRenderer flareRenderer;
	private Material sunFlareMaterial;

	void Awake()
	{
		//if (temperature == 0.0f)
		{
			float rand = Mathf.Pow(Random.Range(0.0f, 1.0f), 2.0f);
			temperature = minTemperature + Random.Range(0.0f, maxTemperature - minTemperature) * rand; 
		}

		meshRenderer = GetComponent<MeshRenderer>();
		sunMaterial = meshRenderer.sharedMaterial;
		flareRenderer = transform.FindChild("Corona").GetComponent<MeshRenderer>();
		sunFlareMaterial = flareRenderer.sharedMaterial;
	}

	void Update()
	{
		if (oldTemperature != temperature || oldRadius != radius)
		{
			Recalculate();
			oldTemperature = temperature;
			oldRadius = radius;
		}

		if (Application.isPlaying == true)
		{
			Camera cam = Camera.main;

			Vector3 cameraPosition = cam.transform.position;
			Vector3 delta = cameraPosition - transform.position;

			//flareRenderer.transform.rotation = Quaternion.LookRotation(delta.normalized) * Quaternion.Euler(90.0f, 0.0f, 0.0f);
			//flareRenderer.transform.localPosition = flareRenderer.transform.up * (radius / 2 + 1.0f);

			Vector3 faceForward = -cam.transform.forward;
			flareRenderer.transform.rotation = Quaternion.LookRotation(faceForward) * Quaternion.Euler(90.0f, 0.0f, 0.0f);
			flareRenderer.transform.localPosition = delta.normalized * (radius / 2 + 1.0f);
			
			float distance = delta.magnitude;
			const float DSUN = 1392684.0f;
			const float TSUN = 5778.0f;
			float diameter = radius * 2 * DSUN;
			float L = (diameter * diameter) * Mathf.Pow(temperature / TSUN, 4.0f);
			flareRenderer.transform.localScale = Vector3.one * (0.016f * Mathf.Pow(L, 0.25f)) * 0.1f * coronaScale;// / Mathf.Pow(distance, 0.5f));
		}
	}

	private void Recalculate()
	{
		sunMaterial.SetFloat("_Radius", radius);
		sunMaterial.SetFloat("_Temperature", temperature);
		sunFlareMaterial.SetFloat("_Temperature", temperature);
	}
}
