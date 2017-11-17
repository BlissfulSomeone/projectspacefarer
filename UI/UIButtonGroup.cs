using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonGroup : MonoBehaviour
{
	private Button[] buttonGroup;
	private Color[] normalColor;
	private Color[] selectedColor;
	private Button selectedButton;

	void Awake()
	{
		CollectButtonGroup();
	}

	private void Select(int aIndex)
	{
		for (int i = 0; i < buttonGroup.Length; ++i)
		{
			ColorBlock colors = buttonGroup[i].colors;
			colors.normalColor = (i == aIndex ? selectedColor[i] : normalColor[i]);
			buttonGroup[i].colors = colors;
		}
	}

	public void ManualTrigger()
	{
		if (buttonGroup.Length == 0)
		{
			CollectButtonGroup();
		}
	}

	private void CollectButtonGroup()
	{
		buttonGroup = GetComponentsInChildren<Button>();
		normalColor = new Color[buttonGroup.Length];
		selectedColor = new Color[buttonGroup.Length];

		for (int i = 0; i < buttonGroup.Length; ++i)
		{
			int tempIndex = i;
			buttonGroup[i].onClick.AddListener(() => { Select(tempIndex); });
			normalColor[i] = buttonGroup[i].colors.normalColor;
			selectedColor[i] = buttonGroup[i].colors.highlightedColor;
		}

		Select(0);
	}
}
