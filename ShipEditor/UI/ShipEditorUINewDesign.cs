using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipEditorUINewDesign : MonoBehaviour
{
	[SerializeField]
	private Button myButtonPrefab;
	[SerializeField]
	private Transform myListContent;

	[SerializeField]
	private Text myStatsText;
	[SerializeField]
	private Text myDescriptionText;

	void Awake()
	{
		for (int i = 0; i < ContentLoader.Instance.ShipData.Length; ++i)
		{
			Button button = Instantiate(myButtonPrefab);
			button.transform.SetParent(myListContent);
			button.image.rectTransform.anchoredPosition3D = Vector3.zero;
			//button.image.rectTransform.sizeDelta = new Vector2(button.image.rectTransform.sizeDelta.x, 48.0f);
			button.transform.localScale = Vector3.one;

			UIMouseHoverHandler mouseHandler = button.gameObject.AddComponent<UIMouseHoverHandler>();
			ShipBaseData tempData = ContentLoader.Instance.ShipData[i];
			mouseHandler.onPointerEnter.AddListener(() => { ShowInfo(tempData); });
			mouseHandler.onPointerExit.AddListener(() => { HideInfo(); });

			Text text = button.GetComponentInChildren<Text>();
			text.text = ContentLoader.Instance.ShipData[i].modelName;

			button.onClick.AddListener(() => { SelectShip(tempData); });
		}

		HideInfo();
	}

	private void ShowInfo(ShipBaseData aData)
	{
		myStatsText.gameObject.SetActive(true);
		myDescriptionText.gameObject.SetActive(true);
		myStatsText.text = aData.className + "\n";
		myStatsText.text += aData.modelName + "\n\n";
		myStatsText.text += aData.maxSmallHardpoints + "\n";
		myStatsText.text += aData.maxMediumHardpoints + "\n";
		myStatsText.text += aData.maxLargeHardpoints + "\n";
		myStatsText.text += aData.maxSoftpoints + "\n";
		myStatsText.text += aData.hull + "\n";
		myStatsText.text += aData.armour + "\n";
		myStatsText.text += (int)aData.shield + "\n";
		myStatsText.text += (int)aData.speed + "\n";
		myStatsText.text += (int)aData.powerOutput + "\n";
		myStatsText.text += aData.cost;
		myDescriptionText.text = aData.description;
	}

	private void HideInfo()
	{
		myStatsText.gameObject.SetActive(false);
		myDescriptionText.gameObject.SetActive(false);
	}

	private void SelectShip(ShipBaseData aData)
	{
		//EventManager.Instance.TriggerEvent(eEventType.EditorStarted, aData);
		//EventManager.Instance.TriggerEvent(eEventType.EditorUIModeChanged, ShipEditorUI.eShipEditorUIMode.Editor);
		ShipEditor.Instance.myUI.SetMode(ShipEditorUI.eShipEditorUIMode.Editor);
		ShipEditor.Instance.StartEditor(aData);
		ShipEditor.Instance.BuildModule();
	}
}
