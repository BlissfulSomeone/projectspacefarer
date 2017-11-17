using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
	private PooledObject myPrefab;

	private List<PooledObject> myAvailableObjects = new List<PooledObject>();

	public PooledObject GetObject()
	{
		PooledObject obj;
		int lastAvailableIndex = myAvailableObjects.Count - 1;
		if (lastAvailableIndex >= 0)
		{
			obj = myAvailableObjects[lastAvailableIndex];
			myAvailableObjects.RemoveAt(lastAvailableIndex);
			obj.gameObject.SetActive(true);
		}
		else
		{
			obj = Instantiate(myPrefab);
			obj.transform.SetParent(transform, false);
			obj.myPool = this;
		}
		return obj;
	}

	public void AddObject(PooledObject aObject)
	{
		aObject.gameObject.SetActive(false);
		myAvailableObjects.Add(aObject);
	}
	
	public static ObjectPool GetPool(PooledObject aPrefab)
	{
		GameObject obj;
		ObjectPool pool;
		if (Application.isEditor == true)
		{
			obj = GameObject.Find(aPrefab.name + " Pool");
			if (obj != null)
			{
				pool = obj.GetComponent<ObjectPool>();
				if (pool != null)
				{
					return pool;
				}
			}
		}
		obj = new GameObject(aPrefab.name + " Pool");
		pool = obj.AddComponent<ObjectPool>();
		pool.myPrefab = aPrefab;
		return pool;
	}
}
