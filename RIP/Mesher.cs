using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesher : MonoBehaviour
{

	public Mesh baseMesh;
	public Mesh mesh;
	public Vector2Int size;
	public Material mat;

	Vector3[] baseVerts;
	int[] baseTris;

	Vector3[] verts;
	int[] tris;

	void OnEnable()
	{
		baseVerts = baseMesh.vertices;
		baseTris = baseMesh.triangles;

		verts = new Vector3[baseVerts.Length * size.x * size.y];
		tris = new int[baseTris.Length * size.x * size.y];
		int i = 0;
		int ii = 0;
		for (int x = 0; x < size.x; x++)
		{
			for (int y = 0; y < size.y; y++)
			{
				for (int t = 0; t < baseVerts.Length; t++)
				{
					verts[i] = baseVerts[t] + new Vector3(x, 0, y);
					i++;
				}
				for (int tt = 0; tt < baseTris.Length; tt++)
				{
					tris[ii] = baseTris[tt];
					ii++;
				}
			}
		}

		mesh = new Mesh();
		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.RecalculateNormals();
	}

	void Update()
	{
		Graphics.DrawMesh(mesh, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one), mat, 0);
	}
}
