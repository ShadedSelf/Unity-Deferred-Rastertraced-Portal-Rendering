using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class X : MonoBehaviour
{
	[Header("NO")]
	public ComputeShader shader;
	public Mesh instanceMesh;
	public Material instanceMat;
	[Header("BOOLS")]
	public bool run = true;
	public bool draw = true;
	[Header("YES")]
	public Vector2Int num;
	[Range(0, 1)]
	public float damp = .995f;
	public int difRa = 3;
	public float radius = 3;
	public float grav = 2;
	public float waterLVL = 5;
	public float lvlT = 10;
	public float speed;
	[Header("TREES")]
	public float colRes = 4;
	public Vector2 scale = Vector2.one;
	public Vector3 position;
	public float wall = 10;

	RenderTexture u, v, dir;
	public Texture2D ground, stiff;
	ComputeBuffer argsBuffer, cells;
	CommandBuffer cb;
	uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

	public struct H
	{
		public Vector4 posh;
		public Vector4 col;
	}

	void OnEnable()
	{
		Textures();
		Buffers();
		Commands();
	}

	void Update()
	{
		shader.SetVector("pos", transform.position);
		shader.SetVector("scale", scale);
		shader.SetVector("position", position);
		shader.SetInts("num", new int[2] { num.x, num.y });
		shader.SetInt("difRa", (difRa % 2 == 0) ? difRa + 1 : difRa);
		shader.SetFloat("damp", damp);
		shader.SetFloat("radius", radius);
		shader.SetFloat("grav", grav);
		shader.SetFloat("waterLVL", waterLVL);
		shader.SetFloat("lvlT", lvlT);
		shader.SetFloat("dT", speed * Time.deltaTime);
		shader.SetFloat("wall", wall);

		if (run)
			Graphics.ExecuteCommandBuffer(cb);

		instanceMat.SetFloat("_Res", colRes);
		instanceMat.SetVector("_Scale", scale);
		instanceMat.SetVector("_Pos", position);
		if (draw)
			Graphics.DrawMeshInstancedIndirect(instanceMesh, 0, instanceMat, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), argsBuffer);
	}

	void Commands()
	{
		cb = new CommandBuffer();
		cb.name = "Sim";

		cb.BeginSample("Go");
		cb.DispatchCompute(shader, 0, num.x / 8, num.y / 8, 1);
		cb.EndSample("Go");

		cb.BeginSample("Act");
		cb.DispatchCompute(shader, 1, num.x / 8, num.y / 8, 1);
		cb.EndSample("Act");
	}

	void Textures()
	{
		u = new RenderTexture(num.x, num.y, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
		v = new RenderTexture(num.x, num.y, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
		dir = new RenderTexture(num.x, num.y, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		u.enableRandomWrite = true;
		v.enableRandomWrite = true;
		dir.enableRandomWrite = true;
		u.filterMode = FilterMode.Point;
		v.filterMode = FilterMode.Point;
		dir.filterMode = FilterMode.Point;
		u.Create();
		v.Create();
		dir.Create();
		shader.SetTexture(0, "u", u);
		shader.SetTexture(0, "v", v);
		shader.SetTexture(0, "dir", dir);
		shader.SetTexture(1, "u", u);

		if (ground == null)
		{
			ground = new Texture2D(num.x, num.y, TextureFormat.RFloat, false, true);
			ground.filterMode = FilterMode.Point;
			for (int x = 0; x < num.x; x++)
			{
				for (int y = 0; y < num.y; y++)
				{
					/*if (x == 0 || y == 0 || x == num.x - 1 || y == num.y - 1)
						ground.SetPixel(x, y, Color.white);
					else*/
						ground.SetPixel(x, y, Color.black);
				}
			}
			ground.Apply();
		}
		shader.SetTexture(0, "ground", ground);
		shader.SetTexture(0, "stiff", stiff);
	}

	void Buffers()
	{

		argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
		uint numIndices = instanceMesh.GetIndexCount(0);
		args[0] = numIndices;
		args[1] = (uint)(num.x * num.y);
		argsBuffer.SetData(args);

		cells = new ComputeBuffer(num.x * num.y, sizeof(float) * 4 * 2);
		H[] temp = new H[num.x * num.y];
		int i = 0;
		for (int x = 0; x < num.x; x++)
		{
			for (int z = 0; z < num.y; z++)
			{
				temp[i].posh = new Vector4(x, 0, z, Random.value * 5);
				temp[i].col = Vector4.zero;
				i++;
			}
		}
		cells.SetData(temp);

		shader.SetBuffer(0, "buff", cells);
		instanceMat.SetBuffer("buff", cells);
	}

	void OnGUI()
	{
		GUI.DrawTexture(new Rect(Vector2.zero, new Vector2(Screen.width, Screen.height * num.y / num.x)), dir);
	}


	void OnDisable()
	{
		argsBuffer.Release();
		cells.Release();
	}
}
