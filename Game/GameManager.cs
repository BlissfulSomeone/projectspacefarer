using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public class ShipGroup
	{
		public List<Ship> myGroup = new List<Ship>();
	}

	public enum eGameState
	{
		PlanningBuild,
		PlanningPickTarget,
		Playing,
	}

	private bool myIsGamePaused = false;
	//private bool myIsGameSpeedUp = false;

	private static GameManager myInstance = null;
	public static GameManager Instance { get { return myInstance; } }

	private eGameState myState = eGameState.PlanningBuild;
	public eGameState State { get { return myState; } }

	public AudioMixer myMixer;

	public delegate void GameStartHandler();
	public GameStartHandler OnGameStarted;

	public delegate void SquadSelectedHandler(Ship aShip);
	public SquadSelectedHandler OnShipSelected;

	public delegate void SquadDeselectedHandler();
	public SquadDeselectedHandler OnShipDeselected;

	[SerializeField]
	private Transform myShipDesignList;
	[SerializeField]
	private FleetSizeUI myFleetSizeUI;
	public FleetSizeUI FleetSizeUI { get { return myFleetSizeUI; } }
	[SerializeField]
	private Transform myPauseMenu;
	[SerializeField]
	private Transform mySettingsMenu;
	[SerializeField]
	private Button myPauseContinueButton;
	[SerializeField]
	private Button myPauseSettingsButton;
	[SerializeField]
	private Button myPauseSettingsBackButton;
	[SerializeField]
	private Button myPauseQuitButton;
	[SerializeField]
	private Button myStartGameButton;
	[SerializeField]
	private Button mySpeedUpButton;

	[SerializeField]
	private float[] mySpeedUpFactor = { 1.0f, 3.0f, 5.0f };
	[SerializeField]
	private string[] mySpeedUpLabels = { "►", ">>", ">>>" };

	private int mySpeedUpIndex = 0;

	private int mySelectedTeamIndex;
	private ShipLoader.ShipLoaderData mySelectedShipType;
	private Transform myShipBuilderTransform;
	private MeshFilter myShipBuilderMeshFilter;
	private MeshRenderer myShipBuilderMeshRenderer;
	[SerializeField]
	private Material myShipBuilderMaterial;

	[SerializeField]
	private Ship myShipPrefab;

	//private Ship myHoverShip;
	//private Ship mySelectedShip;

	[SerializeField]
	private ShipController myShipControllerPrefab;
	private ShipController myShipController;
	public ShipController ShipController { get { return myShipController; } }

	[SerializeField]
	private CameraController myCameraController;

	[SerializeField]
	private Explosion[] myExplosionPrefabs;

	[SerializeField]
	private Transform[] myStartingAreas;

	private Vector2 mySelectionBoxStart;
	private bool mySelectionBoxInProgress = false;

	private List<Ship> myShips = new List<Ship>();
	private List<Ship> myShipsUnderMouse = new List<Ship>();
	private List<Ship> myHoverShips = new List<Ship>();
	private List<Ship> mySelectedShips = new List<Ship>();

	public List<Ship> Ships { get { return myShips; } }

	private bool myIsCameraLocked = false;

	private bool myDeselectedThisFrame;
	private int myNumberOfSelectedShipsBeforeClicking;

	private Vector3[] myClickedMousePosition = new Vector3[2];
	private bool ClickCheck(int index)
	{
		Vector3 delta = myClickedMousePosition[index] - Input.mousePosition;
		return (delta.magnitude < 6.0f);
	}

	void Awake()
	{
		if (myInstance != null)
		{
			Destroy(gameObject);
			return;
		}
		myInstance = this;

		SetTeamIndex(0);

		myPauseMenu.gameObject.SetActive(false);
		mySettingsMenu.gameObject.SetActive(false);

		myShipBuilderTransform = new GameObject("Ship Builder").transform;
		myShipBuilderTransform.localScale = Vector3.one * 0.75f;
		myShipBuilderMeshFilter = myShipBuilderTransform.gameObject.AddComponent<MeshFilter>();
		myShipBuilderMeshRenderer = myShipBuilderTransform.gameObject.AddComponent<MeshRenderer>();
		myShipBuilderMeshRenderer.material = myShipBuilderMaterial;

		myPauseContinueButton.onClick.AddListener(() => { TogglePause(); });
		myPauseSettingsButton.onClick.AddListener(() => { ToggleSettings(); });
		myPauseSettingsBackButton.onClick.AddListener(() => { ToggleSettings(); });
		myPauseQuitButton.onClick.AddListener(() => { TogglePause(); UnityEngine.SceneManagement.SceneManager.LoadScene("SceneMainMenu", UnityEngine.SceneManagement.LoadSceneMode.Single); });

		myStartGameButton.onClick.AddListener(() => { if (State == eGameState.PlanningBuild) { StartGame(); myStartGameButton.gameObject.SetActive(false); mySpeedUpButton.gameObject.SetActive(true); } });

		mySpeedUpButton.onClick.AddListener(() => { ToggleSpeedUp(); });
		mySpeedUpButton.gameObject.SetActive(false);

		myCameraController.SetOrigin(new Vector3(0.0f, 0.0f, -400.0f), true);
		myCameraController.SetRotation(new Vector3(45.0f, 0.0f, 0.0f), true);
		myCameraController.SetZoom(1200.0f, true);

		Demo();
	}

	private void Demo()
	{
		for (int ti = 0; ti < 2; ++ti)
		{
			Quaternion rotation = Quaternion.Euler(0.0f, 180.0f * ti, 0.0f);
			Ship ac = null;
			SetShipType("DefaultArmouredCruiser");
			for (int i = 0; i < 3; ++i)
			{
				Ship cruiser = ac = SpawnShip(ti, new Vector3(-16 - 32 * i, 2, 752 * (ti * 2 - 1)), rotation);
				//cruiser.AddOrder(new ShipOrder());
				//cruiser.SetManouverType(0, ti == 0 ? eOrderType.Skirmish : eOrderType.Support);
			}
			SetShipType("DefaultBattlecruiser");
			SpawnShip(ti, new Vector3(16, 2, 752 * (ti * 2 - 1)), rotation);
			SpawnShip(ti, new Vector3(48, 2, 752 * (ti * 2 - 1)), rotation);
			SetShipType("DefaultForward");
			SpawnShip(ti, new Vector3(-80, 2, 784 * (ti * 2 - 1)), rotation);
			SpawnShip(ti, new Vector3(-112, 2, 784 * (ti * 2 - 1)), rotation);
			SpawnShip(ti, new Vector3(-144, 2, 784 * (ti * 2 - 1)), rotation);
			SpawnShip(ti, new Vector3(16, 2, 784 * (ti * 2 - 1)), rotation);
			SpawnShip(ti, new Vector3(48, 2, 784 * (ti * 2 - 1)), rotation);
			SpawnShip(ti, new Vector3(80, 2, 784 * (ti * 2 - 1)), rotation);
			SetShipType("Point");
			SpawnShip(ti, new Vector3(-16, 2, 784 * (ti * 2 - 1)), rotation);
			SpawnShip(ti, new Vector3(-48, 2, 784 * (ti * 2 - 1)), rotation);
			SetShipType("DefaultGun");
			for (int i = 0; i < 16; ++i)
			{
				Ship screen0 = SpawnShip(ti, new Vector3(-144 + 32 * i, 2, 656 * (ti * 2 - 1)), rotation);
				//screen0.AddOrder(new ShipOrder());
				//screen0.SetOrderTargetType(0, eTargetType.Protect);
				//screen0.SetOrderTargetShip(0, ac);
				Ship screen1 = SpawnShip(ti, new Vector3(-144 + 32 * i, 2, 688 * (ti * 2 - 1)), rotation);
				//screen1.AddOrder(new ShipOrder());
				//screen1.SetOrderTargetType(0, eTargetType.Protect);
				//screen1.SetOrderTargetShip(0, ac);
			}
			for (int i = 0; i < 16; ++i)
			{
				/*Ship screen = */SpawnShip(ti, new Vector3(-144 + 32 * i, 2, 816 * (ti * 2 - 1)), rotation);
				//screen.AddOrder(new ShipOrder());
				//screen.SetOrderType(0, eOrderType.Protect_Target);
				//screen.SetOrderTarget(0, ac);
			}
		}
	}

	public void SetShipType(string aFileName)
	{
		mySelectedShipType = ShipLoader.Instance.GetData(aFileName);
		myShipBuilderMeshFilter.mesh = mySelectedShipType.mesh;
	}

	public void SetTeamIndex(int aTeamIndex)
	{
		mySelectedTeamIndex = aTeamIndex;
	}

	private void TogglePause()
	{
		bool settingsOpen = mySettingsMenu.gameObject.activeSelf;
		if (settingsOpen == false)
		{
			myIsGamePaused = !myIsGamePaused;
			Time.timeScale = (myIsGamePaused == false ? mySpeedUpFactor[mySpeedUpIndex] : 0.0f);
			myPauseMenu.gameObject.SetActive(myIsGamePaused);
		}
		else
		{
			ToggleSettings();
		}
		myStartGameButton.gameObject.SetActive(!myIsGamePaused && State != eGameState.Playing);
		mySpeedUpButton.gameObject.SetActive(!myIsGamePaused && State == eGameState.Playing);
	}

	private void ToggleSettings()
	{
		bool settingsOpen = mySettingsMenu.gameObject.activeSelf;
		mySettingsMenu.gameObject.SetActive(!settingsOpen);
		myPauseMenu.gameObject.SetActive(settingsOpen);
	}

	private void ToggleSpeedUp()
	{
		//myIsGameSpeedUp = !myIsGameSpeedUp;
		mySpeedUpIndex = (mySpeedUpIndex + 1) % 3;
		//Time.timeScale = (myIsGameSpeedUp == true ? mySpeedUpFactor : 1.0f);
		Time.timeScale = mySpeedUpFactor[mySpeedUpIndex];
		//mySpeedUpButton.GetComponentInChildren<Text>().text = (myIsGameSpeedUp == true ? ">>" : "►");
		mySpeedUpButton.GetComponentInChildren<Text>().text = mySpeedUpLabels[mySpeedUpIndex];
	}

	void Update()
	{
		for (int i = 0; i < 2; ++i)
		{
			if (Input.GetMouseButtonDown(i) == true)
			{
				myClickedMousePosition[i] = Input.mousePosition;
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape) == true)
		{
			TogglePause();
		}
		if (EventSystem.current.IsPointerOverGameObject() == true) return;
		if (myIsGamePaused == true)
		{
			mySelectionBoxInProgress = false;
			return;
		}
		if (Input.GetMouseButtonDown(0) == true)
		{
			myNumberOfSelectedShipsBeforeClicking = mySelectedShips.Count;
			StartDragging();
			myIsCameraLocked = false;
		}
		if (Input.GetMouseButtonUp(0) == true)
		{
			FinishDragging();
		}
		myDeselectedThisFrame = (mySelectedShips.Count == 0 && myNumberOfSelectedShipsBeforeClicking > 0);
		if (mySelectionBoxInProgress == true)
		{
			//if ((mySelectionBoxStart.ToVector3() - Input.mousePosition).magnitude < 4)
			if (ClickCheck(0) == true)
			{
				HoverShipsInRay();
			}
			else
			{
				HoverShipsInArea();
			}
		}
		else
		{
			GetShipsUnderMouse();
			HoverShipsInRay();
			
			if (State == eGameState.PlanningBuild)
			{
				TryPlaceShip();
				TryRemoveShip();

				if (Input.GetKeyDown(KeyCode.G) == true &&
					Input.GetKey(KeyCode.LeftAlt) == false &&
					Input.GetKey(KeyCode.RightAlt) == false &&
					Input.GetKey(KeyCode.AltGr) == false &&
					mySelectedShips.Count > 1)
				{
					CreateGroup();
				}
			}
			else if (State == eGameState.Playing)
			{
				if (Input.GetKey(KeyCode.F) == true && mySelectedShips.Count > 0)
				{
					myIsCameraLocked = true;
					myCameraController.SetZoom(50.0f, false);
				}
			}
		}
		if (myIsCameraLocked == true)
		{
			Vector3 medianPoint = Vector3.zero;
			for (int i = 0; i < mySelectedShips.Count; ++i)
			{
				medianPoint += mySelectedShips[i].transform.position;
			}
			medianPoint /= mySelectedShips.Count;
			myCameraController.SetOrigin(medianPoint, false);
		}
		//if (Input.GetKeyDown(KeyCode.Space) == true && State == eGameState.PlanningBuild)
		//{
		//	StartGame();
		//}
		//if (Input.GetKeyDown(KeyCode.Escape) == true)
		//{
		//	TogglePause();
		//}
		//if (myIsGamePaused == false)
		//{
		//	//if (myState == eGameState.Planning)
		//	{
		//		if (mySelectedShipType != null)
		//		{
		//			Vector3 cursorPoint = GetCursorPoint();
		//			Vector3 gridPoint = WorldToGridPosition(cursorPoint);
		//			myShipBuilderTransform.position = gridPoint;
		//
		//			if (EventSystem.current.IsPointerOverGameObject() == false)
		//			{
		//				if (mySelectedShip != null && Input.GetMouseButtonDown(0) == true)
		//				{
		//					DeselectShip();
		//				}
		//				if (myHoverShip != null)
		//				{
		//					DehoverShip();
		//				}
		//
		//				Ship[] ships = FindObjectsOfType<Ship>();
		//				for (int i = 0; i < ships.Length; ++i)
		//				{
		//					CheckShipSelection(ships[i], gridPoint);
		//				}
		//
		//				if (State == eGameState.PlanningBuild)
		//				{
		//					int pointInsideStartingArea = IsPointInsideStartingArea(gridPoint);
		//					myShipBuilderMeshRenderer.enabled = (myHoverShip == null && pointInsideStartingArea >= 0);
		//					if (pointInsideStartingArea == 0)
		//					{
		//						myShipBuilderTransform.eulerAngles = Vector3.zero;
		//					}
		//					else if (pointInsideStartingArea == 1)
		//					{
		//						myShipBuilderTransform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
		//					}
		//					if (mySelectedShip == null && Input.GetMouseButtonDown(0) == true && mySelectedShipType != null && pointInsideStartingArea >= 0)
		//					{
		//						SpawnShip(pointInsideStartingArea);
		//					}
		//				}
		//			}
		//		}
		//		if (Input.GetKeyDown(KeyCode.Space) == true && State == eGameState.PlanningBuild)
		//		{
		//			StartGame();
		//		}
		//	}
		//}
	}

	private void StartDragging()
	{
		mySelectionBoxStart = Input.mousePosition;
		mySelectionBoxInProgress = true;
		if (!(Input.GetKey(KeyCode.LeftControl) == true || Input.GetKey(KeyCode.RightControl) == true))
		{
			for (int i = 0; i < mySelectedShips.Count; ++i)
			{
				mySelectedShips[i].SetHoverType("Default");
			}
			mySelectedShips.Clear();
		}
		myShipBuilderMeshRenderer.enabled = false;
	}

	private void FinishDragging()
	{
		mySelectionBoxInProgress = false;
		mySelectedShips.AddRange(myHoverShips);
		myHoverShips.Clear();
		if (mySelectedShips.Count == 1)
		{
			if (mySelectedShips[0].Group != null)
			{
				for (int i = 0; i < mySelectedShips[0].Group.myGroup.Count; ++i)
				{
					if (mySelectedShips[0].Group.myGroup[i] != mySelectedShips[0])
					{
						mySelectedShips.Add(mySelectedShips[0].Group.myGroup[i]);
					}
				}
			}
			else
			{
				if (OnShipSelected != null)
				{
					OnShipSelected(mySelectedShips[0]);
				}
			}
		}
		else
		{
			if (OnShipDeselected != null)
			{
				OnShipDeselected();
			}
		}
		for (int i = 0; i < mySelectedShips.Count; ++i)
		{
			mySelectedShips[i].SetHoverType("Selected");
		}
	}

	private void HoverShipsInRay()
	{
		Camera camera = Camera.main;
		Ray ray = new Ray(new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, 1), Vector3.forward);
		//Ship[] ships = FindObjectsOfType<Ship>();
		for (int i = 0; i < myHoverShips.Count; ++i)
		{
			myHoverShips[i].SetHoverType("Default");
		}
		myHoverShips.Clear();
		for (int i = 0; i < myShips.Count; ++i)
		{
			Bounds viewportBounds = myShips[i].GetViewportBounds(camera);
			if (/*ships[i].TeamIndex == 0 && */viewportBounds.IntersectRay(ray) == true)
			{
				if (mySelectedShips.Contains(myShips[i]) == false)
				{
					myHoverShips.Add(myShips[i]);
					myShips[i].SetHoverType("SelectHover");
					break;
				}
			}
		}
	}

	private void HoverShipsInArea()
	{
		Camera camera = Camera.main;
		Bounds bounds = UIUtilities.GetViewportBounds(camera, mySelectionBoxStart, Input.mousePosition);
		//Ship[] ships = FindObjectsOfType<Ship>();
		for (int i = 0; i < myHoverShips.Count; ++i)
		{
			myHoverShips[i].SetHoverType("Default");
		}
		myHoverShips.Clear();
		for (int i = 0; i < myShips.Count; ++i)
		{
			Bounds viewportBounds = myShips[i].GetViewportBounds(camera);
			if (/*ships[i].TeamIndex == 0 && */bounds.Intersects(viewportBounds) == true)
			{
				myHoverShips.Add(myShips[i]);
			}
		}
		for (int i = 0; i < myHoverShips.Count; ++i)
		{
			myHoverShips[i].SetHoverType("SelectHover");
		}
	}

	private void GetShipsUnderMouse()
	{
		myShipsUnderMouse.Clear();
		Camera camera = Camera.main;
		Ray ray = new Ray(new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, 1), Vector3.forward);
		//Ship[] ships = FindObjectsOfType<Ship>();
		for (int i = 0; i < myShips.Count; ++i)
		{
			Bounds viewportBounds = myShips[i].GetViewportBounds(camera);
			if (viewportBounds.IntersectRay(ray) == true)
			{
				myShipsUnderMouse.Add(myShips[i]);
			}
		}
	}

	private void TryPlaceShip()
	{
		//bool isSelectionBox = ((mySelectionBoxStart.ToVector3() - Input.mousePosition).magnitude > 4);
		bool isSelectionBox = (ClickCheck(0) == false);
		Vector3 cursorPoint = GetCursorPoint();
		int pointInsideStartingArea = IsPointInsideStartingArea(cursorPoint);
		bool shipBuilderAvailable = (myShipsUnderMouse.Count == 0 && pointInsideStartingArea >= 0);
		myShipBuilderMeshRenderer.enabled = shipBuilderAvailable;
		if (shipBuilderAvailable == true)
		{
			myShipBuilderTransform.position = cursorPoint;
			if (pointInsideStartingArea == 0)
			{
				myShipBuilderTransform.eulerAngles = Vector3.zero;
			}
			else if (pointInsideStartingArea == 1)
			{
				myShipBuilderTransform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
			}
			if (Input.GetMouseButtonUp(0) == true && mySelectedShipType != null && pointInsideStartingArea >= 0 && isSelectionBox == false && myDeselectedThisFrame == false)
			{
				SpawnShip(pointInsideStartingArea);
			}
		}
	}
	
	private void TryRemoveShip()
	{
		if (Input.GetKeyDown(KeyCode.Delete) == true)
		{
			if (mySelectedShips.Count > 0)
			{
				for (int i = 0; i < mySelectedShips.Count; ++i)
				{
					myShips.Remove(mySelectedShips[i]);
					myFleetSizeUI.RemoveShip(mySelectedShips[i]);
					Destroy(mySelectedShips[i].gameObject);
				}
				mySelectedShips.Clear();
			}
		}
	}

	private void CreateGroup()
	{
		ShipGroup shipGroup = new ShipGroup();
		for (int i = 0; i < mySelectedShips.Count; ++i)
		{
			if (mySelectedShips[i].Group != null)
			{
				mySelectedShips[i].Group.myGroup.Remove(mySelectedShips[i]);
			}
			mySelectedShips[i].Group = shipGroup;
			shipGroup.myGroup.Add(mySelectedShips[i]);
		}
	}
	
	private Vector3 GetCursorPoint()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Plane plane = new Plane(Vector3.up, Vector3.zero);
		float distanceToHit;
		plane.Raycast(ray, out distanceToHit);
		return ray.GetPoint(distanceToHit);
	}

	private Vector3 WorldToGridPosition(Vector3 aWordPoint)
	{
		Vector3 result = aWordPoint;
		result.x = Mathf.Floor(result.x / 32.0f) * 32.0f + 16.0f;
		result.z = Mathf.Floor(result.z / 32.0f) * 32.0f + 16.0f;
		result.y = 2.0f;
		return result;
	}

	//private void DeselectShip()
	//{
	//	if (State == eGameState.PlanningBuild)
	//	{
	//		mySelectedShip.gameObject.layer = LayerMask.NameToLayer("Default");
	//		mySelectedShip.SetHoverType("Default");
	//		mySelectedShip = null;
	//		if (OnShipDeselected != null)
	//		{
	//			OnShipDeselected();
	//		}
	//	}
	//}
	//
	//private void DehoverShip()
	//{
	//	if (myHoverShip != mySelectedShip)
	//	{
	//		myHoverShip.gameObject.layer = LayerMask.NameToLayer("Default");
	//		myHoverShip.SetHoverType("Default");
	//	}
	//	myHoverShip = null;
	//}
	//
	//private void CheckShipSelection(Ship aShip, Vector3 aGridPoint)
	//{
	//	Vector3 position = aShip.transform.position;
	//	Vector3 delta = aGridPoint - position;
	//	float distanceToMouse = delta.magnitude;
	//	if (distanceToMouse < 1.0f)
	//	{
	//		myHoverShip = aShip;
	//		if (myHoverShip != mySelectedShip)
	//		{
	//			myHoverShip.gameObject.layer = LayerMask.NameToLayer("SelectHover");
	//			myHoverShip.SetHoverType("SelectHover");
	//		}
	//		if (Input.GetMouseButtonDown(0) == true)
	//		{
	//			if (State == eGameState.PlanningBuild && mySelectedShip == null)
	//			{
	//				mySelectedShip = aShip;
	//				mySelectedShip.gameObject.layer = LayerMask.NameToLayer("Selected");
	//				mySelectedShip.SetHoverType("Selected");
	//			}
	//			if (OnShipSelected != null)
	//			{
	//				OnShipSelected(aShip);
	//			}
	//		}
	//	}
	//}

	private int IsPointInsideStartingArea(Vector3 aGridPosition)
	{
		for (int i = 0; i < myStartingAreas.Length; ++i)
		{
			float width = myStartingAreas[i].localScale.x / 2;
			float depth = myStartingAreas[i].localScale.y / 2;
			float z = myStartingAreas[i].localPosition.z;
			if (aGridPosition.x >= -width && aGridPosition.x <= width && aGridPosition.z >= z - depth && aGridPosition.z <= z + depth)
			{
				return i;
			}
		}
		return -1;
	}

	private Ship SpawnShip(int aTeamIndex)
	{
		//Ship ship = Instantiate(myShipPrefab);
		//ship.Init(mySelectedShipType, aTeamIndex);
		//ship.transform.position = myShipBuilderTransform.position;
		//ship.transform.rotation = myShipBuilderTransform.rotation;
		//ship.SetHoverType("Default");
		//
		//ship.OnShipDestroyed += OnShipDestroyed;
		//
		//myFleetSizeUI.AddShip(ship);
		//
		//return ship;
		return SpawnShip(aTeamIndex, myShipBuilderTransform.position, myShipBuilderTransform.rotation);
	}

	private Ship SpawnShip(int aTeamIndex, Vector3 aPosition, Quaternion aRotation)
	{
		Ship ship = Instantiate(myShipPrefab);
		ship.Init(mySelectedShipType, aTeamIndex);
		ship.transform.position = aPosition;
		ship.transform.rotation = aRotation;
		ship.SetHoverType("Default");

		ship.OnShipDestroyed += OnShipDestroyed;

		myShips.Add(ship);

		myFleetSizeUI.AddShip(ship);

		return ship;
	}

	private void OnShipDestroyed(Ship aShip)
	{
		myShips.Remove(aShip);
		if (myHoverShips.Contains(aShip) == true)
		{
			myHoverShips.Remove(aShip);
		}
		if (mySelectedShips.Contains(aShip) == true)
		{
			mySelectedShips.Remove(aShip);
			if (myIsCameraLocked == true && mySelectedShips.Count == 0)
			{
				myIsCameraLocked = false;
			}
		}
		aShip.OnShipDestroyed -= OnShipDestroyed;
	}

	private void StartGame()
	{
		//if (mySelectedShip != null)
		//{
		//	DeselectShip();
		//}
		//if (myHoverShip != null)
		//{
		//	DehoverShip();
		//}
		RandomizeEnemyOrders();
		myShipBuilderTransform.gameObject.SetActive(false);
		myState = eGameState.Playing;
		myShipController = Instantiate(myShipControllerPrefab);
		myShipController.Init();
		myShipDesignList.gameObject.SetActive(false);
		myStartingAreas[0].gameObject.SetActive(false);
		myStartingAreas[1].gameObject.SetActive(false);
	}

	private void RandomizeEnemyOrders()
	{
		Ship[] ships = FindObjectsOfType<Ship>();
		for (int i = 0; i < myShips.Count; ++i)
		{
			if (myShips[i].GetNumberOfOrders() == 0)
			{
				eManouverType manouverType = (eManouverType)(Random.Range(0, 3));
				eTargetType targetType = (eTargetType)(Random.Range(0, 4));
				myShips[i].AddOrder(new ShipOrder(manouverType, targetType, ShipData.eShipModel.PatrolBoat, null));
				//eOrderType randomOrderType = eOrderType.None;
				//while (randomOrderType == eOrderType.None || randomOrderType == eOrderType.COUNT_DONT_USE)
				//{
				//	randomOrderType = (eOrderType)Random.Range(0, (int)eOrderType.COUNT_DONT_USE);
				//}
				//myShips[i].AddOrder(new ShipOrder(randomOrderType, ShipData.eShipModel.PatrolBoat, null));
			}
		}
	}

	public void SetState(eGameState aState)
	{
		myState = aState;
	}

	public Explosion GetExplosionPrefabByTeamIndex(int aTeamIndex)
	{
		return myExplosionPrefabs[aTeamIndex];
	}

	void OnGUI()
	{
		if (mySelectionBoxInProgress == true)
		{
			Vector2 point0 = mySelectionBoxStart;
			Vector2 point1 = Input.mousePosition;

			point0.y = Screen.height - point0.y;
			point1.y = Screen.height - point1.y;

			Vector2 topLeft = Vector2.Min(point0, point1);
			Vector2 bottomRight = Vector2.Max(point0, point1);

			Rect rect = Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);

			UIUtilities.DrawRect(rect, new Color(0.5f, 0.65f, 1.0f, 0.1f));
			UIUtilities.DrawRectBorder(rect, 2.0f, new Color(0.5f, 0.65f, 1.0f, 0.8f));
		}

		//Ship[] ships = FindObjectsOfType<Ship>();
		//Camera cam = Camera.main;
		//Color c = new Color(1, 1, 1, 0.5f);
		//for (int i = 0; i < ships.Length; ++i)
		//{
		//	UIUtilities.DrawRect(ships[i].GetScreenRect(cam), c);
		//}
	}
}
