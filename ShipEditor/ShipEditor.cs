using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShipEditor : MonoBehaviour
{
	public enum eShipEditorMode
	{
		Build = 0,
		Move = 1,
		Hardpoints = 2,
		Softpoints = 3,
		Accessories = 4,
	}

	private static ShipEditor myInstance = null;
	public static ShipEditor Instance { get { return myInstance; } }

	[Header("UI")]
	[SerializeField]
	public ShipEditorUI myUI;
	[SerializeField]
	private ShipEditorUIToolbar myToolbar;
	[SerializeField]
	private ShipEditorUIToolbarModule myToolbarModule;
	[SerializeField]
	private ShipEditorUIToolbarTransform myToolbarTransform;
	[SerializeField]
	private ShipEditorUIToolbarHardpoints myToolbarHardpoints;
	[SerializeField]
	private ShipEditorUIToolbarSoftpoints myToolbarSoftpoints;
	[SerializeField]
	private ShipEditorUIToolbarAccessories myToolbarAccessories;
	[SerializeField]
	private ShipEditorUISoftpointSlots mySoftpointSlots;
	[SerializeField]
	private ShipEditorUIStats myStats;

	[Header("Module")]
	[SerializeField]
	private Transform myModulePrefab;
	[SerializeField]
	private LayerMask myModuleLayerMask;
	[SerializeField]
	private string myModuleDefaultLayer;
	[SerializeField]
	private string myModuleHoverLayer;
	[SerializeField]
	private string myModuleSelectedLayer;
	[SerializeField]
	private Material myGhostModuleMaterial;
	[SerializeField]
	private string myGhostModuleLayer;

	[Header("Turret")]
	[SerializeField]
	private LayerMask myTurretLayerMask;

	[Header("Accessory")]
	[SerializeField]
	private LayerMask myAccessoryLayerMask;
	
	private eShipEditorMode myMode = eShipEditorMode.Build;
	
	private Vector3[] myClickedMousePosition = new Vector3[2];
	//private bool ClickCheck { get { Vector3 delta = myClickedMousePosition[] - Input.mousePosition; return (delta.magnitude < 2.0f); } }
	private bool ClickCheck(int index)
	{
		Vector3 delta = myClickedMousePosition[index] - Input.mousePosition;
		return (delta.magnitude < 2.0f);
	}

	private ShipData myShipData;

	private Transform myGhostModule;
	private MeshFilter myGhostModuleMesh;
	private MeshCollider myGhostCollider;
	private Rigidbody myGhostBody;
	private Transform myMirrorGhostModule;
	private MeshFilter myMirrorGhostModuleMesh;

	private ModuleData mySelectedModuleData;
	private WeaponData mySelectedWeaponData;
	private SoftpointData mySelectedSoftpointData;
	private PropData mySelectedPropData;
	private Vector3 myModuleScale = Vector3.one;
	private int mySelectedWeaponSize;
	private Vector3 myAccessoryScale = Vector3.one;

	private Transform myHoverModule;
	private Transform mySelectedModule;

	private Turret myCurrentTurret;

	private Dictionary<Transform, Transform> myMirrorModules = new Dictionary<Transform, Transform>();
	private Dictionary<Transform, string> myModuleNames = new Dictionary<Transform, string>();
	private Dictionary<Transform, string> myPropNames = new Dictionary<Transform, string>();
	
	void Awake()
	{
		if (myInstance != null)
		{
			Destroy(gameObject);
			return;
		}
		myInstance = this;

		//myShipData = new ShipData();
		//myShipData.Init(ContentLoader.Instance.ShipData[0]);

		myGhostModule = Instantiate(myModulePrefab);
		myGhostModule.gameObject.name = "Ghost Module";
		myGhostModule.gameObject.layer = LayerMask.NameToLayer(myGhostModuleLayer);
		myGhostModuleMesh = myGhostModule.GetComponentInChildren<MeshFilter>();
		MeshRenderer ghostModuleRenderer = myGhostModule.GetComponentInChildren<MeshRenderer>();
		ghostModuleRenderer.material = myGhostModuleMaterial;
		ghostModuleRenderer.gameObject.layer = LayerMask.NameToLayer(myGhostModuleLayer);
		myGhostBody = myGhostModule.gameObject.AddComponent<Rigidbody>();
		myGhostBody.useGravity = false;
		myGhostBody.isKinematic = true;
		myGhostCollider = myGhostModule.GetComponentInChildren<MeshCollider>();

		myMirrorGhostModule = Instantiate(myModulePrefab);
		myMirrorGhostModule.gameObject.name = "Ghost Module (Mirror)";
		myMirrorGhostModule.gameObject.layer = LayerMask.NameToLayer(myGhostModuleLayer);
		myMirrorGhostModuleMesh = myMirrorGhostModule.GetComponentInChildren<MeshFilter>();
		MeshRenderer mirrorGhostModuleRenderer = myMirrorGhostModule.GetComponentInChildren<MeshRenderer>();
		mirrorGhostModuleRenderer.material = myGhostModuleMaterial;
		mirrorGhostModuleRenderer.gameObject.layer = LayerMask.NameToLayer(myGhostModuleLayer);
		myMirrorGhostModule.GetComponentInChildren<MeshCollider>().enabled = false;
		myMirrorGhostModule.localScale = MirrorScale(myGhostModule.localScale);

		Miss();
		
		//BuildModule();
		//BuildModule(Vector3.forward * 4.0f);
		TransformTool.Instance.SetMode(TransformTool.eTransformMode.Position);
	}

	void OnEnable()
	{
		myToolbar.OnToolbarChanged += ToolbarChanged;
		myToolbarModule.OnModuleScaleChanged += ModuleScaleChanged;
		myToolbarModule.OnModuleChanged += SelectedModuleChanged;
		myToolbarTransform.OnTransformModeChanged += TransformModeChanged;
		myToolbarHardpoints.OnWeaponSizeChanged += SelectedWeaponSizeChanged;
		myToolbarHardpoints.OnWeaponChanged += SelectedWeaponChanged;
		myToolbarSoftpoints.OnSoftpointChanged += SelectedSoftpointChanged;
		myToolbarAccessories.OnAccessoryScaleChanged += AccessoryScaleChanged;
		myToolbarAccessories.OnAccessoryChanged += SelectedAccessoryChanged;
		TransformTool.Instance.OnMoved += TransformToolMoved;
		TransformTool.Instance.OnRotated += TransformToolRotated;
	}

	void OnDisable()
	{
		myToolbar.OnToolbarChanged -= ToolbarChanged;
		myToolbarModule.OnModuleScaleChanged -= ModuleScaleChanged;
		myToolbarModule.OnModuleChanged -= SelectedModuleChanged;
		myToolbarTransform.OnTransformModeChanged -= TransformModeChanged;
		myToolbarHardpoints.OnWeaponSizeChanged -= SelectedWeaponSizeChanged;
		myToolbarHardpoints.OnWeaponChanged -= SelectedWeaponChanged;
		myToolbarSoftpoints.OnSoftpointChanged -= SelectedSoftpointChanged;
		myToolbarAccessories.OnAccessoryScaleChanged -= AccessoryScaleChanged;
		myToolbarAccessories.OnAccessoryChanged -= SelectedAccessoryChanged;
		TransformTool.Instance.OnMoved -= TransformToolMoved;
		TransformTool.Instance.OnRotated -= TransformToolRotated;
	}

	private void ToolbarChanged(eShipEditorMode aMode)
	{
		if (aMode == eShipEditorMode.Build)
		{
			SelectedModuleChanged(mySelectedModuleData);
			myGhostModule.rotation = Quaternion.identity;
			myGhostModule.localScale = myModuleScale;
			myMirrorGhostModule.rotation = Quaternion.identity;
			myMirrorGhostModule.localScale = MirrorScale(myModuleScale);
		}
		else if (aMode == eShipEditorMode.Hardpoints)
		{
			SelectedWeaponChanged(mySelectedWeaponData);
			myGhostModule.localScale = Vector3.one * GetHardpointScale(mySelectedWeaponSize);
			myMirrorGhostModule.localScale = Vector3.one * GetHardpointScale(mySelectedWeaponSize);
		}
		else if (aMode == eShipEditorMode.Accessories)
		{
			SelectedAccessoryChanged(mySelectedPropData);
			myGhostModule.rotation = Quaternion.identity;
			myGhostModule.localScale = myAccessoryScale;
			myMirrorGhostModule.rotation = Quaternion.identity;
			myMirrorGhostModule.localScale = myAccessoryScale;
		}
		if (aMode != eShipEditorMode.Move)
		{
			if (mySelectedModule != null)
			{
				DeselectModule();
			}
			if (myHoverModule != null)
			{
				DehoverModule();
			}
		}
		myMode = aMode;
	}

	private void ModuleScaleChanged(float aScale, int aAxis)
	{
		myModuleScale[aAxis] = aScale;
		myGhostModule.localScale = myModuleScale;
		myMirrorGhostModule.localScale = MirrorScale(myModuleScale);
	}

	private void SelectedModuleChanged(ModuleData aModuleData)
	{
		myGhostModuleMesh.mesh = aModuleData.modulePrefab.GetComponentInChildren<MeshFilter>().sharedMesh;
		myGhostCollider.sharedMesh = myGhostModuleMesh.mesh;
		myMirrorGhostModuleMesh.mesh = aModuleData.modulePrefab.GetComponentInChildren<MeshFilter>().sharedMesh;
		mySelectedModuleData = aModuleData;

		Debug.Log(myGhostModuleMesh.sharedMesh.name);
		Debug.Log(myGhostCollider.sharedMesh.name);
	}

	private void TransformModeChanged(TransformTool.eTransformMode aMode)
	{
		TransformTool.Instance.SetMode(aMode);
	}

	private void SelectedWeaponSizeChanged(int aSizeIndex)
	{
		mySelectedWeaponSize = aSizeIndex;
		myGhostModule.localScale = Vector3.one * GetHardpointScale(aSizeIndex);
		myMirrorGhostModule.localScale = Vector3.one * GetHardpointScale(aSizeIndex);
	}

	private void SelectedWeaponChanged(WeaponData aWeaponData)
	{
		myGhostModuleMesh.mesh = aWeaponData.mesh;
		myGhostCollider.sharedMesh = aWeaponData.mesh;
		myMirrorGhostModuleMesh.mesh = aWeaponData.mesh;
		mySelectedWeaponData = aWeaponData;
	}

	private void SelectedSoftpointChanged(SoftpointData aSoftpointData)
	{
		mySelectedSoftpointData = aSoftpointData;
	}

	private void AccessoryScaleChanged(float aScale)
	{
		myAccessoryScale = Vector3.one * aScale;
		myGhostModule.localScale = myAccessoryScale;
		myMirrorGhostModule.localScale = myAccessoryScale;
	}

	private void SelectedAccessoryChanged(PropData aPropData)
	{
		myGhostModuleMesh.mesh = aPropData.propPrefab.GetComponentInChildren<MeshFilter>().sharedMesh;
		myGhostCollider.sharedMesh = myGhostModuleMesh.sharedMesh;
		myMirrorGhostModuleMesh.mesh = myGhostModuleMesh.sharedMesh;
		mySelectedPropData = aPropData;
	}

	private void TransformToolMoved(Vector3 aPosition)
	{
		mySelectedModule.position = aPosition;
		if (myMirrorModules.ContainsKey(mySelectedModule) == true)
		{
			myMirrorModules[mySelectedModule].position = MirrorPosition(mySelectedModule.position);
		}
	}

	private void TransformToolRotated(Quaternion aRotation)
	{
		mySelectedModule.rotation = aRotation;
		if (myMirrorModules.ContainsKey(mySelectedModule) == true)
		{
			Vector3 mirrorEuler = aRotation.eulerAngles;
			mirrorEuler.y *= -1.0f;
			mirrorEuler.x *= -1.0f;
			myMirrorModules[mySelectedModule].rotation = Quaternion.Euler(mirrorEuler);
		}
	}

	public void StartEditor(ShipBaseData aData)
	{
		myShipData = new ShipData();
		myShipData.Init(aData);
		//BuildModule();
		mySoftpointSlots.Init(aData.maxSoftpoints);
		myStats.UpdateStats(myShipData);
	}

	public void BuildModule()
	{
		Transform module = Instantiate(myModulePrefab);
		module.position = myGhostModule.transform.position;
		module.rotation = Quaternion.identity;
		module.localScale = myGhostModule.localScale;

		MeshFilter moduleMesh = module.GetComponentInChildren<MeshFilter>();
		moduleMesh.mesh = myGhostModuleMesh.sharedMesh;

		MeshCollider moduleCollider = module.GetComponentInChildren<MeshCollider>();
		moduleCollider.sharedMesh = myGhostModuleMesh.sharedMesh;

		myShipData.blocks.Add(module);
		myModuleNames.Add(module, mySelectedModuleData.moduleName);

		if (ShipEditorUIToolbarMirror.IsMirrorOn == true)
		{
			Transform mirrorModule = Instantiate(myModulePrefab);
			mirrorModule.position = MirrorPosition(myGhostModule.transform.position);
			mirrorModule.rotation = Quaternion.identity;
			mirrorModule.localScale = MirrorScale(myGhostModule.localScale);

			MeshFilter mirrorMesh = mirrorModule.GetComponentInChildren<MeshFilter>();
			mirrorMesh.sharedMesh = myGhostModuleMesh.sharedMesh;

			MeshCollider mirrorCollider = mirrorModule.GetComponentInChildren<MeshCollider>();
			mirrorCollider.sharedMesh = myGhostModuleMesh.sharedMesh;

			myShipData.blocks.Add(mirrorModule);
			myModuleNames.Add(mirrorModule, mySelectedModuleData.moduleName);
			myMirrorModules.Add(module, mirrorModule);
			myMirrorModules.Add(mirrorModule, module);
		}
	}

	private void BuildModule(Vector3 aPosition)
	{
		Transform module = Instantiate(myModulePrefab);
		module.position = aPosition;
		module.rotation = Quaternion.identity;
		module.localScale = Vector3.one;
		myShipData.blocks.Add(module);
		myModuleNames.Add(module, mySelectedModuleData.moduleName);
	}

	private void BuildTurret()
	{
		Turret turret = Instantiate(mySelectedWeaponData.turretPrefab);
		turret.transform.position = myGhostModule.position;
		turret.transform.rotation = myGhostModule.rotation;
		turret.transform.localScale = myGhostModule.localScale;
		turret.Init(mySelectedWeaponData, mySelectedWeaponSize);
		turret.ShowRangeDisplay(true);

		turret.GetComponentInChildren<MeshFilter>().mesh = myGhostModuleMesh.sharedMesh;
		turret.GetComponentInChildren<MeshCollider>().sharedMesh = myGhostModuleMesh.sharedMesh;
		myCurrentTurret = turret;

		if (ShipEditorUIToolbarMirror.IsMirrorOn == true)
		{
			Turret mirrorTurret = Instantiate(mySelectedWeaponData.turretPrefab);
			mirrorTurret.transform.position = myMirrorGhostModule.position;
			mirrorTurret.transform.rotation = myMirrorGhostModule.rotation;
			mirrorTurret.transform.localScale = myMirrorGhostModule.localScale;
			mirrorTurret.Init(mySelectedWeaponData, mySelectedWeaponSize);
			mirrorTurret.ShowRangeDisplay(true);

			mirrorTurret.GetComponentInChildren<MeshFilter>().mesh = myMirrorGhostModuleMesh.sharedMesh;
			mirrorTurret.GetComponentInChildren<MeshCollider>().sharedMesh = myMirrorGhostModuleMesh.sharedMesh;

			myMirrorModules.Add(turret.transform, mirrorTurret.transform);
			myMirrorModules.Add(mirrorTurret.transform, turret.transform);
		}
	}

	private void BuildAccessory()
	{
		Transform prop = Instantiate(mySelectedPropData.propPrefab);
		prop.transform.position = myGhostModule.position;
		prop.transform.localScale = myGhostModule.localScale;
		myShipData.props.Add(prop);
		myPropNames.Add(prop, mySelectedPropData.propName);
		TrailRenderer trail = prop.GetComponentInChildren<TrailRenderer>();
		if (trail != null)
		{
			trail.gameObject.SetActive(false);
		}
		if (ShipEditorUIToolbarMirror.IsMirrorOn == true)
		{
			Transform mirrorProp = Instantiate(mySelectedPropData.propPrefab);
			mirrorProp.position = myMirrorGhostModule.position;
			mirrorProp.transform.localScale = myMirrorGhostModule.localScale;
			myShipData.props.Add(mirrorProp);
			myPropNames.Add(mirrorProp, mySelectedPropData.propName);
			TrailRenderer mirrorTrail = mirrorProp.GetComponentInChildren<TrailRenderer>();
			if (mirrorTrail != null)
			{
				mirrorTrail.gameObject.SetActive(false);
			}

			myMirrorModules.Add(prop, mirrorProp);
			myMirrorModules.Add(mirrorProp, prop);
		}
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
			myUI.SetMode(ShipEditorUI.eShipEditorUIMode.Pause);
		}

		if (myUI.Mode == ShipEditorUI.eShipEditorUIMode.Editor)
		{
			if (EventSystem.current.IsPointerOverGameObject() == false)
			{
				if (myCurrentTurret == null)
				{
					if (myMode == eShipEditorMode.Move || myMode == eShipEditorMode.Hardpoints || myMode == eShipEditorMode.Accessories)
					{
						if (mySelectedModule != null && Input.GetMouseButtonDown(0) == true && TransformTool.Instance.IsMouseOver == false)
						{
							DeselectModule();
						}
						if (myHoverModule != null)
						{
							DehoverModule();
						}
					}
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hitInfo;
					bool doNormalRaycast = true;
					if (myMode == eShipEditorMode.Hardpoints)
					{
						if (Physics.Raycast(ray, out hitInfo, 256.0f, myTurretLayerMask) == true)
						{
							Turret turret = hitInfo.collider.transform.GetComponentInParent<Turret>();
							myHoverModule = turret.transform;
							turret.gameObject.SetLayer(LayerMask.NameToLayer(myModuleHoverLayer));
							turret.gameObject.layer = LayerMask.NameToLayer(myModuleDefaultLayer);
							if (myMirrorModules.ContainsKey(turret.transform) == true)
							{
								myMirrorModules[turret.transform].gameObject.SetLayer(LayerMask.NameToLayer(myModuleHoverLayer));
								myMirrorModules[turret.transform].gameObject.layer = LayerMask.NameToLayer(myModuleDefaultLayer);
							}
							doNormalRaycast = false;
							Miss();
							if (Input.GetMouseButtonUp(1) == true && ClickCheck(1) == true)
							{
								myShipData.RemoveHardpoint(turret);

								if (myMirrorModules.ContainsKey(turret.transform) == true)
								{
									Turret mirrorTurret = myMirrorModules[turret.transform].GetComponentInParent<Turret>();
									myShipData.RemoveHardpoint(mirrorTurret);

									myMirrorModules.Remove(myMirrorModules[turret.transform]);
									myMirrorModules.Remove(myHoverModule);

									Destroy(mirrorTurret.gameObject);

								}
								Destroy(turret.gameObject);
								myStats.UpdateStats(myShipData);
							}
						}
					}
					else if (myMode == eShipEditorMode.Accessories)
					{
						if (Physics.Raycast(ray, out hitInfo, 256.0f, myAccessoryLayerMask) == true)
						{
							Thruster accessory = hitInfo.collider.transform.GetComponentInParent<Thruster>();
							myHoverModule = accessory.transform;
							accessory.gameObject.SetLayer(LayerMask.NameToLayer(myModuleHoverLayer));
							accessory.gameObject.layer = LayerMask.NameToLayer(myModuleDefaultLayer);
							if (myMirrorModules.ContainsKey(accessory.transform) == true)
							{
								myMirrorModules[accessory.transform].gameObject.SetLayer(LayerMask.NameToLayer(myModuleHoverLayer));
								myMirrorModules[accessory.transform].gameObject.layer = LayerMask.NameToLayer(myModuleDefaultLayer);
							}
							doNormalRaycast = false;
							Miss();
							if (Input.GetMouseButtonUp(1) == true && ClickCheck(1) == true)
							{
								myShipData.props.Remove(accessory.transform);
								if (myMirrorModules.ContainsKey(accessory.transform) == true)
								{
									Transform mirrorAccessory = myMirrorModules[accessory.transform];
									myShipData.props.Remove(mirrorAccessory);
									myMirrorModules.Remove(myMirrorModules[accessory.transform]);
									myMirrorModules.Remove(myHoverModule);
									Destroy(mirrorAccessory.gameObject);
								}
								Destroy(accessory.gameObject);
							}
						}
					}
					if (doNormalRaycast == true)
					{
						if (Physics.Raycast(ray, out hitInfo, 256.0f, myModuleLayerMask | myAccessoryLayerMask) == true)
						{
							Hit(hitInfo);
						}
						else
						{
							Miss();
						}
					}
				}
				else
				{
					RotateTurret();
				}
			}
		}
	}

	private void DeselectModule()
	{
		string layerName = myModuleDefaultLayer;
		if (myMode == eShipEditorMode.Hardpoints)
		{
			layerName = "Turret";
		}
		else if (myMode == eShipEditorMode.Accessories)
		{
			layerName = "Accessory";
		}
		else if (mySelectedModule.GetComponent<Thruster>() != null)
		{
			layerName = "Accessory";
		}
		mySelectedModule.GetComponentInChildren<MeshRenderer>().gameObject.layer = LayerMask.NameToLayer(layerName);
		if (myMirrorModules.ContainsKey(mySelectedModule) == true)
		{
			myMirrorModules[mySelectedModule].GetComponentInChildren<MeshRenderer>().gameObject.layer = LayerMask.NameToLayer(layerName);
		}
		mySelectedModule = null;
		TransformTool.Instance.Show(false);
	}

	private void DehoverModule()
	{
		if (myHoverModule != mySelectedModule)
		{
			string layerName = myModuleDefaultLayer;
			if (myMode == eShipEditorMode.Hardpoints)
			{
				layerName = "Turret";
			}
			else if (myMode == eShipEditorMode.Accessories)
			{
				layerName = "Accessory";
			}
			else if (myHoverModule.GetComponent<Thruster>() != null)
			{
				layerName = "Accessory";
			}
			myHoverModule.GetComponentInChildren<MeshRenderer>().gameObject.layer = LayerMask.NameToLayer(layerName);
			if (myMirrorModules.ContainsKey(myHoverModule) == true)
			{
				myMirrorModules[myHoverModule].GetComponentInChildren<MeshRenderer>().gameObject.layer = LayerMask.NameToLayer(layerName);
			}
			myHoverModule = null;
		}
	}

	private void Hit(RaycastHit aHitInfo)
	{
		if (myMode == eShipEditorMode.Build)
		{
			myGhostModule.gameObject.SetActive(true);
			myMirrorGhostModule.gameObject.SetActive(ShipEditorUIToolbarMirror.IsMirrorOn);
			aHitInfo.collider.gameObject.layer = LayerMask.NameToLayer(myGhostModuleLayer);
			myGhostModule.position = aHitInfo.point + aHitInfo.normal * 20.0f;
			Vector3 n = -aHitInfo.normal;
			RaycastHit rigidBodyHitInfo;
			if (myGhostBody.SweepTest(n, out rigidBodyHitInfo, 30.0f) == true)
			{
				myGhostModule.position += n * rigidBodyHitInfo.distance;
			}
			if (Input.GetKey(KeyCode.LeftControl) == true || Input.GetKey(KeyCode.RightControl) == true)
			{
				Vector3 pos = myGhostModule.position;
				pos.x = Mathf.Round(pos.x * TransformTool.SNAP_FACTOR) / TransformTool.SNAP_FACTOR;
				pos.y = Mathf.Round(pos.y * TransformTool.SNAP_FACTOR) / TransformTool.SNAP_FACTOR;
				pos.z = Mathf.Round(pos.z * TransformTool.SNAP_FACTOR) / TransformTool.SNAP_FACTOR;
				myGhostModule.position = pos;
			}
			if (ShipEditorUIToolbarMirror.IsMirrorOn == true)
			{
				myMirrorGhostModule.position = MirrorPosition(myGhostModule.position);
			}
			if (aHitInfo.collider.gameObject.GetComponentInParent<Thruster>() != null)
			{
				//aHitInfo.collider.transform.parent.gameObject.SetLayer(LayerMask.NameToLayer("Accessory"));
				aHitInfo.collider.gameObject.SetLayer(LayerMask.NameToLayer("Accessory"));
			}
			else
			{
				aHitInfo.collider.gameObject.layer = LayerMask.NameToLayer(myModuleDefaultLayer);
			}

			if (Input.GetMouseButtonDown(0) == true)
			{
				BuildModule();
			}
		}
		else if (myMode == eShipEditorMode.Move)
		{
			myHoverModule = aHitInfo.collider.transform.parent;
			if (myHoverModule != mySelectedModule && (myMirrorModules.ContainsKey(myHoverModule) == false || (myMirrorModules.ContainsKey(myHoverModule) == true && myMirrorModules[myHoverModule] != mySelectedModule)))
			{
				myHoverModule.GetComponentInChildren<MeshRenderer>().gameObject.layer = LayerMask.NameToLayer(myModuleHoverLayer);
				if (myMirrorModules.ContainsKey(myHoverModule) == true)
				{
					myMirrorModules[myHoverModule].GetComponentInChildren<MeshRenderer>().gameObject.layer = LayerMask.NameToLayer(myModuleHoverLayer);
				}
			}
			if (Input.GetMouseButtonDown(0) == true && TransformTool.Instance.IsMouseOver == false)
			{
				if (mySelectedModule == null)
				{
					mySelectedModule = aHitInfo.collider.transform.parent;
					mySelectedModule.GetComponentInChildren<MeshRenderer>().gameObject.layer = LayerMask.NameToLayer(myModuleSelectedLayer);
					if (myMirrorModules.ContainsKey(mySelectedModule) == true)
					{
						myMirrorModules[mySelectedModule].GetComponentInChildren<MeshRenderer>().gameObject.layer = LayerMask.NameToLayer(myModuleSelectedLayer);
					}
					TransformTool.Instance.Show(true);
					TransformTool.Instance.transform.position = mySelectedModule.position;
					TransformTool.Instance.SetRadiRotation(mySelectedModule.rotation);
				}
			}
		}
		else if (myMode == eShipEditorMode.Hardpoints)
		{
			Vector3 lookNormal = aHitInfo.normal;
			lookNormal.y = 0.0f;
			if (lookNormal == Vector3.zero) lookNormal = Vector3.right;
			else lookNormal.Normalize();

			myGhostModule.gameObject.SetActive(true);
			myGhostModule.position = aHitInfo.point + aHitInfo.normal * 0.05f;
			myGhostModule.rotation = Quaternion.LookRotation(lookNormal, Vector3.up);

			myMirrorGhostModule.gameObject.SetActive(ShipEditorUIToolbarMirror.IsMirrorOn);
			if (ShipEditorUIToolbarMirror.IsMirrorOn == true)
			{
				myMirrorGhostModule.position = MirrorPosition(myGhostModule.position);
				Vector3 mirrorNormal = lookNormal;
				mirrorNormal.z *= -1;
				myMirrorGhostModule.rotation = Quaternion.LookRotation(mirrorNormal, Vector3.up);
			}
			if (Input.GetMouseButtonDown(0) == true && myShipData.CanAffordHardpoints(ShipEditorUIToolbarMirror.IsMirrorOn ? 2 : 1, mySelectedWeaponSize) == true)
			{
				BuildTurret();
			}
		}
		else if (myMode == eShipEditorMode.Accessories)
		{
			myGhostModule.gameObject.SetActive(true);
			myGhostModule.position = aHitInfo.point;

			myMirrorGhostModule.gameObject.SetActive(ShipEditorUIToolbarMirror.IsMirrorOn);
			if (ShipEditorUIToolbarMirror.IsMirrorOn == true)
			{
				myMirrorGhostModule.position = MirrorPosition(myGhostModule.position);
			}
			if (Input.GetMouseButtonDown(0) == true)
			{
				BuildAccessory();
			}
		}
	}

	private void Miss()
	{
		if (myMode == eShipEditorMode.Build || myMode == eShipEditorMode.Hardpoints || myMode == eShipEditorMode.Accessories)
		{
			myGhostModule.gameObject.SetActive(false);
			myMirrorGhostModule.gameObject.SetActive(false);
		}
	}

	private void RotateTurret()
	{
		Plane plane = new Plane(Vector3.up, myCurrentTurret.transform.position);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float distance;
		plane.Raycast(ray, out distance);
		Vector3 point = ray.GetPoint(distance);
		Vector3 deltaTurretToPoint = point - myCurrentTurret.transform.position;
		myCurrentTurret.transform.rotation = Quaternion.LookRotation(deltaTurretToPoint, Vector3.up);

		if (myMirrorModules.ContainsKey(myCurrentTurret.transform) == true)
		{
			Vector3 mirrorNormal = deltaTurretToPoint;
			mirrorNormal.z *= -1.0f;
			myMirrorModules[myCurrentTurret.transform].rotation = Quaternion.LookRotation(mirrorNormal, Vector3.up);
		}

		if (Input.GetMouseButtonDown(0) == true)
		{
			myShipData.AddHardpoint(myCurrentTurret, mySelectedWeaponSize);
			myCurrentTurret.ShowRangeDisplay(false);

			if (myMirrorModules.ContainsKey(myCurrentTurret.transform) == true)
			{
				Turret mirrorTurret = myMirrorModules[myCurrentTurret.transform].GetComponent<Turret>();
				myShipData.AddHardpoint(mirrorTurret, mySelectedWeaponSize);
				mirrorTurret.ShowRangeDisplay(false);
			}

			myCurrentTurret = null;
			myStats.UpdateStats(myShipData);
		}
	}

	public void SetSoftpointAtIndex(int aIndex)
	{
		myShipData.SetSoftpoint(aIndex, mySelectedSoftpointData);
		mySoftpointSlots.SetIcon(aIndex, mySelectedSoftpointData);
		myStats.UpdateStats(myShipData);
	}

	private void ClearShip()
	{
		if (myShipData == null)
		{
			myShipData = new ShipData();
		}
		for (int i = 0; i < myShipData.blocks.Count; ++i)
		{
			Destroy(myShipData.blocks[i].gameObject);
		}
		for (int i = 0; i < myShipData.smallHardpoints.Count; ++i)
		{
			Destroy(myShipData.smallHardpoints[i].gameObject);
		}
		for (int i = 0; i < myShipData.mediumHardpoints.Count; ++i)
		{
			Destroy(myShipData.mediumHardpoints[i].gameObject);
		}
		for (int i = 0; i < myShipData.largeHardpoints.Count; ++i)
		{
			Destroy(myShipData.largeHardpoints[i].gameObject);
		}
		myShipData.blocks.Clear();
		myShipData.smallHardpoints.Clear();
		myShipData.mediumHardpoints.Clear();
		myShipData.largeHardpoints.Clear();
		myMirrorModules.Clear();
		mySoftpointSlots.Clear();
	}

	#region Helpers
	private Vector3 MirrorPosition(Vector3 aPosition)
	{
		Vector3 mirror = aPosition;
		mirror.z = -mirror.z;
		return mirror;
	}

	private Vector3 MirrorEuler(Vector3 aEuler)
	{
		Vector3 mirrorEuler = aEuler;
		mirrorEuler.y *= -1.0f;
		mirrorEuler.x *= -1.0f;
		return mirrorEuler;
	}

	private Quaternion MirrorQuaternion(Quaternion aQuaternion)
	{
		Vector3 mirrorEuler = aQuaternion.eulerAngles;
		mirrorEuler.y *= -1.0f;
		mirrorEuler.x *= -1.0f;
		return Quaternion.Euler(mirrorEuler);
	}

	private Vector3 MirrorNormal(Vector3 aNormal)
	{
		Vector3 mirrorNormal = aNormal;
		mirrorNormal.z *= -1.0f;
		return mirrorNormal;
	}

	private Vector3 MirrorScale(Vector3 aScale)
	{
		Vector3 mirrorScale = aScale;
		mirrorScale.z *= -1.0f;
		return mirrorScale;
	}

	private float GetHardpointScale(float aSizeIndex)
	{
		float result = 2.5f;
		if (aSizeIndex == 0) result = 1.0f;
		else if (aSizeIndex == 1) result = 1.5f;
		//return 2.5f;
		return result * 0.2f;
	}
	#endregion

	#region Serializing
	public void SaveShip(string aFileName)
	{
		XmlDocument document = new XmlDocument();
		XmlElement root = document.CreateElement("ship");
		document.AppendChild(root);

		XmlElement modelElement = document.CreateElement("model");
		modelElement.SetAttribute("class", myShipData.shipClassIndex.ToString());
		modelElement.SetAttribute("hull", myShipData.shipModelIndex.ToString());
		document.DocumentElement.AppendChild(modelElement);

		XmlElement piecesElement = document.CreateElement("blocks");
		for (int i = 0; i < myShipData.blocks.Count; ++i)
		{
			XmlElement pieceElement = document.CreateElement("block");
			pieceElement.SetAttribute("m", myModuleNames[myShipData.blocks[i]]);
			pieceElement.SetAttribute("px", myShipData.blocks[i].transform.position.x.ToString());
			pieceElement.SetAttribute("py", myShipData.blocks[i].transform.position.y.ToString());
			pieceElement.SetAttribute("pz", myShipData.blocks[i].transform.position.z.ToString());
			pieceElement.SetAttribute("rx", myShipData.blocks[i].transform.eulerAngles.x.ToString());
			pieceElement.SetAttribute("ry", myShipData.blocks[i].transform.eulerAngles.y.ToString());
			pieceElement.SetAttribute("rz", myShipData.blocks[i].transform.eulerAngles.z.ToString());
			pieceElement.SetAttribute("sx", myShipData.blocks[i].transform.localScale.x.ToString());
			pieceElement.SetAttribute("sy", myShipData.blocks[i].transform.localScale.y.ToString());
			pieceElement.SetAttribute("sz", myShipData.blocks[i].transform.localScale.z.ToString());
			pieceElement.SetAttribute("s", myMirrorModules.ContainsKey(myShipData.blocks[i]) == true ? myShipData.blocks.IndexOf(myMirrorModules[myShipData.blocks[i]]).ToString() : "-1");
			piecesElement.AppendChild(pieceElement);
		}
		document.DocumentElement.AppendChild(piecesElement);

		XmlElement softpointsElement = document.CreateElement("softpoints");
		for (int i = 0; i < myShipData.softpoints.Length; ++i)
		{
			XmlElement softpointElement = document.CreateElement("softpoint");
			string softpointName = string.Empty;
			if (myShipData.softpoints[i] != null)
			{
				softpointName = myShipData.softpoints[i].softpointName;
			}
			softpointElement.SetAttribute("id", softpointName);
			softpointsElement.AppendChild(softpointElement);
		}
		document.DocumentElement.AppendChild(softpointsElement);

		XmlElement smallHardpointsElement = document.CreateElement("smallHardpoints");
		for (int i = 0; i < myShipData.smallHardpoints.Count; ++i)
		{
			XmlElement smallHardpointElement = document.CreateElement("smallHardpoint");
			smallHardpointElement.SetAttribute("x", myShipData.smallHardpoints[i].transform.position.x.ToString());
			smallHardpointElement.SetAttribute("y", myShipData.smallHardpoints[i].transform.position.y.ToString());
			smallHardpointElement.SetAttribute("z", myShipData.smallHardpoints[i].transform.position.z.ToString());
			smallHardpointElement.SetAttribute("r", myShipData.smallHardpoints[i].WeaponAngle.ToString());
			smallHardpointElement.SetAttribute("id", myShipData.smallHardpoints[i].Weapon.weaponName);
			smallHardpointElement.SetAttribute("s", myMirrorModules.ContainsKey(myShipData.smallHardpoints[i].transform) == true ? myShipData.smallHardpoints.IndexOf(myMirrorModules[myShipData.smallHardpoints[i].transform].GetComponent<Turret>()).ToString() : "-1");
			smallHardpointsElement.AppendChild(smallHardpointElement);
		}
		document.DocumentElement.AppendChild(smallHardpointsElement);

		XmlElement mediumHardpointsElement = document.CreateElement("mediumHardpoints");
		for (int i = 0; i < myShipData.mediumHardpoints.Count; ++i)
		{
			XmlElement mediumHardpointElement = document.CreateElement("mediumHardpoint");
			mediumHardpointElement.SetAttribute("x", myShipData.mediumHardpoints[i].transform.position.x.ToString());
			mediumHardpointElement.SetAttribute("y", myShipData.mediumHardpoints[i].transform.position.y.ToString());
			mediumHardpointElement.SetAttribute("z", myShipData.mediumHardpoints[i].transform.position.z.ToString());
			mediumHardpointElement.SetAttribute("r", myShipData.mediumHardpoints[i].WeaponAngle.ToString());
			mediumHardpointElement.SetAttribute("id", myShipData.mediumHardpoints[i].Weapon.weaponName);
			mediumHardpointElement.SetAttribute("s", myMirrorModules.ContainsKey(myShipData.mediumHardpoints[i].transform) == true ? myShipData.mediumHardpoints.IndexOf(myMirrorModules[myShipData.mediumHardpoints[i].transform].GetComponent<Turret>()).ToString() : "-1");
			mediumHardpointsElement.AppendChild(mediumHardpointElement);
		}
		document.DocumentElement.AppendChild(mediumHardpointsElement);

		XmlElement largeHardpointsElement = document.CreateElement("largeHardpoints");
		for (int i = 0; i < myShipData.largeHardpoints.Count; ++i)
		{
			XmlElement largeHardpointElement = document.CreateElement("largeHardpoint");
			largeHardpointElement.SetAttribute("x", myShipData.largeHardpoints[i].transform.position.x.ToString());
			largeHardpointElement.SetAttribute("y", myShipData.largeHardpoints[i].transform.position.y.ToString());
			largeHardpointElement.SetAttribute("z", myShipData.largeHardpoints[i].transform.position.z.ToString());
			largeHardpointElement.SetAttribute("r", myShipData.largeHardpoints[i].WeaponAngle.ToString());
			largeHardpointElement.SetAttribute("id", myShipData.largeHardpoints[i].Weapon.weaponName);
			largeHardpointElement.SetAttribute("s", myMirrorModules.ContainsKey(myShipData.largeHardpoints[i].transform) == true ? myShipData.largeHardpoints.IndexOf(myMirrorModules[myShipData.largeHardpoints[i].transform].GetComponent<Turret>()).ToString() : "-1");
			largeHardpointsElement.AppendChild(largeHardpointElement);
		}
		document.DocumentElement.AppendChild(largeHardpointsElement);

		XmlElement accessoriesElement = document.CreateElement("accessories");
		for (int i = 0; i < myShipData.props.Count; ++i)
		{
			XmlElement accessoryElement = document.CreateElement("accessory");
			accessoryElement.SetAttribute("m", myPropNames[myShipData.props[i]]);
			accessoryElement.SetAttribute("px", myShipData.props[i].position.x.ToString());
			accessoryElement.SetAttribute("py", myShipData.props[i].position.y.ToString());
			accessoryElement.SetAttribute("pz", myShipData.props[i].position.z.ToString());
			accessoryElement.SetAttribute("s", myShipData.props[i].localScale.x.ToString());
			accessoriesElement.AppendChild(accessoryElement);
		}
		document.DocumentElement.AppendChild(accessoriesElement);

		document.Save("Data/Ships/" + aFileName + ".xml");
	}

	public void LoadShip(string aFileName)
	{
		if (myShipData != null)
		{
			ClearShip();
		}

		XmlDocument document = new XmlDocument();
		document.Load(Environment.CurrentDirectory + "\\Data\\Ships\\" + aFileName + ".xml");
		XmlNode modelNode = document.DocumentElement.SelectSingleNode("model");
		{
			int classIndex = 0;
			int hullIndex = 0;
			classIndex = int.Parse(modelNode.Attributes["class"].Value);
			hullIndex = int.Parse(modelNode.Attributes["hull"].Value);
			string hullName = "Patrol Boat";
			if (hullIndex == 1) { hullName = "Gun Boat"; }
			else if (hullIndex == 2) { hullName = "Forward Offence Ship"; }
			else if (hullIndex == 3) { hullName = "Mainline Ship"; }
			else if (hullIndex == 4) { hullName = "Armoured Cruiser"; }
			else if (hullIndex == 5) { hullName = "Battle Cruiser"; }
			ShipBaseData baseData = ContentLoader.Instance.GetShipData(hullName);
			//myShipData.Init(baseData);
			StartEditor(baseData);
			//EventManager.Instance.TriggerEvent(eEventType.EditorShipCreated, myShipData);
		}

		XmlNodeList blockNodes = document.DocumentElement.SelectNodes("blocks//block");
		foreach (XmlNode blockNode in blockNodes)
		{
			string moduleName = "Cube";
			Vector3 position = Vector3.zero;
			Vector3 euler = Vector3.zero;
			Vector3 scale = Vector3.zero;
			if (blockNode.Attributes["m"] != null) moduleName = blockNode.Attributes["m"].Value;
			position.x = float.Parse(blockNode.Attributes["px"].Value);
			position.y = float.Parse(blockNode.Attributes["py"].Value);
			position.z = float.Parse(blockNode.Attributes["pz"].Value);
			if (blockNode.Attributes["rx"] != null) euler.x = float.Parse(blockNode.Attributes["rx"].Value);
			if (blockNode.Attributes["ry"] != null) euler.y = float.Parse(blockNode.Attributes["ry"].Value);
			if (blockNode.Attributes["rz"] != null) euler.z = float.Parse(blockNode.Attributes["rz"].Value);
			scale.x = float.Parse(blockNode.Attributes["sx"].Value);
			scale.y = float.Parse(blockNode.Attributes["sy"].Value);
			scale.z = float.Parse(blockNode.Attributes["sz"].Value);
			
			Transform module = Instantiate(ContentLoader.Instance.GetModuleData(moduleName).modulePrefab);
			module.position = position;
			module.eulerAngles = euler;
			module.localScale = scale;

			myModuleNames.Add(module, moduleName);

			if (blockNode.Attributes["s"] != null)
			{
				int mirrorIndex = int.Parse(blockNode.Attributes["s"].Value);
				if (mirrorIndex >= 0 && myShipData.blocks.Count >= mirrorIndex)
				{
					if (myMirrorModules.ContainsKey(module) == false)
					{
						myMirrorModules.Add(module, myShipData.blocks[mirrorIndex]);
						myMirrorModules.Add(myShipData.blocks[mirrorIndex], module);
					}
				}
			}
			
			myShipData.blocks.Add(module);
		}

		XmlNodeList softpointNodes = document.DocumentElement.SelectNodes("softpoints//softpoint");
		int slotIndex = 0;
		foreach (XmlNode softpointNode in softpointNodes)
		{
			string softpointName = string.Empty;
			softpointName = softpointNode.Attributes["id"].Value;
			SoftpointData softpointData = ContentLoader.Instance.GetSoftpointData(softpointName);
			if (softpointData != null)
			{
				SelectedSoftpointChanged(softpointData);
				SetSoftpointAtIndex(slotIndex);
				//myShipData.SetSoftpoint(slotIndex, softpointData);
				//mySoftpointSlots.SetIcon(slotIndex, softpointData);
			}
			++slotIndex;
		}

		XmlNodeList smallHardpointNodes = document.DocumentElement.SelectNodes("smallHardpoints//smallHardpoint");
		foreach (XmlNode smallHardpointNode in smallHardpointNodes)
		{
			Vector3 position = Vector3.zero;
			float angle = 0.0f;
			string weaponName = string.Empty;
			position.x = float.Parse(smallHardpointNode.Attributes["x"].Value);
			position.y = float.Parse(smallHardpointNode.Attributes["y"].Value);
			position.z = float.Parse(smallHardpointNode.Attributes["z"].Value);
			angle = float.Parse(smallHardpointNode.Attributes["r"].Value);
			weaponName = smallHardpointNode.Attributes["id"].Value;

			WeaponData weaponData = ContentLoader.Instance.GetWeaponData(weaponName);

			Turret turret = Instantiate(weaponData.turretPrefab);
			turret.transform.position = position;
			turret.transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
			turret.transform.localScale = Vector3.one * GetHardpointScale(0);

			turret.Init(weaponData, 0);

			if (smallHardpointNode.Attributes["s"] != null)
			{
				int mirrorIndex = int.Parse(smallHardpointNode.Attributes["s"].Value);
				if (mirrorIndex >= 0 && myShipData.smallHardpoints.Count >= mirrorIndex)
				{
					if (myMirrorModules.ContainsKey(turret.transform) == false)
					{
						myMirrorModules.Add(turret.transform, myShipData.smallHardpoints[mirrorIndex].transform);
						myMirrorModules.Add(myShipData.smallHardpoints[mirrorIndex].transform, turret.transform);
					}
				}
			}

			myShipData.AddHardpoint(turret, 0);
		}

		XmlNodeList mediumHardpointNodes = document.DocumentElement.SelectNodes("mediumHardpoints//mediumHardpoint");
		foreach (XmlNode mediumHardpointNode in mediumHardpointNodes)
		{
			Vector3 position = Vector3.zero;
			float angle = 0.0f;
			string weaponName = string.Empty;
			position.x = float.Parse(mediumHardpointNode.Attributes["x"].Value);
			position.y = float.Parse(mediumHardpointNode.Attributes["y"].Value);
			position.z = float.Parse(mediumHardpointNode.Attributes["z"].Value);
			angle = float.Parse(mediumHardpointNode.Attributes["r"].Value);
			weaponName = mediumHardpointNode.Attributes["id"].Value;

			WeaponData weaponData = ContentLoader.Instance.GetWeaponData(weaponName);

			Turret turret = Instantiate(weaponData.turretPrefab);
			turret.transform.position = position;
			turret.transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
			turret.transform.localScale = Vector3.one * GetHardpointScale(1);

			turret.Init(weaponData, 1);

			if (mediumHardpointNode.Attributes["s"] != null)
			{
				int mirrorIndex = int.Parse(mediumHardpointNode.Attributes["s"].Value);
				if (mirrorIndex >= 0 && myShipData.mediumHardpoints.Count >= mirrorIndex)
				{
					if (myMirrorModules.ContainsKey(turret.transform) == false)
					{
						myMirrorModules.Add(turret.transform, myShipData.mediumHardpoints[mirrorIndex].transform);
						myMirrorModules.Add(myShipData.mediumHardpoints[mirrorIndex].transform, turret.transform);
					}
				}
			}

			myShipData.AddHardpoint(turret, 1);
		}

		XmlNodeList largeHardpointNodes = document.DocumentElement.SelectNodes("largeHardpoints//largeHardpoint");
		foreach (XmlNode largeHardpointNode in largeHardpointNodes)
		{
			Vector3 position = Vector3.zero;
			float angle = 0.0f;
			string weaponName = string.Empty;
			position.x = float.Parse(largeHardpointNode.Attributes["x"].Value);
			position.y = float.Parse(largeHardpointNode.Attributes["y"].Value);
			position.z = float.Parse(largeHardpointNode.Attributes["z"].Value);
			angle = float.Parse(largeHardpointNode.Attributes["r"].Value);
			weaponName = largeHardpointNode.Attributes["id"].Value;

			WeaponData weaponData = ContentLoader.Instance.GetWeaponData(weaponName);

			Turret turret = Instantiate(weaponData.turretPrefab);
			turret.transform.position = position;
			turret.transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
			turret.transform.localScale = Vector3.one * GetHardpointScale(2);

			turret.Init(weaponData, 2);

			if (largeHardpointNode.Attributes["s"] != null)
			{
				int mirrorIndex = int.Parse(largeHardpointNode.Attributes["s"].Value);
				if (mirrorIndex >= 0 && myShipData.largeHardpoints.Count >= mirrorIndex)
				{
					if (myMirrorModules.ContainsKey(turret.transform) == false)
					{
						myMirrorModules.Add(turret.transform, myShipData.largeHardpoints[mirrorIndex].transform);
						myMirrorModules.Add(myShipData.largeHardpoints[mirrorIndex].transform, turret.transform);
					}
				}
			}

			myShipData.AddHardpoint(turret, 2);
		}

		if (document.DocumentElement.SelectSingleNode("accessories") != null)
		{
			XmlNodeList accessoryNodes = document.DocumentElement.SelectNodes("accessories//accessory");
			foreach (XmlNode accessoryNode in accessoryNodes)
			{
				string propName = "";
				Vector3 position = Vector3.zero;
				float scale;
				if (accessoryNode.Attributes["m"] != null) propName = accessoryNode.Attributes["m"].Value;
				position.x = float.Parse(accessoryNode.Attributes["px"].Value);
				position.y = float.Parse(accessoryNode.Attributes["py"].Value);
				position.z = float.Parse(accessoryNode.Attributes["pz"].Value);
				scale = float.Parse(accessoryNode.Attributes["sx"].Value);

				Transform prop = Instantiate(ContentLoader.Instance.GetPropData(propName).propPrefab);
				prop.position = position;
				prop.localScale = Vector3.one * scale;

				if (accessoryNode.Attributes["s"] != null)
				{
					int mirrorIndex = int.Parse(accessoryNode.Attributes["s"].Value);
					if (mirrorIndex >= 0 && myShipData.props.Count >= mirrorIndex)
					{
						if (myMirrorModules.ContainsKey(prop) == false)
						{
							myMirrorModules.Add(prop, myShipData.props[mirrorIndex]);
							myMirrorModules.Add(myShipData.props[mirrorIndex], prop);
						}
					}
				}

				myPropNames.Add(prop, propName);

				myShipData.props.Add(prop);
			}
		}

		//XmlNodeList softpointNodes = document.DocumentElement.SelectNodes("softpoints//softpoint");
		//foreach (XmlNode softpointNode)

		myUI.SetMode(ShipEditorUI.eShipEditorUIMode.Editor);
		myStats.UpdateStats(myShipData);
		//EventManager.Instance.TriggerEvent(eEventType.EditorUIEnableContinueButton);
		//EventManager.Instance.TriggerEvent(eEventType.EditorShipChanged, myShipData);
	}
	#endregion
}
