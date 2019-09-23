using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class ComputeKernel
{
	public static ComputeKernel GetKernelInstance(string csName, string kernelName, Vector3Int threads)
	{
		var cs = GameObject.Instantiate<ComputeShader>(Finder.Find<ComputeShader>(csName));
		return new ComputeKernel(kernelName, cs, threads);
	}
}
