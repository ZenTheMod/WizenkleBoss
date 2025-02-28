using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using WizenkleBoss.Common.Config;
using WizenkleBoss.Common.Helpers;
using Microsoft.Xna.Framework;
using WizenkleBoss.Common.Registries;
using static WizenkleBoss.Common.StarRewrite.SkyTextures;
using static WizenkleBoss.Common.StarRewrite.SunAndMoonSystem;

namespace WizenkleBoss.Common.StarRewrite
{
    public class SunAndMoonRendererSystem : ModSystem
    {
        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<VFXConfig>().SunAndMoonRework;

        public class SunAndMoonTargetContent : ARenderTargetContentByRequest
        {
            protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
            {
                Vector2 size = Helper.ScreenSize;

                bool pixelated = ModContent.GetInstance<VFXConfig>().PixelatedSunAndMoon;

                if (pixelated)
                    size = Helper.HalfScreenSize;

                PrepareARenderTarget_AndListenToEvents(ref _target, device, (int)size.X, (int)size.Y, (RenderTargetUsage)1);

                device.SetRenderTarget(_target);
                device.Clear(Color.Transparent);

                    // More pixelated garbage.
                if (pixelated)
                    spriteBatch.BeginHalfScale(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                else
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                    // Params.
                Vector2 position = SunMoonPosition;
                Color color = SunMoonColor;
                float rotation = SunMoonRotation;
                float scale = SunMoonScale;
                float centerX = Main.screenWidth / 2f;

                Color sky = Main.ColorOfTheSkies.MultiplyRGB(new Color(128, 168, 248));
                Color moonShadowColor = ModContent.GetInstance<VFXConfig>().TransparentMoonShadow ? Color.Transparent : sky;
                Color moonColor = sky.MultiplyRGB(color) * 16f * scale;

                    // Actually draw shit.
                if (Main.dayTime)
                    DrawSun(spriteBatch, position, color, rotation, scale, centerX, device);
                else
                    DrawMoon(spriteBatch, position, color, rotation, scale, moonColor, moonShadowColor, device);

                spriteBatch.End();
                device.SetRenderTarget(null);
                _wasPrepared = true;
            }
        }

        public static void DrawSun(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale, float centerX, GraphicsDevice device)
        {
            if (Main.eclipse)
            {
                DrawEclipse(spriteBatch, position, color, rotation, scale, device);
                return;
            }

            Texture2D bloom = Bloom.Value;
            Vector2 origin = bloom.Size() / 2f;

            // calculate how big the flare should be
            float distanceFromCenter = MathF.Abs(centerX - position.X) / centerX;
            float distanceFromTop = (position.Y + 50) / Main.screenHeight;

            float flareWidth = distanceFromCenter * distanceFromTop * Utils.Remap(distanceFromCenter, 1f, 1.11f, 1f, 0f);

            // outer vauge glow
            spriteBatch.Draw(bloom, position, null, (color * 0.2f) with { A = 0 }, 0, origin, 0.35f * scale, SpriteEffects.None, 0f);

            // inner glow
            Color innerColor = (color * (1f + (distanceFromCenter * 5f))) with { A = 0 };
            spriteBatch.Draw(bloom, position, null, innerColor, 0, origin, 0.23f * scale, SpriteEffects.None, 0f);

            // for the "line" effect like the one on the 1.4.5 sun.
            spriteBatch.Draw(bloom, position, null, (color * 0.6f) with { A = 0 }, 0, origin, new Vector2(6 * flareWidth, 0.02f) * scale, SpriteEffects.None, 0f); // thinest and longest
            spriteBatch.Draw(bloom, position, null, (color * 0.3f) with { A = 0 }, 0, origin, new Vector2(3.3f * flareWidth, 0.09f) * scale, SpriteEffects.None, 0f); // shorter glow
            spriteBatch.Draw(bloom, position, null, color with { A = 0 }, 0, origin, new Vector2(2f * flareWidth, 0.06f) * scale, SpriteEffects.None, 0f); // make it blend in more

            // sungalses.
            if (!Main.gameMenu && Main.player[Main.myPlayer].head == 12)
                spriteBatch.Draw(Sunglasses.Value, position, null, Color.White, 0, Sunglasses.Value.Size() / 2f, 0.3f * scale, SpriteEffects.None, 0f);
        }

        private static void DrawEclipse(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale, GraphicsDevice device)
        {
            Texture2D texture = Bloom.Value;
            spriteBatch.Draw(texture, position, null, color with { A = 0 }, 0, texture.Size() / 2f, scale * 0.4f, SpriteEffects.None, 0f);
            // DrawMoon(spriteBatch, position, Color.Black, rotation, scale, device);
        }

