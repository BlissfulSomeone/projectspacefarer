using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
	public float updateInterval;
	private float accum = 0.0f;
	private int frames = 0;
	private float timeLeft;
	private string[] numbers;
	private Text text;
	private Material material;

	void Awake()
	{
		numbers = new string[100];
		for (int i = 0; i < 100; ++i)
		{
			numbers[i] = i.ToString();
		}
		text = GetComponent<Text>();

		timeLeft = updateInterval;
	}

	void Update()
	{
		timeLeft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		++frames;

		if (timeLeft <= 0.0f)
		{
			float fps = accum / frames;
			string format = string.Format("FPS: {0:F2}", fps);
			text.text = format;
			text.material.color = Color.white;
			if (fps < 30)
			{
				text.color = Color.yellow;
			}
			else if (fps < 10)
			{
				text.color = Color.red;
			}
			else
			{
				text.color = Color.green;
			}
			timeLeft = updateInterval;
			accum = 0.0f;
			frames = 0;
		}
	}
}
