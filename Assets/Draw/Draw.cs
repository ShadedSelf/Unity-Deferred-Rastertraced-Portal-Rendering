using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Draw
{
	public static Material mat;

	static List<Vector3> vectors = new List<Vector3>();

	public static void Print()
	{
		GL.PushMatrix();
		mat.SetPass(0);
		GL.Begin(GL.LINES);

		GL.Color(Color.red);
		foreach (var vector in vectors)
		{
			GL.Vertex(vector);
		}

		GL.End();
		GL.PopMatrix();

		vectors.Clear();
	}

	public static void Line(Vector3 start, Vector3 end)
	{
		vectors.Add(start);
		vectors.Add(end);
	}

	public static void Cube(Vector3 offSet, float scale)
	{
		vectors.Add((new Vector3(.5f, .5f, .5f) + offSet) * scale);
		vectors.Add((new Vector3(-.5f, .5f, .5f) + offSet) * scale);
		vectors.Add((new Vector3(-.5f, .5f, .5f) + offSet) * scale);
		vectors.Add((new Vector3(-.5f, .5f, -.5f) + offSet) * scale);
		vectors.Add((new Vector3(-.5f, .5f, -.5f) + offSet) * scale);
		vectors.Add((new Vector3(.5f, .5f, -.5f) + offSet) * scale);
		vectors.Add((new Vector3(.5f, .5f, -.5f) + offSet) * scale);
		vectors.Add((new Vector3(.5f, .5f, .5f) + offSet) * scale);
		//
		vectors.Add((new Vector3(.5f, -.5f, .5f) + offSet) * scale);
		vectors.Add((new Vector3(-.5f, -.5f, .5f) + offSet) * scale);
		vectors.Add((new Vector3(-.5f, -.5f, .5f) + offSet) * scale);
		vectors.Add((new Vector3(-.5f, -.5f, -.5f) + offSet) * scale);
		vectors.Add((new Vector3(-.5f, -.5f, -.5f) + offSet) * scale);
		vectors.Add((new Vector3(.5f, -.5f, -.5f) + offSet) * scale);
		vectors.Add((new Vector3(.5f, -.5f, -.5f) + offSet) * scale);
		vectors.Add((new Vector3(.5f, -.5f, .5f) + offSet) * scale);
		//
		vectors.Add((new Vector3(.5f, .5f, .5f) + offSet) * scale);
		vectors.Add((new Vector3(.5f, -.5f, .5f) + offSet) * scale);
		vectors.Add((new Vector3(-.5f, .5f, .5f) + offSet) * scale);
		vectors.Add((new Vector3(-.5f, -.5f, .5f) + offSet) * scale);
		vectors.Add((new Vector3(.5f, .5f, -.5f) + offSet) * scale);
		vectors.Add((new Vector3(.5f, -.5f, -.5f) + offSet) * scale);
		vectors.Add((new Vector3(-.5f, .5f, -.5f) + offSet) * scale);
		vectors.Add((new Vector3(-.5f, -.5f, -.5f) + offSet) * scale);
	}
}
