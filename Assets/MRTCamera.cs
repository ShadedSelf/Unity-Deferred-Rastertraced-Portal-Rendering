using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public static int ToIndex(this Enum);

public class EnumIndexer<T> where T : Enum
{
	private Dictionary<T, int> indices = new Dictionary<T, int>();
	private T[] values;

	public T this[int index] => values[index];
	public int this[T index] => indices[index];

	public EnumIndexer()
	{
		values = (T[])Enum.GetValues(typeof(T));
		for (int i = 0; i < values.Length; i++)
			indices.Add(values[i], i);

		// Enum.TryParse<T>("Active", false, out T result);
		// T a = (T)Enum.Parse(typeof(T), "A");
	}
}

public class MRTCamera<T> where T : Enum
{
	private Camera cam;
	private Shader shader;

	private int mrtCount;

	private RenderBuffer[] renderBuffers;
	private RenderTexture[] renderTextures;

	public RenderTexture this[Enum index] => renderTextures[(int)(object)index]; // ewww, fix

    public MRTCamera(Camera cam, Shader shader) // pass indexerType in constructor instead of enum generic?
    {
		this.cam = cam;
		this.shader = shader;

		var enumArray = (T[])Enum.GetValues(typeof(T));
		mrtCount = enumArray.Length;

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
        cam.RenderWithShader(shader, "Replace");
    }

	public void ReleaseBuffers()
	{
		for (int i = 0; i < mrtCount; i++)
			RenderTexture.ReleaseTemporary(renderTextures[i]);
	}
}
