using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PropData", menuName = "Project Spacefare/Prop Data", order = 1)]
public class PropData : ScriptableObject
{
	public string propName = string.Empty;
	public Transform propPrefab;
	public Sprite icon;
}
