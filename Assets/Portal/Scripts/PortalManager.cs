using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;
using static Unity.Mathematics.math;

public class PortalManager : MonoBehaviour
{
	public GameObject otherPort;
	public Portal port;

	void OnEnable()
	{
		port = new Portal(transform, GetComponent<MeshRenderer>());

		GetComponent<MeshCollider>().convex		= true;
		GetComponent<MeshCollider>().isTrigger	= true;
	}

	void Update() //AAA	
	{
		port.other = otherPort.GetComponent<PortalManager>().port;
	}

	void OnTriggerStay(Collider thing) // Find better way, also modify how camera can get to the actual plane
	{
		IPortableObject ob = thing.GetComponent<IPortableObject>();

		if (ob != null && transform.InverseTransformPoint(ob.center).z >= -0.1f)
		{
			Vector3 localPos = transform.InverseTransformPoint(ob.transform.position);
			ob.transform.position = transform.TransformPoint(new Vector3(localPos.x, localPos.y, 0)); //Hmm

			port.Send(ob, new Vector3(0, 0, 0.11f));
		}

		// if (ob is IPortableMesh)
		// {
		// 	port.DrawMesh(ob as IPortableMesh);
		// }
	}

	void OnDisable()
	{
		port.Dispose();
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