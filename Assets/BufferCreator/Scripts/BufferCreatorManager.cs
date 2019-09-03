using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct BufferConfig
{
	public Shader shader;
	public string name;
	public string tag;
	public RenderTextureFormat format;
	public bool render;
}

[RequireComponent(typeof(Camera))]
public class BufferCreatorManager : MonoBehaviour
{
	public List<BufferConfig> configs = new List<BufferConfig>();

	private BufferCreator _bc;

	void OnEnable()
	{
		_bc = new BufferCreator(GetComponent<Camera>());

		foreach (var config in configs)
		{
			var rt = GpuUtils.GetScreenRenderTexture(24, false, config.format);
			_bc.Add(config.shader, config.name, config.tag, rt);
		}
	}

	void LateUpdate()
	{
		_bc.RenderAll();
	}
}