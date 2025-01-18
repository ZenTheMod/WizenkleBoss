using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.Utilities;
using WizenkleBoss.Common.Helpers;

namespace WizenkleBoss.Content.UI
{
    public partial class TelescopeUISystem : ModSystem
    {
        public static Vector2 TargetSize = new(540);
        public class TelescopeTargetContent : ARenderTargetContentByRequest
        {
            protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
            {
                PrepareARenderTarget_AndListenToEvents(ref _target, device, (int)TargetSize.X, (int)TargetSize.Y, (RenderTargetUsage)1);

                device.SetRenderTarget(_target);
                device.Clear(Color.Transparent);

                Vector2 Center = new(_target.Width / 2f, _target.Height / 2f);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                    // Example mod's ui uses like 17x this for their gradient.
                int steps = 120;

                Color GradientA;
                Color GradientB;

                float starAlpha = 0f;

                    // This is how vanilla's time hair dye works************, and i cant be fucked to do litterally anything else :3
                if (Main.dayTime)
                {
                    if (Main.time < 6700f)
                    {
                        float interpolator = (float)Main.time / 6700f;
                        starAlpha = 1 - interpolator;
                        GradientA = Color.Lerp(new Color(57, 84, 204), new Color(126, 165, 248), interpolator);
                        GradientB = Color.Lerp(new Color(36, 56, 145), new Color(80, 86, 245), interpolator);
                    }
                    else if (Main.time < 40000f)
                    {
                        GradientA = new Color(126, 165, 248);
                        GradientB = new Color(80, 86, 245);
                        starAlpha = 0;
                    }
                    else
                    {
                        float interpolator = ((float)Main.time - 40000f) / (54000f - 40000f);
                        GradientA = Color.Lerp(new Color(126, 165, 248), new Color(46, 35, 101), interpolator);
                        GradientB = Color.Lerp(new Color(80, 86, 245), new Color(63, 59, 104), interpolator);
                        starAlpha = 0;

                        if (Main.time > 48000f)
                        {
                            starAlpha = ((float)Main.time - 48000f) / (54000f - 48000f);
                        }
                    }
                }
                else if (Main.time < 1100f)
                {
                    float interpolator = (float)Main.time / 1100f;
                    starAlpha = 1;
                    GradientA = Color.Lerp(new Color(46, 35, 101), new Color(2, 3, 18), interpolator);
                    GradientB = Color.Lerp(new Color(63, 59, 104), new Color(15, 10, 61), interpolator);
                }
                else if (Main.time < 28000f)
                {
                    starAlpha = 1;
                    GradientA = new Color(2, 3, 18);
                    GradientB = new Color(15, 10, 61);
                }
                else
                {
                    float interpolator = ((float)Main.time - 28000f) / (32400f - 28000f);
                    starAlpha = 1;
                    GradientA = Color.Lerp(new Color(2, 3, 18), new Color(57, 84, 204), interpolator);
                    GradientB = Color.Lerp(new Color(15, 10, 61), new Color(36, 56, 145), interpolator);
                }

                // Draw gradient.
                for (int i = 0; i < steps; i++)
                {
                    spriteBatch.Draw(TextureRegistry.Pixel.Value, new Rectangle(0, i * (_target.Height / steps) + 10, _target.Width, _target.Height / steps), Color.Lerp(GradientA, GradientB, (float)i / (float)steps));
                }

                // Clouds and Stars.

                if (starAlpha > 0)
                {
                    foreach (var star in BarrierStarSystem.Stars.Where(s => s.State <= SupernovaState.Shrinking))
                    {
                        float starRotation = star.BaseRotation + Main.GlobalTimeWrappedHourly * (star.BaseRotation - MathHelper.Pi) * 0.2f;

                        float starSize = star.BaseSize * (MathF.Sin(Main.GlobalTimeWrappedHourly / 3f + star.BaseRotation) + 1.2f);

                        starSize *= star.SupernovaSize;
                        starSize = MathF.Max(starSize * 1.1f, 0.2f);

                        Color starColor = Color.Lerp(star.Color * starAlpha, Color.Orange, 1f - star.SupernovaSize) with { A = 0 };

                        Vector2 starPosition = Center + telescopeUIOffset + star.Position;

                        Texture2D starTexture = TextureRegistry.Stars[star.Texture].Value;
                        Vector2 starOrigin = starTexture.Size() / 2;

                        spriteBatch.Draw(starTexture, starPosition, null, starColor, starRotation, starOrigin, starSize * 0.4f, SpriteEffects.None, 0f);
                        spriteBatch.Draw(TextureRegistry.Bloom.Value, starPosition, null, starColor * 0.15f, 0, TextureRegistry.Bloom.Value.Size() / 2, 0.07f * starSize, SpriteEffects.None, 0f);
                    }
                }

                if (BarrierStarSystem.BigStar.State <= SupernovaState.Shrinking)
                {
                    float BigStarAlpha = MathHelper.Clamp(starAlpha, 0.1f, 1f);
                    Color[] colors = [(Color.Orange * BigStarAlpha * 0.4f), (Color.Orange * BigStarAlpha), (Color.Yellow * BigStarAlpha), (Color.White * BigStarAlpha)];

                    float interpolator = BarrierStarSystem.BigStar.SupernovaSize;
                    Vector2 position = Center + telescopeUIOffset + BarrierStarSystem.BigStar.Position;
                    Vector2 origin = TextureRegistry.Star.Size() / 2;

                    spriteBatch.Draw(TextureRegistry.Bloom.Value, position, null, Color.Lerp(colors[0], Color.Red, 1f - interpolator) with { A = 0 }, 0, TextureRegistry.Bloom.Value.Size() / 2, (0.2f + (MathF.Sin(Main.GlobalTimeWrappedHourly) / 30f)) * interpolator, SpriteEffects.None, 0f);
                    spriteBatch.Draw(TextureRegistry.Star.Value, position, null, Color.Lerp(colors[1], Color.Red, 1f - interpolator) with { A = 0 }, Main.GlobalTimeWrappedHourly / -25f, origin, (0.25f + (MathF.Sin(Main.GlobalTimeWrappedHourly) / 19f)) * interpolator, SpriteEffects.None, 0f);
                    spriteBatch.Draw(TextureRegistry.Star.Value, position, null, Color.Lerp(colors[2], Color.Orange, 1f - interpolator) with { A = 0 }, Main.GlobalTimeWrappedHourly / 15f, origin, (0.13f + (MathF.Sin(Main.GlobalTimeWrappedHourly) / 15f)) * interpolator, SpriteEffects.None, 0f);
                    spriteBatch.Draw(TextureRegistry.Star.Value, position, null, Color.Lerp(colors[3], Color.Yellow, 1f - interpolator) with { A = 0 }, Main.GlobalTimeWrappedHourly / -10f, origin, (0.07f + (MathF.Sin(Main.GlobalTimeWrappedHourly) / 14f)) * interpolator, SpriteEffects.None, 0f);
                }

                DrawSuperNovae(spriteBatch, _target.Size() / 2);

                UnifiedRandom rand = new(Main.worldID);

                int cloudCount = rand.Next(800 / 7, 800 / 5);
                for (int i = 0; i < cloudCount; i++)
                {
                    Color cloudColor = Color.Lerp(Color.White, new Color(0, 0, 0, 0.3f), MathHelper.Clamp(starAlpha, 0.3f, 1f));

                    float cloudX = ((Math.Abs(windGlobal) * rand.NextFloat(13, 120)) + rand.NextFloat(_target.Width * 5)) % (_target.Width * 5);
                    if (windGlobal < 0)
                        cloudX = (_target.Width * 5) - cloudX;
                    cloudX -= _target.Width;

                    Vector2 cloudPosition = new Vector2(cloudX, (_target.Height / 2f) - 700 + rand.NextFloat(1400)) + telescopeUIOffset;

                    Texture2D cloudTexture = TextureAssets.Cloud[rand.NextBool(20) ? rand.Next(22, 36) : rand.Next(0, 21)].Value;
                    Vector2 cloudOrigin = cloudTexture.Size() / 2;

                    spriteBatch.Draw(cloudTexture, cloudPosition, null, cloudColor, 0, cloudOrigin, rand.NextFloat(0.4f, 0.6f), SpriteEffects.None, 0f);
                }
                spriteBatch.End();
                device.SetRenderTarget(null);
                _wasPrepared = true;
            }
        }
        private static void DrawSuperNovae(SpriteBatch spriteBatch, Vector2 size)
        {
            if (!BarrierStarSystem.Stars.Where(s => s.State == SupernovaState.Expanding).Any() && BarrierStarSystem.BigStar.State != SupernovaState.Expanding)
                return;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            foreach (var supernovae in BarrierStarSystem.Stars.Where(s => s.State == SupernovaState.Expanding))
            {
                Vector2 position = size + telescopeUIOffset + supernovae.Position;

                spriteBatch.Draw(TextureRegistry.Shockwave.Value, new Rectangle((int)position.X, (int)position.Y, (int)(60 * supernovae.SupernovaSize), (int)(100 * supernovae.SupernovaSize)), null, Color.White * (1f - supernovae.SupernovaSize), supernovae.BaseRotation, TextureRegistry.Shockwave.Size() / 2, SpriteEffects.None, 0f);

                spriteBatch.Draw(TextureRegistry.Bloom.Value, position, null, Color.MediumBlue * (0.87f - supernovae.SupernovaSize), 0, TextureRegistry.Bloom.Value.Size() / 2, supernovae.SupernovaSize * 0.45f, SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureRegistry.Bloom.Value, position, null, Color.Cyan * (0.9f - supernovae.SupernovaSize), 0, TextureRegistry.Bloom.Value.Size() / 2, supernovae.SupernovaSize * 0.27f, SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureRegistry.Bloom.Value, position, null, Color.White * (1f - supernovae.SupernovaSize), 0, TextureRegistry.Bloom.Value.Size() / 2, supernovae.SupernovaSize * 0.14f, SpriteEffects.None, 0f);
            }

            if (BarrierStarSystem.BigStar.State == SupernovaState.Expanding)
            {
                BarrierStar bigstar = BarrierStarSystem.BigStar;
                Vector2 position = size + telescopeUIOffset + bigstar.Position;

                spriteBatch.Draw(TextureRegistry.Shockwave.Value, new Rectangle((int)position.X, (int)position.Y, (int)(110 * bigstar.SupernovaSize), (int)(175 * bigstar.SupernovaSize)), null, Color.White * (1f - bigstar.SupernovaSize), bigstar.BaseRotation, TextureRegistry.Shockwave.Size() / 2, SpriteEffects.None, 0f);

                spriteBatch.Draw(TextureRegistry.Bloom.Value, position, null, Color.MediumBlue * (0.87f - bigstar.SupernovaSize), 0, TextureRegistry.Bloom.Value.Size() / 2, bigstar.SupernovaSize * 0.62f, SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureRegistry.Bloom.Value, position, null, Color.Cyan * (0.9f - bigstar.SupernovaSize), 0, TextureRegistry.Bloom.Value.Size() / 2, bigstar.SupernovaSize * 0.46f, SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureRegistry.Bloom.Value, position, null, Color.White * (1f - bigstar.SupernovaSize), 0, TextureRegistry.Bloom.Value.Size() / 2, bigstar.SupernovaSize * 0.25f, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            Vector2[] positions = BarrierStarSystem.OldMeteoritePosition.Where(p => p != Vector2.Zero).ToArray();
            if (positions.Length >= 2)
            {
                for (int i = positions.Length - 1; i > 0; i--)
                {
                    Color color = Color.OrangeRed * (((float)positions.Length - i) / (float)positions.Length);
                    spriteBatch.Draw(TextureRegistry.Stars[1].Value, positions[i] + telescopeUIOffset, null, color with { A = 0 }, Main.GlobalTimeWrappedHourly / 2f, TextureRegistry.Stars[1].Value.Size() / 2, (0.1f + (BarrierStarSystem.BigStar.SupernovaSize * 4)) * (((float)positions.Length - i) / (float)positions.Length) + 0.02f, SpriteEffects.None, 0);
                }
            }
        }
        public static TelescopeTargetContent telescopeTargetByRequest;
        public override void Load()
        {
            telescopeTargetByRequest = new();
            Main.ContentThatNeedsRenderTargets.Add(telescopeTargetByRequest);
        }
        public override void Unload()
        {
            Main.ContentThatNeedsRenderTargets.Remove(telescopeTargetByRequest);
        }
    }
}
