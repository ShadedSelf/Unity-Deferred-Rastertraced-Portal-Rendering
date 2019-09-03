using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PortalExtensions
{
	//-- Position: ------------
	public static Vector3 TransformPosition(this Portal portal, Vector3 position, Vector3 offset = default(Vector3))
		=> PortalUtils.GetAbsolutePosition(position, portal.transform, portal.other.transform, offset);

	//-- Rotation: ------------
	public static Quaternion TransformRotation(this Portal portal, Quaternion rotation)
		=> PortalUtils.GetAbsoluteRotation(rotation, portal.transform, portal.other.transform);

	//-- Scale: ---------------
	public static Vector3 TransformScale(this Portal portal, Vector3 scale)
		=> PortalUtils.GetAbsoluteScale(scale, portal.transform, portal.other.transform);

	//-- Direction: -----------
	public static Vector3 TransformDirection(this Portal portal, Vector3 direction)
		=> PortalUtils.GetAbsoluteDirection(direction, portal.transform, portal.other.transform);
		
	//-- Matrix: --------------
	public static Matrix4x4 TransformMatrix(this Portal portal, Matrix4x4 matrix)
		=> PortalUtils.GetWorldToPortalMatrix(matrix, portal.transform, portal.other.transform);
}