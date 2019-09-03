using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/PipeTest")]
public class PipeAsset : RenderPipelineAsset
{
	protected override RenderPipeline CreatePipeline()
	{
		return new Pipe();
	}
}
