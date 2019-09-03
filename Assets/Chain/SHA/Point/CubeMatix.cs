using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMatix : MonoBehaviour
{

    public Mesh cube;

    //Vector3[] verts;
    int[] tris;
    Mesh mesh;

    void Start()
    {
        //verts = cube.vertices;
        tris = cube.triangles;
        mesh = new Mesh();
    }

    void Update()
    {
        mesh.Clear();

        Vector3[] vertices = cube.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            /* vertices[i] = Camera.main.cameraToWorldMatrix.MultiplyPoint3x4(vertices[i] - Vector3.forward * 5);
             vertices[i] = Camera.main.projectionMatrix.inverse.MultiplyPoint3x4(vertices[i]);*/

            vertices[i] = (Camera.main.projectionMatrix * Camera.main.cameraToWorldMatrix).MultiplyPoint3x4(vertices[i] - Vector3.forward * 5);
        }

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}
