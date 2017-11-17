using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformToolAxis : MonoBehaviour
{
	[HideInInspector]
	public int axisIndex;
	[HideInInspector]
	public TransformTool.eTransformMode mode;

	private MeshRenderer myMeshRenderer;
	private Material myDefaultMaterial;

	private MeshRenderer MeshRenderer
	{ 
		get
		{
			if (myMeshRenderer == null)
			{
				myMeshRenderer = GetComponent<MeshRenderer>();
			}
			return myMeshRenderer;
		}
	}

	private Material DefaultMaterial
	{
		get
		{
			if (myDefaultMaterial == null)
			{
				myDefaultMaterial = MeshRenderer.material;
			}
			return myDefaultMaterial;
		}
	}
	
	public void ResetMaterial()
	{
		MeshRenderer.material = DefaultMaterial;
	}

	public void SetMaterial(Material aMaterial)
	{
		MeshRenderer.material = aMaterial;
	}
}
