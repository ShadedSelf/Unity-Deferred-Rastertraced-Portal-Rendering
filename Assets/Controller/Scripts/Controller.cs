using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;
using static Unity.Mathematics.math;

public class Controller : MonoBehaviour, IPortableObject
{
	[Header("Mouse Settings")]
	public float xDelta = 200;
	public float yDelta = 150;

	[Header("Movement Settings")]
	public float movementSpeed	= 15;
    public float jumpForce		= 10;
    public float downForce		= 20;
	public int maxJumps			= 3;

	[Header("Collision Settings")]
	public LayerMask mask;

	[Header("Rotator Settings")]
	public float rotSpeed = 0.15f;

	//-- Globals: ----------------------
	// public static Controller current;
	// public Matrix4x4 matrix = Matrix4x4.identity;

	//-- ActualStuff: ------------------
	private CameraController camCntrl;
	private MovementController mover;
	private RotatorController rotator;

	private RenderSystemTest renderTest;

    private Transform pivotTrs, camTrs;

	//-- IPortable: -------------------------------------------
 	public Rigidbody rb		=> GetComponent<Rigidbody>();
	public Vector3 center	=> camTrs.position;
	public Vector3 down		{ get { return mover.down;	} set { mover.down	= value; }	}
	public float scale		{ get { return mover.scale;	} set { mover.scale	= value; }	}

    void OnEnable()
    {
		pivotTrs	= transform.GetChild(0).transform;
		camTrs		= GetComponentInChildren<Camera>().transform;

		camCntrl	= new CameraController(pivotTrs, camTrs);
		mover		= new MovementController(transform, pivotTrs);
		rotator		= new RotatorController(transform);
		
		renderTest	= new RenderSystemTest(camTrs.GetComponent<Camera>());

		UpdateSettings();

		GetComponentInChildren<Camera>().nearClipPlane = 0.0001f;
		// Physics.autoSimulation = false;
		// Physics.autoSyncTransforms = false;
		Time.fixedDeltaTime = 0.01f;

		rb.solverIterations = 32;
	}

	public void UpdateSettings()
	{
		camCntrl.xDelta		= xDelta;
		camCntrl.yDelta		= yDelta;

		mover.downForce		= downForce;
		mover.jumpForce		= jumpForce;
		mover.maxJumps		= maxJumps;
		mover.movementSpeed	= movementSpeed;
		mover.mask			= mask;

		rotator.rotSpeed	= rotSpeed;
	}

    void Update()
    {
		camCntrl.Lock();
		camCntrl.Rotate(Time.deltaTime);

		var rot = camTrs.rotation;
		rotator.RotateBody(down, Time.deltaTime); //Lock rotation till reaches new normal
		var rel = Quaternion.Inverse(rot) * camTrs.rotation; //Relative
		camCntrl.Correct(rel);

		// Physics.SyncTransforms();
		// Physics.Simulate(min(Time.deltaTime, 0.03f));
    }

	void FixedUpdate()
	{
		mover.Test(Time.fixedDeltaTime);
		mover.TestDown(Time.fixedDeltaTime);
	}

	void LateUpdate()
	{
		// Portal.time = 0f;

		Vector3 p = camTrs.position;
		Quaternion r = camTrs.rotation;
		Matrix4x4 tester = Matrix4x4.Scale(new Vector3(1f / scale, 1f / scale, 1f / scale));

		renderTest.ClearBuffers();
		Shader.SetGlobalMatrix("portalTransform", tester);

		var map = PortalRay.GetPortalMap(camTrs.GetComponent<Camera>());
		using (var dummyTrs = new TemporaryTransform(camTrs.position, camTrs.rotation, new Vector3(scale, scale, scale))) //Scale for oblique plane offset?
			renderTest.RenderPortalMap(dummyTrs.transform, map, 0, tester, 0); //Portal rendering might overlap
		PortalRay.ClearAndReleaseRecursive(map);

		// using (var dummyTrs = new TemporaryTransform(camTrs.position, camTrs.rotation, new Vector3(scale, scale, scale))) //Scale for oblique plane offset?
		// 	renderTest.LazyRender(dummyTrs.transform, 0, tester, 0); //Portal rendering might overlap

		renderTest.Edge();

		camTrs.SetPositionAndRotation(p, r);
		camTrs.GetComponent<Camera>().ResetProjectionMatrix();

		// dynamic why = 0;
		// why.HelpMeNotToCrash();
	}

	void OnGUI()
    {
        if (Event.current.type.Equals(EventType.Repaint))
            Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), renderTest.render);
    }

	void OnDrawGizmos()
	{

	}

	void OnDisable()
	{

	}
}
