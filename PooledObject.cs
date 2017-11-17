using UnityEngine;
using System.Collections;

public class PooledObject : MonoBehaviour
{
	public ObjectPool myPool { get; set; }

	[System.NonSerialized]
	ObjectPool poolInstanceForPrefab;

	public void ReturnToPool()
	{
		if (myPool != null)
		{
			myPool.AddObject(this);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public T GetPooledInstance<T>() where T : PooledObject
	{
		if (poolInstanceForPrefab == null)
		{
			poolInstanceForPrefab = ObjectPool.GetPool(this);
		}
		return (T)poolInstanceForPrefab.GetObject();
	}
}
