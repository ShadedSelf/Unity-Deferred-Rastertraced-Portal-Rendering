using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TPoint : MonoBehaviour
{

    public Mesh mesh;
    public int numPoints = 60000;
    public float spread = 5;

    void Awake()
    {
        mesh = new Mesh();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    void OnEnable()
    {
        mesh.Clear();
        CreateMesh();
    }

    void CreateMesh()
    {
        Vector3[] points = new Vector3[numPoints];
        int[] indecies = new int[numPoints];
        for (int i = 0; i < points.Length; ++i)
        {
            //points[i] = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
            points[i] = new Vector3(Random.value - .5f, Random.value - .5f, Random.value - .5f) * spread;
            indecies[i] = i;
        }

        mesh.vertices = points;
        mesh.SetIndices(indecies, MeshTopology.Points, 0);

    }
}