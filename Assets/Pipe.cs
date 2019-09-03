using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Pipe : RenderPipeline
{
	protected override void Render(ScriptableRenderContext context, Camera[] cameras)
	{
		foreach (var camera in cameras)
			RenderCamera(context, camera);

		context.Submit();
	}

	private void RenderCamera(ScriptableRenderContext context, Camera camera)
	{
		context.SetupCameraProperties(camera);
		// context.DrawSkybox(camera);

		CullingResults cullResults = new CullingResults();
		if (camera.TryGetCullingParameters(out var cullingParams))
			cullResults = context.Cull(ref cullingParams);

		var buffer = new CommandBuffer();
		buffer.ClearRenderTarget(true, true, Color.cyan);
		context.ExecuteCommandBuffer(buffer);
		buffer.Release();

		var drawSettings = new DrawingSettings(new ShaderTagId("SRPDefaultUnlit"), new SortingSettings(camera));
		var filterSettings = new FilteringSettings(new RenderQueueRange(0, 5000));

		context.DrawRenderers(cullResults, ref drawSettings, ref filterSettings);
	}
}
