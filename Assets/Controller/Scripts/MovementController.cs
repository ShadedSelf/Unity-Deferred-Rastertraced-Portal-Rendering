using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;
using static Unity.Mathematics.math;

public class MovementController
{
	//-- Settings: --------------------------------------------
	public float movementSpeed	{ get; set; }
    public float jumpForce		{ get; set; }
    public float downForce		{ get; set; }
	public int maxJumps			{ get; set; }
	public LayerMask mask		{ get; set; }

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
	
	private Rigidbody rb;
	private Transform body, pivot;

	public MovementController(Rigidbody rb, Transform body, Transform pivot)
	{
		this.rb		= rb;
		this.body	= body;
		this.pivot	= pivot;
	}

	public void Test(float deltaTime)
	{
		var dir = pivot.TransformDirection(InputUtils.KeyboardDirection());

		rb.velocity = dir * movementSpeed * scale  * (Input.GetKey(KeyCode.LeftShift) ? 0.5f : 1);
	}

	public void TestDown(float deltaTime) //calculate height manually?
	{
		rb.velocity -= body.up * mag * scale;

		mag += downForce * deltaTime * ((Input.GetKey(KeyCode.Space) && mag < 0) ? 1 : 2);

		var coll = body.GetComponent<CapsuleCollider>();
		Vector3 from = body.TransformPoint(coll.center + new Vector3(0, (coll.height * 0.5f - coll.radius) * scale, 0));
		if (Physics.SphereCast(from, coll.radius * scale, down, out var hit, 10f, mask)) //cast small box to get propper normal
		{
			if (hit.distance < 1.01f * scale)
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
}
