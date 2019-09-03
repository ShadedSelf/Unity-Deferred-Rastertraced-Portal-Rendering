using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Unity.Mathematics;
using static Unity.Mathematics.math;

public class WorldToCamera : MonoBehaviour
{
	Camera cam;

	void OnEnable()
	{
		cam = GetComponent<Camera>();
	}

	void LateUpdate()
	{
		Shader.SetGlobalMatrix("me_WorldToCamera", cam.worldToCameraMatrix);

		// SceneView.lastActiveSceneView.camera.fieldOfView = 100;
		// SceneView.lastActiveSceneView.Repaint();
	}
}
