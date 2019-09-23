using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Me : MonoBehaviour 
{
    Vector3[] normals = new Vector3[8] 
	{
		 (Vector3.up + Vector3.forward + Vector3.right).normalized,
		 (Vector3.up + Vector3.forward - Vector3.right).normalized, 
		 (Vector3.up - Vector3.forward + Vector3.right).normalized, 
		 (Vector3.up - Vector3.forward - Vector3.right).normalized,
		-(Vector3.up + Vector3.forward + Vector3.right).normalized,
		-(Vector3.up + Vector3.forward - Vector3.right).normalized,
		-(Vector3.up - Vector3.forward + Vector3.right).normalized,
		-(Vector3.up - Vector3.forward - Vector3.right).normalized
	};

    public Material mat;

    GameObject[] corners;
	void Start ()
	{
        corners = new GameObject[8];
        for (int i = 0; i < 8; i++)
        {
            corners[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            corners[i].transform.parent = transform;
            corners[i].transform.localScale = Vector3.one * .5f;
            corners[i].transform.localPosition = normals[i] * .5f;

            corners[i].GetComponent<MeshRenderer>().sharedMaterial = mat;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
