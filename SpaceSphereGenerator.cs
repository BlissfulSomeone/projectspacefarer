using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSphereGenerator : MonoBehaviour 
{
	public Material spaceSphereMaterial;
	public Transform spaceSpherePrefab;
	public bool quickApproximation = true;
	public Texture2D[] lookupTextures;
	
	public Vector3 skySeed;
	public Color skyColor;
	public Color nebulaColor;
	public float nebulaLookupMultiplier;
	public float nebulaBrightness;
	public float nebulaFrequency;

	void Start()
	{
		int randomLookupTextureIndex = Random.Range(0, lookupTextures.Length);
		Texture2D lookupTexture = lookupTextures[randomLookupTextureIndex];
		//skySeed = new Vector3(Random.Range(-256, 256), Random.Range(-256, 256), Random.Range(-256, 256));
		//Debug.Log(skySeed);
		//skyColor = Random.ColorHSV(0.0f, 1.0f, 0.6f, 0.8f, 0.0f, 0.2f);
		//nebulaColor = Random.ColorHSV(0.0f, 1.0f, 0.3f, 0.5f, 0.0f, 0.5f);
		//nebulaLookupMultiplier = Mathf.Pow(Random.Range(0.5f, 1.0f), 2.0f) * Random.Range(0.8f, 6.0f);

		// TEMP
		//skyColor = new Color(0.2f, 0.075f, 0.05f);
		//nebulaColor = new Color(0.3f, 0.4f, 0.25f);
		//nebulaLookupMultiplier = 6.0f;
		// /TEMP

		spaceSphereMaterial.SetTexture("_MainTex", lookupTexture);
		spaceSphereMaterial.SetVector("_Seed", skySeed);
		spaceSphereMaterial.SetColor("_SkyColor", skyColor);
		spaceSphereMaterial.SetColor("_NebulaColor", nebulaColor);
		spaceSphereMaterial.SetFloat("_NebulaLookupMultiplier", nebulaLookupMultiplier);
		spaceSphereMaterial.SetFloat("_NebulaBrightness", nebulaBrightness);
		spaceSphereMaterial.SetFloat("_NebulaFrequency", nebulaFrequency);

		if (quickApproximation == true)
		{
			spaceSphereMaterial.SetInt("_Steps", 5);
			spaceSphereMaterial.SetFloat("_SampleDistance", 0.02f);
		}
		else
		{
			spaceSphereMaterial.SetInt("_Steps", 80);
			spaceSphereMaterial.SetFloat("_SampleDistance", 0.02f);
		}

		Transform spaceSphere = Instantiate(spaceSpherePrefab);
		MeshRenderer meshRenderer = spaceSphere.GetComponent<MeshRenderer>();
		meshRenderer.material = spaceSphereMaterial;

		Cubemap cubemap = new Cubemap(2048, TextureFormat.RGB24, false);

		Camera camera = spaceSphere.GetComponentInChildren<Camera>();

		StartCoroutine(Coroutine_SnapSpace(camera, cubemap, spaceSphere));
	}

	IEnumerator Coroutine_SnapSpace(Camera aCamera, Cubemap aCubemap, Transform aSpaceSphere)
	{
		yield return new WaitForEndOfFrame();

		aCamera.RenderToCubemap(aCubemap);

		Material mat = new Material(Shader.Find("Skybox/Cubemap"));
		mat.SetTexture("_Tex", aCubemap);
		RenderSettings.skybox = mat;
		RenderSettings.customReflection = aCubemap;

		DestroyImmediate(aSpaceSphere.gameObject);
		DestroyImmediate(gameObject);
	}
}
