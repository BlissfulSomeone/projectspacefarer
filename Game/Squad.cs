using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squad : MonoBehaviour
{
	private ShipLoader.ShipLoaderData myData;
	private List<Ship> myShips;

	[SerializeField]
	private Ship myShipPrefab;

	private int myOrder;
	public int Order { get { return myOrder; } }

	public void Init(ShipLoader.ShipLoaderData aData, int aTeamIndex)
	{
		myShips = new List<Ship>();
		myData = aData;

		int squadSize = aData.baseData.squadSize;
		int rows = Mathf.Max(1, Mathf.FloorToInt(Mathf.Sqrt(squadSize)));

		int width = squadSize / rows;

		for (int i = 0; i < squadSize; ++i)
		{
			int x = i % rows;
			int z = i / rows;

			Ship ship = Instantiate(myShipPrefab);
			ship.Init(myData, aTeamIndex);
			ship.transform.SetParent(transform);
			ship.transform.localPosition = new Vector3(x * 8.0f - (width - 1) * 4.0f, 0.0f, z * 8.0f - (width - 1) * 4.0f);
			myShips.Add(ship);
		}

		myOrder = 0;
	}

	public void SetHoverType(string aType)
	{
		int layer = LayerMask.NameToLayer(aType);
		for (int i = 0; i < myShips.Count; ++i)
		{
			if (myShips[i] != null)
			{
				myShips[i].SetHoverType(aType);
			}
		}
	}

	//public void SetOrder(int aOrder)
	//{
	//	myOrder = aOrder;
	//	for (int i = 0; i < myShips.Count; ++i)
	//	{
	//		if (myShips[i] != null)
	//		{
	//			myShips[i].SetOrder(aOrder);
	//		}
	//	}
	//}
}
