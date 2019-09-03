using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public static class VectorExtensions
{
	public static Vector3 DeEps(this Vector3 me, float eps = .00001f)
	{
		return new Vector3
		(
			(Abs(me.x) < eps) ? 0 : me.x, 
			(Abs(me.y) < eps) ? 0 : me.y,
			(Abs(me.z) < eps) ? 0 : me.z
		);
	}

	public static Vector3 MultVector(this Vector3 me, Vector3 you)
	{
		return new Vector3(me.x * you.x, me.y * you.y, me.z * you.z);
	}

	public static Vector3 AbsVector(this Vector3 me)
	{
		return new Vector3(Abs(me.x), Abs(me.y), Abs(me.z));
	}

	public static Vector3 SignVector(this Vector3 me)
	{
		return new Vector3(Sign(me.x), Sign(me.y), Sign(me.z));
	}

	public static Vector3 ClampVector(this Vector3 me, float min = 0, float max = 1)
	{
		return new Vector3(Clamp(me.x, min, max), Clamp(me.y, min, max), Clamp(me.z, min, max));
	}

	public static Vector3 NormalizeVector0(this Vector3 me, Vector3 you)
	{
		return new Vector3(me.x.NormalizeFloat(you.x, 0), me.y.NormalizeFloat(you.y, 0), me.z.NormalizeFloat(you.z, 0));
	}

	public static Vector3 MaxVector(this Vector3 me, Vector3 you)
	{
		return new Vector3(Max(me.x, you.x), Max(me.y, you.y), Max(me.z, you.z));
	}


	public static Vector3 AlignNormal(this Vector3 normal)
	{
		Vector3 absNormal = normal.AbsVector();
		float max = Max(Max(absNormal.x, absNormal.y), absNormal.z);
		Vector3 alignedNormal = new Vector3((absNormal.x >= max) ? 1 : 0, (absNormal.y >= max) ? 1 : 0, (absNormal.z >= max) ? 1 : 0);
		return alignedNormal = alignedNormal.MultVector(normal.SignVector());
	}
}
