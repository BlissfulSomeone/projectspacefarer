using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamera : MonoBehaviour
{
	Vector3 targetPosition;

	void Awake()
	{
		targetPosition = transform.position;
	}

	void Update()
	{
		float right = Input.GetAxisRaw("Horizontal");
		float forward = Input.GetAxisRaw("Vertical");
		float up = Input.GetAxisRaw("Upward");

		targetPosition += transform.right * right * Time.deltaTime;
		targetPosition += transform.forward * forward * Time.deltaTime;
		targetPosition += Vector3.up * up * Time.deltaTime;

		float horizontal = Input.GetAxisRaw("Mouse X");
		float vertical = Input.GetAxisRaw("Mouse Y");
		Vector3 rotation = transform.eulerAngles;
		rotation.x -= vertical * 4.0f;
		rotation.y += horizontal * 4.0f;
		rotation.x = Mathf.Clamp(rotation.x, -90.0f, 90.0f);
		transform.eulerAngles = rotation;
	}
}
