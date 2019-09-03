using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

using static UnityEngine.Mathf;

public static class Finder
{
	public static T Find<T>(string name) where T : UnityEngine.Object
	{
		T[] results = (T[])Resources.FindObjectsOfTypeAll(typeof(T));
		
		foreach (var result in results)
			if (result.name == name)
				return result;
		return null;
	}
}

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


public enum RenderTargetID
{
	RenderIDs,
	EdgeBuffer,
}

public class RenderSystemTest
{
	public RenderTexture render { get; private set; }

	private Camera cam;

	private MRTCamera<RenderTargetID> mrtCam;

	public ComputeSystem computeSystem;

	public static ComputeKernel mergeTexture;
	public static ComputeKernel mergeBuffer;
	public static ComputeKernel mergeColor;
	public static ComputeKernel clearColor;
	public static ComputeKernel mergePIDs;


	public Dictionary<string, ComputeBuffer> deferredBuffers = new Dictionary<string, ComputeBuffer>();

	public static readonly string EDGE_BUFFER = "EdgeBuffer";
	public static readonly string PIDS_BUFFER = "PIDsBuffer";

	static RenderSystemTest()
	{
		mergeTexture = GetComputeKernel("MergeTexture", "MergeTexture", new Vector3Int(Screen.width, Screen.height, 1));
		mergeBuffer = GetComputeKernel("MergeBuffer", "MergeBuffer", new Vector3Int(Screen.width, Screen.height, 1));
		mergeColor = GetComputeKernel("MergeColor", "MergeColor", new Vector3Int(Screen.width, Screen.height, 1));
		clearColor = GetComputeKernel("MergeColor", "ClearColor", new Vector3Int(Screen.width, Screen.height, 1));
		mergePIDs = GetComputeKernel("MergePIDs", "MergePIDs", new Vector3Int(Screen.width, Screen.height, 1));
	}
	public static ComputeKernel GetComputeKernel(string csName, string kernelName, Vector3Int threads)
	{
		var cs = GameObject.Instantiate<ComputeShader>(Finder.Find<ComputeShader>(csName));
		return new ComputeKernel(kernelName, cs, threads);
	}


	public RenderSystemTest(Camera cam)
	{
		this.cam = cam;

		var computeShader = GameObject.Instantiate<ComputeShader>(Finder.Find<ComputeShader>("RenderTest"));

		computeSystem = new ComputeSystem(computeShader);

		mrtCam = new MRTCamera<RenderTargetID>(cam, Shader.Find("Test/Test"));
		mrtCam.GatherBuffers();

		render = GpuUtils.GetScreenRenderTexture(0, true);

		deferredBuffers.Add(EDGE_BUFFER, new ComputeBuffer(Screen.width * Screen.height, sizeof(float) * 4));
		deferredBuffers.Add(PIDS_BUFFER, new ComputeBuffer(Screen.width * Screen.height, sizeof(float) * 4)); //int

		Set();
	}

	private void Set()
	{
		computeSystem.data.AddRenderTexture("_Result", render);

		computeSystem.data.AddBuffer("normals", deferredBuffers[EDGE_BUFFER]);
		computeSystem.data.AddBuffer("pids", deferredBuffers[PIDS_BUFFER]);
		computeSystem.data.AddBuffer("portalInfo", 8, sizeof(uint));
		
		computeSystem.AddKernel("FindEdges", new Vector3Int(Screen.width, Screen.height, 1));
		computeSystem.AddKernel("FindPortals", new Vector3Int(Screen.width, Screen.height, 1));
		computeSystem.AddKernel("ResetPortals", new Vector3Int(1, 1, 1));

		computeSystem.BindComputeData();

		computeSystem.shader.SetInt("width", Screen.width);
	}


	//-- Kernels: -- [
	// public static void MergeBuffer(ComputeBuffer src, ComputeBuffer dst, Texture condition, int id)
	// {
	// 	mergeBuffer.SetBuffer("src", src);				//Check setting cost
	// 	mergeBuffer.SetBuffer("dst", dst);				//Same
	// 	mergeBuffer.SetTexture("condition", condition); //Etc...
	// 	mergeBuffer.shader.SetInt("mergeID", id);
	// 	mergeBuffer.shader.SetInt("width", Screen.width);
	// 	mergeBuffer.Dispacth();
	// }

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

