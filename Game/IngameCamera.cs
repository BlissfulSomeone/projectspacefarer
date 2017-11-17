using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameCamera : MonoBehaviour
{
	private Vector3 myTargetPosition;

	private Transform myCam;
	private ShipEditorCamera myMount;

	void Awake()
	{
		myCam = transform.GetChild(0);
		myMount = GetComponent<ShipEditorCamera>();
	}

	void Update()
	{
		float speed = Mathf.Abs(myCam.localPosition.z);
		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");
		myTargetPosition += transform.right * horizontal * Time.deltaTime * speed;
		myTargetPosition += (Quaternion.Euler(0.0f, -90.0f, 0.0f) * transform.right) * vertical * Time.deltaTime * speed;
		myMount.SetOrigin(myTargetPosition);
	}
}
