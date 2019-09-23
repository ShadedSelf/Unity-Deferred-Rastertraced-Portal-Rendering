using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

using Unity.Mathematics;
using static Unity.Mathematics.math;

public static partial class GpuUtils
{
	public static RenderTexture GetRenderTexture(int width, int height, int depth, bool writable = false, RenderTextureFormat format = RenderTextureFormat.ARGBFloat)
	{
		var rt = new RenderTexture(width, height, depth, format);
		rt.enableRandomWrite = writable;
		rt.Create();
		return rt;
	}

	public static RenderTexture GetScreenRenderTexture(int depth, bool writable = false, RenderTextureFormat format = RenderTextureFormat.ARGBFloat)
	{
		return GetRenderTexture(Screen.width, Screen.height, depth, writable, format);
	}
}

public enum DefferredBufferID
{
	TemporayEditor,
	Normals,
	Position,
	RenderIDs,
}

public class RenderSystemTest
{
	public RenderTexture render { get; private set; }

	private Camera cam;
	private MRTCamera<DefferredBufferID> mrtCam;
	
	private ComputeSystem computeSystem;
	private Dictionary<DefferredBufferID, ComputeBuffer> deferredBuffers = new Dictionary<DefferredBufferID, ComputeBuffer>();

	public static ComputeKernel mergeTexture, clearColor, mergePIDs;

	static RenderSystemTest()
	{
		mergeTexture = ComputeKernel.GetKernelInstance("MergeTexture", "MergeTexture", new Vector3Int(Screen.width, Screen.height, 1));
		clearColor = ComputeKernel.GetKernelInstance("MergeColor", "ClearColor", new Vector3Int(Screen.width, Screen.height, 1));
		mergePIDs = ComputeKernel.GetKernelInstance("MergePIDs", "MergePIDs", new Vector3Int(Screen.width, Screen.height, 1));
	}

	public RenderSystemTest()
	{
		cam = new GameObject().AddComponent<Camera>();
		cam.CopyFrom(Controller.currentCamera);
		cam.enabled = false;

		var computeShader = GameObject.Instantiate<ComputeShader>(Finder.Find<ComputeShader>("RenderTest"));

		computeSystem = new ComputeSystem(computeShader);

		mrtCam = new MRTCamera<DefferredBufferID>(cam);
		// mrtCam = new MRTCamera(cam, new DefferredBufferID());
		mrtCam.GatherBuffers();

		render = GpuUtils.GetScreenRenderTexture(0, true);

		deferredBuffers.Add(DefferredBufferID.Normals, new ComputeBuffer(Screen.width * Screen.height, sizeof(float) * 4));
		deferredBuffers.Add(DefferredBufferID.Position, new ComputeBuffer(Screen.width * Screen.height, sizeof(float) * 4));
		deferredBuffers.Add(DefferredBufferID.RenderIDs, new ComputeBuffer(Screen.width * Screen.height, sizeof(float) * 4)); //int

		Set();
	}

	private void Set()
	{
		computeSystem.data.AddRenderTexture("_Result", render);

		computeSystem.data.AddBuffer("normals", deferredBuffers[DefferredBufferID.Normals]);
		computeSystem.data.AddBuffer("position", deferredBuffers[DefferredBufferID.Position]);
		computeSystem.data.AddBuffer("pids", deferredBuffers[DefferredBufferID.RenderIDs]);
		
		computeSystem.AddKernel("FindEdges", new Vector3Int(Screen.width, Screen.height, 1));

		computeSystem.BindComputeData();

		computeSystem.shader.SetInt("width", Screen.width);
	}


	//-- Kernels: -- [
	public static void MergeTexture(Texture src, ComputeBuffer dst, ComputeBuffer condition, int prevPID, int depth)
	{
		mergeTexture.SetTexture("src", src);
		mergeTexture.SetBuffer("dst", dst);
		mergeTexture.SetBuffer("condition", condition);
		mergeTexture.shader.SetInt("width", Screen.width);
		mergeTexture.shader.SetInt("prevPID", prevPID);
		mergeTexture.shader.SetInt("depth", depth);
		mergeTexture.Dispacth();
	}

