using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Disclaimer : MonoBehaviour
{
	public Text disclaimerText;

	private float currentAlpha = 0.0f;
	private bool fadingIn = true;

	void Awake()
	{
		PlayerPreferences.CurrentResolution = Screen.currentResolution;
		PlayerPreferences.IsFullscreen = Screen.fullScreen;
	}

	void Update()
	{
		if (Input.anyKeyDown == true)
		{
			fadingIn = false;
		}

		if (fadingIn == true)
		{
			if (currentAlpha < 1.0f)
			{
				currentAlpha += Time.deltaTime * 0.25f;
				Color c = disclaimerText.color;
				c.a = currentAlpha;
				disclaimerText.color = c;
			}
		}
		else
		{
			if (currentAlpha > 0.0f)
			{
				currentAlpha -= Time.deltaTime * 0.25f;
				Color c = disclaimerText.color;
				c.a = currentAlpha;
				disclaimerText.color = c;
			}
			else
			{
				SceneManager.LoadScene("SceneMainMenu", LoadSceneMode.Single);
			}
		}
	}
}
