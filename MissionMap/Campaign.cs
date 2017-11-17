using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Campaign : MonoBehaviour
{
	[SerializeField]
	private Button myMissionButtonPrefab;
	[SerializeField]
	private Transform myMissionList;

	private List<Button> myMissionButtons;

	[SerializeField]
	private Texture2D myCampaignIcon;

	void Awake()
	{
		myMissionButtons = new List<Button>();
	}

	void OnGUI()
	{
		float iconSize = 48.0f;
		Rect rect = new Rect();
		rect.width = iconSize;
		rect.height = iconSize;
		for (int i = 0; i < ContentLoader.Instance.CampaignData.Length; ++i)
		{
			CampaignData data = ContentLoader.Instance.CampaignData[i];
			Vector3 worldPosition = new Vector3(data.galaxyPosition.x, 0.0f, data.galaxyPosition.y);
			Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
			screenPosition.z = 0.0f;
			rect.x = screenPosition.x - rect.width / 2.0f;
			rect.y = Screen.height - screenPosition.y - rect.height / 2.0f;

			bool mouseOver = false;
			Vector3 delta = Input.mousePosition - screenPosition;
			float dist = delta.magnitude;
			if (dist < iconSize / 2.0f)
			{
				mouseOver = true;
				if (Input.GetMouseButtonDown(0) == true)
				{
					SelectCampaign(data);
				}
			}

			GUI.color = (mouseOver == true ? Color.red : Color.white);

			GUI.DrawTexture(rect, myCampaignIcon);
		}
	}

	private void SelectCampaign(CampaignData aData)
	{
		for (int i = 0; i < myMissionButtons.Count; ++i)
		{
			Destroy(myMissionButtons[i].gameObject);
		}
		myMissionButtons.Clear();

		for (int i = 0; i < aData.missions.Length; ++i)
		{
			Button button = Instantiate(myMissionButtonPrefab);
			button.transform.SetParent(myMissionList);
			button.GetComponentInChildren<Text>().text = aData.missions[i].missionName;
			string tmp = aData.missions[i].missionName;
			button.onClick.AddListener(() => { StartMission(tmp); });
			myMissionButtons.Add(button);
		}
	}

	private void StartMission(string aMission)
	{
		//GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		//go.name = aMission;
		//DontDestroyOnLoad(go);
		SceneManager.LoadScene("SceneCombat", LoadSceneMode.Single);
	}
}
