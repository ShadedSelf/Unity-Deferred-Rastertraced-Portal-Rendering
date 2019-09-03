using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;
using static Unity.Mathematics.math;

public class MovementController
{
	private Transform body, pivot;

	//-- Settings: --------------------------------------------
	public float movementSpeed;
    public float jumpForce;
    public float downForce;
	public int maxJumps;
	public LayerMask mask;

	//-- Global State: ----------------------------------------
	public Vector3 down	= Vector3.down;
	public float scale
	{
		get { return _scale;											}
		set { _scale = value; body.localScale = Vector3.one * value;	}
	} private float _scale = 1;

	//-- Internal State: --------------------------------------
	private float mag = 0;
    private int jumpsLeft;

	public MovementController(Transform body, Transform pivot)
	{
		this.body	= body;
		this.pivot	= pivot;
	}

	public Vector3 TransformDirection(Vector3 dir)
	{
		Vector3 tmp = (pivot.localRotation * dir).normalized;
		return (pivot.rotation * (Quaternion.Inverse(pivot.localRotation) * tmp)).normalized;
	}

	public void Test(float deltaTime)
	{
		Vector3 rawDirection = new Vector3
		(
			Input.GetKey(KeyCode.D).ToInt() - Input.GetKey(KeyCode.A).ToInt(), 
			0,
			Input.GetKey(KeyCode.W).ToInt() - Input.GetKey(KeyCode.S).ToInt()
		).normalized;
		Vector3 dir = TransformDirection(rawDirection);

		var rb = body.GetComponent<Rigidbody>();

		rb.velocity = dir * movementSpeed * scale  * (Input.GetKey(KeyCode.LeftShift) ? 0.5f : 1);
		// body.position += dir * movementSpeed * scale  * (Input.GetKey(KeyCode.LeftShift) ? 0.5f : 1) * deltaTime;
		// rb.position += dir * movementSpeed * scale  * (Input.GetKey(KeyCode.LeftShift) ? 0.5f : 1) * deltaTime;
	}

	public void TestDown(float deltaTime) //calculate height manually?
	{
		var rb = body.GetComponent<Rigidbody>();

		rb.velocity -= body.up * mag * scale;
		// body.position -= body.up * mag * scale * deltaTime;
		// rb.position -= body.up * mag * scale * deltaTime;

		mag += downForce * deltaTime * ((Input.GetKey(KeyCode.Space) && mag < 0) ? 1 : 2);

		var coll = body.GetComponent<CapsuleCollider>();
		Vector3 from = body.TransformPoint(coll.center + new Vector3(0, coll.height * 0.5f - coll.radius, 0));
		if (Physics.SphereCast(from, coll.radius, down, out var hit, 10f, mask)) //cast small box to get propper normal
		{
			if (hit.distance < 1.01f)
			{
				if (Physics.BoxCast(body.position, Vector3.one * 0.25f * scale, down, out var nHit, body.rotation, scale, mask) && hit.collider.CompareTag("Wakker")) //cast to hit.pint
					down = -nHit.normal;
				if (mag > 0) { mag = 0; }
				jumpsLeft = maxJumps;
			}
		}
	}

	public void Jump()
	{
		if (jumpsLeft > 0)
		{
			jumpsLeft--;
			mag = -jumpForce;
		}
	}

	// public void Move(float deltaTime)
	// {
	// 	Vector3 rawDirection = new Vector3
	// 	(
	// 		Input.GetKey(KeyCode.D).ToInt() - Input.GetKey(KeyCode.A).ToInt(), 
	// 		0,
	// 		Input.GetKey(KeyCode.W).ToInt() - Input.GetKey(KeyCode.S).ToInt()
	// 	).normalized;

	// 	Vector3 dir = pivot.localRotation * rawDirection * movementSpeed * deltaTime * scale;

	// 	CheckAxis(Vector3.forward,	new Vector3(0.5f, 1f, 0.001f) * scale, ref dir.z);
	// 	CheckAxis(-Vector3.forward,	new Vector3(0.5f, 1f, 0.001f) * scale, ref dir.z);
	// 	CheckAxis(Vector3.right,	new Vector3(0.001f, 1f, 0.5f) * scale, ref dir.x);
	// 	CheckAxis(-Vector3.right,	new Vector3(0.001f, 1f, 0.5f) * scale, ref dir.x);

	// 	body.position += pivot.rotation * (Quaternion.Inverse(pivot.localRotation) * dir) * ((Input.GetKey(KeyCode.LeftShift) ? 0.5f : 1));
	// }

	// public void ApplyGravity(float deltaTime)
	// {
	// 	float maxD = float.PositiveInfinity;
	// 	RaycastHit hit;
	// 	if (CheckBox(-body.up, new Vector3(0.5f, 0.1f, 0.5f) * scale, 2f, out hit))
	// 	{
	// 		maxD = hit.distance - 1f * scale;
	// 		down = -hit.normal;
	// 	}

	// 	float nDelta = Mathf.Min(mag * deltaTime * scale, maxD);
	// 	body.position -= body.up * nDelta;

    //     mag += downForce * deltaTime * ((Input.GetKey(KeyCode.Space) && mag < 0) ? 1 : 2);

	// 	if (maxD < 0.00001f)
	// 	{
	// 		if (mag > 0) { mag = 0; }
	// 		jumpsLeft = maxJumps;
	// 	}
	// }
	



	// private void CheckAxis(Vector3 direction, Vector3 halfExtents, ref float localAxis) // Differetn distance per layer, ie to approach portal
	// {
	// 	float sign(Vector3 a) => a.x + a.y + a.z;

	// 	Vector3 bodyDirection = body.rotation * direction;

	// 	if (CheckBox(bodyDirection, halfExtents, 2f, out RaycastHit hit))
	// 	{
	// 		float maxD = hit.distance - 0.5f * scale;
	// 		if (maxD < 0 && localAxis.AlmostFloat(0))
	// 			localAxis = maxD * sign(direction);
	// 		else
	// 			localAxis = (sign(direction) > 0)
	// 				? Mathf.Min(localAxis, maxD)
	// 				: Mathf.Max(localAxis, -maxD);
	// 	}
	// }

	// private bool CheckBox(Vector3 direction, Vector3 halfExtents, float maxDistance, out RaycastHit oHit)
	// {
	// 	return Physics.BoxCast(body.position, halfExtents, direction, out oHit, body.rotation, maxDistance, mask, QueryTriggerInteraction.Ignore);
	// }
}
