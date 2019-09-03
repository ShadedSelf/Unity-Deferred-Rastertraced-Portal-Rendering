using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Show : MonoBehaviour {

	public bool moving = false;
	public float topSpeed = 10;
	public float grav = 15;

	Rigidbody rb;
	Vector3 inputVector;
	//float inputST;

	void Start () {
		rb = GetComponent<Rigidbody>();
	}

	void Update()
	{
		if (!Input.GetAxisRaw("Horizontal").AlmostFloat(0) || !Input.GetAxisRaw("Vertical").AlmostFloat(0))
			moving = true;
		else
			moving = false;
		if (Input.GetKeyDown(KeyCode.Joystick1Button0))
			rb.AddForce(Vector3.up * 10, ForceMode.Impulse);

		inputVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
		inputVector = Camera.main.transform.TransformDirection(inputVector);
		//inputST = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).magnitude;
	}

	void FixedUpdate ()
	{
		Vector3 currVector = rb.velocity.MultVector(new Vector3(1, 0, 1)).normalized;
		float currST = rb.velocity.MultVector(new Vector3(1, 0, 1)).magnitude;

		rb.AddForce(inputVector * topSpeed, ForceMode.Acceleration);
		rb.AddForce(-rb.velocity.MultVector(new Vector3(1, 0, 1)), ForceMode.Acceleration);
		rb.AddForce(Vector3.down * grav, ForceMode.Acceleration);
	}
}
