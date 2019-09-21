using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GpuResourceType
{
	Buffer,
	Texture,
	// RenderTexture,
}
public class GpuResource
{
	public GpuResourceType type	{ get; private set; }
	public string name			{ get; private set; }

	private object resource;

	private GpuResource(object resource, GpuResourceType type)
	{
		 this.resource = resource;
		 this.type = type;
	}
	public GpuResource(ComputeBuffer resource)	: this(resource, GpuResourceType.Buffer)	{ }
	public GpuResource(Texture resource)		: this(resource, GpuResourceType.Texture)	{ }
}

public class ComputeData
{
	// private Dictionary<GpuResourceType, GpuResource> resources = new Dictionary<GpuResourceType, GpuResource>();

	private Dictionary<string, ComputeBuffer> _buffers	= new Dictionary<string, ComputeBuffer>();
	private Dictionary<string, RenderTexture> _textures	= new Dictionary<string, RenderTexture>();

	public IReadOnlyDictionary<string, ComputeBuffer> buffers	=> _buffers as IReadOnlyDictionary<string, ComputeBuffer>;
	public IReadOnlyDictionary<string, RenderTexture> textures	=> _textures as IReadOnlyDictionary<string, RenderTexture>;

	//-- Add: -----------------------------------------------------------------------------------------------------------------------
	public void AddBuffer(string name, int count, int stride)				=> _buffers.Add(name, new ComputeBuffer(count, stride));
	public void AddBuffer(string name, ComputeBuffer buffer)				=> _buffers.Add(name, buffer);

	public void AddRenderTexture(string name, RenderTextureDescriptor desc)	=> _textures.Add(name, new RenderTexture(desc));
	public void AddRenderTexture(string name, RenderTexture rt)				=> _textures.Add(name, rt);

	// //-- Get: -----------------------------------------------------------------------------------------------------------------------
	// public ComputeBuffer GetBuffer(string name)								=> return _buffers[name];
	// public Texture GetTexture(string name) 									=> return _textures[name];

	// //-- Set: -----------------------------------------------------------------------------------------------------------------------
	// public void SetBufferData(string bufferName, System.Array data)			=> _buffers[bufferName].SetData(data);

	//-- Clean: ---------------------------------------------------------------------------------------------------------------------
	public void Cleanup()
	{
		foreach (var buffer in _buffers)	{ buffer.Value.Release();	}
		foreach (var texture in _textures)	{ texture.Value.Release();	}
		_buffers.Clear();
		_textures.Clear();
	}
}

// public class ComputeData
// {
// 	private Dictionary<string, ComputeBuffer> _buffers	= new Dictionary<string, ComputeBuffer>();
// 	private Dictionary<string, RenderTexture> _textures	= new Dictionary<string, RenderTexture>();

// 	public IReadOnlyDictionary<string, ComputeBuffer> buffers	=> _buffers as IReadOnlyDictionary<string, ComputeBuffer>;
// 	public IReadOnlyDictionary<string, RenderTexture> textures	=> _textures as IReadOnlyDictionary<string, RenderTexture>;

// 	//-- Add: -----------------------------------------------------------------------------------------------------------------------
// 	public void AddBuffer(string name, int count, int stride)				=> _buffers.Add(name, new ComputeBuffer(count, stride));
// 	public void AddBuffer(string name, ComputeBuffer buffer)				=> _buffers.Add(name, buffer);

// 	public void AddRenderTexture(string name, RenderTextureDescriptor desc)	=> _textures.Add(name, new RenderTexture(desc));
// 	public void AddRenderTexture(string name, RenderTexture rt)				=> _textures.Add(name, rt);

// 	// //-- Get: -----------------------------------------------------------------------------------------------------------------------
// 	// public ComputeBuffer GetBuffer(string name)								=> return _buffers[name];
// 	// public Texture GetTexture(string name) 									=> return _textures[name];

// 	// //-- Set: -----------------------------------------------------------------------------------------------------------------------
// 	// public void SetBufferData(string bufferName, System.Array data)			=> _buffers[bufferName].SetData(data);

// 	//-- Clean: ---------------------------------------------------------------------------------------------------------------------
// 	public void Cleanup()
// 	{
// 		foreach (var buffer in _buffers)	{ buffer.Value.Release();	}
// 		foreach (var texture in _textures)	{ texture.Value.Release();	}
// 		_buffers.Clear();
// 		_textures.Clear();
// 	}
// }