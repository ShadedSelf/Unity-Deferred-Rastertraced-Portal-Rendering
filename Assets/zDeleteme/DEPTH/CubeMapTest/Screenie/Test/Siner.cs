using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Siner : MonoBehaviour
{
	public Vector4[] c;

	void Update()
	{
		Shader.SetGlobalInt("_N", c.Length);
		Shader.SetGlobalVectorArray("_B", c);
	}
}
