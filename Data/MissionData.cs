using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "Project Spacefare/Mission Data", order = 1)]
public class MissionData : ScriptableObject
{
	public string missionName;
	public int order;
}
