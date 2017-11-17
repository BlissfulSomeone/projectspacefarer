using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModuleData", menuName = "Project Spacefare/Module Data", order = 1)]
public class ModuleData : ScriptableObject
{
	public string moduleName;
	public Transform modulePrefab;
	public Sprite icon;
}
