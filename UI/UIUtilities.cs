using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIUtilities
{
	private static Texture2D myBlankTexture;
	private static Texture2D BlankTexture
	{
		get
		{
			if (myBlankTexture == null)
			{
				myBlankTexture = new Texture2D(1, 1);
				myBlankTexture.SetPixel(0, 0, Color.white);
				myBlankTexture.Apply();
			}
			return myBlankTexture;
		}
	}

	public static void DrawRect(Rect aRect, Color aColor)
	{
		GUI.color = aColor;
		GUI.DrawTexture(aRect, BlankTexture);
		GUI.color = Color.white;
	}

	public static void DrawRectBorder(Rect aRect, float aThickness, Color aColor)
	{
		DrawRect(new Rect(aRect.xMin, aRect.yMin, aRect.width, aThickness), aColor);
		DrawRect(new Rect(aRect.xMin, aRect.yMax - aThickness, aRect.width, aThickness), aColor);
		DrawRect(new Rect(aRect.xMin, aRect.yMin, aThickness, aRect.height), aColor);
		DrawRect(new Rect(aRect.xMax - aThickness, aRect.yMin, aThickness, aRect.height), aColor);
	}

	public static Bounds GetViewportBounds(Camera aCamera, Vector3 aScreenPosition1, Vector3 aScreenPosition2)
	{
		Vector3 v1 = aCamera.ScreenToViewportPoint(aScreenPosition1);
		Vector3 v2 = aCamera.ScreenToViewportPoint(aScreenPosition2);
		Vector3 min = Vector3.Min(v1, v2);
		Vector3 max = Vector3.Max(v1, v2);
		min.z = aCamera.nearClipPlane;
		max.z = aCamera.farClipPlane;

		Bounds bounds = new Bounds();
		bounds.SetMinMax(min, max);
		return bounds;
	}

	public static Vector2 WorldToGUIPoint(Vector3 world, Camera cam)
	{
		Vector2 screenPoint = cam.WorldToScreenPoint(world);
		screenPoint.y = (float)Screen.height - screenPoint.y;
		return screenPoint;
	}
}
