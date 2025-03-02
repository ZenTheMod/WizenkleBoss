using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using WizenkleBoss.Common.Config;
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Common.Registries;
using static WizenkleBoss.Common.StarRewrite.StarSystem;
using static WizenkleBoss.Common.StarRewrite.RealisticSkyCompatHelper;

namespace WizenkleBoss.Common.StarRewrite
{
    public class StarRendererSystem : ModSystem
    {
        public class StarTargetContent : ARenderTargetContentByRequest
        {
            protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
            {
                Vector2 size = Helper.ScreenSize;
                bool pixelated = ModContent.GetInstance<VFXConfig>().PixelatedStars;

                if (pixelated)
                    size = Helper.HalfScreenSize;

                PrepareARenderTarget_AndListenToEvents(ref _target, device, (int)size.X, (int)size.Y, (RenderTargetUsage)1);

                device.SetRenderTarget(_target);
                device.Clear(Color.Transparent);

                Vector2 center = Helper.HalfScreenSize;

                spriteBatch.BeginToggledHalfScaleWrap(SpriteSortMode.Deferred, BlendState.AlphaBlend, pixelated);

                float alpha = starAlpha;

                if (alpha != 0)
                    DrawStars(spriteBatch, center, alpha);

                DrawRealisticGalaxy(Helper.ScreenSize * 0.6f, Helper.ScreenSize.X);

                if (RealisticSkyCompatSystem.RealisticSkyEnabled)
                {
                    spriteBatch.End();
                    spriteBatch.BeginToggledHalfScale(SpriteSortMode.Immediate, BlendState.AlphaBlend, pixelated);

                    Vector2 sunPos = SunAndMoonSystem.SunMoonPosition;
                    float falloff = Main.eclipse ? 0.05f : 0.3f;
                    if (pixelated)
                    {
                        sunPos /= 2f;
                        falloff /= 2f;
                    }

                    DrawRealisticStars(device, alpha * 0.5f, size, sunPos, Matrix.Identity, Main.GlobalTimeWrappedHourly, falloff);
                }
                    // InteractableStar inkStar = StarSystem.stars[0];

                spriteBatch.End();
                device.SetRenderTarget(null);
                _wasPrepared = true;
            }

            private static void DrawStars(SpriteBatch spriteBatch, Vector2 center, float alpha)
            {
                Texture2D flare = Textures.Star.Value;
                Vector2 flareOrigin = flare.Size() / 2f;
                for (int i = 1; i < starCount - 1; i++)
                {
                    if (supernovae[i] >= (byte)SupernovaProgress.Exploding)
                        continue;

                    InteractableStar star = stars[i];

                    Vector2 position = center + star.position.RotatedBy(starRotation);
                    float rotation = star.rotation;
                    float twinkle = (MathF.Sin(star.twinkle + Main.GlobalTimeWrappedHourly / 12) * 0.2f) + 1f;
                    float scale = star.baseSize * (1 - star.compression) * twinkle;

                    Color color = star.GetColor() * star.baseSize * alpha;
                    Texture2D texture = TextureAssets.Star[star.starType].Value;
                    Vector2 origin = texture.Size() / 2f;

                    spriteBatch.Draw(texture, position, null, color, rotation, origin, scale / 1.3f, SpriteEffects.None, 0f);

                    spriteBatch.Draw(flare, position, null, (color * 0.13f) with { A = 0 }, 0, flareOrigin, scale / 3.2f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(flare, position, null, (color * 0.6f) with { A = 0 }, 0, flareOrigin, scale / 5f, SpriteEffects.None, 0f);
                }
            }
        }

        public static StarTargetContent starTargetByRequest;

        public override void Load()
        {
            On_Main.DrawStarsInBackground += DrawTargetToSky;
            starTargetByRequest = new();
            Main.ContentThatNeedsRenderTargets.Add(starTargetByRequest);
        }

        public override void Unload()
        {
            On_Main.DrawStarsInBackground -= DrawTargetToSky;
            Main.ContentThatNeedsRenderTargets.Remove(starTargetByRequest);
        }

        private void DrawTargetToSky(On_Main.orig_DrawStarsInBackground orig, Main self, Main.SceneArea sceneArea, bool artificial)
        {
            if (!canDrawStars || Main.starGame)
            {
                orig(self, sceneArea, artificial); 
                return;
            }

            var snapshit = Main.spriteBatch.CaptureSnapshot();

            if (RealisticSkyCompatSystem.RealisticSkyEnabled)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.BackgroundViewMatrix.EffectMatrix);

                ApplyStarAtmosphereShader(Main.instance.GraphicsDevice, 1f);
            }

            Main.spriteBatch.RequestAndDrawRenderTarget(starTargetByRequest);

            if (RealisticSkyCompatSystem.RealisticSkyEnabled)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(in snapshit);
            }
        }
    }
}
