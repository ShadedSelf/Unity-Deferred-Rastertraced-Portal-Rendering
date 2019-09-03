using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;
using static Unity.Mathematics.math;

public static class PortalUtils
{
	//-- Position: --
	public static Vector3 GetLocalPosition(Vector3 pos, Transform from, Vector3 offset = default(Vector3))
	{
		Vector3 res = from.InverseTransformPoint(pos) * float3(-1, 1, -1);
		return res - offset;
	}

	public static Vector3 GetAbsolutePosition(Vector3 pos, Transform from, Transform to, Vector3 offset = default(Vector3))
	{
		Vector3 res = GetLocalPosition(pos, from, offset);
		return to.TransformPoint(res);
	}

	//-- Rotation: --
	public static Quaternion GetLocalRotation(Quaternion rot, Transform from)
	{
		Quaternion local = Quaternion.Inverse(from.rotation) * rot;
		return Quaternion.AngleAxis(180f, Vector3.up) * local;
	}

	public static Quaternion GetAbsoluteRotation(Quaternion rot, Transform from, Transform to)
	{
		return to.rotation * GetLocalRotation(rot, from);
	}

	//-- Scale: --
	public static Vector3 GetLocalScale(Vector3 scale, Transform from)
	{
		return scale / (float3)from.lossyScale;
	}

	public static Vector3 GetAbsoluteScale(Vector3 scale, Transform from, Transform to)
	{
		return GetLocalScale(scale, from) * (float3)to.lossyScale;
	}

	//-- Direction: --
	public static Vector3 GetLocalDirection(Vector3 direction, Transform from)
	{
		return from.InverseTransformDirection(direction).normalized;
	}

	public static Vector3 GetAbsoluteDirection(Vector3 direction, Transform from, Transform to)
	{
		Vector3 res = Quaternion.AngleAxis(180f, Vector3.up) * GetLocalDirection(direction, from);
		return to.TransformDirection(res).normalized;
	}

	//-- WholeMatrix: --
	public static Matrix4x4 GetWorldToPortalMatrix(Matrix4x4 current, Transform from, Transform to)
	{
		Matrix4x4 rotOffset		= Matrix4x4.Rotate(Quaternion.AngleAxis(180f, Vector3.up));
		Matrix4x4 localOffset	= from.localToWorldMatrix * (rotOffset * to.worldToLocalMatrix);
		return current * localOffset;
	}
}
