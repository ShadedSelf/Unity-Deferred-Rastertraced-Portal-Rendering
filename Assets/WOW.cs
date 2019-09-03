using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WOW : MonoBehaviour
{
	public Mesh mesh;

    void OnEnable()
    {
        GameObject[] obj = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		CombineInstance[] a = new CombineInstance[obj.Length];

		for (int i = 0; i < obj.Length; i++)
		{
			var renderer = obj[i].GetComponent<MeshRenderer>();
			if (renderer != null)
			{
				// renderer.enabled = false;
				renderer.receiveShadows = false;
				renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

				// a[i] = new CombineInstance();
				// a[i].mesh = obj[i].GetComponent<MeshFilter>().mesh;
				// a[i].subMeshIndex = 0;
				// a[i].transform = renderer.transform.localToWorldMatrix;
			}
		}

		// mesh = new Mesh();
		// mesh.CombineMeshes(a, false, true);
    }

    // void LateUpdate()
    // {
    //     Graphics.DrawMeshNow(mesh, Vector3.zero, Quaternion.identity);
    // }
}