	public static void ClearColor(ComputeBuffer buffer, Vector4 color)
	{
		clearColor.SetBuffer("dst", buffer);
		clearColor.shader.SetInt("width", Screen.width);
		clearColor.shader.SetVector("color", color);
		clearColor.Dispacth();
	}

	public static void MergePortalIDs(Texture pids, ComputeBuffer buffer, int prevPID, int depth)
	{
		mergePIDs.SetBuffer("buff", buffer);
		mergePIDs.SetTexture("pids", pids);
		mergePIDs.shader.SetInt("width", Screen.width);
		mergePIDs.shader.SetInt("prevPID", prevPID);
		mergePIDs.shader.SetInt("depth", depth);
		mergePIDs.Dispacth();
	}
	//-- Kennels: -- ]


	public void ClearBuffers()
	{
		foreach (var buffer in deferredBuffers.Values)
			ClearColor(buffer, Vector4.zero);
	}

	//-- Rendering: -- [
	private void Render(int depth, int prevPID, Matrix4x4 matrix)
	{
		Shader.SetGlobalMatrix("portalTransform", matrix);

		mrtCam.Render();
		MergeTexture(mrtCam[DefferredBufferID.Normals], deferredBuffers[DefferredBufferID.Normals], deferredBuffers[DefferredBufferID.RenderIDs], prevPID, depth);
		MergeTexture(mrtCam[DefferredBufferID.Position], deferredBuffers[DefferredBufferID.Position], deferredBuffers[DefferredBufferID.RenderIDs], prevPID, depth);
		MergePortalIDs(mrtCam[DefferredBufferID.RenderIDs], deferredBuffers[DefferredBufferID.RenderIDs], prevPID, depth);		
	}

	public void RenderPortalMap(Node<Portal> map, TransformData trs, Matrix4x4 matrix, int depth, int prevPID)
	{
		if (depth <= Portal.maxRecursions)
		{
			var portal = map.data;

			if (map.data != null)
			{
				Vector3		vPos = portal.TransformPosition(trs.position);
				Quaternion	vRot = portal.TransformRotation(trs.rotation);
				Matrix4x4	vMat = portal.TransformMatrix(matrix);

				cam.transform.SetPositionAndRotation(vPos, vRot);
				cam.ResetProjectionMatrix();
				// float testN = trs.scale.x * (portal.other.transform.lossyScale.x / portal.transform.lossyScale.x);
				// cam.projectionMatrix = cam.CalculateObliqueMatrix(cam.CameraSpacePlane(portal.other.transform.position, portal.other.transform.forward, -1, 0.09f * testN));
				cam.projectionMatrix = cam.CalculateObliqueMatrix(cam.CameraSpacePlane(portal.other.transform.position, portal.other.transform.forward, -1, 0f));

				ContinueRender(new TransformData(vPos, vRot, trs.scale), vMat, prevPID, depth);
			}
			else
			{
				ContinueRender(trs, matrix, prevPID, depth);
			}

			void ContinueRender(TransformData ltrs, Matrix4x4 lmatrix, int lprevPID, int ldepth) // Check sacle for oblique plane
			{ 
				Render(ldepth, lprevPID, lmatrix);
				foreach (var node in map.children)
					RenderPortalMap(node, ltrs, lmatrix, ldepth + 1, node.data.id);
			} 
		}
	}

	public void LazyRender(TransformData trs, Matrix4x4 matrix, int depth, int prevPID)
	{
		if (depth <= Portal.maxRecursions)
		{
			Render(depth, prevPID, matrix);

			foreach (var portal in Portal.portals)
			{
				// if (portal.renderer.IsVisibleFrom(cam))
				{
					Vector3		vPos = portal.TransformPosition(trs.position);
					Quaternion	vRot = portal.TransformRotation(trs.rotation);
					Matrix4x4	vMat = portal.TransformMatrix(matrix);

					cam.transform.SetPositionAndRotation(vPos, vRot);
					cam.ResetProjectionMatrix();
					float testN = trs.scale.x * (portal.other.transform.lossyScale.x / portal.transform.lossyScale.x);
					cam.projectionMatrix = cam.CalculateObliqueMatrix(cam.CameraSpacePlane(portal.other.transform.position, portal.other.transform.forward, -1, 0.09f * testN));

					LazyRender(new TransformData(vPos, vRot, trs.scale), vMat, depth + 1, portal.id);
				}
			}
		}
	}

