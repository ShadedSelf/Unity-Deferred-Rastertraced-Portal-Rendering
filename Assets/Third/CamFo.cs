using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFo : MonoBehaviour {

	public GameObject f;
	public Vector3 off;

	float rot = 0;
	void Start () {
		
	}

	Vector3 velocity;
	public bool locker = false;
	void FixedUpdate () {

		off.y += Input.GetAxisRaw("YAxis") * Time.fixedDeltaTime * 3;
		if (Mathf.Abs(Input.GetAxisRaw("XAxis")) > .9f && !locker)
		{
			locker = true;
			rot += 45 * Mathf.Sign(Input.GetAxisRaw("XAxis"));
		}
		if (Mathf.Abs(Input.GetAxisRaw("XAxis")) < .9f && locker)
			locker = false;

		transform.LookAt(f.transform.position);
		transform.position = Vector3.SmoothDamp(transform.position, f.transform.position + Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0, rot, 0))).MultiplyPoint3x4(off), ref velocity, .1f);
	}
}