	public static void MergePortalIDs(ComputeBuffer buffer, Texture pids, int prevPID, int depth)
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
		MergeTexture(mrtCam[RenderTargetID.EdgeBuffer], deferredBuffers[EDGE_BUFFER], deferredBuffers[PIDS_BUFFER], prevPID, depth);
		MergePortalIDs(deferredBuffers[PIDS_BUFFER], mrtCam[RenderTargetID.RenderIDs], prevPID, depth);		
	}

	public void RenderPortalMap(Transform trs, Node<Portal> map, int depth, Matrix4x4 matrix, int prevPID)
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
				float testN = trs.lossyScale.x * (portal.other.transform.lossyScale.x / portal.transform.lossyScale.x);
				cam.projectionMatrix = cam.CalculateObliqueMatrix(cam.CameraSpacePlane(portal.other.transform.position, portal.other.transform.forward, -1, 0.09f * testN));

				ContinueRender(vPos, vRot, trs.localScale, vMat, prevPID, depth);
			}
			else
				ContinueRender(trs.position, trs.rotation, trs.localScale, matrix, prevPID, depth);

			void ContinueRender(Vector3 lpos, Quaternion lrot, Vector3 lscale, Matrix4x4 lmatrix, int lprevPID, int ldepth) // Check sacle for oblique plane
			{ 
				Render(ldepth, lprevPID, lmatrix);
				foreach (var node in map.children)
					using (var dummyTrs = new TemporaryTransform(lpos, lrot, lscale))
						RenderPortalMap(dummyTrs.transform, node, ldepth + 1, lmatrix, node.data.id);
			} 

		}
	}

	public void LazyRender(Transform trs, int depth, Matrix4x4 matrix, int prevPID)
	{
		if (depth <= Portal.maxRecursions)
		{
			Render(depth, prevPID, matrix);

			foreach (var portal in Portal.portals)
			{
				if (portal.IsVisibleFromCam(cam))
				{
					Vector3		vPos = portal.TransformPosition(trs.position);
					Quaternion	vRot = portal.TransformRotation(trs.rotation);
					Matrix4x4	vMat = portal.TransformMatrix(matrix);

					cam.transform.SetPositionAndRotation(vPos, vRot);
					cam.ResetProjectionMatrix();
					float testN = trs.lossyScale.x * (portal.other.transform.lossyScale.x / portal.transform.lossyScale.x);
					cam.projectionMatrix = cam.CalculateObliqueMatrix(cam.CameraSpacePlane(portal.other.transform.position, portal.other.transform.forward, -1, 0.09f * testN));

					using (var dummyTrs = new TemporaryTransform(trs.position, trs.rotation, trs.localScale))
						LazyRender(dummyTrs.transform, depth + 1, vMat, portal.id);
				}
			}
		}
	}

	public void Edge()
	{
		computeSystem.Dispatch("FindEdges");
	}
	//-- Rendering: -- ]

	// private int[] _portalCount = new int[8];
	// public int[] portalCount { get { UpdateVisiblePortalCount(); return _portalCount; } }

	private int[] _portalCountTmp = new int[8];
	private List<Portal> _visiblePortals = new List<Portal>();
	public List<Portal> visiblePortals => PortalSomething.GetVisiblePortals(cam, PortalCheckType.CpuLazy);





	private void UpdateVisiblePortalList()
	{
		_visiblePortals.Clear();
		for	(int i = 0; i < _portalCountTmp[0]; i++)
		{
			int pID = _portalCountTmp[i + 1];
			var portal = Portal.portals[pID - 1];
			_visiblePortals.Add(portal);
		}
	}

	//-- SyncReadback: --
	// public void UpdateVisiblePortalCount()
	// {
	// 	computeSystem.Dispatch("FindPortals");
	// 	computeSystem.data.Buffers["portalInfo"].GetData(_portalCountTmp);
	// 	computeSystem.Dispatch("ResetPortals");

	// 	UpdateVisiblePortalList();
	// }


	//-- AsyncReadback: --
	// Queue<AsyncGPUReadbackRequest> _requests = new Queue<AsyncGPUReadbackRequest>();

	// public void UpdateVisiblePortalCount()
	// {
	// 	computeSystem.Dispatch("FindPortals");

	// 	_requests.Enqueue(AsyncGPUReadback.Request(computeSystem.data.Buffers["portalInfo"]));

	// 	while (_requests.Count > 0)
    //     {
	// 		var req = _requests.Peek();

    //         if (req.hasError)
    //         {
	// 			Debug.Log("GPU readback error detected.");
	// 			 _requests.Dequeue();
    //         }
    //         else if (req.done)
    //         {
    //             var buffer = req.GetData<int>();
	// 			buffer.CopyTo(_portalCount);
	// 			 _requests.Dequeue();
	// 			break;
    //         }
    //         else break;
    //     }

	// 	computeSystem.Dispatch("ResetPortals");
	// }
}

public enum PortalCheckType
{
	//-- CPU: --
	CpuAccurate	= 0,
	CpuLazy		= 1,
	//-- GPU: --
	GpuSync		= 2,
	GpuAsyc		= 3,
}

public static class PortalSomething
{
	public static Vector3 TrueVieportToWorldPoint(this Camera cam, Vector3 viewPoint) // 0 to 1
	{
		Matrix4x4 m = cam.projectionMatrix * cam.worldToCameraMatrix;
		Vector4 v = new Vector4(viewPoint.x * 2f - 1f, viewPoint.y * 2f - 1f, viewPoint.z * 2f - 1f, 1f);
		v = m.inverse * v;
		v /= v.w;
		return new Vector3(v.x, v.y, v.z);
	}

