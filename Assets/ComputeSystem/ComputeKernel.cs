using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ComputeKernel
{
	public ComputeShader shader	{ get; private set; }
	public string name			{ get; private set; }
	public int index			{ get; private set; }

	private Vector3Int _threads;
	private Vector3Int _groupSizes;

	public ComputeKernel(string name, ComputeShader shader, Vector3Int threads)
	{
		this.shader	= shader;
		this.name	= name;
		this.index	= shader.FindKernel(name);
		_threads	= threads;
		
		shader.GetKernelThreadGroupSizes(index, out uint x, out uint y, out uint z);
		_groupSizes = new Vector3Int((int)x, (int)y, (int)z);
	}

	//-- Set: ------------
	public void SetBuffer(string bufferName, ComputeBuffer buffer)	=> shader.SetBuffer(index, bufferName, buffer);
	public void SetTexture(string textureName, Texture texture)		=> shader.SetTexture(index, textureName, texture);

	//-- Dispatch: -------- 
	public void Dispacth()
	{
		shader.Dispatch(index, 
			_threads.x / _groupSizes.x, 
			_threads.y / _groupSizes.y, 
			_threads.z / _groupSizes.z);
	}

	public void RecordDispatch(CommandBuffer cmdBuff, bool profile)
	{
		if (profile) { cmdBuff.BeginSample(name); }
		
		cmdBuff.DispatchCompute(shader, index, 
			_threads.x / _groupSizes.x, 
			_threads.y / _groupSizes.y, 
			_threads.z / _groupSizes.z);
			
		if (profile) { cmdBuff.EndSample(name); }
	}
}
