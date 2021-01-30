using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ShadowingMaskRenderPassFeature : ScriptableRendererFeature
{
    class BlitCameraContent : ScriptableRenderPass
    {
        Material BlitMaterial;
        int blitRT;

        public BlitCameraContent()
        {
            blitRT = Shader.PropertyToID("_BackgroundRT");
            Shader blitShader = Shader.Find("Unlit/BlitBackground");
            BlitMaterial = new Material(blitShader);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(blitRT, cameraTextureDescriptor);
            ConfigureTarget(blitRT);
            ConfigureClear(ClearFlag.All, Color.clear);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            Camera cam = renderingData.cameraData.camera;
            CommandBuffer cmd = CommandBufferPool.Get("RenderMask");
            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, BlitMaterial);
            cmd.SetViewProjectionMatrices(cam.worldToCameraMatrix, cam.projectionMatrix);
            cmd.SetGlobalTexture(blitRT, blitRT);
            context.ExecuteCommandBuffer(cmd);
        }

        /// Cleanup any allocated resources that were created during the execution of this render pass.
        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(blitRT);
        }
    }

    class ShadowingMaskRenderPass : ScriptableRenderPass
    {
        int forground;

        public ShadowingMaskRenderPass()
        {
            forground = Shader.PropertyToID("_ForgroundMask");
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(forground, cameraTextureDescriptor);
            cmd.SetGlobalTexture(forground, forground);
            ConfigureTarget(forground);
            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("RenderMask");
            RenderQueueRange renderQueueRange = RenderQueueRange.all;
            FilteringSettings filters = new FilteringSettings(renderQueueRange);
            filters.layerMask = LayerMask.GetMask(new string[] { "ViewMasks" });
            List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();
            m_ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));
            m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
            ShaderTagId id = new ShaderTagId("Opaque");
            DrawingSettings drawingSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, SortingCriteria.CommonOpaque);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filters);
        }

        /// Cleanup any allocated resources that were created during the execution of this render pass.
        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(forground);
        }
    }

    class BlitMaskRenderPass : ScriptableRenderPass
    {
        Material BlitMaterial;
        int forground;

        public BlitMaskRenderPass()
        {
            forground = Shader.PropertyToID("_ForgroundMask");
            Shader blitShader = Shader.Find("Unlit/BlitShader");
            BlitMaterial = new Material(blitShader);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            ConfigureClear(ClearFlag.None, Color.clear);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            Camera cam = renderingData.cameraData.camera;
            CommandBuffer cmd = CommandBufferPool.Get("RenderMask");
            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, BlitMaterial);
            cmd.SetViewProjectionMatrices(cam.worldToCameraMatrix, cam.projectionMatrix);
            context.ExecuteCommandBuffer(cmd);
        }

        /// Cleanup any allocated resources that were created during the execution of this render pass.
        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }

    class FullyOccludedRenderPass : ScriptableRenderPass
    {
        public FullyOccludedRenderPass()
        {
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            ConfigureClear(ClearFlag.None, Color.clear);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("FullyCulled");
            context.ExecuteCommandBuffer(cmd);
            RenderQueueRange renderQueueRange = RenderQueueRange.all;
            FilteringSettings filters = new FilteringSettings(renderQueueRange);
            filters.layerMask = LayerMask.GetMask(new string[] { "FullyMaskedObjs" });
            List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();
            m_ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));
            m_ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
            DrawingSettings drawingSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, SortingCriteria.CommonOpaque);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filters);
        }

        /// Cleanup any allocated resources that were created during the execution of this render pass.
        public override void FrameCleanup(CommandBuffer cmd)
        {
            //cmd.ReleaseTemporaryRT(maskedRT);
        }
    }

    //拷贝已经画的物体
    BlitCameraContent m_BlitContentPass;
    //画会被隐藏的物体
    FullyOccludedRenderPass m_DrawFullyOccludePass;
    //画mask
    ShadowingMaskRenderPass m_ScriptablePass;
    //最终合并
    BlitMaskRenderPass m_BlitPass;

    public override void Create()
    {
        m_BlitContentPass = new BlitCameraContent();
        m_DrawFullyOccludePass = new FullyOccludedRenderPass();
        m_ScriptablePass = new ShadowingMaskRenderPass();
        m_BlitPass = new BlitMaskRenderPass();
        // Configures where the render pass should be injected.
        m_BlitContentPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        m_DrawFullyOccludePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        m_BlitPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_BlitContentPass);
        renderer.EnqueuePass(m_DrawFullyOccludePass);

        renderer.EnqueuePass(m_ScriptablePass);
        renderer.EnqueuePass(m_BlitPass);
    }
}


