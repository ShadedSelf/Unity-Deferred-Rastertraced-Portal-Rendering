using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Unity.Mathematics;
using static Unity.Mathematics.math;

public static class CameraExtensions
{
	public static Vector4 CameraSpacePlane(this Camera cam, Vector3 pos, Vector3 normal, float sideSign, float offSet)
	{
		Vector3 offsetPos = pos + normal * offSet;
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cpos = m.MultiplyPoint(offsetPos);
		Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
		return float4(cnormal, -dot(cpos, cnormal));
	}
}
