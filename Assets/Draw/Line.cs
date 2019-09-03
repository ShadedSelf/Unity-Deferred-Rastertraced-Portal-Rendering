using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{

	public Material mat;
	public float maxDist = 100;

	void Start()
	{
		mat = new Material(Shader.Find("Hidden/Internal-Colored"));
		for (int i = 0; i < 10; i++)
		{
			Cube(Vector3.zero + Vector3.right * i, 1);
		}
	}

	void OnPostRender()
	{
		GL.PushMatrix();
		mat.SetPass(0);
		GL.Begin(GL.LINES);

		foreach (var item in list)
		{
			float col = Mathf.Clamp01(transform.InverseTransformPoint(item).z / Mathf.Abs(maxDist));
			GL.Color(new Color(col, col, col));
			GL.Vertex(item);
		}

		GL.End();
		GL.PopMatrix();
	}

	List<Vector3> list =  new List<Vector3>();
	void Cube(Vector3 offSet, float scale)
	{
		list.Add((new Vector3(.5f, .5f, .5f) + offSet) * scale);
		list.Add((new Vector3(-.5f, .5f, .5f) + offSet) * scale);
		list.Add((new Vector3(-.5f, .5f, .5f) + offSet) * scale);
		list.Add((new Vector3(-.5f, .5f, -.5f) + offSet) * scale);
		list.Add((new Vector3(-.5f, .5f, -.5f) + offSet) * scale);
		list.Add((new Vector3(.5f, .5f, -.5f) + offSet) * scale);
		list.Add((new Vector3(.5f, .5f, -.5f) + offSet) * scale);
		list.Add((new Vector3(.5f, .5f, .5f) + offSet) * scale);
		//
		list.Add((new Vector3(.5f, -.5f, .5f) + offSet) * scale);
		list.Add((new Vector3(-.5f, -.5f, .5f) + offSet) * scale);
		list.Add((new Vector3(-.5f, -.5f, .5f) + offSet) * scale);
		list.Add((new Vector3(-.5f, -.5f, -.5f) + offSet) * scale);
		list.Add((new Vector3(-.5f, -.5f, -.5f) + offSet) * scale);
		list.Add((new Vector3(.5f, -.5f, -.5f) + offSet) * scale);
		list.Add((new Vector3(.5f, -.5f, -.5f) + offSet) * scale);
		list.Add((new Vector3(.5f,- .5f, .5f) + offSet) * scale);
		//
		list.Add((new Vector3(.5f, .5f, .5f) + offSet) * scale);
		list.Add((new Vector3(.5f, -.5f, .5f) + offSet) * scale);
		list.Add((new Vector3(-.5f, .5f, .5f) + offSet) * scale);
		list.Add((new Vector3(-.5f, -.5f, .5f) + offSet) * scale);
		list.Add((new Vector3(.5f, .5f, -.5f) + offSet) * scale);
		list.Add((new Vector3(.5f, -.5f, -.5f) + offSet) * scale);
		list.Add((new Vector3(-.5f, .5f, -.5f) + offSet) * scale);
		list.Add((new Vector3(-.5f, -.5f, -.5f) + offSet) * scale);
	}
}
