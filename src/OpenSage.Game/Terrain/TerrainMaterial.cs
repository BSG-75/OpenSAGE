﻿using OpenSage.Graphics;
using OpenSage.Graphics.Effects;
using Veldrid;

namespace OpenSage.Terrain
{
    public sealed class TerrainMaterial : EffectMaterial
    {
        public override uint? SlotGlobalConstantsShared => 0;
        public override uint? SlotGlobalConstantsVS => 1;
        public override uint? SlotRenderItemConstantsVS => 2;
        public override uint? SlotLightingConstants_Terrain => 3;

        public TerrainMaterial(Effect effect)
            : base(effect)
        {
            SetProperty(8, effect.GraphicsDevice.Aniso4xSampler);

            PipelineState = new EffectPipelineState(
                RasterizerStateDescriptionUtility.DefaultFrontIsCounterClockwise,
                DepthStencilStateDescription.DepthOnlyLessEqual,
                BlendStateDescription.SingleDisabled);
        }

        public void SetTileData(Texture tileDataTexture)
        {
            SetProperty(4, tileDataTexture);
        }

        public void SetCliffDetails(DeviceBuffer cliffDetailsBuffer)
        {
            SetProperty(5, cliffDetailsBuffer);
        }

        public void SetTextureDetails(DeviceBuffer textureDetailsBuffer)
        {
            SetProperty(6, textureDetailsBuffer);
        }

        public void SetTextureArray(Texture textureArray)
        {
            SetProperty(7, textureArray);
        }

        public static ResourceLayoutElementDescription[] ResourceLayoutDescriptions = new[]
        {
            new ResourceLayoutElementDescription("GlobalConstantsShared", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment),
            new ResourceLayoutElementDescription("GlobalConstantsVS", ResourceKind.UniformBuffer, ShaderStages.Vertex),

            new ResourceLayoutElementDescription("RenderItemConstantsVS", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            
            new ResourceLayoutElementDescription("LightingConstants_Terrain", ResourceKind.UniformBuffer, ShaderStages.Fragment),

            new ResourceLayoutElementDescription("TileData", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
            new ResourceLayoutElementDescription("CliffDetails", ResourceKind.StructuredBufferReadOnly, ShaderStages.Fragment),
            new ResourceLayoutElementDescription("TextureDetails", ResourceKind.StructuredBufferReadOnly, ShaderStages.Fragment),
            new ResourceLayoutElementDescription("Textures", ResourceKind.TextureReadOnly, ShaderStages.Fragment),

            new ResourceLayoutElementDescription("Sampler", ResourceKind.Sampler, ShaderStages.Fragment)
        };
    }
}
