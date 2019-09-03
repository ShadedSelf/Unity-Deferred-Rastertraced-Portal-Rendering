using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuber : MonoBehaviour
{
	public float min = .5f;
	public float max = 3;

	public Mesh mesh;
	public Material mat;

	Vector3 alignedNormal;
	Transform holderTransform;

	void Start()
	{
		holderTransform = GetComponent<HolderCuberHold>().joint.transform;
	}

	void Update()
	{
		alignedNormal = VectorExtensions.AlignNormal(transform.position - Camera.main.transform.position);
		Vector3 add = (alignedNormal * Input.GetAxis("Mouse ScrollWheel")).MultVector(alignedNormal.SignVector());
		transform.localScale = Vector3.MoveTowards(transform.localScale, (transform.localScale + add), .1f).ClampVector(min, max);

		RaycastHit hit = new RaycastHit();
		Ray ray = new Ray(transform.position - transform.right * .5f, -transform.right);
		if (Physics.Raycast(ray, out hit, 2))
		{
			//Graphics.DrawMesh(mesh, hit.point, Quaternion.identity, mat, 0);
		}
	}

	void FixedUpdate()
	{
		Vector3 elipse = (transform.lossyScale / 2).MultVector((transform.position - Camera.main.transform.position).normalized);
		Vector3 position = transform.position - elipse;
		holderTransform.localPosition = Vector3.forward * (4 + Vector3.Distance(transform.position, position) - .5f);
	}
}
