using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using Terraria.Graphics.Effects;
using WizenkleBoss.Common.Ink;

namespace WizenkleBoss.Common.Registries
{
    public class Shaders : ModSystem
    {
        internal static Asset<Effect> RotationShader;
        internal static Asset<Effect> ObjectInkShader;
        internal static Asset<Effect> PixelationShader;
        internal static Asset<Effect> CoronariesShader;
        internal static Asset<Effect> GlitchyShader;
        internal static Asset<Effect> PlanetShader;
        internal static Asset<Effect> FrostyLensShader;
        internal static Asset<Effect> TransmitShader;
        internal static Asset<Effect> OldMonitorShader;
        internal static Asset<Effect> StarAtmosphereShader;

        internal static Asset<Effect> WaterProcessor;

        internal static Filter InkShader;

        private const string ShaderPath = "WizenkleBoss/Assets/Effects/";

        public override void Load()
        {
            static Asset<Effect> LoadShader(string path) => ModContent.Request<Effect>($"{ShaderPath}{path}"); // wooooo async loadinggg !! (lolxd would be proud)
            string pass = "AutoloadPass";

            RotationShader = LoadShader("Shaders/Rotation");
            PixelationShader = LoadShader("Shaders/PixelationShader");

            ObjectInkShader = LoadShader("Shaders/BarrierButSingleLayer");

            CoronariesShader = LoadShader("Shaders/Coronaries");

            GlitchyShader = LoadShader("Shaders/Glitchy");

            PlanetShader = LoadShader("Shaders/Planet");

            FrostyLensShader = LoadShader("Shaders/FrostyLens");

            TransmitShader = LoadShader("Shaders/Deathray");

            OldMonitorShader = LoadShader("Shaders/OldMonitor");

            StarAtmosphereShader = LoadShader("Shaders/StarAtmosphere");

            WaterProcessor = LoadShader("Water/WaterProcesser");

            Asset<Effect> BarrierShader = LoadShader("Shaders/Barrier");
            InkShader = new(new InkShaderData(BarrierShader, pass)); // EffectPriority is completely irrelevant (I am forcefully applying my filter after normal filters are applied).
            InkShader.Load(); // not necessary ?
        }
    }
}
