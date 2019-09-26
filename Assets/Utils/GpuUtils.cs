using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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