using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roll : MonoBehaviour {

	Vector3 center;
	Quaternion q = Quaternion.identity;
	Matrix4x4 rot;

	float x;
	float z;

	Vector3 last;
	void OnEnable()
	{
		x = z = 0;
		center = Vector3.up;
		last = transform.position;
	}

	public float r;

	void Update()
	{
		Vector3 moveDelta = new Vector3(Input.GetAxisRaw("Horizontal") * 1 * Time.deltaTime, 0, Input.GetAxisRaw("Vertical") * 1 * Time.deltaTime);
		transform.Translate(moveDelta, Space.World);
		Vector3 rotationAxis = Vector3.Cross(moveDelta.normalized, Vector3.up);
		transform.RotateAround(transform.position, rotationAxis, -Mathf.Sin(moveDelta.magnitude * r * 2 * Mathf.PI) * Mathf.Rad2Deg);

		rot = Matrix4x4.Rotate(transform.rotation);
		center = transform.position;

		float t = 100;
		for (int x = -1; x < 2; x += 2)
			for (int y = -1; y < 2; y += 2)
				for (int z = -1; z < 2; z += 2)
				{
					Vector3 curr = center + rot.MultiplyPoint(new Vector3(x, y, z) / 2);
					t = Mathf.Min(t, curr.y);
				}
		center.y -= t - .5f;

		transform.position = center;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(new Vector3(0, 1, 0) - new Vector3(.5f, .5f, .5f), .05f);

		Gizmos.color = Color.black;

		for (int x = -1; x < 2; x += 2)
			for (int y = -1; y < 2; y += 2)
				for (int z = -1; z < 2; z += 2)
					Gizmos.DrawWireSphere(center + rot.MultiplyPoint(new Vector3(x, y, z) / 2), .05f);

		Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
	}

	float sdBox(Vector3 p, Vector3 b)
	{
		Vector3 d = p.AbsVector() - b;
		return Mathf.Min(Mathf.Max(d.x, Mathf.Max(d.y, d.z)), 0f) + (d.MaxVector(Vector3.zero)).magnitude;
	}
}