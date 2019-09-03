using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;
using static Unity.Mathematics.math;

/*-- TODO: --
Remove the 180 y offset
-----------*/

public class Portal : IDisposable//, IEquatable<Portal>
{
	public static List<Portal> portals = new List<Portal>();
	public static readonly int maxRecursions = 4;

	public Portal other			{ get; set; }
	public Transform transform	{ get; private set; }
	public int id				{ get; private set; }

	public Renderer renderer => transform.GetComponent<Renderer>();
	public Collider collider => transform.GetComponent<Collider>();

	public Portal(Transform transform, MeshRenderer meshRenderer)
	{
		portals.Add(this);
		id = portals.Count;
		this.transform = transform;

		meshRenderer.sharedMaterial = new Material(Shader.Find("UV/UV Remap"));
		meshRenderer.sharedMaterial.SetFloat("_ID", id);
	}

	// public void OnBecomeVisible

	public void Send(IPortableObject viewer, Vector3 offset) // Utils: Get transfor, roation, scale, offset multipliers
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

	//-- Interfaces: --
	public void Dispose()				=> portals.Remove(this);
	// public bool Equals(Portal other)	=> other.id == id;
}
