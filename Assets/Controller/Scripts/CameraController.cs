using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController
{
	public float xDelta { get; set; }
	public float yDelta { get; set; }

	private Transform pivot, cam;

	public CameraController(Transform pivot, Transform cam)
	{
		this.pivot	= pivot;
		this.cam	= cam;
	}

	public void Rotate(float deltaTime)
	{
		cam.localRotation	*= Quaternion.Euler(  -Input.GetAxisRaw("Mouse Y") * xDelta * 0.015f, 0, 0);
		pivot.localRotation	*= Quaternion.Euler(0, Input.GetAxisRaw("Mouse X") * yDelta * 0.015f, 0);
	}

	public void Correct(Quaternion rot)
	{
		cam.localRotation *= Quaternion.Euler(-rot.eulerAngles.x, 0, 0);
	}

	public void Lock()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Cursor.lockState	= CursorLockMode.None;
			Cursor.visible		= true;
		}	
		else if (Input.GetMouseButtonUp(0))
		{
			Cursor.lockState	= CursorLockMode.Locked;
			Cursor.visible		= false;
		}
	}
}
