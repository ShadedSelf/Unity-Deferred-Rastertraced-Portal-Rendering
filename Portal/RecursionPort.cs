using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursionPort : MonoBehaviour
{
	public GameObject otherPort;
	[Range(0, 100)]
	public int recursions;
	public LayerMask mask = -1;

	private Camera cam { get; set; }
	private RenderTexture tex;
	private RenderTexture tempTex;

	void OnEnable()
	{
		gameObject.layer = 8;
		if (GetComponent<MeshCollider>() != null)
		{
			GetComponent<MeshCollider>().convex = true;
			GetComponent<MeshCollider>().isTrigger = true;
		}
		tex = new RenderTexture(Screen.width, Screen.height, 24);
		tempTex = new RenderTexture(Screen.width, Screen.height, 24);
		GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("UV/UV Remap"));
		UpdateCam();
	}

	void Start()
	{
		GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MainTex", otherPort.GetComponent<RecursionPort>().tex);
	}

	void LateUpdate()
	{
		// if (otherPort.GetComponent<Renderer>().IsVisibleFrom(Camera.main))
			GetCamPos();
	}

	public bool obliqueMatrix = false;
	public float cameraOffset = 0.0f;
	public float obliquePlaneOffset = 0.01f;

	private Vector3 camPos;
	private bool resetProjectionMatrix = true;
	public void GetCamPos()
	{
		for (int i = recursions; i >= 0; i--)
		{
			camPos = otherPort.transform.InverseTransformPoint(Camera.main.transform.position);
			camPos.x = -camPos.x;
			camPos.z = (-camPos.z + i * (Vector3.Distance(transform.localPosition, otherPort.transform.localPosition) / transform.localScale.z) - cameraOffset);
			cam.transform.localPosition = camPos;
			cam.transform.localRotation = Quaternion.AngleAxis(180.0f, new Vector3(0, 1, 0)) * Quaternion.LookRotation(otherPort.transform.InverseTransformDirection(Camera.main.transform.forward), otherPort.transform.InverseTransformDirection(Camera.main.transform.up));

			if (obliqueMatrix)
			{
				cam.projectionMatrix = cam.CalculateObliqueMatrix(RandomExtensions.CameraSpacePlane(cam, transform.position, transform.forward, -1, obliquePlaneOffset));
				resetProjectionMatrix = true;
			}
			else if (resetProjectionMatrix)
			{
				cam.ResetProjectionMatrix();
				resetProjectionMatrix = false;
			}
					
			if (recursions == 0)
			{
				cam.Render();
				return;
			}
			
			cam.targetTexture = tempTex;
			cam.Render();
			Graphics.Blit(tempTex, tex);
			cam.targetTexture = null;
		}
	}

	private float eps = 0.0010011f;
	void OnTriggerStay(Collider thing)
	{
		if (transform.InverseTransformPoint(thing.transform.position).z >= -0.001001)
		{
			if (thing.GetComponent<RBControllerTest>() != null)
			{
				thing.GetComponent<RBControllerTest>().downNormal = -otherPort.transform.up;
				// thing.GetComponent<RBControllerTest>().speed *= 1 / (transform.lossyScale.y / otherPort.transform.lossyScale.y);
				// thing.GetComponent<RBControllerTest>().jumpForce *= 1 / (transform.lossyScale.y / otherPort.transform.lossyScale.y);
				// thing.GetComponent<RBControllerTest>().downForce *= 1 / (transform.lossyScale.y / otherPort.transform.lossyScale.y);
				// thing.GetComponent<RBControllerTest>().downForceHolder *= 1 / (transform.lossyScale.y / otherPort.transform.lossyScale.y);
				var tmp = thing.transform;
				GetPos(ref tmp, eps);
			}
			if (thing.GetComponent<GravityModifier>() != null && transform.InverseTransformPoint(thing.transform.position).z >= -Mathf.Epsilon)
			{
				thing.GetComponent<GravityModifier>().gravityNormal = -otherPort.transform.up;
				thing.GetComponent<GravityModifier>().gravity *= 1 / (transform.lossyScale.y / otherPort.transform.lossyScale.y);
				var tmp = thing.transform;
				GetPos(ref tmp, Mathf.Epsilon);
			}
		}
	}

	void GetPos(ref Transform relativePos, float epsislon)
	{
		Vector3 tempPos = transform.InverseTransformPoint(relativePos.position);
		tempPos.x = -tempPos.x;
		tempPos.z = -epsislon;
		relativePos.position = otherPort.transform.TransformPoint(tempPos);


		GameObject tmpOb = new GameObject();
		Transform tmpForm = tmpOb.transform;

		tmpForm.transform.parent = otherPort.transform;
		tmpForm.transform.localRotation = Quaternion.AngleAxis(180.0f, new Vector3(0, 1, 0)) * Quaternion.LookRotation(transform.InverseTransformDirection(relativePos.forward), transform.InverseTransformDirection(relativePos.up));
		relativePos.transform.rotation = tmpForm.transform.rotation;
		relativePos.localScale = relativePos.lossyScale * 1 / (transform.lossyScale.y / otherPort.transform.lossyScale.y);

		tmpForm.transform.forward = relativePos.gameObject.GetComponent<Rigidbody>().velocity.normalized;
		tmpForm.transform.localRotation = Quaternion.AngleAxis(180.0f, new Vector3(0, 1, 0)) * Quaternion.LookRotation(transform.InverseTransformDirection(tmpForm.transform.forward), transform.InverseTransformDirection(tmpForm.transform.up));
		relativePos.gameObject.GetComponent<Rigidbody>().velocity = tmpForm.transform.forward * relativePos.gameObject.GetComponent<Rigidbody>().velocity.magnitude * 1 / (transform.lossyScale.y / otherPort.transform.lossyScale.y);

	}

	public void UpdateCam()
	{
		var camObject = new GameObject("cam");
		camObject.transform.parent = transform;
		cam = camObject.AddComponent<Camera>();
		cam.enabled = false;
		cam.targetTexture = tex;

		cam.nearClipPlane = Camera.main.nearClipPlane;
		cam.farClipPlane = Camera.main.farClipPlane;
		cam.fieldOfView = Camera.main.fieldOfView;
		cam.backgroundColor = Camera.main.backgroundColor;
		cam.cullingMask = mask;
	}

	void OnDisable()
	{
		Destroy(cam);
		tex.Release();
		tempTex.Release();
	}
}
