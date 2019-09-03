using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraThing : MonoBehaviour {

	public Shader depth;
	public RenderTexture temp;

	Camera cam;

	void Start()
	{
		cam = GetComponent<Camera>();
		//cam.SetReplacementShader(depth, "RenderType");
	}

	void Update()
	{
		cam.SetReplacementShader(depth, "RenderType");
		Shader.SetGlobalTexture("_LightDepthTex", temp);
		Shader.SetGlobalMatrix("me_WorldToCamera", cam.worldToCameraMatrix);
		Shader.SetGlobalFloat("_FarClip", cam.farClipPlane);
		Shader.SetGlobalFloat("_NearClip", cam.nearClipPlane);
		Shader.SetGlobalVector("_LightPos", transform.position);

		cam.RenderToCubemap(temp);
	}
}
