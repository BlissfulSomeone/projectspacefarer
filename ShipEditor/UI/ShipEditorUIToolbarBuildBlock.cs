using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipEditorUIToolbarBuildBlock : MonoBehaviour
{
	[Header("Scale Sliders")]
	[SerializeField]
	private Slider myScaleXSlider;
	[SerializeField]
	private Slider myScaleYSlider;
	[SerializeField]
	private Slider myScaleZSlider;

	[Header("Preview Block")]
	[SerializeField]
	private Camera myPreviewBlockCamera;
	[SerializeField]
	private RawImage myPreviewBlockWindow;
	[SerializeField]
	private RenderTexture myPreviewBlockRenderTexture;
	[SerializeField]
	private Transform myPreviewBlock;

	void Awake()
	{
		myPreviewBlockCamera.targetTexture = myPreviewBlockRenderTexture;
		myPreviewBlockWindow.texture = myPreviewBlockRenderTexture;

		myScaleXSlider.onValueChanged.AddListener((value) => { ScaleBlock(value, 0); });
		myScaleYSlider.onValueChanged.AddListener((value) => { ScaleBlock(value, 1); });
		myScaleZSlider.onValueChanged.AddListener((value) => { ScaleBlock(value, 2); });
	}

	void OnEnable()
	{
		//EventManager.Instance.AddListener(eEventType.EditorScaleCopied, CopyScale);
	}

	void OnDisable()
	{
		//EventManager.Instance.RemoveListener(eEventType.EditorScaleCopied, CopyScale);
	}

	private void ScaleBlock(float aScale, int aAxis)
	{
		Vector3 scale = myPreviewBlock.localScale;
		scale[aAxis] = aScale;
		myPreviewBlock.localScale = scale;

		//EventManager.Instance.TriggerEvent(eEventType.EditorScaleChanged, aScale, aAxis);
	}

	/// <summary>
	/// CopyScale(Vector3 aScale)
	/// </summary>
	/// <param name="args"></param>
	private void CopyScale(params object[] args)
	{
		Vector3 aScale = (Vector3)args[0];
		myScaleXSlider.value = aScale.x;
		myScaleYSlider.value = aScale.y;
		myScaleZSlider.value = aScale.z;
		myPreviewBlock.localScale = aScale;
	}

	void Update()
	{
		myPreviewBlock.rotation = Quaternion.Euler(0.0f, Time.timeSinceLevelLoad * 90.0f, 0.0f);
	}
}
