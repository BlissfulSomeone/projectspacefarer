using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
	[SerializeField]
	private AnimationCurve myFalloffCurve;
	[SerializeField]
	private float myRadius;

	private Texture2D myFalloffTexture;
	private const int TEXTURE_RESOLUTION = 64;

	private Vector4[] myImpactPoints = new Vector4[10];
	private int myNextPoint = 0;

	private Material myMaterialInstance;
	
	void Awake()
	{
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "SceneShieldTest")
		{
			Init(myRadius);
		}
	}

	public void Init(float aRadius)
	{
		myRadius = aRadius;

		myFalloffTexture = new Texture2D(TEXTURE_RESOLUTION, 1, TextureFormat.RGB24, false);
		for (int i = 0; i < TEXTURE_RESOLUTION; ++i)
		{
			float value = myFalloffCurve.Evaluate(i / ((float)TEXTURE_RESOLUTION));
			myFalloffTexture.SetPixel(i, 0, new Color(value, value, value));
		}
		myFalloffTexture.wrapMode = TextureWrapMode.Clamp;
		myFalloffTexture.Apply();

		myMaterialInstance = GetComponent<MeshRenderer>().material;
		myMaterialInstance.SetTexture("_FalloffTex", myFalloffTexture);
		myMaterialInstance.SetFloat("_Radius", myRadius);
		transform.localScale = Vector3.one * myRadius;
	}

	void Update()
	{
		for (int i = 0; i < 10; ++i)
		{
			myImpactPoints[i].w += Time.deltaTime * 10.0f;
		}
		myMaterialInstance.SetVectorArray("_ImpactPoints", myImpactPoints);
	}

	public void Trigger(Vector3 aPoint)
	{
		Vector3 p = aPoint.normalized * myRadius * 0.5f;
		//Debug.Log(p);
		myImpactPoints[myNextPoint] = p;
		myImpactPoints[myNextPoint].w = 0.0f;
		myNextPoint = (myNextPoint + 1) % 10;
	}
}
