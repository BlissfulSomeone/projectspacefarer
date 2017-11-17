using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class ShipLoader : MonoBehaviour
{
	public class ShipLoaderSerializerData
	{
		public class Piece
		{
			public string moduleName;
			public Vector3 position;
			public Vector3 euler;
			public Vector3 scale;
			public Piece(string aModuleName, Vector3 aPosition, Vector3 aEuler, Vector3 aScale) { moduleName = aModuleName; position = aPosition; euler = aEuler;  scale = aScale; }
		}

		public class Hardpoint
		{ 
			public Vector3 position;
			public float angle;
			public WeaponData weapon;
			public Hardpoint(Vector3 aPosition, float aAngle, WeaponData aWeaponData) { position = aPosition; angle = aAngle; weapon = aWeaponData; }
		}

		public class Accessory
		{
			public string propName;
			public Vector3 position;
			public float scale;
			public Accessory(string aPropName, Vector3 aPosition, float aScale) { propName = aPropName; position = aPosition; scale = aScale; }
		}

		public List<Piece> pieces;
		public List<Hardpoint> smallHardpoints;
		public List<Hardpoint> mediumHardpoints;
		public List<Hardpoint> largeHardpoints;
		public List<SoftpointData> softpoints;
		public List<Accessory> accessories;
		public ShipBaseData baseData;

		public ShipLoaderSerializerData()
		{
			pieces = new List<Piece>();
			smallHardpoints = new List<Hardpoint>();
			mediumHardpoints = new List<Hardpoint>();
			largeHardpoints = new List<Hardpoint>();
			softpoints = new List<SoftpointData>();
			accessories = new List<Accessory>();
		}
	}

	public class ShipLoaderData
	{
		public Mesh mesh;
		public List<ShipLoaderSerializerData.Hardpoint> smallHardpoints;
		public List<ShipLoaderSerializerData.Hardpoint> mediumHardpoints;
		public List<ShipLoaderSerializerData.Hardpoint> largeHardpoints;
		public List<SoftpointData> softpoints;
		public List<ShipLoaderSerializerData.Accessory> accessories;
		public ShipBaseData baseData;
		public float size;
		public int cost;
	}

	private static ShipLoader myInstance = null;
	public static ShipLoader Instance { get { return myInstance; } }

	[SerializeField]
	private Transform myPiecePrefab;

	private Dictionary<string, ShipLoaderData> myLoadedData;
	public Dictionary<string, ShipLoaderData> LoadedData { get { return myLoadedData; } }

	void Awake()
	{
		if (myInstance != null)
		{
			Destroy(gameObject);
			return;
		}
		myInstance = this;

		myLoadedData = new Dictionary<string, ShipLoaderData>();

		string currentDirectory = Environment.CurrentDirectory;
		DirectoryInfo directory = new DirectoryInfo(currentDirectory + "\\Data\\Ships");
		FileInfo[] files = directory.GetFiles();
		for (int i = 0; i < files.Length; ++i)
		{
			FileInfo file = files[i];
			if (file.Extension == ".xml")
			{
				string[] fileName = file.Name.Split('.');
				LoadData(fileName[0]);
			}
		}
	}

	private void LoadData(string aFileName)
	{
		ShipLoaderData data = new ShipLoaderData();
		GameObject shipObject = new GameObject("Temporary Ship");
		MeshFilter meshFilter = shipObject.AddComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh = new Mesh();
		mesh.name = "Ship " + aFileName;

		ShipLoaderSerializerData serializedData = LoadShipData(aFileName);
		for (int i = 0; i < serializedData.pieces.Count; ++i)
		{
			Transform piece = Instantiate(ContentLoader.Instance.GetModuleData(serializedData.pieces[i].moduleName).modulePrefab);
			piece.transform.SetParent(shipObject.transform);
			piece.transform.localPosition = serializedData.pieces[i].position;
			piece.transform.localEulerAngles = serializedData.pieces[i].euler;
			piece.transform.localScale = serializedData.pieces[i].scale;
		}

		shipObject.transform.eulerAngles = new Vector3(0.0f, -90.0f, 0.0f);

		CombineInstance[] combine = new CombineInstance[serializedData.pieces.Count];
		MeshFilter[] childMeshes = GetShipMeshes(shipObject);
		for (int i = 0; i < childMeshes.Length; ++i)
		{
			combine[i].mesh = childMeshes[i].mesh;
			combine[i].transform = childMeshes[i].transform.localToWorldMatrix;
			Destroy(childMeshes[i].transform.gameObject);
		}
		mesh.CombineMeshes(combine);

		data.cost = serializedData.baseData.cost;

		Vector3 centerOffset;
		float size;
		ComputeMesh(mesh, out centerOffset, out size);
		mesh.RecalculateBounds();
		for (int i = 0; i < serializedData.smallHardpoints.Count; ++i)
		{
			serializedData.smallHardpoints[i].position -= centerOffset;
			data.cost += serializedData.smallHardpoints[i].weapon.cost.small;
		}
		for (int i = 0; i < serializedData.mediumHardpoints.Count; ++i)
		{
			serializedData.mediumHardpoints[i].position -= centerOffset;
			data.cost += serializedData.mediumHardpoints[i].weapon.cost.medium;
		}
		for (int i = 0; i < serializedData.largeHardpoints.Count; ++i)
		{
			serializedData.largeHardpoints[i].position -= centerOffset;
			data.cost += serializedData.largeHardpoints[i].weapon.cost.large;
		}

		for (int i = 0; i < serializedData.accessories.Count; ++i)
		{
			serializedData.accessories[i].position -= centerOffset;
		}

		for (int i = 0; i < serializedData.softpoints.Count; ++i)
		{
			data.cost += serializedData.softpoints[i].cost;
		}

		data.mesh = mesh;
		data.smallHardpoints = serializedData.smallHardpoints;
		data.mediumHardpoints = serializedData.mediumHardpoints;
		data.largeHardpoints = serializedData.largeHardpoints;
		data.softpoints = serializedData.softpoints;
		data.accessories = serializedData.accessories;
		data.size = size;
		data.baseData = serializedData.baseData;

		myLoadedData.Add(aFileName, data);

		Destroy(shipObject);
	}

	public ShipLoaderSerializerData LoadShipData(string aFileName)
	{
		ShipLoaderSerializerData data = new ShipLoaderSerializerData();

		XmlDocument document = new XmlDocument();
		document.Load(Environment.CurrentDirectory + "\\Data\\Ships\\" + aFileName + ".xml");
		XmlNode modelNode = document.DocumentElement.SelectSingleNode("model");
		{
			int modelIndex = 0;
			modelIndex = int.Parse(modelNode.Attributes["hull"].Value);
			string hullName = "Patrol Boat";
			if (modelIndex == 1) { hullName = "Gun Boat"; }
			else if (modelIndex == 2) { hullName = "Forward Offence Ship"; }
			else if (modelIndex == 3) { hullName = "Mainline Ship"; }
			else if (modelIndex == 4) { hullName = "Armoured Cruiser"; }
			else if (modelIndex == 5) { hullName = "Battle Cruiser"; }
			ShipBaseData baseData = ContentLoader.Instance.GetShipData(hullName);
			data.baseData = baseData;
		}

		XmlNodeList blockNodes = document.DocumentElement.SelectNodes("blocks//block");
		foreach (XmlNode blockNode in blockNodes)
		{
			string moduleName = "Cube";
			Vector3 position = Vector3.zero;
			Vector3 euler = Vector3.zero;
			Vector3 scale = Vector3.zero;
			if (blockNode.Attributes["m"] != null) moduleName = blockNode.Attributes["m"].Value;
			position.x = float.Parse(blockNode.Attributes["px"].Value);
			position.y = float.Parse(blockNode.Attributes["py"].Value);
			position.z = float.Parse(blockNode.Attributes["pz"].Value);
			if (blockNode.Attributes["rx"] != null) euler.x = float.Parse(blockNode.Attributes["rx"].Value);
			if (blockNode.Attributes["ry"] != null) euler.y = float.Parse(blockNode.Attributes["ry"].Value);
			if (blockNode.Attributes["rz"] != null) euler.z = float.Parse(blockNode.Attributes["rz"].Value);
			scale.x = float.Parse(blockNode.Attributes["sx"].Value);
			scale.y = float.Parse(blockNode.Attributes["sy"].Value);
			scale.z = float.Parse(blockNode.Attributes["sz"].Value);

			data.pieces.Add(new ShipLoaderSerializerData.Piece(moduleName, position, euler, scale));
		}

		XmlNodeList softpointNodes = document.DocumentElement.SelectNodes("softpoints//softpoint");
		foreach (XmlNode softpointNode in softpointNodes)
		{
			string softpointName = string.Empty;
			softpointName = softpointNode.Attributes["id"].Value;
			SoftpointData softpointData = ContentLoader.Instance.GetSoftpointData(softpointName);
			if (softpointData != null)
			{
				data.softpoints.Add(softpointData);
			}
		}

		XmlNodeList smallHardpointNodes = document.DocumentElement.SelectNodes("smallHardpoints//smallHardpoint");
		foreach (XmlNode smallHardpointNode in smallHardpointNodes)
		{
			Vector3 position = Vector3.zero;
			float angle = 0.0f;
			string weaponName = string.Empty;
			position.x = -float.Parse(smallHardpointNode.Attributes["z"].Value);
			position.y = float.Parse(smallHardpointNode.Attributes["y"].Value);
			position.z = float.Parse(smallHardpointNode.Attributes["x"].Value);
			angle = float.Parse(smallHardpointNode.Attributes["r"].Value) - 90.0f;
			weaponName = smallHardpointNode.Attributes["id"].Value;
			WeaponData weapon = ContentLoader.Instance.GetWeaponData(weaponName);

			data.smallHardpoints.Add(new ShipLoaderSerializerData.Hardpoint(position, angle, weapon));
		}

		XmlNodeList mediumHardpointNodes = document.DocumentElement.SelectNodes("mediumHardpoints//mediumHardpoint");
		foreach (XmlNode mediumHardpointNode in mediumHardpointNodes)
		{
			Vector3 position = Vector3.zero;
			float angle = 0.0f;
			string weaponName = string.Empty;
			position.x = -float.Parse(mediumHardpointNode.Attributes["z"].Value);
			position.y = float.Parse(mediumHardpointNode.Attributes["y"].Value);
			position.z = float.Parse(mediumHardpointNode.Attributes["x"].Value);
			angle = float.Parse(mediumHardpointNode.Attributes["r"].Value) - 90.0f;
			weaponName = mediumHardpointNode.Attributes["id"].Value;
			WeaponData weapon = ContentLoader.Instance.GetWeaponData(weaponName);

			data.mediumHardpoints.Add(new ShipLoaderSerializerData.Hardpoint(position, angle, weapon));
		}

		XmlNodeList largeHardpointNodes = document.DocumentElement.SelectNodes("largeHardpoints//largeHardpoint");
		foreach (XmlNode largeHardpointNode in largeHardpointNodes)
		{
			Vector3 position = Vector3.zero;
			float angle = 0.0f;
			string weaponName = string.Empty;
			position.x = -float.Parse(largeHardpointNode.Attributes["z"].Value);
			position.y = float.Parse(largeHardpointNode.Attributes["y"].Value);
			position.z = float.Parse(largeHardpointNode.Attributes["x"].Value);
			angle = float.Parse(largeHardpointNode.Attributes["r"].Value) - 90.0f;
			weaponName = largeHardpointNode.Attributes["id"].Value;
			WeaponData weapon = ContentLoader.Instance.GetWeaponData(weaponName);

			data.largeHardpoints.Add(new ShipLoaderSerializerData.Hardpoint(position, angle, weapon));
		}

		if (document.DocumentElement.SelectSingleNode("accessories") != null)
		{
			XmlNodeList accessoryElements = document.DocumentElement.SelectNodes("accessories//accessory");
			foreach (XmlNode accessoryElement in accessoryElements)
			{
				string propName = "";
				Vector3 position = Vector3.zero;
				float scale;
				propName = accessoryElement.Attributes["m"].Value;
				position.x = -float.Parse(accessoryElement.Attributes["pz"].Value);
				position.y = float.Parse(accessoryElement.Attributes["py"].Value);
				position.z = float.Parse(accessoryElement.Attributes["px"].Value);
				scale = float.Parse(accessoryElement.Attributes["sx"].Value);
				PropData prop = ContentLoader.Instance.GetPropData(propName);
				data.accessories.Add(new ShipLoaderSerializerData.Accessory(propName, position, scale));
			}
		}

		return data;
	}

	private MeshFilter[] GetShipMeshes(GameObject aShipObject)
	{
		List<MeshFilter> elements = aShipObject.GetComponentsInChildren<MeshFilter>().ToList();
		for (int i = 0; i < elements.Count; ++i)
		{
			if (elements[i].gameObject == aShipObject)
			{
				elements.RemoveAt(i);
				--i;
			}
		}
		return elements.ToArray();
	}

	private void ComputeMesh(Mesh aMesh, out Vector3 aCenterOffset, out float aSize)
	{
		aCenterOffset = Vector3.zero;
		aSize = 0.0f;

		Vector3[] vertices = aMesh.vertices;
		for (int i = 0; i < vertices.Length; ++i)
		{
			aCenterOffset += vertices[i];
		}
		aCenterOffset /= vertices.Length;
		for (int i = 0; i < vertices.Length; ++i)
		{
			vertices[i] -= aCenterOffset;
			aSize = Mathf.Max(aSize, vertices[i].x, vertices[i].y, vertices[i].z);
		}
		aMesh.vertices = vertices;
	}

	public ShipLoaderData GetData(string aFileName)
	{
		if (myLoadedData.ContainsKey(aFileName) == true)
		{
			return myLoadedData[aFileName];
		}
		return null;
	}
}
