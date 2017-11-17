using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipOrderUIItem : MonoBehaviour
{
	[SerializeField]
	private Dropdown myManouverTypeDropdown;
	[SerializeField]
	private Dropdown myTargetTypeDropdown;
	[SerializeField]
	private Dropdown myClassTypeDropdown;
	[SerializeField]
	private Button myTargetShipButton;
	[SerializeField]
	private Button myDeleteButton;

	[HideInInspector]
	public ShipOrderUI controller;
	[HideInInspector]
	public int id;

	void Awake()
	{
		myManouverTypeDropdown.AddOptions(new List<Dropdown.OptionData>
			{
				new Dropdown.OptionData("Head On"),
				new Dropdown.OptionData("Broadside"),
				new Dropdown.OptionData("Artillery"),
			});
		myTargetTypeDropdown.AddOptions(new List<Dropdown.OptionData> 
			{ 
				new Dropdown.OptionData("Closest"),
				new Dropdown.OptionData("Farthest"),
				new Dropdown.OptionData("Weakest"),
				new Dropdown.OptionData("Strongest"),
				new Dropdown.OptionData("Specific"),
				new Dropdown.OptionData("Assist"),
				new Dropdown.OptionData("Protect"),
			});
		myClassTypeDropdown.AddOptions(new List<Dropdown.OptionData>
			{
				new Dropdown.OptionData("All"),
				new Dropdown.OptionData("Patrol Boat"),
				new Dropdown.OptionData("Gun Boat"),
				new Dropdown.OptionData("Forward Offence Ship"),
				new Dropdown.OptionData("Mainline Ship"),
				new Dropdown.OptionData("Armoured Cruiser"),
				new Dropdown.OptionData("Battle Cruiser"),
			});

		myDeleteButton.onClick.AddListener(() => { controller.DeleteOrder(id); });
		myManouverTypeDropdown.onValueChanged.AddListener((value) => { UpdateInput(value); });
		myTargetTypeDropdown.onValueChanged.AddListener((value) => { UpdateTargetType(value); });
		myTargetShipButton.onClick.AddListener(() => { StartTargetingShip(); });
		SetTarget(null);
		UpdateInput(0);
		//UpdateTargetType(0);
	}

	private void UpdateInput(int aOrderType)
	{
		UpdateVisuals(aOrderType);
		if (controller != null)
		{
			//controller.SetOrderType(id, aOrderType);
		}
	}

	private void UpdateVisuals(int aTargetType)
	{
		//if (aOrderType == 0)
		//{
		//	myTargetTypeDropdown.gameObject.SetActive(true);
		//	myTargetShipButton.gameObject.SetActive(false);
		//}
		//else if (aOrderType == 1 || aOrderType == 2 || aOrderType == 7)
		//{
		//	myTargetTypeDropdown.gameObject.SetActive(false);
		//	myTargetShipButton.gameObject.SetActive(true);
		//}
		//else
		//{
		//	myTargetTypeDropdown.gameObject.SetActive(false);
		//	myTargetShipButton.gameObject.SetActive(false);
		//}
		bool useClassType = (aTargetType <= 3);
		myTargetTypeDropdown.gameObject.SetActive(useClassType);
		myTargetShipButton.gameObject.SetActive(!useClassType);
	}

	private void UpdateTargetType(int aTargetType)
	{
		controller.SetTargetType(id, aTargetType);
	}

	private void StartTargetingShip()
	{
		GameManager.Instance.SetState(GameManager.eGameState.PlanningPickTarget);
		controller.AwaitingTargetId = id;
	}

	public void SetTarget(Ship aShip)
	{ 
		myTargetShipButton.GetComponentInChildren<Text>().text = (aShip != null ? "Target assigned" : "No target");
	}

	//public void SetOrderType(int aOrderType)
	//{
	//	myManouverTypeDropdown.value = aOrderType; 
	//	UpdateVisuals(aOrderType);
	//}

	public void SetManouverType(int aManouverType)
	{
		myManouverTypeDropdown.value = aManouverType;
	}

	public void SetTargetType(int aTargetType)
	{
		myTargetTypeDropdown.value = aTargetType;
		UpdateVisuals(aTargetType);
	}

	public void SetClassType(int aClassType)
	{
		myClassTypeDropdown.value = aClassType;
	}
}
