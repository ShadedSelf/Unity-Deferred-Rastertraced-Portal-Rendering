using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Mathematics;
using static Unity.Mathematics.math;

public class TransformData
{
	public Vector3 position		{ get; set; } = Vector3.zero;
	public Quaternion rotation	{ get; set; } = Quaternion.identity;
	public Vector3 scale		{ get; set; } = Vector3.one;

	public TransformData() { }
	public TransformData(Transform transform) : this(transform.position, transform.rotation, transform.lossyScale) { }
	public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
	{
		this.position	= position;
		this.rotation	= rotation;
		this.scale		= scale;
	}
}

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
	public static Camera currentCamera;
	// public static Controller current;
	// public Matrix4x4 matrix = Matrix4x4.identity;

	//-- ActualStuff: ------------------
	private CameraController camCntrl;
	private MovementController mover;
	private RotatorController rotator;

	private RenderSystemTest renderTest;

    private Transform pivotTrs, camTrs;
	private Camera cam;

	//-- IPortable: -------------------------------------------
 	public Rigidbody rb		=> GetComponent<Rigidbody>();
	public Vector3 center	=> camTrs.position;
	public Vector3 down		{ get { return mover.down;	} set { mover.down	= value; }	}
	public float scale		{ get { return mover.scale;	} set { mover.scale	= value; }	}

    void OnEnable()
    {
		cam					= GetComponentInChildren<Camera>();
		pivotTrs			= transform.GetChild(0).transform;
		camTrs				= cam.transform;
		cam.nearClipPlane	= 0.00001f;
		currentCamera		= cam;

		camCntrl	= new CameraController(pivotTrs, camTrs);
		mover		= new MovementController(transform, pivotTrs);
		rotator		= new RotatorController(transform);
		
		renderTest	= new RenderSystemTest();

		UpdateSettings();

		// Physics.autoSimulation = false;
		// Physics.autoSyncTransforms = false;
		Time.fixedDeltaTime = 0.01f;

		rb.solverIterations = 32;
		// scale = 0.1f;
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

		// if (needsToRotate) {
		var rot = camTrs.rotation;
		rotator.RotateBody(down, Time.deltaTime); //Lock rotation till reaches new normal
		var rel = Quaternion.Inverse(rot) * camTrs.rotation; //Relative
		camCntrl.Correct(rel);
    }

	void FixedUpdate()
	{
		mover.Test(Time.fixedDeltaTime);
		mover.TestDown(Time.fixedDeltaTime);
	}

	void LateUpdate()
	{

		renderTest.ClearBuffers();
		renderTest.SetInitialTransform(camTrs);

		var matrix = Matrix4x4.Scale(float3(1f / scale));

		// var map = PortalRay.GetPortalMap(cam);
		// renderTest.RenderPortalMap(map, new TransformData(camTrs), matrix, 0, 0); //Allocations
		// PortalRay.ClearAndReleaseRecursiveDown(map);

		renderTest.LazyRender(new TransformData(camTrs), matrix, 0, 0);

		renderTest.Edge();

		// dynamic why = 0;
		// why.HelpMeNotToCrash(); //int extension
	}

	void OnGUI()
    {
        if (Event.current.type.Equals(EventType.Repaint))
            Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), renderTest.render);
    }

	void OnDisable()
	{
		renderTest.Cleanup();
	}

	void OnDrawGizmos()
	{

	}
}