        public static void DrawMoon(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale, Color moonColor, Color shadowColor, GraphicsDevice device)
        {
            if (WorldGen.drunkWorldGen)
            {
                DrawSmiley(spriteBatch, position, color, rotation, scale, moonColor, shadowColor, device);
                return;
            }

            Texture2D moon = Moon[Main.moonType].Value; // Moon[Main.moonType].Value

            if (Main.pumpkinMoon)
                moon = PumpkinMoon.Value;
            else if (Main.snowMoon)
                moon = SnowMoon.Value;

            Effect planet = Shaders.PlanetShader.Value; // ingore the fact it says planet.

            PlanetSetup(planet, 0.25f, 0.25f + (Main.moonPhase * 0.125f), shadowColor, device);

            int size = (int)(55 * scale);
            Rectangle rect = new((int)position.X, (int)position.Y, size, size);

            spriteBatch.Draw(moon, rect, null, moonColor, rotation, moon.Size() / 2f, SpriteEffects.None, 0f);

            // Gives it a subtle bloom effect to make it softer on the eyes.
            DrawBloom(spriteBatch, position, rotation, scale * 1.2f, moonColor, planet, 0.8f);
        }

        private static void DrawSmiley(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale, Color moonColor, Color shadowColor, GraphicsDevice device)
        {
            Texture2D moon = Moon[0].Value;
            Texture2D star = Textures.Star.Value;

            Vector2 starOneOffset = new Vector2(-24, -32).RotatedBy(rotation);
            Vector2 starTwoOffset = new Vector2(13, -44).RotatedBy(rotation);

            spriteBatch.Draw(star, position + starOneOffset, null, (moonColor * 0.4f) with { A = 0 }, 0, star.Size() / 2f, scale / 3f, SpriteEffects.None, 0f);
            spriteBatch.Draw(star, position + starTwoOffset, null, (moonColor * 0.4f) with { A = 0 }, 0, star.Size() / 2f, scale / 3f, SpriteEffects.None, 0f);

            spriteBatch.Draw(star, position + starOneOffset, null, color with { A = 0 }, MathHelper.PiOver4, star.Size() / 2f, scale / 5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(star, position + starTwoOffset, null, color with { A = 0 }, MathHelper.PiOver4, star.Size() / 2f, scale / 5f, SpriteEffects.None, 0f);

            Effect planet = Shaders.PlanetShader.Value; // ingore the fact it says planet.

            PlanetSetup(planet, 0.25f, 0.645f, shadowColor, device);

            int size = (int)(50 * scale);
            Rectangle rect = new((int)position.X, (int)position.Y, size, size);

            spriteBatch.Draw(moon, rect, null, moonColor, rotation - MathHelper.PiOver2, moon.Size() / 2f, SpriteEffects.None, 0f);

            // Gives it a subtle bloom effect to make it softer on the eyes.
            DrawBloom(spriteBatch, position, rotation - MathHelper.PiOver2, scale * 1.45f, moonColor, planet, 0.8f);
        }

        private static void PlanetSetup(Effect planet, float baseAngle, float shadowAngle, Color shadowColor, GraphicsDevice device)
        {
            planet.Parameters["shadowColor"]?.SetValue(shadowColor.ToVector4()); // to replicate the fact that shadows cannot be darker then the atmosphere infront of them.

            planet.Parameters["textureAngle"]?.SetValue(baseAngle); // meh
            planet.Parameters["shadowAngle"]?.SetValue(shadowAngle); // smiley

            planet.Parameters["falloffStart"]?.SetValue(0.95f);

            planet.CurrentTechnique.Passes[0].Apply();

            device.SamplerStates[0] = SamplerState.LinearWrap; // make sure everything wraps cleanly.

            device.Textures[1] = MoonShadow.Value; // use our shadow texture.
            device.SamplerStates[1] = SamplerState.LinearWrap; // make sure everything wraps cleanly.
        }

        private static void DrawBloom(SpriteBatch spriteBatch, Vector2 position, float rotation, float scale, Color moonColor, Effect planet, float falloff)
        {
            planet.Parameters["shadowColor"]?.SetValue(Color.Transparent.ToVector4());
            planet.Parameters["falloffStart"]?.SetValue(falloff);

            planet.CurrentTechnique.Passes[0].Apply();

            int size = (int)(50 * scale);
            Rectangle rect = new((int)position.X, (int)position.Y, size, size);

            Texture2D texture = Textures.Pixel.Value;

            spriteBatch.Draw(texture, rect, null, (moonColor * 0.3f) with { A = 0 }, rotation, texture.Size() / 2f, SpriteEffects.None, 0f);
        }

        public static SunAndMoonTargetContent sunAndMoonTargetByRequest;

        public override void Load()
        {
            On_Main.DrawSunAndMoon += DrawSunTarget;
            sunAndMoonTargetByRequest = new();
            Main.ContentThatNeedsRenderTargets.Add(sunAndMoonTargetByRequest);
        }

        public override void Unload()
        {
            On_Main.DrawSunAndMoon -= DrawSunTarget;
            Main.ContentThatNeedsRenderTargets.Remove(sunAndMoonTargetByRequest);
        }

        private void DrawSunTarget(On_Main.orig_DrawSunAndMoon orig, Main self, Main.SceneArea sceneArea, Color moonColor, Color sunColor, float tempMushroomInfluence)
        {
            orig(self, sceneArea, moonColor, sunColor, tempMushroomInfluence);

            if (!StarSystem.canDrawStars)
                return;

            Main.spriteBatch.RequestAndDrawRenderTarget(sunAndMoonTargetByRequest);
        }
    }
}
