using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeData
{
	private Dictionary<string, ComputeBuffer> _buffers	= new Dictionary<string, ComputeBuffer>();
	private Dictionary<string, RenderTexture> _textures	= new Dictionary<string, RenderTexture>();

	public IReadOnlyDictionary<string, ComputeBuffer> buffers	=> _buffers as IReadOnlyDictionary<string, ComputeBuffer>;
	public IReadOnlyDictionary<string, RenderTexture> textures	=> _textures as IReadOnlyDictionary<string, RenderTexture>;

	//-- Add: -----------------
	public void AddBuffer(string name, int count, int stride)				=> _buffers.Add(name, new ComputeBuffer(count, stride));
	public void AddBuffer(string name, ComputeBuffer buffer)				=> _buffers.Add(name, buffer);

	public void AddRenderTexture(string name, RenderTextureDescriptor desc)	=> _textures.Add(name, new RenderTexture(desc));
	public void AddRenderTexture(string name, RenderTexture rt)				=> _textures.Add(name, rt);

	//-- Clean: ---------------
	public void Cleanup()
	{
		foreach (var buffer in _buffers)	{ buffer.Value.Release();	}
		foreach (var texture in _textures)	{ texture.Value.Release();	}
		_buffers.Clear();
		_textures.Clear();
	}
}

// public enum GpuResourceType
// {
// 	Buffer,
// 	RenderTexture,
// }

// public class GpuResource
// {
// 	public GpuResourceType type	{ get; private set; }
// 	public string name			{ get; private set; }
// 	public bool isFreed			{ get; private set; } = false;

// 	private object resource;

// 	private GpuResource(object resource, GpuResourceType type)
// 	{
// 		 this.resource = resource;
// 		 this.type = type;
// 	}
// 	public GpuResource(ComputeBuffer resource)	: this(resource, GpuResourceType.Buffer)		{ }
// 	public GpuResource(RenderTexture resource)	: this(resource, GpuResourceType.RenderTexture)	{ }

// 	public void Bind(ComputeShader shader, int kernelIndex)
// 	{
// 		switch (type)
// 		{
// 			case (GpuResourceType.Buffer)			: shader.SetBuffer(kernelIndex, name, (ComputeBuffer)resource); break;
// 			case (GpuResourceType.RenderTexture)	: shader.SetTexture(kernelIndex, name, (RenderTexture)resource); break;
// 		}
// 	}

// 	public void Release()
// 	{
// 		switch (type)
// 		{
// 			case (GpuResourceType.Buffer)			: ((ComputeBuffer)resource).Release(); break;
// 			case (GpuResourceType.RenderTexture)	: ((RenderTexture)resource).Release(); break;
// 		}
// 		isFreed = true;
// 	}
// }