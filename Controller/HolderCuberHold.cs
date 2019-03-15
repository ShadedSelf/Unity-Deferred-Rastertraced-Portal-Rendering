using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolderCuberHold : MonoBehaviour {

	public ConfigurableJoint joint;
	public Rigidbody rb;
	public bool preventSleep = true;

	bool attached = false;

	void Update ()
	{
		if (attached)
		{
			if (Input.GetKeyDown(KeyCode.E))
			{
				joint.connectedBody = null;
				attached = false;
				return;
			}
			if (preventSleep && rb.IsSleeping())
				rb.WakeUp();
		}

		if (!attached && Input.GetKeyDown(KeyCode.E) && Vector3.Distance(joint.transform.position, rb.transform.position) < 6)
		{
			joint.connectedBody = rb;
			attached = true;
			return;
		}
	}

	void FixedUpdate()
	{

	}
}
