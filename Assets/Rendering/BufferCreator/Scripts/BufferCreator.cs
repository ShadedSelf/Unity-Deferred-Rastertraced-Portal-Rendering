using UnityEngine;
using System.Collections.Generic;

public struct ReplaceData
{
	public Shader shader;
	public string name;
	public string tag;
	public RenderTexture rt;
}

public class BufferCreator 
{
	private List<ReplaceData> _replacements = new List<ReplaceData>();
	private Camera _cam;

	public IReadOnlyList<ReplaceData> replacements => _replacements as IReadOnlyList<ReplaceData>;

	public BufferCreator(Camera cam)
	{
		_cam = cam;
	}

	public void Add(Shader shader, string name, string tag)	=> Add(shader, name, tag, GpuUtils.GetScreenRenderTexture(32));
	public void Add(Shader shader, string name, string tag, RenderTexture rt)
	{
		var data = new ReplaceData
		{
			shader	= shader,
			name	= name,
			tag		= tag,
			rt		= rt
		};
		_replacements.Add(data);
	}

	public void RenderAll()
	{
		foreach (var replacement in _replacements)
			Render(replacement);
	}

	public void Render(string name)
	{
		foreach (var replacement in _replacements)
			if (replacement.name == name)
				Render(replacement);
	}

	public void Render(ReplaceData replacement)
	{
		_cam.targetTexture = replacement.rt;
		if (!replacement.shader)	{ _cam.Render();												}
		else						{ _cam.RenderWithShader(replacement.shader, replacement.tag);	}
		_cam.targetTexture = null;
	}
}