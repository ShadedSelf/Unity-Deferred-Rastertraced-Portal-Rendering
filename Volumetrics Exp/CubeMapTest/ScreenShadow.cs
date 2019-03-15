using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShadow : MonoBehaviour
{

	public Shader screenShadowShader;
	public RenderTexture screenShadowTexture;

	Camera cam;


	void Start()
	{
		cam = GetComponent<Camera>();
		screenShadowTexture = new RenderTexture(Screen.width, Screen.height, 32);
	}


	void Update()
	{
		cam.targetTexture = screenShadowTexture;
		cam.RenderWithShader(screenShadowShader, "RenderType");
		cam.targetTexture = null;
	}
}
