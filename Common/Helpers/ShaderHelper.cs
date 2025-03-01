﻿using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using Terraria.Graphics.Effects;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using WizenkleBoss.Common.Ink;

namespace WizenkleBoss.Common.Helpers
{
    public static partial class Helper
    {
        internal static Asset<Effect> RotationShader;
        internal static Asset<Effect> ObjectBarrierShader;
        internal static Asset<Effect> PixelationShader;
        internal static Asset<Effect> CoronariesShader;
        internal static Asset<Effect> GlitchyShader;
        internal static Asset<Effect> PlanetShader;
        internal static Asset<Effect> FrostyLensShader;
        internal static Asset<Effect> TransmitShader;
        internal static Asset<Effect> OldMonitorShader;

        internal static Asset<Effect> WaterProcessor;
        internal static Asset<Effect> WaterInkColorizer;

        internal static Filter InkShader;

        private const string ShaderPath = "WizenkleBoss/Assets/Effects/";

        public static void LoadShaders()
        {
            static Asset<Effect> LoadShader(string path) => ModContent.Request<Effect>($"{ShaderPath}{path}"); // wooooo async loadinggg !! (lolxd would be proud)
            string pass = "AutoloadPass";

            RotationShader = LoadShader("Shaders/Rotation");
            PixelationShader = LoadShader("Shaders/PixelationShader");

            ObjectBarrierShader = LoadShader("Shaders/BarrierButSingleLayer");

            CoronariesShader = LoadShader("Shaders/Coronaries");

            GlitchyShader = LoadShader("Shaders/Glitchy");

            PlanetShader = LoadShader("Shaders/Planet");

            FrostyLensShader = LoadShader("Shaders/FrostyLens");

            TransmitShader = LoadShader("Shaders/Deathray");

            OldMonitorShader = LoadShader("Shaders/OldMonitor");

            WaterProcessor = LoadShader("Water/WaterProcesser");

            WaterInkColorizer = LoadShader("Water/WaterProcesser");

            Asset<Effect> BarrierShader = LoadShader("Shaders/Barrier");
            InkShader = new(new InkShaderData(BarrierShader, pass)); // EffectPriority is completely irrelevant (I am forcefully applying my filter after normal filters are applied).
            InkShader.Load(); // not necessary ?
        }
    }
}
