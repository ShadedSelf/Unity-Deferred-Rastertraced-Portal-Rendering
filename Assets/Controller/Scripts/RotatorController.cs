using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorController
{
	//-- Settings: ------------
	public float rotSpeed { get; set; }

	//-- Global State: --------
	// public bool isRotatig => vel.magnitude > 0.1f;

	private Transform body;
	private Vector3 vel;

	public RotatorController(Transform body)
	{
		this.body = body;
	}

	public void RotateBody(Vector3 down, float deltaTime)
	{
		var nDown	= Vector3.SmoothDamp(-body.up, down, ref vel, rotSpeed, 100f, deltaTime);
		var rot		= Quaternion.FromToRotation(-body.up, nDown);

		body.rotation = rot * body.rotation;
	}

	// private void DoAtSomePoint()
	// {
		// Ray ray = new Ray(cam.body.position, -cam.body.up); //make dummy body, smooth from last rotation to new, also use the dummy for the camera thing
		// RaycastHit hit = new RaycastHit();
        // if (Physics.Raycast(ray, out hit, 20))
        // {
        //     if (Vector3.Dot(hit.normal, -cam.body.up) < -.999f && Vector3.Dot(hit.normal, body.up) < .5f)
        //     {
		// 		pivotTrs.localPosition = new Vector3(0, .16f / body.localScale.y, 0);
		// 		rb.body.localScale = new Vector3(body.localScale.x, Vector3.Distance(hit.point, body.position), body.localScale.z);

		// 		downNormal = new Vector3(
		// 			(Mathf.Abs(hit.normal.x) < .00001f) ? 0 : -hit.normal.x,
		// 			(Mathf.Abs(hit.normal.y) < .00001f) ? 0 : -hit.normal.y,
		// 			(Mathf.Abs(hit.normal.z) < .00001f) ? 0 : -hit.normal.z);
		// 		rb.rotation = Quaternion.FromToRotation(body.up, -downNormal) * rb.rotation;
        //         cam.body.rotation = Quaternion.FromToRotation(-downNormal, body.up) * cam.body.rotation;
        //     }
        // }
	// }
}
