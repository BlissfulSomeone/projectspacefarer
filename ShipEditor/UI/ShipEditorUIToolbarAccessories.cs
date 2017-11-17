using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipEditorUIToolbarAccessories : MonoBehaviour
{
	[Header("Scaling")]
	[SerializeField]
	private Slider myScaleSlider;

	[Header("Accessories")]
	[SerializeField]
	private Button myPropButtonPrefab;
	[SerializeField]
	private Transform myListContent;

	public delegate void SelectedAccessoryChangedHander(PropData aPropData);
	public SelectedAccessoryChangedHander OnAccessoryChanged;

	public delegate void AccessoryScaleChangedHandler(float aScale);
	public AccessoryScaleChangedHandler OnAccessoryScaleChanged;

	void Awake()
	{
		foreach (PropData data in ContentLoader.Instance.PropData)
		{
			Button button = Instantiate(myPropButtonPrefab);
			button.transform.SetParent(myListContent);
			button.image.rectTransform.anchoredPosition3D = Vector3.zero;
			button.image.rectTransform.localScale = Vector3.one;

			Text text = button.GetComponentInChildren<Text>();
			text.text = data.propName;

			Image icon = button.transform.FindChild("Icon").GetComponent<Image>();
			icon.sprite = data.icon;

			PropData tempData = data;
			button.onClick.AddListener(() => { SelectProp(tempData); });
		}
		
		myListContent.GetComponentInChildren<UIButtonGroup>().ManualTrigger();

		myScaleSlider.onValueChanged.AddListener((value) => { ScaleChanged(value); });

		SelectProp(ContentLoader.Instance.PropData[0]);
	}

	private void SelectProp(PropData aPropData)
	{
		if (OnAccessoryChanged != null)
		{
			OnAccessoryChanged(aPropData);
		}
	}

	private void ScaleChanged(float aScale)
	{
		if (OnAccessoryScaleChanged != null)
		{
			OnAccessoryScaleChanged(aScale);
		}
	}
}
