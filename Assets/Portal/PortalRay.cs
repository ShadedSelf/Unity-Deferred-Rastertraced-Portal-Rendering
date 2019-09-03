using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

// using Unity.Burst;
// using Unity.Jobs;
// using Unity.Collections;

using Unity.Mathematics;
using static Unity.Mathematics.math;


public class Pool<T> where T : new()
{
	private ConcurrentBag<T> obs = new ConcurrentBag<T>();

	public int count => obs.Count;

	public void Release(T ob)
	{
		obs.Add(ob);
	}

	public T Get()
	{
		if (obs.TryTake(out var ob))
			return ob;
		return new T();
	}

	// public void Flood(int n)
	// {
		
	// }
}

public static class PortalRay
{
	private static Pool<Node<Portal>> pool = new Pool<Node<Portal>>();

	//-- Pool Management: ---------
	private static void ClearAndRelease(Node<Portal> node)
	{
		node.children.Clear();
		node.parent = null;
		pool.Release(node);
	}

	public static void ClearAndReleaseRecursive(Node<Portal> root)
	{
		if (root == null)
			return;

		foreach (var node in root.children)
			ClearAndReleaseRecursive(node);
		ClearAndRelease(root);
	}

	private static Node<Portal> GetPooledNode(Portal data)
	{
		var node = pool.Get();
		node.data = data;
		return node;
	}

	//-- Raycast: ----------------
	public static bool CastPortalRay(Ray ray, out Node<Portal> head)
	{
		var tmp = pool.Get();
		bool hitPortal = Cast(ray, tmp, Portal.maxRecursions);
		if (hitPortal)
		{
			head = tmp.children[0];
			head.parent = null;
		}
		else
			head = null;
		ClearAndRelease(tmp);
		return hitPortal;
	}

	private static bool Cast(Ray ray, Node<Portal> parent, int recursions)
	{
		if (recursions > 0 && Physics.Raycast(ray, out var hit) && hit.collider.CompareTag("Portal"))
		{
			var portal = hit.collider.GetComponent<PortalManager>().port;
			parent.Insert(GetPooledNode(portal));

			Vector3 vPos = portal.TransformPosition(hit.point);
			Vector3 vDir = portal.TransformDirection(ray.direction);

			ray = new Ray(vPos + vDir * 0.001f, vDir); //Query hit backfaces or linecast

			return Cast(ray, parent.children[0], recursions - 1) || true;
		}
		return false;
	}

	//-- Map: --------------------
	private static bool HasPortalInChildren(Node<Portal> treeNode, Node<Portal> rayNode, out Node<Portal> outNode)
	{
		outNode = null;
		foreach (var node in treeNode.children)
			if (node.data.id == rayNode.data.id)
				outNode = node;
		return outNode != null;
	}

	public static void MergeRayPath(Node<Portal> treeNode, Node<Portal> rayNode)
	{
		if (!HasPortalInChildren(treeNode, rayNode, out var n))
		{
			if (rayNode.parent != null)
				ClearAndRelease(rayNode.parent);
			treeNode.Insert(rayNode);
		}
		else if (rayNode.children.Count > 0)
			MergeRayPath(n, rayNode.children[0]);
	}

	public static Node<Portal> GetPortalMap(Camera cam) // Allocations...
	{
		var map = pool.Get();
		const int res = 32;

		for	(int i = 0; i < res; i++)
		{	for	(int j = 0; j < res; j++)
			{
				Vector3 uv = new Vector3((float)i / (float)(res - 1), (float)j / (float)(res - 1), 0);
				Ray ray = cam.ViewportPointToRay(uv);
					
				if (PortalRay.CastPortalRay(ray, out var rayPath))
					MergeRayPath(map, rayPath);
			}
		}
		return map;
	}





	// public static void Hmm(Camera cam)
    // {
    //     const int res = 32;

    //     var results = new NativeArray<RaycastHit>(res * res, Allocator.TempJob);
    //     var commands = new NativeArray<RaycastCommand>(res * res, Allocator.TempJob);


	// 	for	(int i = 0; i < res; i++)
	// 	{	for	(int j = 0; j < res; j++)
	// 		{
	// 			Vector3 uv = new Vector3((float)i / (float)(res - 1), (float)j / (float)(res - 1), 0);
	// 			Ray ray = cam.ViewportPointToRay(uv);
					
	// 			commands[j * res + i] = new RaycastCommand(ray.origin, ray.direction);
	// 		}
	// 	}

    //     JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, 1, default(JobHandle));
    //     handle.Complete();

    //     results.Dispose();
    //     commands.Dispose();
    // }
}
