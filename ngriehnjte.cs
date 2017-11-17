using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ngriehnjte : MonoBehaviour
{
	//void Awake()
	//{
	//	Shield[] shields = FindObjectsOfType<Shield>();
	//	for (int i = 0; i < shields.Length; ++i)
	//	{
	//		shields[i].Init(40.0f);
	//	}
	//}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space) == true)
		{
			Shield[] shields = FindObjectsOfType<Shield>();
			for (int i = 0; i < shields.Length; ++i)
			{
				shields[i].Trigger(Vector3.forward);
			}
		}
	}
}
