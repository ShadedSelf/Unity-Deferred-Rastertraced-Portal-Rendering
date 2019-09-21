using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;
using static Unity.Mathematics.math;

public class roter : MonoBehaviour
{
	void OnWillRenderObject()
	{
		// float t = Portal.time;
		float t = 20;
		transform.rotation = Quaternion.AngleAxis(Time.time * t * 2, Vector3.right);
	}

	// void FixedUpdate()
	// {
	// 	// float t = Portal.time;
	// 	float t = 20 * Time.fixedTime;
	// 	transform.localRotation = Quaternion.AngleAxis(Time.fixedTime * t * 2, Vector3.right);
	// }
}
