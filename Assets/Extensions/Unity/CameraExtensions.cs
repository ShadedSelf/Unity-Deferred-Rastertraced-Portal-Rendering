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

	public static Vector3 TrueViewportToWorldPoint(this Camera cam, Vector3 viewPoint) // 0 to 1
	{
		Matrix4x4 m = cam.projectionMatrix * cam.worldToCameraMatrix;
		float4 v = float4(viewPoint, 1) * 2 - 1;
		v = m.inverse * v;
		v /= v.w;
		return v.xyz;
	}
}
