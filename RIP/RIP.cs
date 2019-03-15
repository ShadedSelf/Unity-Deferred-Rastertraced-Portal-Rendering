using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct R
{
	public Vector3 pos;
	public float st;
}

public class RIP : MonoBehaviour {

	[Header("No")]
	public Mesh mes;
	public Material mat;
	[Header("Yes")]
	public int num;
	public float upSin = 1;
	public float damp = .6f;
	public bool wl = true;
	public float waterLVL = 1;
	public bool start = false;
	public int pp = 2;

	float[,] u;
	float[,] v;
	float need;

	void OnEnable()
	{
		u = new float[num, num];
		v = new float[num, num];

		ReSet();
	}

	void Update()
	{
		if (start)
		{
			start = false;
			ReSet();
		}

		for (int x = 0; x < num; x++)
		{
			for (int z = 0; z < num; z++)
			{
				float t = 0;
				for (int xt = 0; xt < 2; xt++)
				{
					for (int zt = 0; zt < 2; zt++)
					{
						for (int p = 1; p < pp + 1; p++)
						{
							int xer = Mathf.Clamp(x + (xt * 2 - 1) * p, 0, num - 1);
							int zer = Mathf.Clamp(z + (zt * 2 - 1) * p, 0, num - 1);
							t += u[xer, zer];
						}
					}
				}
				t = t / (4 * pp) - u[x, z];
				v[x, z] += t;
				v[x, z] *= damp;
				u[x, z] += v[x, z];
			}
		}

		if (wl)
		{
			float total = 0;
			for (int x = 0; x < num; x++)
			{
				for (int z = 0; z < num; z++)
				{
					total += u[x, z];
				}
			}

			total /= num * num;
			need = waterLVL - total;

			for (int x = 0; x < num; x++)
			{
				for (int z = 0; z < num; z++)
				{
					u[x, z] += need;
				}
			}
		}

		for (int x = 0; x < num; x++)
		{
			for (int z = 0; z < num; z++)
			{
				if (Vector3.Distance(transform.position, new Vector3(x, waterLVL, z)) < 3)
					u[x, z] = Vector3.Distance(transform.position, new Vector3(x, waterLVL, z)) - 2;
			}
		}


		Matrix4x4[] matrix = new Matrix4x4[num * num];
		Vector4[] vecs = new Vector4[num * num];
		MaterialPropertyBlock pb = new MaterialPropertyBlock();

		int i = 0;
		for (int x = 0; x < num; x++)
		{
			for (int z = 0; z < num; z++)
			{
				matrix[i] = Matrix4x4.TRS(new Vector3(x, 0, z) + Vector3.up * u[x, z] / 2, Quaternion.identity, Vector3.one + Vector3.up * u[x, z]);
				vecs[i] = new Vector4(u[x, z] / waterLVL, 1, 1, 1);
				i++;
			}
		}
		pb.SetVectorArray("_Color", vecs);

		Graphics.DrawMeshInstanced(mes, 0, mat, matrix, num * num, pb);
	}

	void ReSet()
	{
		for (int x = 0; x < num; x++)
		{
			for (int z = 0; z < num; z++)
			{
				u[x, z] = Mathf.Max(Mathf.Sin(Vector3.Distance(new Vector3(x, 0, z), transform.position)) * upSin, .1f);
				v[x, z] = 0;
			}
		}
	}
}
