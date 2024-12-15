using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using Terraria.Graphics.Effects;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;

namespace WizenkleBoss.Assets.Helper
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

        private const string ShaderPath = "WizenkleBoss/Assets/Effects/";
        private static void RegisterMiscShader(Asset<Effect> shader, string passName, string registrationName)
        {
            Asset<Effect> shaderPointer = shader;
            MiscShaderData passParamRegistration = new(shaderPointer, passName);
            GameShaders.Misc[$"WizenkleBoss:{registrationName}"] = passParamRegistration;
        }
        public static void LoadShaders()
        {
            static Asset<Effect> LoadShader(string path) => ModContent.Request<Effect>($"{ShaderPath}{path}", AssetRequestMode.ImmediateLoad);
            string pass = "AutoloadPass";

            RotationShader = LoadShader("Shaders/Rotation");
            RegisterMiscShader(RotationShader, pass, "Rotation");
            PixelationShader = LoadShader("Shaders/PixelationShader");
            RegisterMiscShader(PixelationShader, pass, "Pixelation");

            ObjectBarrierShader = LoadShader("Shaders/BarrierButSingleLayer");
            RegisterMiscShader(ObjectBarrierShader, pass, "BarrierShader");

            CoronariesShader = LoadShader("Shaders/Coronaries");
            RegisterMiscShader(CoronariesShader, pass, "CoronariesShader");

            GlitchyShader = LoadShader("Shaders/Glitchy");
            RegisterMiscShader(GlitchyShader, pass, "GlitchyShader");

            PlanetShader = LoadShader("Shaders/Planet");
            RegisterMiscShader(PlanetShader, pass, "PlanetShader");

            FrostyLensShader = LoadShader("Shaders/FrostyLens");
            RegisterMiscShader(FrostyLensShader, pass, "FrostyLensShader");

            TransmitShader = LoadShader("Shaders/Deathray");
            RegisterMiscShader(TransmitShader, pass, "TransmitShader");

            OldMonitorShader = LoadShader("Shaders/OldMonitor");
            RegisterMiscShader(OldMonitorShader, pass, "OldMonitor");

            Asset<Effect> BarrierShader = LoadShader("Shaders/Barrier");
            Filters.Scene[$"WizenkleBoss:{InkShaderData.ShaderKey}"] = new Filter(new InkShaderData(BarrierShader, pass), EffectPriority.VeryHigh);
            Filters.Scene[$"WizenkleBoss:{InkShaderData.ShaderKey}"].Load();
        }
    }
}
