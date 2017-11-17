using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoftpointData", menuName = "Project Spacefare/Softpoint Data", order = 1)]
public class SoftpointData : ScriptableObject
{
	public string softpointName = string.Empty;
	public float powerBonus = 0.0f;
	public float speedBonus = 0.0f;
	public int armourBonus = 0;
	public float shieldBonus = 0.0f;
	public float powerUsage = 0.0f;
	public int cost = 0;
	public Sprite icon;
	public string description = string.Empty;
}
