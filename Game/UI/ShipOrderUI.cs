using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShipOrderUI : MonoBehaviour
{
	[SerializeField]
	private Transform myPanelTransform;
	[SerializeField]
	private ShipOrderUIItem myOrderItemPrefab;
	[SerializeField]
	private Button myAddOrderButton;
	[SerializeField]
	private Transform myOrderList;

	private Ship myActiveShip;

	private List<ShipOrderUIItem> items = new List<ShipOrderUIItem>();

	private int myAwaitingTargetId;
	public int AwaitingTargetId { get { return myAwaitingTargetId; } set { myAwaitingTargetId = value; } }

	void Awake()
	{
		myAddOrderButton.onClick.AddListener(() => { AddOrder(); });
	}

	void OnEnable()
	{
		GameManager.Instance.OnShipSelected += ShipSelected;
		GameManager.Instance.OnShipDeselected += ShipDeselected;
	}

	void OnDisable()
	{
		GameManager.Instance.OnShipSelected -= ShipSelected;
		GameManager.Instance.OnShipDeselected -= ShipDeselected;
	}

	void Start()
	{
		ShipDeselected();
	}

	private void ShipSelected(Ship aShip)
	{
		if (GameManager.Instance.State == GameManager.eGameState.PlanningBuild)
		{
			myPanelTransform.gameObject.SetActive(true);
			myActiveShip = aShip;

			for (int i = 0; i < items.Count; ++i)
			{
				Destroy(items[i].gameObject);
			}
			items.Clear();

			for (int i = 0; i < myActiveShip.GetNumberOfOrders(); ++i)
			{
				ShipOrderUIItem item = Instantiate(myOrderItemPrefab);
				item.transform.SetParent(myOrderList);
				item.transform.localRotation = Quaternion.identity;
				item.transform.localScale = Vector3.one;
				item.controller = this;
				item.id = items.Count;
				items.Add(item);
				//item.SetOrderType((int)myActiveShip.GetOrder(i).orderType);
				//item.SetTargetType((int)myActiveShip.GetOrder(i).targetType);
				//item.SetTarget(myActiveShip.GetOrder(i).targetShip);
				item.SetManouverType((int)myActiveShip.GetOrder(i).manouverType);
				item.SetTargetType((int)myActiveShip.GetOrder(i).targetType);
				item.SetClassType((int)myActiveShip.GetOrder(i).classType);
				item.SetTarget(myActiveShip.GetOrder(i).targetShip);
			}

			//int currentOrderIndex = (int)aShip.Order;
			//UpdateTargetType(currentOrderIndex);
		}
		else if (GameManager.Instance.State == GameManager.eGameState.PlanningPickTarget)
		{
			GameManager.Instance.SetState(GameManager.eGameState.PlanningBuild);
			//myActiveShip.SetOrderTarget(AwaitingTargetId, aShip);
			//items[AwaitingTargetId].SetTarget(aShip);
			myActiveShip.SetOrderTargetShip(AwaitingTargetId, aShip);
			items[AwaitingTargetId].SetTarget(aShip);
		}
	}

	private void ShipDeselected()
	{ 
		myPanelTransform.gameObject.SetActive(false);
		myActiveShip = null;
	}

	//private void SetOrderToShip(int aOrderIndex)
	//{
	//	myActiveShip.SetOrder(aOrderIndex);
	//	UpdateTargetType(aOrderIndex);
	//}
	//
	//private void StartTargetShip()
	//{
	//	GameManager.Instance.SetState(GameManager.eGameState.PlanningPickTarget);
	//	//myTargetShipButton.GetComponentInChildren<Text>().text = "[PROCESSING]";
	//}

	//private void UpdateTargetType(int aOrderIndex)
	//{
	//	if (aOrderIndex == 2 || aOrderIndex == 3)
	//	{
	//		//myTargetTypeTransform.gameObject.SetActive(true);
	//		//myTargetShipTransform.gameObject.SetActive(false);
	//	}
	//	else if (aOrderIndex == 0 || aOrderIndex == 1)
	//	{
	//		//myTargetTypeTransform.gameObject.SetActive(false);
	//		//myTargetShipTransform.gameObject.SetActive(true);
	//	}
	//}

	private void AddOrder()
	{
		myActiveShip.AddOrder(new ShipOrder());

		ShipOrderUIItem item = Instantiate(myOrderItemPrefab);
		item.transform.SetParent(myOrderList);
		item.transform.localRotation = Quaternion.identity;
		item.transform.localScale = Vector3.one;

		item.controller = this;
		item.id = items.Count;

		items.Add(item);
	}

	public void DeleteOrder(int aIndex)
	{
		myActiveShip.RemoveOrder(aIndex);

		ShipOrderUIItem itemToDelete = items[aIndex];
		Destroy(itemToDelete.gameObject);
		items.RemoveAt(aIndex);

		for (int i = 0; i < items.Count; ++i)
		{
			items[i].id = i;
		}
	}

	//public void SetOrderType(int aOrderIndex, int aOrderType)
	//{
	//	myActiveShip.SetOrderType(aOrderIndex, (eOrderType)aOrderType);
	//}
	//
	//public void SetTargetType(int aOrderIndex, int aTargetType)
	//{
	//	myActiveShip.SetOrderTarget(aOrderIndex, (ShipData.eShipModel)aTargetType);
	//}
	//
	//public void SetTargetShip(int aOrderIndex, Ship aTargetShip)
	//{
	//	myActiveShip.SetOrderTarget(aOrderIndex, aTargetShip);
	//}

	public void SetManouverType(int aOrderIndex, int aManouverType)
	{
		myActiveShip.SetOrderManouverType(aOrderIndex, (eManouverType)aManouverType);
	}

	public void SetTargetType(int aOrderIndex, int aTargetType)
	{
		myActiveShip.SetOrderTargetType(aOrderIndex, (eTargetType)aTargetType);
	}

	public void SetClassType(int aOrderIndex, int aClassType)
	{
		myActiveShip.SetOrderTargetClass(aOrderIndex, (ShipData.eShipModel)aClassType);
	}

	public void SetTargetShip(int aOrderIndex, Ship aTargetShip)
	{
		myActiveShip.SetOrderTargetShip(aOrderIndex, aTargetShip);
	}
}
