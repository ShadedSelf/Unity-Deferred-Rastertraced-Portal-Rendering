using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct BufferConfig
{
	public Shader shader;
	public string name;
	public RenderTextureFormat format;
	public bool render;
}

public class BufferCreator : MonoBehaviour 
{
	public List<BufferConfig> configs = new List<BufferConfig>();
	public RenderTexture[] textures;

	Camera cam;

	void OnEnable()
	{
		cam = GetComponent<Camera>();
		textures = new RenderTexture[configs.Count];
		for (int i = 0; i < configs.Count; i++)
		{
			textures[i] = new RenderTexture(Screen.width, Screen.height, 32, configs[i].format, RenderTextureReadWrite.Linear);
			textures[i].filterMode = FilterMode.Point;
			textures[i].Create();
			Shader.SetGlobalTexture(configs[i].name, textures[i]);
		}
	}

	void Update()
	{
		for (int i = 0; i < configs.Count; i++)
		{
			if (configs[i].render == true)
			{
				cam.targetTexture = textures[i];
				cam.RenderWithShader(configs[i].shader, "RenderType");
				cam.targetTexture = null;
			}
		}
	}
}
