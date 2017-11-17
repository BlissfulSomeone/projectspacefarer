using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour
{
	[SerializeField]
	private Gradient myBlueTeamColor;
	[SerializeField]
	private Gradient myRedTeamColor;

	public void Init(int aTeamIndex)
	{
		TrailRenderer trail = GetComponentInChildren<TrailRenderer>();
		if (trail != null)
		{
			if (aTeamIndex == 0)
			{
				trail.colorGradient = myBlueTeamColor;
			}
			else if (aTeamIndex == 1)
			{
				trail.colorGradient = myRedTeamColor;
			}
			trail.widthMultiplier = transform.localScale.x;
			trail.time = transform.localScale.x;
		}
	}
}
