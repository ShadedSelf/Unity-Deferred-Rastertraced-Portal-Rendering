using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic : MonoBehaviour {

	Camera cam;
	Camera parCam;

	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera>();
		parCam = transform.parent.gameObject.GetComponent<Camera>();
		
	}
	
	// Update is called once per frame
	void Update () {
		cam.fieldOfView = parCam.fieldOfView;
		cam.farClipPlane = parCam.farClipPlane;
		cam.nearClipPlane = parCam.nearClipPlane;
	}
}