	public static List<Portal> GetVisiblePortals(Camera camera, PortalCheckType checkType)
	{
		var portals = new List<Portal>();

		switch (checkType)
		{
			case PortalCheckType.CpuAccurate	: GetCpuAccurateList(camera, portals);	break;
			case PortalCheckType.CpuLazy		: GetCpuLazyList(camera, portals);		break;
		}
		return portals;
	}

	//-- CPU Acurate, ish: --
	private static void GetCpuAccurateList(Camera cam, List<Portal> list)
	{
		foreach (var portal in Portal.portals)
			if (portal.IsVisibleFromCam(cam))
				list.Add(portal);
	}

	// const int density = 5;
	//Make rays go through
	public static bool IsVisibleFromCam(this Portal portal, Camera cam) //TODO: Optimize, Multithread?, density relative to view angle?, check around previous hitPoint first?
	{
		if (!portal.renderer.IsVisibleFrom(cam)
		|| portal.transform.InverseTransformPoint(cam.transform.position).z > 0f)
			return false;

		// Vector3 dumb = (portal.transform.position - cam.transform.position).normalized;
		// float density = Abs(5f * Vector3.Dot(dumb, portal.transform.forward));
		float density = 5;

		Vector3 p = portal.transform.position;
		Vector3 up = portal.transform.up;
		Vector3 right = portal.transform.right;
		Vector3 s = portal.transform.lossyScale * 0.5f;
		Vector2Int n = new Vector2Int(RoundToInt(s.x * density), RoundToInt(s.y * density));

		// var rng = new RandomIndexer(n.x * 2 + 1, n.y * 2 + 1);
		// rng.Shuffle();
		// Vector2Int index = rng.Next();
		// Vector2 uv = new Vector2((index.x - n.x) / (float)n.x, (index.y - n.y) / (float)n.y);

		for (int i = -n.x; i < n.x + 1; i++)
		{	for (int j = -n.y; j < n.y + 1; j++)
			{
				Vector2 uv = new Vector2(i / (float)n.x, j / (float)n.y);
				Vector3 castPoint = p + (right * uv.x * s.x) + (up * uv.y * s.y);
				Vector3 viewPoint = cam.WorldToViewportPoint(castPoint); //z prob fucked

				if(viewPoint.x > 1f
				|| viewPoint.x < 0f
				|| viewPoint.y > 1f
				|| viewPoint.y < 0f
				|| viewPoint.z < 0
				) continue;

				Vector3 startPoint = cam.TrueVieportToWorldPoint(new Vector3(viewPoint.x, viewPoint.y, 0f));
				// startPoint = cam.transform.position;
				if (Physics.Raycast(startPoint, (castPoint - startPoint).normalized, out var hit)) //add max dist
					if (hit.collider.CompareTag("Portal"))
						return true;
			}
		}
		return false;
	}

	//-- CPU Lazy: --
	private static void GetCpuLazyList(Camera cam, List<Portal> list)
	{
		foreach (var portal in Portal.portals)
			if (portal.renderer.IsVisibleFrom(cam))
				list.Add(portal);
	}

	// 	private void UpdateVisiblePortalList()
	// {
	// 	_visiblePortals.Clear();
	// 	for	(int i = 0; i < _portalCountTmp[0]; i++)
	// 	{
	// 		int pID = _portalCountTmp[i + 1];
	// 		var portal = Portal.portals[pID - 1];
	// 		_visiblePortals.Add(portal);
	// 	}
	// }

	// //-- GPU SyncReadback: --
	// public void UpdateVisiblePortalCount()
	// {
	// 	computeSystem.Dispatch("FindPortals");
	// 	computeSystem.data.Buffers["portalInfo"].GetData(_portalCountTmp);
	// 	computeSystem.Dispatch("ResetPortals");

	// 	UpdateVisiblePortalList();
	// }

}

public static class ArrayExtensions
{
	public static void Shuffle<T>(this T[] array)  
	{
		var rng = new System.Random(); 
		for (int i = 0; i < array.Length; i++) // - 1?
		{  
			int n = rng.Next(array.Length - 1);  
			T tmp		= array[n];  
			array[n]	= array[i];  
			array[i]	= tmp;  
		}  
	}
}


public class RandomIndexer //Tuples?
{
	private Vector2Int[] indices;

	private int c;

	public RandomIndexer(int x, int y)
	{
		indices = new Vector2Int[x * y];

		for (int i = 0; i < x; i++)
			for (int j = 0; j < y; j++)
				indices[j + (i * y)] = new Vector2Int(i, j);
	}

	public void Shuffle()
	{
		c = 0;
		indices.Shuffle();
	}

	public Vector2Int Next()
	{
		return indices[c++];
	}
}