using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Vol : MonoBehaviour
{

    public ComputeShader cs;
    public FilterMode filter;
    public RenderTexture volume;
    public Material mat;

    public Vector3Int res = Vector3Int.one * 128;
    public float pow = 1;
    public float c = 5;

    Camera cam;

    [ContextMenu("Reload")]
    void OnEnable()
    {
        volume = new RenderTexture(res.x, res.y, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear); // RFloat read and write
        volume.enableRandomWrite = true;
        volume.dimension = TextureDimension.Tex3D;
        volume.volumeDepth = res.z;
        volume.filterMode = filter;
        volume.Create();

        cs.SetTexture(0, "_Volume", volume);
        cs.SetTexture(1, "_Volume", volume);

        mat.SetTexture("_MainTex", volume);

        Shader.SetGlobalTexture("_Volume", volume);
        cam = GetComponent<Camera>();

        //Buff();
    }

    void Update()
    {
        SetVariables();

        cs.Dispatch(0, res.x / 8, res.y / 8, res.z / 8);
        cs.Dispatch(1, res.x / 32, res.y / 32, 1);
    }

    void SetVariables()
    {
        Matrix4x4 matrix = cam.worldToCameraMatrix.inverse * cam.projectionMatrix.inverse;

        cs.SetMatrix("_LocalToWorldFrustrum", matrix);
        cs.SetFloat("_POW", pow);
        cs.SetVector("_CamDir", cam.transform.forward);
        cs.SetFloat("_EdgeDis", Vector3.Distance(cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)), cam.ScreenToWorldPoint(new Vector3(0, 0, cam.farClipPlane))));
        cs.SetFloat("_C", c);
        cs.SetInts("_Res", new int[3] { res.x, res.y, res.z });

        Shader.SetGlobalMatrix("_LTWF", matrix);
        Shader.SetGlobalMatrix("_iLTWF", cam.projectionMatrix * cam.worldToCameraMatrix);
        Shader.SetGlobalFloat("_POW", pow);

        Vector4[] origins = new Vector4[4]
		{
			cam.ScreenPointToRay(new Vector3(0, Screen.height - 1, 0)).origin,
			cam.ScreenPointToRay(new Vector3(Screen.width - 1, Screen.height - 1, 0)).origin,
            cam.ScreenPointToRay(new Vector3(0, 0, 0)).origin,
			cam.ScreenPointToRay(new Vector3(Screen.width - 1, 0, 0)).origin
		};
        Shader.SetGlobalVectorArray("_Origins", origins);
        cs.SetVectorArray("_Origins", origins);

        Vector4[] directions = new Vector4[4]
		{
			cam.ScreenPointToRay(new Vector3(0, Screen.height - 1, 0)).direction,
			cam.ScreenPointToRay(new Vector3(Screen.width - 1, Screen.height - 1, 0)).direction,
            cam.ScreenPointToRay(new Vector3(0, 0, 0)).direction,
			cam.ScreenPointToRay(new Vector3(Screen.width - 1, 0, 0)).direction
		};
        Shader.SetGlobalVectorArray("_Directions", directions);
        cs.SetVectorArray("_Directions", directions);

        // Shader.SetGlobalFloat("_EdgeDis", Vector3.Distance(cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)), cam.ScreenToWorldPoint(new Vector3(0, 0, cam.farClipPlane))));

        cs.SetFloat("_DT", Time.deltaTime);

        //cs.SetTextureFromGlobal(0, "_Temp", "_CameraDepthTexture");
        float far = cam.farClipPlane;
        float near = cam.nearClipPlane;
        Vector4 depthParams = new Vector4(near, far, (1 - far / near) / far, (far / near) / far);
        cs.SetVector("_DepthParams", depthParams);
    }
}
