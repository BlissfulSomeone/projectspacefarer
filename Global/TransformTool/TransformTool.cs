using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TransformTool : MonoBehaviour
{
	public enum eTransformMode
	{ 
		Position,
		Rotation,
		Scale,
	}

	private static TransformTool myInstance;
	public static TransformTool Instance { get { return myInstance; } }

	public delegate void TransformMovedHandler(Vector3 aPosition);
	public TransformMovedHandler OnMoved;

	public delegate void TransformRotatedHandler(Quaternion aRotation);
	public TransformRotatedHandler OnRotated;

	//private bool myIsShowing = false;
	private bool[] myIsMoving = { false, false, false };
	private Vector3[] myPlaneNormals = { Vector3.forward, Vector3.forward, Vector3.up };
	private float myTranslationOffset = 0.0f;
	private float myStartAngle = 0.0f;
	private Quaternion myStartRotation;
	private float myPreviousAngle;
	private Vector3 myRotation;

	[SerializeField]
	private TransformToolAxis[] myAxis;
	[SerializeField]
	private TransformToolAxis[] myRadi;
	private TransformToolAxis[] myHandles;

	[SerializeField]
	private Transform myContainer;
	[SerializeField]
	private Transform myPositionContainer;
	[SerializeField]
	private Transform myRotationContainer;
	[SerializeField]
	private Transform myScaleContainer;
	[SerializeField]
	private Material myHoverMaterial;

	public const float SNAP_FACTOR = 8.0f;

	private bool myIsMouseOver = false;
	public bool IsMouseOver { get { return myIsMouseOver; } }

	private eTransformMode myMode = eTransformMode.Position;
	public eTransformMode Mode { get { return myMode; } }

	void Awake()
	{
		if (myInstance != null)
		{
			Destroy(gameObject);
			return;
		}
		myInstance = this;
		DontDestroyOnLoad(this);
		
		myHandles = new TransformToolAxis[myAxis.Length + myRadi.Length];
		for (int i = 0; i < myAxis.Length; ++i)
		{
			myAxis[i].axisIndex = i;
			myAxis[i].mode = eTransformMode.Position;
			myHandles[i] = myAxis[i];
		}
		for (int i = 0; i < myRadi.Length; ++i)
		{
			myRadi[i].axisIndex = i;
			myRadi[i].mode = eTransformMode.Rotation;
			myHandles[myAxis.Length + i] = myRadi[i];
		}

		Show(false);
	}

	private void StartMovingAxis(int aAxisIndex)
	{
		Plane plane = new Plane(myPlaneNormals[aAxisIndex], transform.position);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float distance;
		plane.Raycast(ray, out distance);
		Vector3 point = ray.GetPoint(distance);
		Vector3 position = transform.position;
		myTranslationOffset = position[aAxisIndex] - point[aAxisIndex];
		myIsMoving[aAxisIndex] = true;
	}

	private void StartRotatingAxis(int aAxisIndex)
	{
		Vector3[] normals = { myRotationContainer.right, myRotationContainer.up, myRotationContainer.forward };
		Vector3[] right = { myRotationContainer.up * -1.0f, myRotationContainer.right, myRotationContainer.right * -1.0f };
		Plane plane = new Plane(normals[aAxisIndex], transform.position);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float distance;
		plane.Raycast(ray, out distance);
		Vector3 point = ray.GetPoint(distance);
		myStartAngle = Angle360(myRotationContainer.rotation * myPlaneNormals[aAxisIndex], (transform.position - point).normalized, right[aAxisIndex]);
		myStartRotation = myRotationContainer.rotation;
		myIsMoving[aAxisIndex] = true;
		myPreviousAngle = 0;
	}

	private void ResetAxis(int aAxisIndex)
	{
		Vector3 position = transform.position;
		position[aAxisIndex] = 0.0f;
		transform.position = position;
		//EventManager.TriggerEvent(eEventType.EditorTransformToolMoved, position);
		if (OnMoved != null)
		{
			OnMoved(position);
		}
	}

	private void ResetRotation(int aAxisIndex)
	{
		Vector3[] normals = { Vector3.right, Vector3.up, Vector3.forward };
		Vector3[] right = { Vector3.up * -1.0f, Vector3.right, Vector3.right * -1.0f };

		Vector3 euler = myRotationContainer.eulerAngles;
		euler[aAxisIndex] = 0.0f;
		myRotationContainer.eulerAngles = euler;

		//float angle = Angle360(myRotationContainer.rotation * myPlaneNormals[aAxisIndex], )

		//myRotationContainer.Rotate(normals[aAxisIndex], -myRotation[aAxisIndex], Space.Self);
		//myRotation[aAxisIndex] = 0.0f;
		if (OnRotated != null)
		{
			OnRotated(myRotationContainer.rotation);
		}
		//myRotation[aAxisIndex] = 0.0f;
		//myRotationContainer.eulerAngles = myRotation;
		//if (OnRotated != null)
		//{
		//	OnRotated(myRotation); 
		//}
	}

	void Update()
	{
		bool isMovingAny = false;
		for (int i = 0; i < myIsMoving.Length; ++i)
		{
			if (myIsMoving[i] == true)
			{
				isMovingAny = true;
				break;
			}
		}
		if (myIsMouseOver == true && isMovingAny == false)
		{
			for (int i = 0; i < myHandles.Length; ++i)
			{
				myHandles[i].ResetMaterial();
			}
		}
		myIsMouseOver = false;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 256.0f, 1 << gameObject.layer) == true)
		{
			myIsMouseOver = true;
			if (isMovingAny == false)
			{
				hitInfo.collider.gameObject.GetComponent<TransformToolAxis>().SetMaterial(myHoverMaterial);
			}
			if (Input.GetMouseButtonDown(0) == true)
			{
				TransformToolAxis hitAxis = hitInfo.collider.gameObject.GetComponent<TransformToolAxis>();
				if (hitAxis.mode == myMode)
				{
					if (myMode == eTransformMode.Position)
					{
						StartMovingAxis(hitAxis.axisIndex);
					}
					else if (myMode == eTransformMode.Rotation)
					{
						StartRotatingAxis(hitAxis.axisIndex);
					}
				}
			}
			if (Input.GetMouseButtonDown(1) == true)
			{ 
				TransformToolAxis hitAxis = hitInfo.collider.gameObject.GetComponent<TransformToolAxis>();
				if (hitAxis.mode == myMode)
				{
					if (myMode == eTransformMode.Position)
					{
						ResetAxis(hitAxis.axisIndex);
					}
					else if (myMode == eTransformMode.Rotation)
					{
						ResetRotation(hitAxis.axisIndex);
					}
				}
			}
		}
		for (int i = 0; i < 3; ++i)
		{
			if (myIsMoving[i] == true)
			{
				if (myMode == eTransformMode.Position)
				{
					MoveAxis(i);
				}
				else if (myMode == eTransformMode.Rotation)
				{
					RotateAxis(i);
				}
			}
		}
		if (Input.GetMouseButtonUp(0) == true)
		{
			for (int i = 0; i < 3; ++i)
			{
				myIsMoving[i] = false;
			}
		}
	}

	private void MoveAxis(int aAxisIndex)
	{
		Plane plane = new Plane(myPlaneNormals[aAxisIndex], transform.position);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float distance;
		plane.Raycast(ray, out distance);
		Vector3 point = ray.GetPoint(distance);
		Vector3 position = transform.position;
		position[aAxisIndex] = point[aAxisIndex] + myTranslationOffset;
		if (Input.GetKey(KeyCode.LeftControl) == true || Input.GetKey(KeyCode.RightControl) == true)
		{
			position[aAxisIndex] = Mathf.Round(position[aAxisIndex] * SNAP_FACTOR) / SNAP_FACTOR;
		}
		transform.position = position;
		//EventManager.TriggerEvent(eEventType.EditorTransformToolMoved, position);
		if (OnMoved != null)
		{
			OnMoved(position);
		}
	}
	

	private void RotateAxis(int aAxisIndex)
	{
		Vector3[] normals = { Vector3.right, Vector3.up, Vector3.forward };
		Vector3[] right = { Vector3.up * -1.0f, Vector3.right, Vector3.right * -1.0f };
		Plane plane = new Plane(myStartRotation * normals[aAxisIndex], transform.position);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float distance;
		plane.Raycast(ray, out distance);
		Vector3 point = ray.GetPoint(distance);
		float rotation = Angle360(myStartRotation * myPlaneNormals[aAxisIndex], (transform.position - point).normalized, myStartRotation * right[aAxisIndex]);
		float startAngle = myStartAngle;
		float prevAngle = myPreviousAngle;
		//if (Input.GetKey(KeyCode.LeftControl) == true || Input.GetKey(KeyCode.RightControl) == true)
		//{
		//	float fix = myRotation[aAxisIndex];
		//	fix = Mathf.Round(fix / 10.0f) * 10.0f;
		//	fix = myRotation[aAxisIndex] - fix;
		//	rotation = Mathf.Round(rotation / 10.0f) * 10.0f - fix;
		//	startAngle = Mathf.Round(startAngle / 10.0f) * 10.0f;
		//	prevAngle = Mathf.Round(prevAngle / 10.0f) * 10.0f;
		//}
		float angle = rotation - startAngle;
		myRotation[aAxisIndex] += angle - prevAngle;
		myRotationContainer.Rotate(normals[aAxisIndex], angle - prevAngle, Space.Self);

		if (Input.GetKey(KeyCode.LeftControl) == true || Input.GetKey(KeyCode.RightControl) == true)
		{
			Vector3 euler = myRotationContainer.eulerAngles;
			euler[aAxisIndex] = Mathf.Round(euler[aAxisIndex] / 10.0f) * 10.0f;
			myRotationContainer.eulerAngles = euler;
		}

		myPreviousAngle = angle;

		if (OnRotated != null)
		{
			OnRotated(myRotationContainer.rotation);
		}
	}

	void LateUpdate()
	{
		float distanceToCamera = Vector3.Distance(transform.position, Camera.main.transform.position);
		transform.localScale = Vector3.one * distanceToCamera * 0.2f;
	}

	public void Show(bool aFlag)
	{
		myContainer.gameObject.SetActive(aFlag);
		//myIsShowing = aFlag;
		if (aFlag == false)
		{
			for (int i = 0; i < myHandles.Length; ++i)
			{
				myHandles[i].ResetMaterial();
			}
			for (int i = 0; i < myIsMoving.Length; ++i)
			{
				myIsMoving[i] = false;
			}
			myIsMouseOver = false;
		}
	}

	public void EnableAxis(int aAxisIndex, bool aFlag)
	{
		myAxis[aAxisIndex].gameObject.SetActive(aFlag);
	}

	public void SetMode(eTransformMode aMode)
	{
		myMode = aMode;
		myPositionContainer.gameObject.SetActive(myMode == eTransformMode.Position);
		myRotationContainer.gameObject.SetActive(myMode == eTransformMode.Rotation);
		myScaleContainer.gameObject.SetActive(myMode == eTransformMode.Scale);
	}

	public void SetRadiRotation(Quaternion aRotation)
	{
		myRotationContainer.rotation = aRotation;
	}

	private float Angle360(Vector3 from, Vector3 to, Vector3 right)
	{
		float angle = Vector3.Angle(from, to);
		return (Vector3.Angle(right, to) > 90f) ? 360f - angle : angle;
	}
}
