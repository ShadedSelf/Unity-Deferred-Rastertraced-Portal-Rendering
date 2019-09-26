using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;
using static Unity.Mathematics.math;

public class TransformData
{
	public Vector3 position		{ get; set; } = Vector3.zero;
	public Quaternion rotation	{ get; set; } = Quaternion.identity;
	public Vector3 scale		{ get; set; } = Vector3.one;

	public TransformData() { }
	public TransformData(Transform transform) : this(transform.position, transform.rotation, transform.lossyScale) { }
	public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
	{
		this.position	= position;
		this.rotation	= rotation;
		this.scale		= scale;
	}
}