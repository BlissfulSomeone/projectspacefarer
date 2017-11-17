using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipEditorUIToolbarTransform : MonoBehaviour
{
	[System.Serializable]
	public struct TransformLinker
	{
		public TransformTool.eTransformMode mode;
		public Button button;
	}

	public delegate void TransformModeChangedHandler(TransformTool.eTransformMode aMode);
	public TransformModeChangedHandler OnTransformModeChanged;

	[SerializeField]
	private TransformLinker[] myButtons;

	void Awake()
	{
		for (int i = 0; i < myButtons.Length; ++i)
		{
			int tempIndex = i;
			myButtons[i].button.onClick.AddListener(() => { SetMode(myButtons[tempIndex].mode); });
		}
	}

	private void SetMode(TransformTool.eTransformMode aMode)
	{
		if (OnTransformModeChanged != null)
		{
			OnTransformModeChanged(aMode);
		}
	}
}
