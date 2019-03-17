using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class RBControllerTest : MonoBehaviour
{
    [Header("Useless")]
    public float xMouse = 200;
    public float yMouse = 150;
    public Transform pivot;
    [Header("Body Draw")]
    public Mesh mesh;
    public Material mat;
    public bool castShadows = false;
    [Header("Params")]
    public int maxJumps = 3;
    int currJumps;
    public float moveSpeed = 15;
    public float jumpForce = 10;
    public float downForce = 10;
    public bool normalize = true;
    [Header("Bug")]
    public bool floored = false;
    public bool walled = false;
    public bool rotating = false;

    Rigidbody rb;
    Camera cam;
    Vector3 moveDir;
    Vector3 prevPos;
    public Vector3 downNormal = Vector3.down;
    public float mag;

    void Start()
    {
        currJumps = maxJumps;
		cam = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
		moveDir = pivot.TransformDirection(
			new Vector3(
			Input.GetKey(KeyCode.D) ? 1 :
			Input.GetKey(KeyCode.A) ? -1 : 0, 
			0, 
			Input.GetKey(KeyCode.W) ? 1 : 
			Input.GetKey(KeyCode.S) ? -1 : 0));
		if (normalize) { moveDir.Normalize(); }

		if (currJumps > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            currJumps--;
            mag = -jumpForce;
        }
        //shift for acceleration but hig top speeed


        //Graphics.DrawMesh(mesh, Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one + Vector3.up), mat, 0, null, 0, null, castShadows);
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
    }

    public void OnFloorHit()
    {
        currJumps = maxJumps;
    }

    Vector3 vel;
    Vector3 lastN;
    void FixedUpdate()
    {
        float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * xMouse * Time.fixedDeltaTime;
		float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * yMouse * Time.fixedDeltaTime;
		pivot.localRotation *= Quaternion.Euler(0f, yRot, 0f);
		cam.transform.localRotation *= Quaternion.Euler(-xRot, 0f, 0f);

        Vector3 rotatedNormal = Vector3.SmoothDamp(transform.up, -downNormal, ref vel, .15f);
        rotating = ((rotatedNormal - lastN).magnitude > 0) ? true : false;
        lastN = rotatedNormal;

		if (rotating)
		{
			rb.rotation = Quaternion.FromToRotation(transform.up, rotatedNormal) * rb.rotation;

            pivot.rotation = Quaternion.FromToRotation(rotatedNormal, transform.up) * pivot.rotation;
            pivot.localRotation = Quaternion.Euler(0, pivot.localRotation.eulerAngles.y, 0);

            cam.transform.rotation = Quaternion.FromToRotation(rotatedNormal, transform.up) * cam.transform.rotation;
            cam.transform.localRotation = Quaternion.Euler(cam.transform.localRotation.eulerAngles.x, 0, 0);
        }

		// Ray ray = new Ray(cam.transform.position, -cam.transform.up); //make dummy transform, smooth from last rotation to new, also use the dummy for the camera thing
		// RaycastHit hit = new RaycastHit();
        // if (Physics.Raycast(ray, out hit, 20))
        // {
        //     if (Vector3.Dot(hit.normal, -cam.transform.up) < -.999f && Vector3.Dot(hit.normal, transform.up) < .5f)
        //     {
		// 		pivot.localPosition = new Vector3(0, .16f / transform.localScale.y, 0);
		// 		rb.transform.localScale = new Vector3(transform.localScale.x, Vector3.Distance(hit.point, transform.position), transform.localScale.z);

		// 		downNormal = new Vector3(
		// 			(Mathf.Abs(hit.normal.x) < .00001f) ? 0 : -hit.normal.x,
		// 			(Mathf.Abs(hit.normal.y) < .00001f) ? 0 : -hit.normal.y,
		// 			(Mathf.Abs(hit.normal.z) < .00001f) ? 0 : -hit.normal.z);
		// 		rb.rotation = Quaternion.FromToRotation(transform.up, -downNormal) * rb.rotation;
        //         cam.transform.rotation = Quaternion.FromToRotation(-downNormal, transform.up) * cam.transform.rotation;
        //     }
        // }

        if (floored && mag > 0) { mag = 0; }
        rb.velocity = downNormal * mag;
        rb.velocity += moveDir * moveSpeed * ((Input.GetKey(KeyCode.LeftShift) ? .5f : 1));
        mag = Mathf.Clamp(mag + downForce * Time.fixedDeltaTime * ((Input.GetKey(KeyCode.Space) && mag < 0) ? 1 : 2), -jumpForce, 30); //no space should also make me fall faster
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + downNormal * 3);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * 3);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);

        if (cam != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(cam.transform.position, cam.transform.position - cam.transform.forward);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(cam.transform.position, cam.transform.position - cam.transform.up);
        }
    }
}
