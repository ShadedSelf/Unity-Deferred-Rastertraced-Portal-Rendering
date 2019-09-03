using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenie : MonoBehaviour {

	public Material mat;
	public int loops;

	Camera cam;


	void Start ()
	{
		cam = GetComponent<Camera>();
		cam.depthTextureMode = DepthTextureMode.Depth;
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		Matrix4x4 trs = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.0f), Quaternion.identity, new Vector3(0.5f, 0.5f, 1.0f));
		//Matrix4x4 scrScale = Matrix4x4.Scale(new Vector3(cam.pixelWidth, cam.pixelHeight, 1.0f));
		Matrix4x4 projection = cam.projectionMatrix;

		Matrix4x4 m = /*scrScale **/ trs * projection;

		mat.SetMatrix("_CameraInverseProjectionMatrix", cam.projectionMatrix.inverse);
		mat.SetMatrix("_CameraProjectionMatrix", m);
		mat.SetMatrix("_CameraProjectionMatrixInverted", m.inverse);
		mat.SetMatrix("_W2C", cam.worldToCameraMatrix);
		mat.SetMatrix("_C2W", cam.cameraToWorldMatrix);

		mat.SetFloat("_Farer", cam.farClipPlane);
		if (loops < 1) loops = 1;
		mat.SetInt("_Loops", loops);

		mat.SetInt("screenX", Screen.width);
		mat.SetInt("screenY", Screen.height);

		Graphics.Blit(src, dst, mat);
	}
}
