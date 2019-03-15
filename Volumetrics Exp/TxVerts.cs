using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TxVerts : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Vector3[] verts = GetComponent<MeshFilter>().mesh.vertices;

        for (int i = 0; i < GetComponent<MeshFilter>().mesh.vertices.Length; i++)
        {
            verts[i] = verts[i] * 2;
        }

        GetComponent<MeshFilter>().mesh.vertices = verts;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
