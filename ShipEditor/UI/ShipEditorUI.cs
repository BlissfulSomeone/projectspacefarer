using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipEditorUI : MonoBehaviour
{
	public enum eShipEditorUIMode
	{ 
		Pause,
		Editor,
		New,
		Load,
		Save
	}

	[System.Serializable]
	public class EditorPanel
	{
		public eShipEditorUIMode mode;
		public Button[] buttons;
		public Canvas ui;
	}

	private eShipEditorUIMode myMode;
	public eShipEditorUIMode Mode { get { return myMode; } }

	[SerializeField]
	private eShipEditorUIMode myStartingMode;
	[SerializeField]
	private EditorPanel[] myPanels;

	public delegate void UIModeChangedHandler(eShipEditorUIMode aMode);
	public UIModeChangedHandler OnUIModeChanged;

	void Awake()
	{
		for (int panelIndex = 0; panelIndex < myPanels.Length; ++panelIndex)
		{
			for (int buttonIndex = 0; buttonIndex < myPanels[panelIndex].buttons.Length; ++buttonIndex)
			{
				eShipEditorUIMode tempMode = myPanels[panelIndex].mode;
				myPanels[panelIndex].buttons[buttonIndex].onClick.AddListener(() => { SetMode(tempMode); });
			}
		}
		SetMode(myStartingMode);
	}

	public void SetMode(eShipEditorUIMode aMode)
	{
		myMode = aMode;
		for (int panelIndex = 0; panelIndex < myPanels.Length; ++panelIndex)
		{
			bool uiActive = (myPanels[panelIndex].mode == aMode);
			myPanels[panelIndex].ui.gameObject.SetActive(uiActive);
			myPanels[panelIndex].ui.worldCamera.gameObject.SetActive(uiActive);
			//myPanels[panelIndex].ui.enabled = uiActive;
			//myPanels[panelIndex].ui.worldCamera.enabled = uiActive;
		}
		//EventManager.Instance.TriggerEvent(eEventType.EditorUIModeChanged, aMode);
		if (OnUIModeChanged != null)
		{
			OnUIModeChanged(aMode);
		}
	}
}
