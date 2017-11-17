using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipEditorUIToolbarModule : MonoBehaviour
{
	[Header("Scaling")]
	[SerializeField]
	private Slider mySliderX;
	[SerializeField]
	private Slider mySliderY;
	[SerializeField]
	private Slider mySliderZ;

	[Header("Preview")]
	[SerializeField]
	private MeshFilter myPreviewModule;
	[SerializeField]
	private Camera myPreviewModuleCamera;
	[SerializeField]
	private RawImage myPreviewModuleWindow;
	[SerializeField]
	private RenderTexture myPreviewModuleRenderTexture;

	[Header("Modules")]
	[SerializeField]
	private Button myModuleButtonPrefab;
	[SerializeField]
	private Transform myListContent;

	public delegate void SelectedModuleChanged(ModuleData aModuleData);
	public SelectedModuleChanged OnModuleChanged;

	public delegate void ModuleScaleChanged(float aScale, int aAxis);
	public ModuleScaleChanged OnModuleScaleChanged;

	void Awake()
	{
		myPreviewModuleCamera.targetTexture = myPreviewModuleRenderTexture;
		myPreviewModuleWindow.texture = myPreviewModuleRenderTexture;

		foreach (ModuleData data in ContentLoader.Instance.ModuleData)
		{
			Button button = Instantiate(myModuleButtonPrefab);
			button.transform.SetParent(myListContent);
			button.image.rectTransform.anchoredPosition3D = Vector3.zero;
			button.image.rectTransform.localScale = Vector3.one;

			Text text = button.GetComponentInChildren<Text>();
			text.text = data.moduleName;

			Image icon = button.transform.FindChild("Icon").GetComponent<Image>();
			icon.sprite = data.icon;

			//myButtons.Add(button);
			//myHardpoints.Add(data);
			
			ModuleData tempData = data;
			button.onClick.AddListener(() => { SelectModule(tempData); });
		}

		mySliderX.onValueChanged.AddListener((value) => { ScaleChanged(value, 0); });
		mySliderY.onValueChanged.AddListener((value) => { ScaleChanged(value, 1); });
		mySliderZ.onValueChanged.AddListener((value) => { ScaleChanged(value, 2); });

		SelectModule(ContentLoader.Instance.ModuleData[0]);
	}

	private void SelectModule(ModuleData aModuleData)
	{
		if (OnModuleChanged != null)
		{
			OnModuleChanged(aModuleData);
		}

		myPreviewModule.mesh = aModuleData.modulePrefab.GetComponentInChildren<MeshFilter>().sharedMesh;
	}

	private void ScaleChanged(float aScale, int aAxis)
	{
		if (OnModuleScaleChanged != null)
		{
			OnModuleScaleChanged(aScale, aAxis);
		}

		Vector3 scale = myPreviewModule.transform.parent.localScale;
		scale[aAxis] = aScale;
		myPreviewModule.transform.parent.localScale = scale;
	}

	void Update()
	{
		myPreviewModule.transform.parent.rotation = Quaternion.Euler(0.0f, Time.timeSinceLevelLoad * 90.0f, 0.0f);
	}
}
