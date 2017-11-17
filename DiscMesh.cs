using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscMesh : MonoBehaviour
{
	[SerializeField]
	private float myAngles = 90.0f;
	[SerializeField]
	[Range(3, 64)]
	private int myNumberOfSides = 4;
	[SerializeField]
	private float myRadius = 1.0f;

	private float myLastAngles = float.MinValue;
	private int myLastNumberOfSides = int.MinValue;
	private float myLastRadius = float.MinValue;

	private Mesh myMesh;
	private MeshFilter myMeshFilter;

	void Awake()
	{
		myMeshFilter = GetComponent<MeshFilter>();
		myMeshFilter.mesh = myMesh = new Mesh();
	}

	void Update()
	{
		if (myAngles != myLastAngles || myNumberOfSides != myLastNumberOfSides || myRadius != myLastRadius)
		{
			UpdateMesh(); 
		}
		myLastAngles = myAngles;
		myLastNumberOfSides = myNumberOfSides;
		myLastRadius = myRadius;
	}

	private void UpdateMesh()
	{
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();

		float step = (myAngles * Mathf.Deg2Rad) / (myNumberOfSides);
		for (int i = 0; i < myNumberOfSides; ++i)
		{
			vertices.Add(Vector3.zero);
			vertices.Add(new Vector3(
				Mathf.Cos(step * i),
				0.0f,
				Mathf.Sin(step * i)) * myRadius);
			vertices.Add(new Vector3(
				Mathf.Cos(step * (i + 1)),
				0.0f,
				Mathf.Sin(step * (i + 1))) * myRadius);

			triangles.Add(i * 3 + 0);
			triangles.Add(i * 3 + 2);
			triangles.Add(i * 3 + 1);
		}

		myMesh.vertices = vertices.ToArray();
		myMesh.triangles = triangles.ToArray();
		myMesh.RecalculateBounds();
	}
}
