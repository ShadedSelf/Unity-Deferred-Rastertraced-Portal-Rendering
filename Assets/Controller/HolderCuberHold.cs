using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolderCuberHold : MonoBehaviour, IPortableMesh
{
	public ConfigurableJoint joint;
	public bool preventSleep = true;
	
	//-- IPortableObject: -------------------------------------
	public Rigidbody rb		{ get; private set; }
	public Vector3 down		{ get; set; } = Vector3.down;
	public Vector3 center	=> transform.position;
	public float scale
	{	
		get => _scale;
		set
		{
			_scale = value;
			transform.localScale = Vector3.one * _scale;
		}
	} private float _scale = 1;

	//-- IPortableMesh: ---------------------------------------
	public Mesh mesh	=> GetComponent<MeshFilter>().mesh;
	public Material mat	=> GetComponent<MeshRenderer>().sharedMaterial;

	private bool attached = false;

	void OnEnable()
	{
		rb = GetComponent<Rigidbody>();
		rb.useGravity = false;
		rb.solverIterations = 32;
	}

	void Update ()
	{
		if (attached && Input.GetKeyDown(KeyCode.E))
		{
			joint.connectedBody	= null;
			attached			= false;
			if (preventSleep && rb.IsSleeping()) { rb.WakeUp(); }
		}
		else if (!attached && Input.GetKeyDown(KeyCode.E) && Vector3.Distance(joint.transform.position, rb.transform.position) < 6)
		{
			joint.connectedBody	= rb;
			attached			= true;
		}
	}

	void LateUpdate()
	{
		foreach (var portal in Portal.portals)
			if (this.IntersectsPortalBounds(portal))
				portal.DrawMesh(this);
	}

	void FixedUpdate()
	{
		rb.AddForce(down * Physics.gravity.magnitude * scale, ForceMode.Acceleration);
	}
}
