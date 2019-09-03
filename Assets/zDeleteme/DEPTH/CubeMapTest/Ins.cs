using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Ins : MonoBehaviour {

	public Material mat;

	// Use this for initialization
	void Start ()
	{
		for (int x = 0; x < 20; x++)
		{
			for (int z = 0; z < 20; z++)
			{
				GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
				c.GetComponent<MeshRenderer>().material = mat;
				float sc = Random.value;
				c.transform.localScale = Vector3.one + Vector3.up * sc;
				c.transform.position = new Vector3(x - 9.5f, -9.5f + (sc / 2), z - 9.5f);
				c.transform.parent = transform;
			}
		}
	}
	

}
