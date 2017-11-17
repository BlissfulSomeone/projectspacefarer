using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPreferences
{
	#region Resolution
	public static Resolution CurrentResolution;

	public delegate void ResolutionChangedHandler(Resolution aResolution);
	public static ResolutionChangedHandler OnResolutionChanged;

	public static void SetResolution(Resolution aResolution)
	{
		CurrentResolution = aResolution;
	}
	#endregion

	#region Fullscreen
	public static bool IsFullscreen;

	public delegate void FullscreenChangedHandler(bool aFullscreen);
	public static FullscreenChangedHandler OnFullscreenChanged;

	public static void SetFullscreen(bool aFullscreen)
	{
		IsFullscreen = aFullscreen;
		if (OnFullscreenChanged != null)
		{
			OnFullscreenChanged(aFullscreen);
		}
	}
	#endregion

	#region EnvironmentQuality
	public enum eEnvironmentQuality
	{
		HIGH,
		MEDIUM,
		LOW
	}

	public static eEnvironmentQuality EnvironmentQuality = eEnvironmentQuality.HIGH;

	public delegate void EnvironmentQualityChangedHandler(eEnvironmentQuality aQuality);
	public static EnvironmentQualityChangedHandler OnEnvironmentQualityChanged;

	public static void SetEnvironmentQuality(eEnvironmentQuality aQuality)
	{
		EnvironmentQuality = aQuality;
		if (OnEnvironmentQualityChanged != null)
		{
			OnEnvironmentQualityChanged(aQuality);
		}
	}
	#endregion
}
