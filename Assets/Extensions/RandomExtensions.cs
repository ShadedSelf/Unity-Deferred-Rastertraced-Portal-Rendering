using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class RandomExtensions
{
	public static Vector3[] sixNormals = new Vector3[6] { Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back };

	public static bool AlmostFloat(this float number1, float number2, float precision = 0.0001f)
	{
		return Mathf.Abs(number1 - number2) < precision;
	}

	public static Vector4 CameraSpacePlane(this Camera cam, Vector3 pos, Vector3 normal, float sideSign, float offSet)
	{
		Vector3 offsetPos = pos + normal * offSet;
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cpos = m.MultiplyPoint(offsetPos);
		Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
	}

	public static float NormalizeFloat(this float value, float max, float min)
	{
		return (value - min) / (max - min);
	}
}
