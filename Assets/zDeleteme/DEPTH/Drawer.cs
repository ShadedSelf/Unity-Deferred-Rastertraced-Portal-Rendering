using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class Drawer : MonoBehaviour {

	public Shader depth;
	public RenderTexture temp;

	//Camera camLight;
    Camera cam;

	public Matrix4x4 halfer;

	void Start ()
	{
        temp = new RenderTexture(4096, 4096, 24, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
        temp.Create();
        Shader.SetGlobalTexture("_LightMap", temp);

        /*camLight = GetComponent<Camera>();
        camLight.SetReplacementShader(depth, "RenderType");
        camLight.targetTexture = temp;

        cam = Camera.main;*/

        Halfer();
	}

	void Update()
	{
		/*Shader.SetGlobalMatrix("_Full", halfer * (camLight.projectionMatrix * camLight.worldToCameraMatrix));
		Shader.SetGlobalFloat("_FarClip", camLight.farClipPlane);
        Shader.SetGlobalVector("_LightDir", camLight.transform.forward);*/
    }

    public RenderTexture rt;
	void Halfer()
	{
        RenderTargetIdentifier shadowmap = BuiltinRenderTextureType.CurrentActive;
        rt = new RenderTexture(4096, 4096, 0, RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear);
        CommandBuffer cb = new CommandBuffer();

        cb.SetShadowSamplingMode(shadowmap, ShadowSamplingMode.RawDepth);
        cb.Blit(shadowmap, new RenderTargetIdentifier(rt));
        GetComponentInChildren<Light>().AddCommandBuffer(LightEvent.AfterShadowMap, cb);

       Shader.SetGlobalTexture("_Test", rt);

        halfer = Matrix4x4.zero;
		halfer.m00 = halfer.m03 = halfer.m11 = halfer.m13 = halfer.m22 = halfer.m23 = .5f;
		halfer.m33 = 1;
	}
}
