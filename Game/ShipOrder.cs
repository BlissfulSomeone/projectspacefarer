using System.Collections;
using System.Collections.Generic;

//public enum eOrderType
//{
//	None = -1,
//	Attack_Type,
//	Attack_Target,
//	Protect_Target,
//	Skirmish,
//	Bombard,
//	Broadside,
//	CarrionTactic,
//	Assist_Target,
//	Support,
//	Stop,
//	COUNT_DONT_USE,
//}

public enum eManouverType
{
	None = -1,
	HeadOn,
	Broadside,
	Artillery,
}

public enum eTargetType
{
	None = -1,
	Closest,
	Furthest,
	Weakest,
	Strongest,
	Specific,
	Assist,
	Protect,
}

[System.Serializable]
public struct ShipOrder
{
	public eManouverType manouverType;
	public eTargetType targetType;
	public ShipData.eShipModel classType;
	public Ship targetShip;

	public ShipOrder(eManouverType aManouverType, eTargetType aTargetType, ShipData.eShipModel aClassType, Ship aTargetShip)
	{
		manouverType = aManouverType;
		targetType = aTargetType;
		classType = aClassType;
		targetShip = aTargetShip;
	}
}