	public void Edge()
	{
		computeSystem.Dispatch("FindEdges");
	}
	//-- Rendering: -- ]

	public void SetInitialTransform(Transform transform)
	{
		cam.transform.position = transform.position;
		cam.transform.rotation = transform.rotation;
		cam.transform.localScale = transform.localScale;

		cam.ResetProjectionMatrix();
	}

	public void Cleanup()
	{
		computeSystem.Cleanup();
	}
}

public static class PortalSomething
{
	public static Vector3 TrueViewportToWorldPoint(this Camera cam, Vector3 viewPoint) // 0 to 1
	{
		Matrix4x4 m = cam.projectionMatrix * cam.worldToCameraMatrix;
		Vector4 v = float4(viewPoint, 1) * 2 - 1;// new Vector4(viewPoint.x * 2f - 1f, viewPoint.y * 2f - 1f, viewPoint.z * 2f - 1f, 1f);
		v = m.inverse * v;
		v /= v.w;
		return new Vector3(v.x, v.y, v.z);
	}

	// public static Vector3 TrueWorldToVieportPoint(this Camera cam, Vector3 viewPoint) // 0 to 1
	// {
	// 	Matrix4x4 m = cam.projectionMatrix * cam.worldToCameraMatrix;
	// 	Vector4 v = float4(viewPoint, 1) * 2 - 1;// new Vector4(viewPoint.x * 2f - 1f, viewPoint.y * 2f - 1f, viewPoint.z * 2f - 1f, 1f);
	// 	v = m * v;
	// 	v /= v.w;
	// 	return new Vector3(v.x, v.y, v.z);
	// }


// 	// const int density = 5;
// 	//Make rays go through
// 	public static bool IsVisibleFromCam(this Portal portal, Camera cam) //TODO: Optimize, Multithread?, density relative to view angle?, check around previous hitPoint first?
// 	{
// 		if (!portal.renderer.IsVisibleFrom(cam)
// 		|| portal.transform.InverseTransformPoint(cam.transform.position).z > 0f)
// 			return false;

// 		// Vector3 dumb = (portal.transform.position - cam.transform.position).normalized;
// 		// float density = Abs(5f * Vector3.Dot(dumb, portal.transform.forward));
// 		float density = 5;

// 		Vector3 p = portal.transform.position;
// 		Vector3 up = portal.transform.up;
// 		Vector3 right = portal.transform.right;
// 		Vector3 s = portal.transform.lossyScale * 0.5f;
// 		Vector2Int n = new Vector2Int(RoundToInt(s.x * density), RoundToInt(s.y * density));
// 		for (int i = -n.x; i < n.x + 1; i++)
// 		{	for (int j = -n.y; j < n.y + 1; j++)
// 			{
// 				Vector2 uv = new Vector2(i / (float)n.x, j / (float)n.y);
// 				Vector3 castPoint = p + (right * uv.x * s.x) + (up * uv.y * s.y);
// 				Vector3 viewPoint = cam.WorldToViewportPoint(castPoint); //z prob fucked

// 				if(viewPoint.x > 1f
// 				|| viewPoint.x < 0f
// 				|| viewPoint.y > 1f
// 				|| viewPoint.y < 0f
// 				|| viewPoint.z < 0f
// 				) continue;

// 				Vector3 startPoint = cam.TrueVieportToWorldPoint(new Vector3(viewPoint.x, viewPoint.y, 0f));
// 				// startPoint = cam.transform.position;
// 				if (Physics.Raycast(startPoint, (castPoint - startPoint).normalized, out var hit)) //add max dist
// 					if (hit.collider.CompareTag("Portal"))
// 						return true;
// 			}
// 		}
// 		return false;
// 	}
}
