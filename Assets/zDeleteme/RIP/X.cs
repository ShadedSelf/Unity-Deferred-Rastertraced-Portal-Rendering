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

	public Texture2D ground, stiff;
	ComputeBuffer argsBuffer;

	ComputeSystem system;

	void OnEnable()
	{
		uint[] args = new uint[5] { instanceMesh.GetIndexCount(0), (uint)(num.x * num.y), 0, 0, 0 };
		argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
		argsBuffer.SetData(args);

		system = new ComputeSystem(shader);
		system.AddKernel("Do", new Vector3Int(num.x, num.y , 1));
		// system.data.AddRenderTexture("u", new Vector3Int(num.x, num.y, 0), RenderTextureFormat.RFloat);
		// system.data.AddRenderTexture("v", new Vector3Int(num.x, num.y, 0), RenderTextureFormat.RFloat);
		system.data.AddBuffer("buff", num.x * num.y, sizeof(float) * 4 * 2);
		system.data.AddBuffer("test", 1, sizeof(float));

		system.BindComputeData();

		// instanceMat.SetBuffer("buff", system.data.GetBuffer("buff")); //AAAA

		Time.fixedDeltaTime = 0.015f;
	}

	// Crate custom fixedUpdate class
	public float[] test;
	void FixedUpdate()
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
		shader.SetFloat("dT", speed * Time.fixedDeltaTime);
		shader.SetFloat("wall", wall);

		system.Dispatch("Do");

		instanceMat.SetFloat("_Res", colRes);
		instanceMat.SetVector("_Scale", scale);
		instanceMat.SetVector("_Pos", position);
		if (draw)
			Graphics.DrawMeshInstancedIndirect(instanceMesh, 0, instanceMat, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), argsBuffer);

		// system.GetBuffer("test").GetData(test);
	}


	// void OnAudioFilterRead(float[] data, int channels)
	// {
	// 	for	(int i = 0; i < datat.Length; i++)
	// 	{
	// 		data[i] = datat[i];
	// 	}
	// 	next = true;
	// }

	void OnDisable()
	{
		argsBuffer.Release();
		system.Cleanup();
	}
}
