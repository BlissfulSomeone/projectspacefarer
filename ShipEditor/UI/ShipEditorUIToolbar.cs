using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipEditorUIToolbar : MonoBehaviour
{
	[System.Serializable]
	public class ModeButton
	{
		public ShipEditor.eShipEditorMode mode;
		public Button button;
	}
	
	[SerializeField]
	private ModeButton[] myModeButtons;

	public delegate void OnToolbarChangedHandler(ShipEditor.eShipEditorMode aMode);
	public OnToolbarChangedHandler OnToolbarChanged;

	void Awake()
	{
		for (int i = 0; i < myModeButtons.Length; ++i)
		{
			int tempIndex = i;
			myModeButtons[i].button.onClick.AddListener(() => { SetMode(myModeButtons[tempIndex].mode); });
		}
	}

	private void SetMode(ShipEditor.eShipEditorMode aMode)
	{
		if (OnToolbarChanged != null)
		{
			OnToolbarChanged(aMode);
		}
	}
}
