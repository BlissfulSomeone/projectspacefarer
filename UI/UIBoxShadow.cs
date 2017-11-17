using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBoxShadow : MonoBehaviour
{
	private enum eShadowLayer
	{ 
		Behind,
		Last,
		Over,
		Top,
	}

	[SerializeField]
	private Sprite shadowSprite;
	[SerializeField]
	private eShadowLayer shadowLayer;

	private RectTransform rectTransform;
	private GameObject shadowObject;
	private RectTransform spawnedRect;

	void Awake()
	{
		Refresh();
		
		Image image = shadowObject.AddComponent<Image>();
		image.raycastTarget = false;
		image.sprite = shadowSprite;
		image.type = Image.Type.Sliced;
		image.color = new Color(0, 0, 0, 0.5f);
	}

	public void Refresh()
	{
		ObjectCheck();
		
		if (rectTransform.gameObject.activeSelf == false && shadowObject.activeSelf == true)
		{
			shadowObject.SetActive(false);
		}
		if (rectTransform.gameObject.activeSelf == true && shadowObject.activeSelf == false)
		{
			shadowObject.SetActive(true);
		}

		spawnedRect.anchorMin = rectTransform.anchorMin;
		spawnedRect.anchorMax = rectTransform.anchorMax;
		spawnedRect.offsetMin = rectTransform.offsetMin - new Vector2(46.0f, 46.0f);
		spawnedRect.offsetMax = rectTransform.offsetMax + new Vector2(46.0f, 46.0f);
	}

	private void ObjectCheck()
	{
		if (rectTransform == null)
		{
			rectTransform = GetComponent<RectTransform>();
		}
		if (shadowObject == null)
		{
			shadowObject = new GameObject("UI Box Shadow");
			shadowObject.transform.SetParent(transform.parent);
			shadowObject.transform.SetAsFirstSibling();
			shadowObject.transform.localScale = Vector3.one;
		}
		if (spawnedRect == null)
		{
			spawnedRect = shadowObject.AddComponent<RectTransform>();
			spawnedRect.anchoredPosition3D = Vector3.zero;
		}
	}
}
