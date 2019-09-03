using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zRay : MonoBehaviour {

    public int steps = 256;
    public float pow = 1;
    public Shader shader;
    public Texture noise;

    Material mat;
    Camera cam;

    [ContextMenu("Reload")]
    void OnEnable ()
    {
        mat = new Material(shader);
        cam = GetComponent<Camera>();

        mat.SetTexture("_Noise", noise);
	}

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Vector4[] origins = new Vector4[4] { cam.ScreenPointToRay(new Vector3(0, Screen.height - 1, 0)).origin, cam.ScreenPointToRay(new Vector3(Screen.width - 1, Screen.height - 1, 0)).origin,
            cam.ScreenPointToRay(new Vector3(0, 0, 0)).origin, cam.ScreenPointToRay(new Vector3(Screen.width - 1, 0, 0)).origin};
        //Shader.SetGlobalVectorArray("_Origins", origins);
        mat.SetVectorArray("_Origins", origins);
        mat.SetInt("_Steps", steps);
        mat.SetFloat("_POW", pow);

        Graphics.Blit(src, dst, mat);
    }
}
