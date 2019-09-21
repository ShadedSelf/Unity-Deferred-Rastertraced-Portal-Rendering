using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;
using static Unity.Mathematics.math;

/*-- TODO: --
Remove the 180 y offset?
-----------*/

public class Portal : IDisposable
{
	//-- Globals: -----------
	public static List<Portal> portals = new List<Portal>();
	public static readonly int maxRecursions = 4;

	//-- Properties: --------
	public Portal other			{ get; set; }
	public int id				{ get; private set; }
	
	public Transform transform	{ get; private set; }
	public Renderer renderer	{ get; private set; } 
	public Collider collider	{ get; private set; }

	public Portal(GameObject go)
	{
		portals.Add(this);
		id = this.GetHashCode();

		transform	= go.GetComponent<Transform>();
		renderer	= go.GetComponent<Renderer>();
		collider	= go.GetComponent<Collider>();

		var mat = go.GetComponent<MeshRenderer>().material;
		mat.SetFloat("_ID", id);
	}

	public void Send(IPortableObject viewer, Vector3 offset) // Todo, Utils: Get transform, rotation, scale, offset multipliers
	{
		viewer.transform.position	= this.TransformPosition(viewer.transform.position, offset);
		viewer.transform.rotation	= this.TransformRotation(viewer.transform.rotation);
		viewer.down					= this.TransformDirection(viewer.down);
		viewer.rb.velocity			= this.TransformDirection(viewer.rb.velocity) * length(viewer.rb.velocity) * (other.transform.lossyScale.x / transform.lossyScale.x);
		viewer.scale				= viewer.scale * (other.transform.lossyScale.x / transform.lossyScale.x);
	}

	public void DrawMesh(IPortableMesh viewer)
	{
		Vector3		pos = this.TransformPosition(viewer.transform.position);
		Quaternion	rot = this.TransformRotation(viewer.transform.rotation);

		Graphics.DrawMesh(viewer.mesh, pos, rot, viewer.mat, 0);
	}

	// public void OnBecomeVisible() { }

	//-- Interfaces: --
	public void Dispose() => portals.Remove(this);
}
