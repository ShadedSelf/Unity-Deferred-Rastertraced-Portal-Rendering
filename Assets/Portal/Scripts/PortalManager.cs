using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;
using static Unity.Mathematics.math;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class PortalManager : MonoBehaviour
{
	public GameObject connectedPortal;
	public Portal portal;

	void OnEnable()
	{
		portal = new Portal(gameObject);

		// GetComponent<BoxCollider>().convex		= true;
		GetComponent<BoxCollider>().isTrigger	= true;

		var other = connectedPortal.GetComponent<PortalManager>().portal;
		if (other != null)
		{
			portal.other = other;
			other.other = portal;
		}
	}

	void OnTriggerStay(Collider thing) // Find better way, also modify how camera can get to the actual plane
	{
		IPortableObject ob = thing.GetComponent<IPortableObject>();

		float d = ob.scale / transform.lossyScale.x;

		// if (ob != null && transform.InverseTransformPoint(ob.center).z >= -0.1f)
		if (ob != null && transform.InverseTransformPoint(ob.center).z >= -0.5f * d)
		{
			Vector3 localPos = transform.InverseTransformPoint(ob.transform.position);
			ob.transform.position = transform.TransformPoint(new Vector3(localPos.x, localPos.y, 0)); //Hmm

			portal.Send(ob, new Vector3(0, 0, 0.51f * d));
		}

		// if (ob is IPortableMesh)
		// {
		// 	port.DrawMesh(ob as IPortableMesh);
		// }
	}

	void OnDisable()
	{
		portal.Dispose();
	}

	// void OnDrawGizmos()
	// {
	// 	Gizmos.color = Color.blue;

	// 	var bound = GetComponent<Collider>().bounds;
	// 	Gizmos.DrawWireCube(bound.center, bound.size);
	// }
}

public static class AAA
{
	public static bool IntersectsPortalBounds(this IPortableObject ob, Portal portal)
	{
		var oBounds = ob.rb.GetComponent<Collider>().bounds;
		var pBounds = portal.collider.bounds;

		return pBounds.Intersects(oBounds);
	}

	// public static bool IntersectsPortal(this IPortableObject ob, Portal portal)
	// {
	// 	if (ob.IntersectsPortal(portal))
	// 	{
	// 		Physics.ov

	// 		return ob.rb.GetComponent<Collider>().is
	// 	}
	// }
}