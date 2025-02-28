using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using WizenkleBoss.Common.Config;
using RealisticSky;
using RealisticSky.Content.NightSky;
using RealisticSky.Content.Sun;
using RealisticSky.Assets;
using RealisticSky.Content.Clouds;

namespace WizenkleBoss.Common.StarRewrite
{
        // Disable JIT when loading this class without realistic sky.
    [ExtendsFromMod("RealisticSky")]
    public class RealisticSkyCompatSystem : ModSystem
    {
        public static Hook DisableRealisticStars;
        public delegate void orig_StarsRender(float opacity, Matrix backgroundMatrix);

        public static ILHook MatchStarRotation;

        public static Hook DisableRealisticSun;
        public delegate void orig_SunRender(float sunriseAndSetInterpolant);

        public static Hook IWouldActuallyLikeToMoveMyCelestialBodiesOnTheTitleScreenThankYouVeryMuch;
        public delegate void orig_VerticallyBiasSunAndMoon();

        public static bool RealisticSkyEnabled = false;

        private static bool RealisticClouds => RealisticSkyConfig.Instance.RealisticClouds;
        private static Texture2D CloudDensityMap => TexturesRegistry.CloudDensityMap.Value;
        private static float CloudHorizontalOffsetValue => CloudsRenderer.CloudHorizontalOffset;

        private static bool DrawingRealisticStarsCorrectly;

        public override void Load()
        {
                // If this loads at all realistic sky is enabled.
            RealisticSkyEnabled = true;

            MethodInfo RenderStars = typeof(StarsRenderer).GetMethod("Render", BindingFlags.Public | BindingFlags.Static);

            if (RenderStars != null)
            {
                DisableRealisticStars = new(RenderStars, StopRealisticStarsRendering);
                DisableRealisticStars?.Apply();
            }

            MethodInfo CalculatePerspectiveMatrix = typeof(StarsRenderer).GetMethod("CalculatePerspectiveMatrix", BindingFlags.NonPublic | BindingFlags.Static);

            if (CalculatePerspectiveMatrix != null)
            {
                MatchStarRotation = new(CalculatePerspectiveMatrix, ApplyCorrectStarRotation);
                MatchStarRotation?.Apply();
            }

                // if people want the realistic sun
            if (!ModContent.GetInstance<VFXConfig>().SunAndMoonRework)
                return;

            MethodInfo RenderSun = typeof(SunRenderer).GetMethod("Render", BindingFlags.Public | BindingFlags.Static);

            if (RenderSun != null)
            {
                DisableRealisticSun = new(RenderSun, StopRealisticSunRendering);
                DisableRealisticSun?.Apply();
            }

            MethodInfo VerticallyBiasSunAndMoon = typeof(SunPositionSaver).GetMethod("VerticallyBiasSunAndMoon", BindingFlags.Public | BindingFlags.Static);

            if (VerticallyBiasSunAndMoon != null)
            {
                IWouldActuallyLikeToMoveMyCelestialBodiesOnTheTitleScreenThankYouVeryMuch = new(VerticallyBiasSunAndMoon, HorizontalBias);
                IWouldActuallyLikeToMoveMyCelestialBodiesOnTheTitleScreenThankYouVeryMuch?.Apply();
            }
        }

        public override void Unload()
        {
            DisableRealisticStars?.Dispose();
            MatchStarRotation?.Dispose();
            DisableRealisticSun?.Dispose();
            IWouldActuallyLikeToMoveMyCelestialBodiesOnTheTitleScreenThankYouVeryMuch?.Dispose();
        }

        public static void DrawRealisticStarsAtTheCorrectLayer(float opacity, Matrix backgroundMatrix)
        {
            DrawingRealisticStarsCorrectly = true;
            StarsRenderer.Render(opacity, backgroundMatrix);
            DrawingRealisticStarsCorrectly = false;
        }

        public static bool DrawRealisticCloudsManually(Vector2 worldPosition, Vector2 screenSize, Vector2 sunMoonPosition)
        {
            if (!RealisticClouds) // Seperate check cus jit can suck my ass.
                return false;

                // This is prolly guaranteed to work.
            Effect clouds = GameShaders.Misc["RealisticSky:CloudShader"].Shader;

            float windDensityInterpolant = MathHelper.Clamp(Main.cloudAlpha + MathF.Abs(Main.windSpeedCurrent) * 0.84f, 0f, 1f);
            clouds.Parameters["globalTime"]?.SetValue(Main.GlobalTimeWrappedHourly);
            
            clouds.Parameters["invertedGravity"]?.SetValue(false); // no
            clouds.Parameters["screenSize"]?.SetValue(screenSize);
            
            clouds.Parameters["worldPosition"]?.SetValue(worldPosition);
            clouds.Parameters["sunPosition"]?.SetValue(new Vector3(sunMoonPosition, 5f)); // fucking why is this a vec3.
            
            clouds.Parameters["sunColor"]?.SetValue(Main.ColorOfTheSkies.ToVector4());

            Color cloudColor = Color.Lerp(Color.Wheat, Color.LightGray, 0.85f);
            clouds.Parameters["cloudColor"]?.SetValue(cloudColor.ToVector4());

            clouds.Parameters["densityFactor"]?.SetValue(MathHelper.Lerp(10f, 0.3f, MathF.Pow(windDensityInterpolant, 0.48f)));
            clouds.Parameters["cloudHorizontalOffset"]?.SetValue(CloudHorizontalOffsetValue);

            clouds.CurrentTechnique.Passes[0].Apply();

            Texture2D cloud = CloudDensityMap; // uuuuuuuuhhhhhhhhhhhhhhhhhhhhhhhhh
            Main.spriteBatch.Draw(cloud, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.White);

            return true;
        }

        private void StopRealisticStarsRendering(orig_StarsRender orig, float opacity, Matrix backgroundMatrix)
        {
                // So then I dont call orig*
            if (DrawingRealisticStarsCorrectly)
                orig(opacity, backgroundMatrix);
        }

            // ofc this is not perfect, but it feels better then not having it.
        private void ApplyCorrectStarRotation(ILContext il)
        {
            ILCursor c = new(il);

                // Goto right before the reverse rotation is applied.
            c.GotoNext(MoveType.After,
                i => i.MatchCall("RealisticSky.Content.RealisticSkyManager", "get_StarViewRotation"));

                // Exorcise that shit from the stack.
            c.EmitPop();

                // Make it woke.
            c.EmitDelegate(() => StarSystem.starRotation);
        }

        private void StopRealisticSunRendering(orig_SunRender orig, float sunriseAndSetInterpolant)
        {
                // So then I dont call orig.
        }

        private void HorizontalBias(orig_VerticallyBiasSunAndMoon orig) // THIS IS NOT REALISTIJC YOU STUPID UNINELLIJENT FRICK I HATE YOU (Reaper)
        {
                // So then I dont call orig.
        }
    }
}
