using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CampaignData", menuName = "Project Spacefare/Campaign Data", order = 1)]
public class CampaignData : ScriptableObject
{
	public string campaignName;
	public Vector2 galaxyPosition;
	public MissionData[] missions;
}
