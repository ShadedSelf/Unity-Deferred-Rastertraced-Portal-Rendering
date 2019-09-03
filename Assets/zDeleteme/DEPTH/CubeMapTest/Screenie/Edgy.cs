using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Edgy : MonoBehaviour
{
    public float posBias = .1f;
    public bool gCo;
    public bool mist;

    Material edge;
    Material gamat;
    Material mistMat;

    void OnEnable()
    {
        edge	= new Material(Shader.Find("Hidden/Edgy0"));
        gamat 	= new Material(Shader.Find("Hidden/Gami"));
        mistMat = new Material(Shader.Find("Hidden/Mist"));
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        edge.SetFloat("_PosBias", posBias);
        Graphics.Blit(src, edge);
        if (gCo)
            Graphics.Blit(src, dst, gamat);
        else if (mist)
            Graphics.Blit(src, dst, mistMat);
        else
            Graphics.Blit(src, dst);
    }
}