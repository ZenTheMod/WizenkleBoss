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
using RealisticSky.Content.Atmosphere;
using Terraria.GameContent;
using RealisticSky.Common.Utilities;
using RealisticSky.Content;
using WizenkleBoss.Common.Registries;

namespace WizenkleBoss.Common.StarRewrite
{
        // Disable JIT when loading this class without realistic sky.
    [ExtendsFromMod("RealisticSky")]
    public class RealisticSkyCompatSystem : ModSystem
    {
        public static Hook DisableRealisticStars;
        public delegate void orig_StarsRender(float opacity, Matrix backgroundMatrix);

        public static ILHook MatchStarRotation;

        public static Hook MatchGalaxyRotation;
        public delegate void orig_GalaxyRender();

        public static Hook DisableRealisticSun;
        public delegate void orig_SunRender(float sunriseAndSetInterpolant);

        public static Hook IWouldActuallyLikeToMoveMyCelestialBodiesOnTheTitleScreenThankYouVeryMuch;
        public delegate void orig_VerticallyBiasSunAndMoon();

        public static bool RealisticSkyEnabled = false;

        public static MethodInfo CalculatePerspectiveMatrix;
        public static FieldInfo AtmosphereTarget;

        public static FieldInfo StarVertexBuffer;
        public static FieldInfo StarIndexBuffer;

        public static MethodInfo UpdateOpacity;

        private static bool RealisticClouds => RealisticSkyConfig.Instance.RealisticClouds;
        private static Texture2D CloudDensityMap => TexturesRegistry.CloudDensityMap.Value;
        private static float CloudHorizontalOffsetValue => CloudsRenderer.CloudHorizontalOffset;

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

            StarVertexBuffer = typeof(StarsRenderer).GetField("StarVertexBuffer", BindingFlags.NonPublic | BindingFlags.Static);
            StarIndexBuffer = typeof(StarsRenderer).GetField("StarIndexBuffer", BindingFlags.NonPublic | BindingFlags.Static);

            CalculatePerspectiveMatrix = typeof(StarsRenderer).GetMethod("CalculatePerspectiveMatrix", BindingFlags.NonPublic | BindingFlags.Static);

            if (CalculatePerspectiveMatrix != null)
            {
                MatchStarRotation = new(CalculatePerspectiveMatrix, ApplyCorrectRotation);
                MatchStarRotation?.Apply();
            }

            AtmosphereTarget = typeof(AtmosphereRenderer).GetField("AtmosphereTarget", BindingFlags.NonPublic | BindingFlags.Static);

            MethodInfo RenderGalaxy = typeof(GalaxyRenderer).GetMethod("Render", BindingFlags.Public | BindingFlags.Static);

            if (RenderGalaxy != null)
            {
                MatchGalaxyRotation = new(RenderGalaxy, StopGalaxyRendering);
                MatchGalaxyRotation?.Apply();
            }

            UpdateOpacity = typeof(GalaxyRenderer).GetMethod("UpdateOpacity", BindingFlags.NonPublic | BindingFlags.Static);

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

        public static void DrawRealisticStarsAtTheCorrectLayer(GraphicsDevice device, float opacity, Vector2 screenSize, Vector2 sunPosition, Matrix backgroundMatrix, float globalTime, float falloffsize)
        {
            if (RealisticSkyConfig.Instance.NightSkyStarCount <= 0)
                return;

            Effect stars = GameShaders.Misc["RealisticSky:StarShader"].Shader;

            float num = MathUtils.Saturate(MathF.Pow(1f - Main.atmo, 3f) + MathF.Pow(1f - RealisticSkyManager.SkyBrightness, 5f)) * opacity;

            stars.Parameters["opacity"]?.SetValue(num);
            stars.Parameters["projection"]?.SetValue((Matrix)CalculatePerspectiveMatrix.Invoke(null, null) * backgroundMatrix);
            stars.Parameters["globalTime"]?.SetValue(globalTime);
            stars.Parameters["sunPosition"]?.SetValue(Main.dayTime ? sunPosition : (Vector2.One * 50000f));
            stars.Parameters["minTwinkleBrightness"]?.SetValue(0.2f);
            stars.Parameters["maxTwinkleBrightness"]?.SetValue(3.37f);
                
                // Why on EARTH does this even EXIST.
            stars.Parameters["distanceFadeoff"]?.SetValue(falloffsize);
            stars.Parameters["screenSize"]?.SetValue(screenSize);

            stars.CurrentTechnique.Passes[0].Apply();

                // Why must we be internal?
            AtmosphereTargetContent atmosphere = (AtmosphereTargetContent)AtmosphereTarget?.GetValue(null);
            atmosphere?.Request();

            device.Textures[1] = TexturesRegistry.BloomCircle.Value;
            device.SamplerStates[1] = SamplerState.LinearWrap;

            device.Textures[2] = atmosphere.IsReady ? atmosphere.GetTarget() : TextureAssets.MagicPixel.Value;
            device.SamplerStates[2] = SamplerState.LinearClamp;

            device.RasterizerState = RasterizerState.CullNone;
            
                // Why must we continue to be internal?
            VertexBuffer vertexBuffer = (VertexBuffer)StarVertexBuffer.GetValue(null);
            IndexBuffer indexBuffer = (IndexBuffer)StarIndexBuffer.GetValue(null);

            device.Indices = indexBuffer;
            device.SetVertexBuffer(vertexBuffer);
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexBuffer.VertexCount, 0, indexBuffer.IndexCount / 3);
            device.SetVertexBuffer(null);
            device.Indices = null;
        }

