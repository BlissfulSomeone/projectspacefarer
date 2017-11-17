using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShipEditorCamera : MonoBehaviour
{
	[SerializeField]
	private Transform myCameraTransform;

	[SerializeField]
	private float myRotationSensitivity = 4.0f;

	private float myCameraDistance = 5.0f;
	private float myTargetCameraDistance = 5.0f;
	private Vector3 myCameraRotation = Vector3.zero;
	private Vector3 myTargetCameraRotation = Vector3.zero;
	private Vector3 myTargetCameraPosition = Vector3.zero;
	
	void Update()
	{
		bool inputAllowed = (EventSystem.current.IsPointerOverGameObject() == false);
		Rotate(inputAllowed);
		Zoom(inputAllowed);
		Move();
	}

	private void Rotate(bool aInputAllowed)
	{
		if (Input.GetMouseButton(1) == true && aInputAllowed == true)
		{
			float mouseX = Input.GetAxisRaw("Mouse X");
			float mouseY = Input.GetAxisRaw("Mouse Y");
			myTargetCameraRotation.x -= mouseY * myRotationSensitivity;
			myTargetCameraRotation.y += mouseX * myRotationSensitivity;
			myTargetCameraRotation.x = Mathf.Clamp(myTargetCameraRotation.x, -90.0f, 90.0f);
		}

		myCameraRotation.x = Mathf.MoveTowards(myCameraRotation.x, myTargetCameraRotation.x, Mathf.Abs(myTargetCameraRotation.x - myCameraRotation.x) * Time.deltaTime * 15.0f + 0.05f);
		myCameraRotation.y = Mathf.MoveTowards(myCameraRotation.y, myTargetCameraRotation.y, Mathf.Abs(myTargetCameraRotation.y - myCameraRotation.y) * Time.deltaTime * 15.0f + 0.05f);

		transform.eulerAngles = myCameraRotation;
	}

	private void Zoom(bool aInputAllowed)
	{
		float scrollValue = Input.GetAxisRaw("Mouse ScrollWheel");
		if (scrollValue > 0.0f && aInputAllowed == true)
		{
			myTargetCameraDistance *= 1.0f / 1.2f;
		}
		if (scrollValue < 0.0f && aInputAllowed == true)
		{
			myTargetCameraDistance *= 1.2f;
		}

		myCameraDistance = Mathf.MoveTowards(myCameraDistance, myTargetCameraDistance, Mathf.Abs(myTargetCameraDistance - myCameraDistance) * Time.deltaTime * 5.0f + 0.0001f);

		Vector3 cameraPosition = myCameraTransform.localPosition;
		cameraPosition.z = -myCameraDistance;
		myCameraTransform.localPosition = cameraPosition;
	}

	private void Move()
	{
		Vector3 cameraPosition = transform.position;
		cameraPosition = Vector3.MoveTowards(cameraPosition, myTargetCameraPosition, (myTargetCameraPosition - cameraPosition).magnitude * Time.deltaTime * 4.0f + 0.001f);
		transform.position = cameraPosition;
	}

	public void SetOrigin(Vector3 aOrigin)
	{
		myTargetCameraPosition = aOrigin;
	}

	public void SetRotation(Vector3 aEuler, bool aImmediet)
	{
		myTargetCameraRotation = aEuler;
		if (aImmediet == true)
		{
			myCameraRotation = aEuler; 
		}
	}

	public void SetZoom(float aZoom, bool aImmediet)
	{
		myTargetCameraDistance = aZoom;
		if (aImmediet == true)
		{
			myCameraDistance = aZoom;
		}
	}
}
