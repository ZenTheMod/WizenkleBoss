using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.Utilities;
using WizenkleBoss.Assets.Helper;
using WizenkleBoss.Assets.Textures;

namespace WizenkleBoss.Content.UI
{
    public partial class BarrierTelescopeUISystem : ModSystem
    {
        public class TelescopeTargetContent : ARenderTargetContentByRequest
        {
            protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
            {
                PrepareARenderTarget_AndListenToEvents(ref _target, device, 540, 540, (RenderTargetUsage)1);

                device.SetRenderTarget(_target);
                device.Clear(Color.Transparent);

                Vector2 Center = new(_target.Width / 2f, _target.Height / 2f);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                // Example mod's ui uses like 17x this for their gradient.
                int steps = 120;

                Color GradientA;
                Color GradientB;

                float starAlpha;

                // This is how vanilla's time hair dye works************, and i cant be fucked to do litterally anything else :3
                if (Main.worldSurface * 0.35f < telescopeTilePosition.Y / 16)
                {
                    if (Main.dayTime)
                    {
                        if (Main.time < 32000f)
                        {
                            float interpolator = (float)Main.time / 32000f;
                            GradientA = Color.Lerp(new Color(57, 84, 204), new Color(126, 165, 248), interpolator);
                            GradientB = Color.Lerp(new Color(36, 56, 145), new Color(80, 86, 245), interpolator);
                        }
                        else
                        {
                            float interpolator = ((float)Main.time - 32000f) / (54000f - 32000f);
                            GradientA = Color.Lerp(new Color(126, 165, 248), new Color(214, 69, 124), interpolator);
                            GradientB = Color.Lerp(new Color(80, 86, 245), new Color(255, 237, 102), interpolator);
                        }
                        starAlpha = 0;
                    }
                    else if (Main.time < 16200f)
                    {
                        float interpolator = (float)Main.time / 16200f;
                        starAlpha = interpolator;
                        GradientA = Color.Lerp(new Color(214, 69, 124), new Color(2, 3, 18), interpolator);
                        GradientB = Color.Lerp(new Color(255, 237, 102), new Color(15, 10, 61), interpolator);
                    }
                    else if (Main.time < 24000f)
                    {
                        starAlpha = 1;
                        GradientA = new Color(2, 3, 18);
                        GradientB = new Color(15, 10, 61);
                    }
                    else
                    {
                        float interpolator = ((float)Main.time - 24000f) / (32400f - 24000f);
                        starAlpha = 1 - interpolator;
                        GradientA = Color.Lerp(new Color(2, 3, 18), new Color(57, 84, 204), interpolator);
                        GradientB = Color.Lerp(new Color(15, 10, 61), new Color(36, 56, 145), interpolator);
                    }
                }
                else
                {
                    starAlpha = 1;
                    GradientA = new Color(2, 3, 18);
                    GradientB = new Color(15, 10, 61);
                }

                // Draw gradient.
                for (int i = 0; i < steps; i++)
                {
                    spriteBatch.Draw(TextureRegistry.Pixel, new Rectangle(0, i * (_target.Height / steps) + 10, _target.Width, _target.Height / steps), Color.Lerp(GradientA, GradientB, (float)i / (float)steps));
                }

                // Clouds and Stars.

                if (starAlpha > 0)
                {
                    foreach (var star in BarrierStarSystem.Stars.Where(s => s.State <= SupernovaState.Shrinking))
                    {
                        float starRotation = star.BaseRotation + Main.GlobalTimeWrappedHourly * (star.BaseRotation - MathHelper.Pi) * 0.2f;

                        float starSize = star.BaseSize * (MathF.Sin(Main.GlobalTimeWrappedHourly / 3f + star.BaseRotation) + 1.2f);

                        starSize *= star.SupernovaSize;
                        starSize = MathF.Max(starSize, 0.35f);

                        Color starColor = Color.Lerp(star.Color * starAlpha, Color.Orange, 1f - star.SupernovaSize) with { A = 0 };

                        Vector2 starPosition = new Vector2(Center.X, Center.Y) + telescopeUIOffset + star.Position;

                        Texture2D starTexture = TextureRegistry.Stars[star.Texture];
                        Vector2 starOrigin = starTexture.Size() / 2;

                        spriteBatch.Draw(starTexture, starPosition, null, starColor, starRotation, starOrigin, starSize * 0.4f, SpriteEffects.None, 0f);
                        spriteBatch.Draw(TextureRegistry.Bloom, starPosition, null, starColor * 0.15f, 0, TextureRegistry.Bloom.Size() / 2, 0.07f * starSize, SpriteEffects.None, 0f);
                    }
                }

                if (BarrierStarSystem.TheOneImportantThingInTheSky.State <= SupernovaState.Shrinking)
                {
                    float TheOneImportantThingInTheSkyAlpha = MathHelper.Clamp(starAlpha, 0.1f, 1f);
                    Color[] colors = [(Color.Orange * TheOneImportantThingInTheSkyAlpha * 0.4f), (Color.Orange * TheOneImportantThingInTheSkyAlpha), (Color.Yellow * TheOneImportantThingInTheSkyAlpha), (Color.White * TheOneImportantThingInTheSkyAlpha)];

                    float interpolator = BarrierStarSystem.TheOneImportantThingInTheSky.SupernovaSize;
                    Vector2 position = new Vector2(Center.X, Center.Y) + telescopeUIOffset + BarrierStarSystem.TheOneImportantThingInTheSky.Position;
                    Vector2 origin = TextureRegistry.Star.Size() / 2;

                    spriteBatch.Draw(TextureRegistry.Bloom, position, null, Color.Lerp(colors[0], Color.Red, 1f - interpolator) with { A = 0 }, 0, TextureRegistry.Bloom.Size() / 2, (0.2f + (MathF.Sin(Main.GlobalTimeWrappedHourly) / 30f)) * interpolator, SpriteEffects.None, 0f);
                    spriteBatch.Draw(TextureRegistry.Star, position, null, Color.Lerp(colors[1], Color.Red, 1f - interpolator) with { A = 0 }, Main.GlobalTimeWrappedHourly / -25f, origin, (0.25f + (MathF.Sin(Main.GlobalTimeWrappedHourly) / 19f)) * interpolator, SpriteEffects.None, 0f);
                    spriteBatch.Draw(TextureRegistry.Star, position, null, Color.Lerp(colors[2], Color.Orange, 1f - interpolator) with { A = 0 }, Main.GlobalTimeWrappedHourly / 15f, origin, (0.13f + (MathF.Sin(Main.GlobalTimeWrappedHourly) / 15f)) * interpolator, SpriteEffects.None, 0f);
                    spriteBatch.Draw(TextureRegistry.Star, position, null, Color.Lerp(colors[3], Color.Yellow, 1f - interpolator) with { A = 0 }, Main.GlobalTimeWrappedHourly / -10f, origin, (0.07f + (MathF.Sin(Main.GlobalTimeWrappedHourly) / 14f)) * interpolator, SpriteEffects.None, 0f);
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
            if (!BarrierStarSystem.Stars.Where(s => s.State == SupernovaState.Expanding).Any() && BarrierStarSystem.TheOneImportantThingInTheSky.State != SupernovaState.Expanding)
                return;

            var planetShader = Helper.PlanetShader;
            var gd = Main.instance.GraphicsDevice;

            planetShader.Value.Parameters["uAngle"]?.SetValue(Main.GlobalTimeWrappedHourly / -69f);

            gd.Textures[1] = TextureRegistry.Smoke;
            gd.SamplerStates[1] = SamplerState.LinearWrap;

            planetShader.Value.CurrentTechnique.Passes[0].Apply();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, default, default, default, default, planetShader.Value, default);

            foreach (var supernovae in BarrierStarSystem.Stars.Where(s => s.State == SupernovaState.Expanding))
            {
                Vector2 position = new Vector2(size.X - 700, size.Y - 700) + telescopeUIOffset + supernovae.Position;

                spriteBatch.Draw(TextureRegistry.Star, position, null, (Color.SkyBlue * (0.65f - supernovae.SupernovaSize)) with { A = 0 }, Main.GlobalTimeWrappedHourly / 23f, TextureRegistry.Star.Size() / 2, supernovae.SupernovaSize * 0.35f, SpriteEffects.None, 0f);

                spriteBatch.Draw(TextureRegistry.Star, position, null, (Color.LightCyan * (0.88f - supernovae.SupernovaSize)) with { A = 0 }, Main.GlobalTimeWrappedHourly / -33f, TextureRegistry.Star.Size() / 2, supernovae.SupernovaSize * 0.2f, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            foreach (var supernovae in BarrierStarSystem.Stars.Where(s => s.State == SupernovaState.Expanding))
            {
                Vector2 position = new Vector2(size.X - 700, size.Y - 700) + telescopeUIOffset + supernovae.Position;

                spriteBatch.Draw(TextureRegistry.Shockwave, new Rectangle((int)position.X, (int)position.Y, (int)(60 * supernovae.SupernovaSize), (int)(100 * supernovae.SupernovaSize)), null, Color.White * (1f - supernovae.SupernovaSize), supernovae.BaseRotation, TextureRegistry.Shockwave.Size() / 2, SpriteEffects.None, 0f);

                spriteBatch.Draw(TextureRegistry.Bloom, position, null, Color.MediumBlue * (0.87f - supernovae.SupernovaSize), 0, TextureRegistry.Bloom.Size() / 2, supernovae.SupernovaSize * 0.4f, SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureRegistry.Bloom, position, null, Color.Cyan * (0.9f - supernovae.SupernovaSize), 0, TextureRegistry.Bloom.Size() / 2, supernovae.SupernovaSize * 0.23f, SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureRegistry.Bloom, position, null, Color.White * (1f - supernovae.SupernovaSize), 0, TextureRegistry.Bloom.Size() / 2, supernovae.SupernovaSize * 0.13f, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            Vector2[] positions = BarrierStarSystem.OldMeteoritePosition.Where(p => p != Vector2.Zero).ToArray();
            if (positions.Length >= 2)
            {
                for (int i = positions.Length - 1; i > 0; i--)
                {
                    Color color = Color.OrangeRed * (((float)positions.Length - i) / (float)positions.Length);
                    spriteBatch.Draw(TextureRegistry.Stars[1], positions[i] + telescopeUIOffset, null, color with { A = 0 }, Main.GlobalTimeWrappedHourly / 2f, TextureRegistry.Stars[1].Size() / 2, (0.1f + (BarrierStarSystem.TheOneImportantThingInTheSky.SupernovaSize * 4)) * (((float)positions.Length - i) / (float)positions.Length) + 0.02f, SpriteEffects.None, 0);
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
