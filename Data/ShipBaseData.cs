using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShipData", menuName = "Project Spacefare/Ship Data", order = 1)]
public class ShipBaseData : ScriptableObject
{
	public string className = string.Empty;
	public string modelName = string.Empty;
	public int squadSize = 0;
	public int maxSoftpoints = 0;
	public int maxSmallHardpoints = 0;
	public int maxMediumHardpoints = 0;
	public int maxLargeHardpoints = 0;
	public int hull = 0;
	public int armour = 0;
	public float shield = 0.0f;
	public float powerOutput = 0.0f;
	public float speed = 0.0f;
	public float turnRate = 0.0f;
	public float firingRange = 0.0f;
	public int cost = 0;
	[TextArea(3, 20)]
	public string description = string.Empty;
}
