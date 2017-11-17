using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtension
{
	public static void SetLayer(this GameObject go, string layerName)
	{
		int nrOfChildren = go.transform.childCount;
		for (int i = 0; i < nrOfChildren; ++i)
		{
			go.transform.GetChild(i).gameObject.SetLayer(layerName);
		}
		go.layer = LayerMask.NameToLayer(layerName);
	}

	public static void SetLayer(this GameObject go, int layerMaskValue)
	{
		int nrOfChildren = go.transform.childCount;
		for (int i = 0; i < nrOfChildren; ++i)
		{
			go.transform.GetChild(i).gameObject.SetLayer(layerMaskValue);
		}
		go.layer = layerMaskValue;
	}
}
