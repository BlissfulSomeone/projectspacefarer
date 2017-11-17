using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelGroup : MonoBehaviour
{
	[System.Serializable]
	public struct ButtonToPanelLinker
	{
		public Button button;
		public List<RectTransform> panels;
	}

	[SerializeField]
	private ButtonToPanelLinker[] groups;

	void Awake()
	{
		for (int i = 0; i < groups.Length; ++i)
		{
			int tempIndex = i;
			groups[i].button.onClick.AddListener(() => { SetGroup(tempIndex); });
		}
		SetGroup(0);
	}

	private void SetGroup(int aIndex)
	{
		for (int i = 0; i < groups.Length; ++i)
		{
			if (i == aIndex)
			{
				ActivateGroup(i);
			}
			else
			{
				DisableGroup(i);
			}
		}
	}

	private void ActivateGroup(int aIndex)
	{
		for (int i = 0; i < groups[aIndex].panels.Count; ++i)
		{
			groups[aIndex].panels[i].gameObject.SetActive(true);
		}
	}

	private void DisableGroup(int aIndex)
	{
		for (int i = 0; i < groups[aIndex].panels.Count; ++i)
		{
			groups[aIndex].panels[i].gameObject.SetActive(false);
		}
	}
}
