using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ComputeSystem
{
	private Dictionary<string, ComputeKernel> _kernels	= new Dictionary<string, ComputeKernel>();
	private ComputeData _data = new ComputeData();
	private ComputeShader _shader;
	private bool _profile;

	public ComputeShader shader	=> _shader;
	public ComputeData data	=> _data;

	public ComputeSystem(ComputeShader shader, bool profile = false)
	{
		_shader		= shader;
		_profile	= profile;
	}

	//-- Add: -----------------------------------------------------------------------------------------------------------------------
	public void AddKernel(string name, Vector3Int threads) => _kernels.Add(name, new ComputeKernel(name, _shader, threads));

	//-- Set: -----------------------------------------------------------------------------------------------------------------------
	public void BindComputeData() => BindComputeData(_data);
	public void BindComputeData(ComputeData data)
	{
		foreach (var kernel in _kernels.Values)
		{
			foreach (var buffer in data.buffers)	{ kernel.SetBuffer(buffer.Key, buffer.Value);		}
			foreach (var texture in data.textures)	{ kernel.SetTexture(texture.Key, texture.Value);	}
		}
	}

	public void SetBuffer(string kernel, string bufferName)								=> _kernels[kernel].SetBuffer(bufferName, _data.buffers[bufferName]);
	public void SetBuffer(string kernel, string bufferName, ComputeBuffer buffer)		=> _kernels[kernel].SetBuffer(bufferName, buffer);
	// SetBufferRange

	public void SetRenderTexture(string kernel, string textureName)						=> _kernels[kernel].SetTexture(textureName, _data.textures[textureName]);
	public void SetRenderTexture(string kernel, string textureName, RenderTexture rt)	=> _kernels[kernel].SetTexture(textureName, rt);
	// SetRenderTextureRange

	//-- Dispatch: ------------------------------------------------------------------------------------------------------------------
	public void Dispatch(string kernel)								=> _kernels[kernel].Dispacth();
	public void RecordDispatch(string kernel, CommandBuffer cmdBuf) => _kernels[kernel].RecordDispatch(cmdBuf, _profile);

	//-- Clean: ---------------------------------------------------------------------------------------------------------------------
	public void Cleanup()
	{
		_data.Cleanup();
		_kernels.Clear();
	}
}
