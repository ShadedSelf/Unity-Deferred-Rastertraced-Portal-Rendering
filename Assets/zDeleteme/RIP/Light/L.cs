using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class L : MonoBehaviour
{
	public ComputeShader cs;
	public RenderTexture rt;
	public Texture2D c;
	public int loops = 1;

	CommandBuffer cb;

	void OnEnable()
	{
		rt = new RenderTexture(128, 128, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
		rt.filterMode = FilterMode.Point;
		rt.enableRandomWrite = true;
		rt.Create();

		cs.SetTexture(0, "rt", rt);
		cs.SetTexture(0, "c", c);

		B();
	}

	void Update()
	{
		cs.SetVector("lightPos", new Vector2(Mathf.Round(Input.mousePosition.x / Screen.width * 128), Mathf.Round(Input.mousePosition.y / Screen.height * 128)));
		cs.SetInt("loops", loops);

		Graphics.ExecuteCommandBuffer(cb);
	}

	void B()
	{
		cb = new CommandBuffer();

		cb.BeginSample("Render");
		cb.DispatchCompute(cs, 0, 128 / 8, 128 / 8, 1);
		cb.EndSample("Render");
	}

	void OnGUI()
	{
		GUI.DrawTexture(new Rect(Vector2.zero, new Vector2(Screen.width, Screen.height)), rt);
	}
}
