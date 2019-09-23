using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;
using static Unity.Mathematics.math;

public static class VectorExtensions
{
	public static Vector3 DeEps(this Vector3 me, float eps = 0.00001f)
	{
		return new Vector3
		(
			(abs(me.x) < eps) ? 0 : me.x, 
			(abs(me.y) < eps) ? 0 : me.y,
			(abs(me.z) < eps) ? 0 : me.z
		);
	}

	public static Vector3 AlignNormal(this Vector3 normal)
	{
		Vector3 absNormal = abs(normal);
		float maxL = max(max(absNormal.x, absNormal.y), absNormal.z);
		Vector3 alignedNormal = new Vector3((absNormal.x >= maxL) ? 1 : 0, (absNormal.y >= maxL) ? 1 : 0, (absNormal.z >= maxL) ? 1 : 0);
		return alignedNormal * sign(normal);
	}
}
