using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cc : MonoBehaviour {

	public Vector3 vec;
	[Range(0, 1)]
	public float t;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDrawGizmos()
	{
		/*Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * vec.x);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * vec.y);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * vec.z);
		Gizmos.color = Color.white;
		Gizmos.DrawLine(Vector3.zero, vec);

		float zXRE = vec.z / vec.x;
		float zYRE = vec.z / vec.y;

		Gizmos.DrawWireCube(Vector3.Lerp(Vector3.zero, vec, t), Vector3.one * .1f);

		print("" + new Vector3(vec.z / zXRE, vec.z / zYRE, vec.z));*/




		for (int x = 0; x < 10; x++)
		{
			for (int y = 0; y < 10; y++)
			{
				for (int z = 0; z < 10; z++)
				{
					Gizmos.DrawWireCube(Camera.main.projectionMatrix.MultiplyPoint(new Vector3(x, y, z)), Vector3.one);
				}
			}
		}


	}
}
