using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class c : MonoBehaviour
{
	[Range(0, 1)]
	public float tensor = 1;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnDrawGizmos()
	{
		Vector3 iner = new Vector3(-5, -4, -5);
		Vector3 mid = Vector3.zero;


		for (int i = 0; i < 20; i++)
		{
			float ier = (float)i / 20;
			ier *= Mathf.Lerp(tensor, 1, (float)i / 20);
			Vector3 curr = Vector3.Lerp(iner, mid, ier);;

			Gizmos.color = Color.white;
			Gizmos.DrawWireCube(curr, Vector3.one * .1f);
			Gizmos.color = Color.green;
			Gizmos.DrawCube(curr, Vector3.one * .01f);
		}
	}
}
