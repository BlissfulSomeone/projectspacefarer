using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipEditorUISoftpointSlots : MonoBehaviour
{
	[SerializeField]
	private Button mySoftpointSlotPrefab;

	private Button[] myButtons;
	private Sprite myEmptySprite;

	void Awake()
	{
		Texture2D emptyTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		emptyTexture.SetPixel(0, 0, new Color(0, 0, 0, 0));
		emptyTexture.Apply();
		myEmptySprite = Sprite.Create(emptyTexture, new Rect(0, 0, 1, 1), Vector2.zero);
	}

	public void Init(int aNumberOfSlots)
	{
		myButtons = new Button[aNumberOfSlots];
		for (int i = 0; i < aNumberOfSlots; ++i)
		{
			Button button = Instantiate(mySoftpointSlotPrefab);
			button.transform.SetParent(transform);

			button.image.rectTransform.anchoredPosition3D = Vector3.zero;
			button.transform.localScale = Vector3.one;

			int tempIndex = i;
			button.onClick.AddListener(() => { ShipEditor.Instance.SetSoftpointAtIndex(tempIndex); });
			
			myButtons[i] = button;
			SetIcon(i, null);
		}
	}

	public void Clear()
	{
		for (int i = 0; i < myButtons.Length; ++i)
		{
			Destroy(myButtons[i].gameObject);
		}
	}

	public void SetIcon(int aIndex, SoftpointData aSoftpointData)
	{
		Transform child = myButtons[aIndex].transform.GetChild(0);
		Image image = child.GetComponent<Image>();
		image.sprite = (aSoftpointData != null ? aSoftpointData.icon : myEmptySprite);
	}

	//[SerializeField]
	//private Button mySoftpointSlotPrefab;
	//
	//private List<Button> mySoftpointSlots;
	//private Sprite myEmptySprite;
	//
	//private UIBoxShadow myBoxShadow;
	//
	//void Awake()
	//{
	//	mySoftpointSlots = new List<Button>();
	//
	//	Texture2D emptyTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
	//	emptyTexture.SetPixel(0, 0, new Color(0, 0, 0, 0));
	//	emptyTexture.Apply();
	//	myEmptySprite = Sprite.Create(emptyTexture, new Rect(0, 0, 1, 1), Vector2.zero);
	//
	//	myBoxShadow = GetComponent<UIBoxShadow>();
	//	StartCoroutine(Coroutine_RefreshShadows());
	//	myBoxShadow.Refresh();
	//}
	//
	//private IEnumerator Coroutine_RefreshShadows()
	//{
	//	yield return new WaitForEndOfFrame();
	//	myBoxShadow.Refresh();
	//}
	//
	//void OnEnable()
	//{
	//	//EventManager.Instance.AddListener(eEventType.EditorShipCreated, Init);
	//	//EventManager.Instance.AddListener(eEventType.EditorShipChanged, RefreshSlots);
	//}
	//
	//void OnDisable()
	//{ 
	//	//EventManager.Instance.RemoveListener(eEventType.EditorShipCreated, Init);
	//	//EventManager.Instance.RemoveListener(eEventType.EditorShipChanged, RefreshSlots);
	//}
	//
	///// <summary>
	///// Init(ShipData aData)
	///// </summary>
	///// <param name="args"></param>
	//private void Init(params object[] args)
	//{
	//	//ShipBaseData aData = (ShipBaseData)args[0];
	//	ShipData aData = (ShipData)args[0];
	//
	//	for (int i = 0; i < mySoftpointSlots.Count; ++i)
	//	{
	//		Destroy(mySoftpointSlots[i].gameObject);
	//	}
	//	mySoftpointSlots.Clear();
	//
	//	for (int i = 0; i < aData.maxSoftpoints; ++i)
	//	{
	//		Button softpointSlot = Instantiate(mySoftpointSlotPrefab);
	//		softpointSlot.transform.SetParent(transform);
	//		softpointSlot.image.rectTransform.anchoredPosition3D = Vector3.zero;
	//		softpointSlot.transform.localScale = Vector3.one;
	//
	//		int tempSlotIndex = i;
	//		//softpointSlot.onClick.AddListener(() => { EventManager.Instance.TriggerEvent(eEventType.EditorAddSoftpoint, tempSlotIndex); });
	//
	//		mySoftpointSlots.Add(softpointSlot);
	//	}
	//	myBoxShadow = GetComponent<UIBoxShadow>();
	//	StartCoroutine(Coroutine_RefreshShadows());
	//	myBoxShadow.Refresh();
	//}
	//
	///// <summary>
	///// RefreshSlots(ShipData aShipData)
	///// </summary>
	///// <param name="args"></param>
	//private void RefreshSlots(params object[] args)
	//{
	//	ShipData aShipData = (ShipData)args[0];
	//
	//	for (int i = 0; i < aShipData.maxSoftpoints; ++i)
	//	{
	//		Button button = mySoftpointSlots[i];
	//		Transform child = button.transform.GetChild(0);
	//		Image icon = child.GetComponent<Image>();
	//		icon.sprite = (aShipData.softpoints[i] != null ? aShipData.softpoints[i].icon : myEmptySprite);
	//	}
	//}
}
