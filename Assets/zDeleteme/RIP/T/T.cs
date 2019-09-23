using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T : MonoBehaviour {

	public ComputeShader shader;
	public RenderTexture rt;

	// Use this for initialization
	void Start () {
		rt = new RenderTexture(32, 32, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
		rt.enableRandomWrite = true;
		rt.Create();

		shader.SetTexture(0, "rt", rt);
	}
	
	// Update is called once per frame
	void Update () {
		shader.Dispatch(0, 2, 2, 1);
	}
}
