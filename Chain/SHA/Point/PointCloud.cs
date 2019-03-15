using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PointCloud : MonoBehaviour
{
    public Mesh prim;
    public int num = 100;
    public float spread = 10;
    public float scale = 1;

    private Mesh mesh;

    // Use this for initialization
    void OnEnable()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        CreateMesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void CreateMesh()
    {
        Vector3[] points = prim.vertices;
        int[] tris = prim.triangles;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < num; ++i)
        {
            Vector3 shift = new Vector3(Random.value - .5f, Random.value - .5f, Random.value - .5f) * spread;
            for (int ii = 0; ii < points.Length; ii++)
            {
                vertices.Add(points[ii] * scale + shift);
            }
            for (int iii = 0; iii < tris.Length; iii++)
            {
                triangles.Add(tris[iii] + (i * 24));
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        //mesh.SetIndices(indecies, MeshTopology.Points, 0);

    }
}