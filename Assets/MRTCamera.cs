using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MRTCamera<T> where T : Enum
{
	private Camera cam;
	private int mrtCount;

	private RenderBuffer[] renderBuffers;
	private RenderTexture[] renderTextures;

	public RenderTexture this[T e] => renderTextures[enumIndexer[e]];
	private EnumIndexer<T> enumIndexer;

    public MRTCamera(Camera cam)
    {
		this.cam = cam;

		enumIndexer = new EnumIndexer<T>();
		mrtCount = enumIndexer.count;

		renderBuffers = new RenderBuffer[mrtCount];
		renderTextures = new RenderTexture[mrtCount];
    }

	public void GatherBuffers()
	{
		for (int i = 0; i < mrtCount; i++)
		{
			renderTextures[i] = RenderTexture.GetTemporary(Screen.width, Screen.height, 32, RenderTextureFormat.ARGBFloat);
			renderBuffers[i] = renderTextures[i].colorBuffer;
		}
		cam.SetTargetBuffers(renderBuffers, renderTextures[0].depthBuffer);
	}

    public void Render()
    {
		cam.Render();
    }

	public void ReleaseBuffers()
	{
		for (int i = 0; i < mrtCount; i++)
			RenderTexture.ReleaseTemporary(renderTextures[i]);
	}
}
