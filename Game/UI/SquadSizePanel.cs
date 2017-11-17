using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquadSizePanel : MonoBehaviour
{
	[SerializeField]
	private Button myDownSizeButton;
	[SerializeField]
	private Button myUpSizeButton;
	[SerializeField]
	private Text mySizeText;

	[SerializeField]
	private Transform myContainer;
	[SerializeField]
	private UIBoxShadow myShadow;

	private bool myIsShowing = false;

	private RectTransform myRect = null;
	public RectTransform Rect { get { if (myRect == null) { myRect = GetComponent<RectTransform>(); } return myRect; } }

	void Awake()
	{
		myDownSizeButton.onClick.AddListener(() => { DownSize(); });
		myUpSizeButton.onClick.AddListener(() => { UpSize(); });
	}

	private void DownSize()
	{
		//GameManager.Instance.ChangeSquadSize(-1);
		RefreshText();
	}

	private void UpSize()
	{ 
		//GameManager.Instance.ChangeSquadSize(1);
		RefreshText();
	}

	private void RefreshText()
	{
		//mySizeText.text = GameManager.Instance.SelectedSquadSize.ToString();
	}

	void Update()
	{
		//if (myIsShowing == true && GameManager.Instance.SelectedSquad != null)
		//{
		//	Vector3 worldPosition = GameManager.Instance.SelectedSquad.Ships[0].transform.position;
		//	Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
		//	Rect.anchoredPosition = screenPosition + Vector3.right * 128.0f;
		//	myShadow.Refresh();
		//}
	}

	public void Show(bool aFlag)
	{
		myIsShowing = aFlag;
		myContainer.gameObject.SetActive(aFlag);
		if (aFlag == true)
		{
			RefreshText();
		}
		myShadow.Refresh();
	}
}
