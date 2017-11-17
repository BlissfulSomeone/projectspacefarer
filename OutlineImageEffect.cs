using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineImageEffect : MonoBehaviour
{
	public Camera myAttachedCamera;
	private Camera myTempCamera;
	private Material myMaterial;

	[SerializeField]
	private LayerMask myCullingMask;
	[SerializeField]
	private Shader myPreOutlineShader;
	[SerializeField]
	private Shader myOutlineShader;

	private RenderTexture tempRT;
	private Vector2 previousScreenSize = Vector2.zero;

	void Start()
	{
		myAttachedCamera = GetComponent<Camera>();
		myTempCamera = new GameObject().AddComponent<Camera>();
		myTempCamera.enabled = false;
		myMaterial = new Material(myOutlineShader);

		previousScreenSize.x = Screen.width;
		previousScreenSize.y = Screen.height;
		tempRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
		tempRT.Create();
	}

	void Update()
	{
		if (Screen.width != previousScreenSize.x || Screen.height != previousScreenSize.y)
		{
			tempRT.Release();
			tempRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
			tempRT.Create();
		}
		previousScreenSize.x = Screen.width;
		previousScreenSize.y = Screen.height;
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		myTempCamera.CopyFrom(myAttachedCamera);
		myTempCamera.clearFlags = CameraClearFlags.Color;
		myTempCamera.backgroundColor = Color.black;

		myTempCamera.cullingMask = myCullingMask;

		
		myTempCamera.targetTexture = tempRT;
		myTempCamera.RenderWithShader(myPreOutlineShader, "");
		myMaterial.SetTexture("_SceneTexture", source);
		Graphics.Blit(tempRT, destination, myMaterial);
		//tempRT.Release();
	}
}