        public static void ApplyAtmosphereShader(GraphicsDevice device, float opacity)
        {
            Effect atmo = Shaders.StarAtmosphereShader.Value;

            if (atmo == null)
                return;

            atmo.Parameters["alpha"]?.SetValue(opacity);

            atmo.CurrentTechnique.Passes[0].Apply();

            AtmosphereTargetContent atmosphere = (AtmosphereTargetContent)AtmosphereTarget.GetValue(null);
            atmosphere.Request();

            device.Textures[1] = atmosphere.IsReady ? atmosphere.GetTarget() : TextureAssets.MagicPixel.Value;
            device.SamplerStates[1] = SamplerState.LinearClamp;
        }

        public static void DrawGalaxy(Vector2 position, float screenWidth)
        {
            UpdateOpacity.Invoke(null, null);

            Texture2D galaxy = TexturesRegistry.Galaxy.Value;
            Texture2D bloom = TexturesRegistry.BloomCircleBig.Value;

            float num = screenWidth / galaxy.Width * 1.3f;
            Color color = new Color(1.2f, 0.9f, 1f) * GalaxyRenderer.MovingGalaxyOpacity;

                // I ain't touchin this mess.
            Main.spriteBatch.Draw(bloom, position, null, (color * MathF.Sqrt(GalaxyRenderer.MovingGalaxyOpacity) * 0.55f) with { A = 0 }, 0f, bloom.Size() * 0.5f, num * 0.29f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(bloom, position, null, (color * GalaxyRenderer.MovingGalaxyOpacity * 0.3f) with { A = 0 }, 0f, bloom.Size() * 0.5f, num * 3f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(galaxy, position, null, (color * 0.34f) with { A = 0 }, StarSystem.starRotation, galaxy.Size() * 0.5f, num, SpriteEffects.None, 0f);
        }

        public static bool DrawRealisticCloudsManually(Vector2 worldPosition, Rectangle dimentions, Vector2 sunMoonPosition)
        {
            if (!RealisticClouds) // Seperate check cus jit can suck my ass.
                return false;

                // This is prolly guaranteed to work.
            Effect clouds = GameShaders.Misc["RealisticSky:CloudShader"].Shader;

            float windDensityInterpolant = MathHelper.Clamp(Main.cloudAlpha + MathF.Abs(Main.windSpeedCurrent) * 0.84f, 0f, 1f);
            clouds.Parameters["globalTime"]?.SetValue(Main.GlobalTimeWrappedHourly);
            
            clouds.Parameters["invertedGravity"]?.SetValue(false); // no
            clouds.Parameters["screenSize"]?.SetValue(dimentions.Size());
            
            clouds.Parameters["worldPosition"]?.SetValue(worldPosition);
            clouds.Parameters["sunPosition"]?.SetValue(new Vector3(sunMoonPosition, 5f)); // fucking why is this a vec3.
            
            clouds.Parameters["sunColor"]?.SetValue(Main.ColorOfTheSkies.ToVector4());

            Color cloudColor = Color.Lerp(Color.Wheat, Color.LightGray, 0.85f);
            clouds.Parameters["cloudColor"]?.SetValue(cloudColor.ToVector4());

            clouds.Parameters["densityFactor"]?.SetValue(MathHelper.Lerp(10f, 0.3f, MathF.Pow(windDensityInterpolant, 0.48f)));
            clouds.Parameters["cloudHorizontalOffset"]?.SetValue(CloudHorizontalOffsetValue);

            clouds.CurrentTechnique.Passes[0].Apply();

            Texture2D cloud = CloudDensityMap; // uuuuuuuuhhhhhhhhhhhhhhhhhhhhhhhhh
            Main.spriteBatch.Draw(cloud, dimentions, Color.White);

            return true;
        }

        private void ApplyCorrectRotation(ILContext il)
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

        private void StopRealisticStarsRendering(orig_StarsRender orig, float opacity, Matrix backgroundMatrix)
        {
                // So then I dont call orig*
        }
        private void StopGalaxyRendering(orig_GalaxyRender orig)
        {
                // So then I dont call orig.
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
